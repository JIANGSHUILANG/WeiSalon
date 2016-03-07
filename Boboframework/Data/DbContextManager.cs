using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Boboframework.Data
{
    public class DbContextManager
    {
        /// <summary>
        /// 表实体上下文对象
        /// 在一个http请求的生命周期中，返回唯一的对象实例
        /// </summary>
        /// <returns></returns>
        public static mysqkbobohairEntitesContainer DefaultInstance
        {
            //get { return GetDefaultInstance<EntityDbContext>("bobo_dbcontext_entity"); }
            get { return GetDefaultInstance<mysqkbobohairEntitesContainer>("mysqlbobohairEntites"); }

        }


        ///// <summary>
        ///// 试图上下文
        ///// 在一个http请求的生命周期中，返回唯一的对象实例
        ///// </summary>
        ///// <returns></returns>
        //public static ViewsEntityDbContext ViewsDbContext
        //{
        //    get { return GetDefaultInstance<ViewsEntityDbContext>("bobo_dbcontext_view_entity"); }
        //}    

        static T GetDefaultInstance<T>(string name) where T : class, new()
        {
            if (HttpContext.Current == null) return new T();
            var item = HttpContext.Current.Items[name] as T;
            if (item == null)
            {
              
                item = new T();
                HttpContext.Current.Items.Add(name, item);
            }
            mysqkbobohairEntitesContainer ccc = new mysqkbobohairEntitesContainer();
            var sss = ccc.t_salon.Where(c => true).ToList();
            return item;
        }
    }
}
