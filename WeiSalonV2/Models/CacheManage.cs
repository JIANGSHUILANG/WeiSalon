/*----------------------------------------------------------------
// 莫笛思
// 版权所有。
//
// 文件名：CacheManage.cs
// 功能描述：
// 
// 创建人  ： KK
// 创建标识： 2015/10/16 10:27:55
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
using System.Web.Caching;

namespace WeiSalonV2.Models
{
    public class CacheManage
    {
        /// <summary>
        /// 将目标对象存储到缓存中
        /// </summary>
        /// <typeparam name="T">目标对象的类型</typeparam>
        /// <param name="key">缓存项的键名</param>
        /// <param name="target">目标对象</param>
        public static void SetCache<T>(string key, T target)
        {
            //将目标对象存储到缓存中
            HttpRuntime.Cache.Insert(key, target);
        }

        /// <summary>
        /// 获取缓存中的目标对象
        /// </summary>
        /// <param name="key">目标对象的类型</param>
        /// <returns>Object</returns>
        public static object GetCache(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        /// <summary>
        /// 2015-6-10 13:49:06
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetCache<T>(string key) where T : class
        {
            T result = null;
            try
            {
                result = HttpRuntime.Cache.Get(key) as T;
            }
            catch { }

            return result;
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveCache(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

        /// <summary>
        /// 写入缓存【设置分钟】
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="target">值</param>
        /// <param name="mi">按分钟计算</param>
        public static void SetCacheMinutes<T>(string key, T target, int mi)
        {
            HttpRuntime.Cache.Insert(key, target, null, DateTime.Now.AddMinutes(mi), Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
        }
    }
}