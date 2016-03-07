/*----------------------------------------------------------------
// 莫迪思
// 版权所有。
//
// 文件名：manage.cs
// 功能描述：
// 
// 创建人  ： KK
// 创建标识： 2015/3/10 14:47:42
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
using System.Web.Security;

namespace WeiSalonV2.Models
{
    public class manage
    {
        public static Guid UID
        {
            get
            {
                return new Guid(GetUID());
            }
        }

        public static Users User
        {
            get
            {
                //return new User();
                return GetUsers();
            }
        }
        /// <summary>
        /// 从票据获得UID
        /// </summary>
        private static string GetUID()
        {
            try
            {
                return FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 从票据获得User
        /// </summary>
        private static Users GetUsers()
        {
            try
            {
                var userinfo = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value).UserData.Replace("$bobo$", "|").Split('|');
                var users = new Users
                {
                    Cell = userinfo[0],
                    Name = userinfo[1],
                    Logo = userinfo[2]
                };

                return users;
            }
            catch
            {
                return null;
            }
        }
    }

    public class Users
    {
        public string Name { get; set; }

        public string Cell { get; set; }

        public string Logo { get; set; }
    }
}