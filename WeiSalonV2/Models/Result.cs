/*----------------------------------------------------------------
// 莫笛思
// 版权所有。
//
// 文件名：Result.cs
// 功能描述：
// 
// 创建人  ： KK
// 创建标识： 2015/10/15 13:25:50
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
    public class Result
    {
        public int status { get; set; }
        public string msg { get; set; }
        public object info { get; set; }
    }
}