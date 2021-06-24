using System.Collections.Generic;

namespace CoreData
{
    public class Pager
    {
        /// <summary>
        /// 总分页数
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int Total { get; set; }
    }

    /// <summary>
    /// 分页数据包装类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageResult<T> : Pager where T : class
    {
        public PageResult(IEnumerable<T> datas, int pageSize, int total)
        {
            Datas = datas;
            Total = total;
            if (pageSize > 0 && total > 0)
            {
                //PageCount = (int)Math.Ceiling((double)total / pageSize);
                //计算总页面数
                PageCount = (total + pageSize - 1) / pageSize;
            }
        }
        public PageResult(Pager pager)
        {
            Total = pager.Total;
            //计算总页面数
            PageCount = pager.PageCount;
        }

        public PageResult(Pager pager, IEnumerable<T> datas) : this(pager)
        {
            this.Datas = datas;
        }

        /// <summary>
        ///   初始化 <see cref="T:System.Object" /> 类的新实例。
        /// </summary>
        public PageResult()
        {
        }

        /// <summary>
        /// 分页后的数据
        /// </summary>
        public IEnumerable<T> Datas { get; set; }



        public PageResult<T1> Wrapper<T1>(IEnumerable<T1> datas) where T1 : class
        {
            return new()
            {
                Datas = datas,
                Total = this.Total,
                PageCount = this.PageCount
            };
        }
    }

}