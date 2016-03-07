/*----------------------------------------------------------------
// 莫笛思
// 版权所有。
//
// 文件名：APIModels.cs
// 功能描述：
// 
// 创建人  ： KK
// 创建标识： 2015/10/15 13:23:36
// 
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiSalonV2.Models
{
    public class APIModels
    {
    }

    /// <summary>
    /// 接受Huid
    /// </summary>
    public class HUID
    {
        public string huid { get; set; }
    }

    /// <summary>
    /// 接受bgid
    /// </summary>
    public class BGID
    {
        public string bgid { get; set; }
    }
    /// <summary>
    /// 接受htmlurl
    /// </summary>
    public class HTMLURL
    {
        public string htmlurl { get; set; }
    }

    /// <summary>
    /// 接受Suid
    /// </summary>
    public class SUID
    {
        public Guid suid { get; set; }
    }

    /// <summary>
    /// 接受Suid
    /// </summary>
    public class SUIDPAGE
    {
        public Guid suid { get; set; }
        public string pageindex { get; set; }
        public string pagesize { get; set; }
    }
}