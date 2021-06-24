using Utility.Mapper;

namespace CoreData
{
    public interface IPageQuery
    {
        /// <summary>
        /// 页码
        /// 注意：不设置页码将不会进行分页查询
        /// </summary>
        int PageIndex { get; set; }
        /// <summary>
        /// 页面大小
        /// 注意：不设置页面大小将不会进行分页查询
        /// </summary>
        int PageSize { get; set; }
    }
    /// <summary>
    /// 分页查询请求
    /// </summary>
    public class PageQuery : IPageQuery
    {
        /// <summary>
        /// 页码
        /// 注意：不设置页码将不会进行分页查询
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 页面大小
        /// 注意：不设置页面大小将不会进行分页查询
        /// </summary>
        public int PageSize { get; set; }
    }

    public class OrgPageQuery : PageQuery
    {
        public OrgPageQuery()
        {
        }

        public OrgPageQuery(string orgId)
        {
            OrgId = orgId;
        }

        public OrgPageQuery(string orgId, PageQuery query)
        {
            query.MapTo(this);
            OrgId = orgId;
        }

        public string OrgId { get; set; }
    }
}