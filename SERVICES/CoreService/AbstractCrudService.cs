using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CoreData;
using CoreData.Core.Aggregate;
using CoreData.Core.Attributes;
using CoreData.Models;
using CoreRepository;
using CoreService.Interfaces;
using Fasterflect;
using Utility;
using Utility.Extensions;
namespace CoreService
{
    /// <summary>
    /// 带CURD的抽象服务基类
    /// </summary>
    /// <typeparam name="T">聚合实体</typeparam>
    public abstract class AbstractCrudService<T> : AbstractService, ICrudService<T> where T : AggregateRoot, new()
    {
        #region 构造函数

        private IRepository<T> Repository { get; }

        protected AbstractCrudService(IRepository<T> repository)
        {
            Repository = repository;
        }

        #endregion

        /// <summary>
        /// 执行分页查询
        /// </summary>
        /// <returns></returns>
        public Task<PageResult<T>> PageQueryAsync<TPageQuery, TKey>(TPageQuery pager, Expression<Func<T, bool>> filter,
            Expression<Func<T, TKey>> orderByKeySelector,
            bool desc = false) where TPageQuery : PageQuery, new()
        {
            return Repository.PageQueryAsync(pager, filter, orderByKeySelector, desc);
        }

        public Task<PageResult<T>> PageQueryAsync<TPageQuery>(TPageQuery pager) where TPageQuery : PageQuery, new()
        {
            return Repository.PageQueryAsync(pager, null, p => p.CreatedOn, true);
        }
        /// <summary>
        /// 验证请求模型
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="orgId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<bool> TryValidateModelAsync<TModel>(string orgId, TModel model) where TModel : Model, new()
        {
            var rs = model.Validate();
            if (!rs.Success) throw new Exception(rs.Error);

            var modifyId = string.Empty;
            if (model is IUpdateModel updateModel)
            {
                modifyId = updateModel.Id;
            }

            #region 禁止重复特性判断

            var noRepeatPropertyInfos = NoRepeatPropertiesCache.GetNoRepeatPropertyInfos(model).ToList();
            if (noRepeatPropertyInfos.AnyNullable())
            {
                var tasks = noRepeatPropertyInfos.Select(p => GetValidResultAsync(p, p.GetValue(model), orgId, modifyId));
                var results = await Task.WhenAll(tasks);
                if (results.Any(p => p.IsValid == false))
                {
                    throw new Exception(results.First(p => p.IsValid == false).ErrorMembers.First().ErrorMessage);
                }
            }

            #endregion

            return true;
        }

        public Task<bool> TryValidateModelAsync<TModel>(TModel model) where TModel : Model, new()
        {
            return TryValidateModelAsync(string.Empty, model);
        }

        private async Task<ValidResult> GetValidResultAsync<TField>(PropertyInfo property, TField value, string orgId, string modifyId)
        {
            var nrp = property.GetCustomAttribute<NoRepeatAttribute>();
            var repeat = await Repository.IsFieldRepeatAsync(property.Name, value, nrp.Global ? string.Empty : orgId, modifyId);

            var errorMembers = new List<ErrorMember>();
            if (repeat)
            {
                var displayNameAttr = property.GetCustomAttribute<DisplayNameAttribute>();
                var displayName = displayNameAttr?.DisplayName ?? property.Name;
              
                var member = new ErrorMember
                {
                    ErrorMemberName = property.Name,
                    ErrorMessage = string.Format(nrp.ErrorMessage, displayName, value)
                };
                errorMembers.Add(member);
            }

            return new ValidResult
            {
                ErrorMembers = errorMembers,
                IsValid = !repeat
            };
        }

        /// <summary>
        /// 创建实体数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="orgId">机构ID</param>
        /// <param name="addModel">添加模型</param>
        /// <param name="createHook">插入前对实体进行处理</param>
        /// <returns></returns>
        public virtual async Task<T> CreateAsync<TModel>(string orgId, TModel addModel,
            Func<T, bool> createHook = null) where TModel : Model, new()
        {
            await this.TryValidateModelAsync(orgId, addModel);
            var entity = addModel.MapTo<TModel, T>();
            //entity.TrySetPropertyValue("OrgId", orgId);

            var invoked = await HookBeforeActionAsync(entity, createHook, OnBeforeCreate, OnBeforeCreateAsync);

            if (invoked)
            {
                await Repository.InsertAsync(entity);
                await HookAfterActionAsync(entity, OnAfterCreate, OnAfterCreateAsync);
            }


            return entity;
        }

        /// <summary>
        /// 更新数据实体数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="updateModel">更新模型</param>
        /// <param name="updateHook"></param>
        /// <returns></returns>
        public virtual async Task<T> UpdateAsync<TModel>(TModel updateModel, Func<T, bool> updateHook = null) where TModel : Model, IUpdateModel, new()
        {
            if (string.IsNullOrEmpty(updateModel.Id))
            {
                throw new Exception("更新时须正确传入ID");
            }
            var oldEntity = await Repository.GetAsync(updateModel.Id);//旧版本数据库记录
          //  var orgId = (oldEntity as OEntity)?.OrgId;
            await this.TryValidateModelAsync(string.Empty, updateModel);


            var entity = updateModel.MapTo(oldEntity.DeepClone()); //旧实体->新实体


            var invoked = await HookBeforeActionAsync(entity, updateHook, OnBeforeUpdate, OnBeforeUpdateAsync);
            if (invoked)
            {
                var updated = await Repository.UpdateAsync(entity);
                if (updated)
                {
                    await HookAfterUpdateActionAsync(oldEntity, entity, OnAfterUpdate, OnAfterUpdateAsync);
                }

            }
            return entity;
        }
        /// <summary>
        /// CRUD操作统一拦截处理(BEFORE)
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <param name="param">参数</param>
        /// <param name="hook">方法内Hook拦截</param>
        /// <param name="onBeforeHook">全局同步拦截</param>
        /// <param name="onBeforeHookAsync">全局异步拦截</param>
        /// <returns></returns>
        private async Task<bool> HookBeforeActionAsync<TParam>(TParam param, Func<TParam, bool> hook, Func<TParam, bool> onBeforeHook, Func<TParam, Task<bool>> onBeforeHookAsync)
        {
            var hk = hook ?? onBeforeHook;
            var invoke = hk?.Invoke(param);
            if (invoke.HasValue && !invoke.Value) return false;
            if (onBeforeHook == null) return true;
            var asyncInvoked = await onBeforeHookAsync.Invoke(param);
            return asyncInvoked;
        }

        /// <summary>
        ///  CRUD操作统一拦截处理(AFTER)
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <param name="param"></param>
        /// <param name="onAfterHook"></param>
        /// <param name="onAfterHookAsync"></param>
        /// <returns></returns>
        private async Task HookAfterActionAsync<TParam>(TParam param, Action<TParam> onAfterHook,
            Func<TParam, Task> onAfterHookAsync)
        {
            onAfterHook?.Invoke(param);
            if (onAfterHookAsync != null)
                await onAfterHookAsync.Invoke(param);

        }

        private async Task HookAfterUpdateActionAsync<TParam>(TParam oldEntity, TParam newEntity, Action<TParam, TParam> onAfterHook,
            Func<TParam, TParam, Task> onAfterHookAsync)
        {
            onAfterHook?.Invoke(oldEntity, newEntity);
            if (onAfterHookAsync != null)
                await onAfterHookAsync.Invoke(oldEntity, newEntity);

        }
        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="id">主键ID值</param>
        /// <param name="deleteHook">删除前确认</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(string id, Func<string, bool> deleteHook = null)
        {
            var invoked = await HookBeforeActionAsync(id, deleteHook, OnBeforeDelete, OnBeforeDeleteAsync);

            if (invoked)
            {
                var deleted = await Repository.DeleteAsync(id);
                if (deleted)
                {
                    await HookAfterActionAsync(id, OnAfterDelete, OnAfterDeleteAsync);
                }

                return deleted;
            }

            return false;
        }

        /// <summary>
        /// 获取数据库实体对象
        /// </summary>
        /// <param name="id">主键ID值</param>
        /// <returns></returns>
        public virtual Task<T> GetAsync(string id)
        {
            return Repository.GetAsync(id);
        }

        /// <summary>
        /// 获取数据实体
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="model">数据模型</param>
        /// <returns></returns>
        public virtual async Task<T> GetAsync<TModel>(string id, TModel model = null) where TModel : Model, new()
        {
            var entity = await Repository.GetAsync(id);
            model?.MapTo(entity);
            return entity;
        }
        /// <summary>
        /// 获取数据实体
        /// </summary>
        /// <param name="updateModel">更新数据模型</param>
        /// <returns></returns>
        public virtual Task<T> GetAsync<TModel>(TModel updateModel) where TModel : Model, IUpdateModel, new()
        {
            if (string.IsNullOrEmpty(updateModel.Id)) throw new Exception("修改模型实体的ID不能为空");
            return GetAsync(updateModel.Id, updateModel);
        }

        #region CRUD事件拦截器

        /// <summary>
        /// 当创建实体前触发
        /// </summary>
        public event Func<T, bool> OnBeforeCreate;
        public event Func<T, Task<bool>> OnBeforeCreateAsync;
        public event Action<T> OnAfterCreate;
        public event Func<T, Task> OnAfterCreateAsync;

        /// <summary>
        /// 当实体将更新前触发事件
        /// </summary>
        public event Func<T, bool> OnBeforeUpdate;
        public event Func<T, Task<bool>> OnBeforeUpdateAsync;
        public event Action<T, T> OnAfterUpdate;
        public event Func<T, T, Task> OnAfterUpdateAsync;
        /// <summary>
        /// 删除实体前触发事件
        /// </summary>
        public event Func<string, bool> OnBeforeDelete;
        public event Func<string, Task<bool>> OnBeforeDeleteAsync;
        public event Action<string> OnAfterDelete;
        public event Func<string, Task> OnAfterDeleteAsync;

        #endregion
    }
}