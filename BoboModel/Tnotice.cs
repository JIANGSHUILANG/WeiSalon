/*----------------------------------------------------------------
// 莫迪思
// 版权所有。
//
// 文件名：Tnotice.cs
// 功能描述：
// 
// 创建人  ： KK
// 创建标识： 2015-11-13 09:59:00
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
using System.Text;
using System.Threading.Tasks;

namespace BoboModel
{
    /// <summary>
    /// 沙龙公告表 
    /// </summary>
    public class Tnotice
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 沙龙uid
        /// </summary>
        public Guid? suid { get; set; }

        /// <summary>
        /// 公告内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? date { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? status { get; set; }
    }
}
