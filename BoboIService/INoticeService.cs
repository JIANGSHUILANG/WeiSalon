using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boboframework.Data;
using BoboModel;

namespace BoboIService
{
    public interface INotice
    {
        /// <summary>
        /// 根据suid 获取沙龙最新的公告
        /// </summary>
        /// <param name="suid">沙龙UID</param>
        /// <returns></returns>
        Tnotice GetNotice(Guid suid);
    }


    public class INoticeService : INotice
    {
        private readonly mysqkbobohairEntitesContainer _db = DbContextManager.DefaultInstance;
        private readonly IErrorService _err = new ErrorService();
        #region ISalonImage 成员

        public Tnotice GetNotice(Guid suid)
        {
            var info = new Tnotice();
            try
            {
                info = _db.t_notice
                   .Where(a => a.tn_s_uid == suid && a.tn_status == 1)
                   .OrderByDescending(a => a.tn_date)
                   .Select(a => new Tnotice
                   {
                       id = a.tn_id,
                       content = a.tn_content,
                       date = a.tn_date,
                       status = a.tn_status,
                       suid = a.tn_s_uid
                   })
                   .FirstOrDefault();
            }
            catch
            { }
            return info;
        }

        #endregion

    }


}
