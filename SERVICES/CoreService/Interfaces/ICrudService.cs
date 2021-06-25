using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreData;
using CoreData.Core.Aggregate;
using CoreData.Models;

namespace CoreService.Interfaces
{

    /// <summary>
    /// 增删改查基本服务接口
    /// </summary>
    //[InheritedExport]
    public interface ICrudService<T> : IService where T : AggregateRoot
    {
        /// <summary>
        /// 执行自定义分页查询
        /// </summary>
        /// <returns></returns>
        Task<PageResult<T>> PageQueryAsync<TPageQuery, TKey>(TPageQuery pager, Expression<Func<T, bool>> filter,
            Expression<Func<T, TKey>> orderByKeySelector, bool desc = false) where TPageQuery : PageQuery, new();
        /// <summary>
        /// 执行分页查询
        /// </summary>
        /// <typeparam name="TPageQuery"></typeparam>
        /// <param name="pager"></param>
        /// <returns></returns>
        Task<PageResult<T>> PageQueryAsync<TPageQuery>(TPageQuery pager) where TPageQuery : PageQuery, new();
        /// <summary>
        /// 验证请求模型(指定机构内数据不能重复)
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="orgId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> TryValidateModelAsync<TModel>(string orgId, TModel model) where TModel : Model, new();
        /// <summary>
        /// 验证请求模型(所有机构内数据不能重复)
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> TryValidateModelAsync<TModel>(TModel model) where TModel : Model, new();

        /// <summary>
        /// 创建实体数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="orgId"></param>
        /// <param name="addModel"></param>
        /// <param name="createHook">实体插入到数据库前处理委托</param>
        /// <returns></returns>
        Task<T> CreateAsync<TModel>(string orgId, TModel addModel, Func<T, bool> createHook = null) where TModel : Model, new();

        /// <summary>
        /// 更新数据实体数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="updateModel">更新模型</param>
        /// <param name="updateHook"></param>
        /// <returns></returns>
        Task<T> UpdateAsync<TModel>(TModel updateModel, Func<T, bool> updateHook = null) where TModel : Model, IUpdateModel, new();

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="id">主键ID值</param>
        /// <param name="deleteHook"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string id, Func<string, bool> deleteHook = null);

        #region GET QUERY

        /// <summary>
        /// 获取数据库实体对象
        /// </summary>
        /// <param name="id">主键ID值</param>
        /// <returns></returns>
        Task<T> GetAsync(string id);

        /// <summary>
        /// 获取数据实体
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="model">数据模型</param>
        /// <returns></returns>
        Task<T> GetAsync<TModel>(string id, TModel model = null) where TModel : Model, new();

        /// <summary>
        /// 获取数据实体
        /// </summary>
        /// <param name="updateModel">更新数据模型</param>
        /// <returns></returns>
        Task<T> GetAsync<TModel>(TModel updateModel) where TModel : Model, IUpdateModel, new();

        #endregion

        #region CRUD事件

        /// <summary>
        /// 当实体被插入数据库前触发事件
        /// </summary>
        event Func<T, bool> OnBeforeCreate;
        /// <summary>
        /// 当实体被插入数据库后触发事件
        /// </summary>
        event Action<T> OnAfterCreate;
        /// <summary>
        /// 当实体被插入数据库前触发事件（异步）
        /// </summary>
        event Func<T, Task<bool>> OnBeforeCreateAsync;
        /// <summary>
        /// 当实体被插入数据库后触发事件（异步）
        /// </summary>
        event Func<T, Task> OnAfterCreateAsync;
        /// <summary>
        /// 当实体将更新前触发事件
        /// </summary>
        event Func<T, bool> OnBeforeUpdate;
        /// <summary>
        /// 当实体将更新后触发事件
        /// </summary>
        event Action<T, T> OnAfterUpdate;
        /// <summary>
        /// 当实体将更新前(异步)触发事件
        /// </summary>
        event Func<T, Task<bool>> OnBeforeUpdateAsync;
        /// <summary>
        /// 当实体将更新后(异步)触发事件
        /// </summary>
        event Func<T, T, Task> OnAfterUpdateAsync;
        /// <summary>
        /// 当实体被同步删除前触发事件
        /// </summary>
        event Func<string, bool> OnBeforeDelete;
        /// <summary>
        /// 当实体被同步删除后触发事件
        /// </summary>
        event Action<string> OnAfterDelete;
        /// <summary>
        /// 当实体异步删除前触发事件
        /// </summary>
        event Func<string, Task<bool>> OnBeforeDeleteAsync;
        /// <summary>
        /// 当实体异步删除后触发事件
        /// </summary>
        event Func<string, Task> OnAfterDeleteAsync;

        #endregion


    }
}