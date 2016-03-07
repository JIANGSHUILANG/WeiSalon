using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boboframework.Data;

namespace BoboIService
{
    public interface IErrorService
    {
        /// <summary>
        /// 添加报错信息
        /// </summary>
        /// <param name="name">方法名</param>
        /// <param name="content">报错内容</param>
        /// <returns></returns>
        int AddError(string name, string content);
    }


    public class ErrorService : IErrorService
    {
        private readonly mysqkbobohairEntitesContainer _db = DbContextManager.DefaultInstance;
        #region IErrorService 成员

        public int AddError(string name, string content)
        {
            var model = new t_error()
            {
                eo_content = content,
                eo_date = DateTime.Now,
                eo_name = name
            };

            try
            {
                _db.t_error.Add(model);
                return _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _db.t_error.Remove(model);
                return 0;
            }
        }

        #endregion
    }
}
