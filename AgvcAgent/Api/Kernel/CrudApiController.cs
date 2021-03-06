using System.Threading.Tasks;
using CoreData;
using CoreData.Core.Aggregate;
using CoreData.Models;
using CoreService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgvcAgent.Api.Kernel
{
    /// <summary>
    ///     基本CRUD API服务接口（含更新模型）
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <typeparam name="TModel">基本模型</typeparam>
    /// <typeparam name="TUpdateModel">更新模型</typeparam>
    public abstract class CrudApiController<T, TModel, TUpdateModel> : CrudApiController<T, TModel>
        where T : AggregateRoot
        where TModel : Model, new()
        where TUpdateModel : Model, IUpdateModel, new()
    {
        protected CrudApiController(ICrudService<T> crudService) : base(crudService)
        {
            CrudService = crudService;

            crudService.OnBeforeUpdate += OnBeforeUpdate;
            crudService.OnBeforeUpdateAsync += OnBeforeUpdateAsync;
            crudService.OnAfterUpdate += OnAfterUpdate;
            crudService.OnAfterUpdateAsync += OnAfterUpdateAsync;
        }

        private ICrudService<T> CrudService { get; }

        /// <summary>
        ///     更新实体
        /// </summary>
        /// <param name="updateModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public virtual Task<T> UpdateAsync([FromForm] TUpdateModel updateModel)
        {
            return CrudService.UpdateAsync(updateModel);
        }


        #region UPDATE EVENTS

        protected virtual void OnAfterUpdate(T oldEntity, T newEntity)
        {
        }

        protected virtual Task OnAfterUpdateAsync(T oldEntity, T newEntity)
        {
            return Task.CompletedTask;
        }

        protected virtual bool OnBeforeUpdate(T entity)
        {
            return true;
        }

        protected virtual Task<bool> OnBeforeUpdateAsync(T entity)
        {
            return Task.FromResult(true);
        }

        #endregion
    }

    /// <summary>
    ///     基本CRUD API服务接口(不含更新模型)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public abstract class CrudApiController<T, TModel> : AuthorizedApiController
        where T : AggregateRoot
        where TModel : Model, new()
    {
        /// <summary>
        ///     新建实体数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public virtual Task<T> CreateAsync([FromForm] TModel model)
        {
            return CrudService.CreateAsync(OrgId, model);
        }

        /// <summary>
        ///     删除实体数据
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete/{id}")]
        public virtual Task<bool> DeleteAsync(string id)
        {
            return CrudService.DeleteAsync(id);
        }

        /// <summary>
        ///     获取实体数据
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get/{id}")]
        public virtual Task<T> GetAsync(string id)
        {
            return CrudService.GetAsync(id);
        }

        /// <summary>
        ///     分页获取所有数据(不限制机构)
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("page-query")]
        public virtual Task<PageResult<T>> PageQueryAsync([FromForm] PageQuery page)
        {
            return CrudService.PageQueryAsync(page);
        }

        /// <summary>
        ///     分页获取所有数据(限制机构)
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("org-page-query")]
        public virtual Task<PageResult<T>> OrgPageQueryAsync([FromForm] PageQuery page)
        {
            return CrudService.PageQueryAsync(new OrgPageQuery(OrgId, page));
        }

        #region 构造函数

        private ICrudService<T> CrudService { get; }

        protected CrudApiController(ICrudService<T> crudService)
        {
            CrudService = crudService;
            //create events 
            crudService.OnBeforeCreate += OnBeforeCreate;
            crudService.OnBeforeCreateAsync += OnBeforeCreateAsync;
            crudService.OnAfterCreate += OnAfterCreate;
            crudService.OnAfterCreateAsync += OnAfterCreateAsync;

            //delete events 
            crudService.OnBeforeDelete += OnBeforeDelete;
            crudService.OnBeforeDeleteAsync += OnBeforeDeleteAsync;

            crudService.OnAfterDelete += OnAfterDelete;
            crudService.OnAfterDeleteAsync += OnAfterDeleteAsync;
        }

        #region DELETE EVENTS

        protected virtual Task OnAfterDeleteAsync(string id)
        {
            return Task.CompletedTask;
        }

        protected virtual void OnAfterDelete(string id)
        {
        }

        protected virtual bool OnBeforeDelete(string id)
        {
            return true;
        }

        protected virtual Task<bool> OnBeforeDeleteAsync(string id)
        {
            return Task.FromResult(true);
        }

        #endregion

        #region CREATE EVENTS

        protected virtual Task OnAfterCreateAsync(T entity)
        {
            return Task.CompletedTask;
        }

        protected virtual void OnAfterCreate(T entity)
        {
        }


        protected virtual bool OnBeforeCreate(T entity)
        {
            return true;
        }

        protected virtual Task<bool> OnBeforeCreateAsync(T entity)
        {
            return Task.FromResult(true);
        }

        #endregion

        #endregion
    }
}