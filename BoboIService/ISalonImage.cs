using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boboframework.Data;
using BoboModel;
using System.Data.Entity;

namespace BoboIService
{
    public interface ISalonImage
    {
        /// <summary>
        /// 根据uid 获取美发店相关图片
        /// </summary>
        /// <param name="uid">沙龙UID</param>
        /// <returns></returns>
        List<SalonImage> GetImagesInfo(Guid uid);

        /// <summary>
        /// 根据ID 修改沙龙图片信息
        /// </summary>
        /// <param name="id">图片表ID 主键</param>
        /// <param name="imagePath">新图片路径</param>
        /// <returns></returns>
        int UpdateImageInfo(int id, string imagePath);
    }


    public class SalonImageInfo : ISalonImage
    {
        private readonly mysqkbobohairEntitesContainer _db = DbContextManager.DefaultInstance;
        private readonly IErrorService _err = new ErrorService();
        #region ISalonImage 成员

        public List<SalonImage> GetImagesInfo(Guid uid)
        {
            var list = new List<SalonImage>();

            var info = _db.t_salonimageinfo
                .Where(a => a.si_s_uid == uid && a.si_status == 1 && (a.si_kind == 2 || a.si_kind == 3))
                .OrderBy(a => a.si_kind)
                .Select(a => new SalonImage
                {
                    Id = a.si_id,
                    Suid = a.si_s_uid,
                    Photo = a.si_photo,
                    Kind = a.si_kind,
                    Status = a.si_status
                })
                .ToList();

            list.AddRange(info);
            return list;
        }

        #endregion

        #region ISalonImage 成员


        public int UpdateImageInfo(int id, string imagePath)
        {
            var info = _db.t_salonimageinfo.Find(id);
            var count = 0;
            try
            {
                if (info != null)
                {
                    info.si_photo = imagePath;
                    _db.Entry(info).State = EntityState.Modified;
                    count = _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _db.Entry(info).Reload();
                _err.AddError("", ex.Message);
            }
            return count;
        }
        #endregion
    }

    public interface ISalonImageService : IBaseService<t_salonimageinfo> { }
    public class SalonImageService : BaseService<t_salonimageinfo>, ISalonImageService { }
}
