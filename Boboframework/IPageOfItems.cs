using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boboframework
{

    public interface IPageOfItems<out T> : IEnumerable<T>
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        int PageNumber { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// 所有项的总量
        /// </summary>
        int TotalItemCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        int TotalPageCount { get; }

        /// <summary>
        /// 其实位置
        /// </summary>
        int StartPosition { get; }

        /// <summary>
        /// 结束位置
        /// </summary>
        int EndPosition { get; }

        /// <summary>
        /// 应该为非空
        /// </summary>
        string PageNumberKey { get; set; }

        string UrlMeta { get; set; }
    }
}

