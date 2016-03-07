using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boboframework
{
    public class PageOfItems<T> : List<T>, IPageOfItems<T>
    {
        private string _pageNumberKey = "pageindex";

        public PageOfItems()
        {
        }
        public PageOfItems(IEnumerable<T> items)
        {
            AddRange(items);
        }


        #region IPageOfItems<T> Members

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 所有项的总量
        /// </summary>
        public int TotalItemCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPageCount
        {
            get { return (int)Math.Ceiling((double)TotalItemCount / PageSize); }
        }
        /// <summary>
        /// 其实位置
        /// </summary>
        public int StartPosition
        {
            get { return (PageNumber - 1) * PageSize + 1; }
        }
        /// <summary>
        /// 结束位置
        /// </summary>
        public int EndPosition
        {
            get { return PageNumber * PageSize > TotalItemCount ? TotalItemCount : PageNumber * PageSize; }
        }

        /// <summary>
        /// 默认值:page_num
        /// </summary>
        public string PageNumberKey
        {
            get { return _pageNumberKey ?? "pageindex"; }
            set { _pageNumberKey = value ?? "pageindex"; }
        }

        public string UrlMeta { get; set; }

        #endregion
    }
}
