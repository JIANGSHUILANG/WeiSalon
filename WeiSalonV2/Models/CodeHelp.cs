/*----------------------------------------------------------------
// 莫笛思
// 版权所有。
//
// 文件名：CodeHelp.cs
// 功能描述：
// 
// 创建人  ： KK
// 创建标识： 2015/10/16 10:26:35
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
    public class CodeHelp
    {
        public static string Values = string.Empty;

        /// <summary>
        /// 返回请求结果
        /// </summary>
        /// <param name="status"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetResults(int status, string msg, object data)
        {
            var res = new Result { status = status, msg = msg, info = data };
            var timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss"
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(res, timeConverter);//.ToLower();
        }

        /// <summary>
        /// 返回错误信息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string GetErroe(string msg)
        {
            var res = new Result { status = 0, msg = msg, info = "" };
            return Newtonsoft.Json.JsonConvert.SerializeObject(res);//.ToLower();
        }
    }
}