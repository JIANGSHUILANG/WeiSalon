/*----------------------------------------------------------------
// 莫笛思
// 版权所有。
//
// 文件名：JsonTools.cs
// 功能描述：
// 
// 创建人  ： KK
// 创建标识： 2015/11/9 14:01:20
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
using System.Web.Script.Serialization;

namespace WeiSalonV2.Models
{
    public class JsonTools
    {
        /// <summary>
        /// 对象转Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            try
            {
                return new JavaScriptSerializer().Serialize(obj);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Json转Dictionary
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Dictionary<string, object> Deserialize(string json)
        {
            try
            {
                return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static T JsonToObject<T>(string jsonText)
        {
            var jss = new JavaScriptSerializer();
            try
            {
                return jss.Deserialize<T>(jsonText);
            }
            catch (Exception ex)
            {
                throw new Exception("JSONHelper.JSONToObject():" + ex.Message);
            }
        }
    }
}