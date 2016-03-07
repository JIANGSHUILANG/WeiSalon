using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BoboModel;
using BoboCommon;
using Boboframework;
using Boboframework.Data;

using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Data.Entity;
namespace BoboIService
{
    public partial interface IHairListService : IBaseService<t_salinhairstylist>
    {
        /// <summary>
        /// 根据uid 获取发型师别名信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        HariStyleList GetHariStyleListInfo(Guid uid);

        /// <summary>
        /// 根据uid 获取发型师别名信息
        /// </summary>
        /// <param name="uid"></param> 
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        List<HariStyleList> GetHariStyleListList(Guid uid, int pageindex, int pagesize, out int cou, out int pagcou);

        /// <summary>
        /// 根据沙龙uid 获取发型师id
        /// </summary>
        /// <param name="uid"></param> 
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        string GetHariStyleListID(Guid uid);

        /// <summary>
        /// 修改发型师信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int UpdateHariStyleList(HariStyleList model);

        /// <summary>
        /// 添加发型师别名信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int AddHariStyleList(HariStyleList model);

        /// <summary>
        /// 删除 发型师别名信息
        /// </summary>
        /// <param name="Uid"></param>
        /// <returns></returns>
        int DelHariStyleList(Guid Uid);

    }

    public partial class HairListService : BaseService<t_salinhairstylist>, IHairListService
    {
        private readonly mysqkbobohairEntitesContainer _db = DbContextManager.DefaultInstance;
        private readonly IErrorService _err = new ErrorService();
        #region IHairListService 成员

        /// <summary>
        /// 根据uid 获取发型师别名信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public HariStyleList GetHariStyleListInfo(Guid uid)
        {

            var info = new List<HariStyleList>();
            try
            {
                info = _db.t_salinhairstylist.Where(a => a.shs_h_uid == uid)
                              .Select(a => new HariStyleList()
                              {
                                  Huid = a.shs_h_uid,
                                  Name = a.shs_name,
                                  Hairprice = a.shs_hairprice,
                                  Exp = a.shs_exp,
                                  Cell = a.shs_cell,
                                  Uid = a.shs_s_uid,
                                  Logo = a.shs_logo,
                                  status = a.shs_status
                              }).ToList();
            }
            catch (Exception ex)
            {

                _err.AddError("GetHariStyleListInfo", ex.Message);
            }
            return info.FirstOrDefault();
        }

        /// <summary>
        /// 根据uid 获取发型师别名信息
        /// </summary>
        /// <param name="uid"></param> 
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public List<HariStyleList> GetHariStyleListList(Guid uid, int pageindex, int pagesize, out int cou, out int pagcou)
        {
            cou = 0;
            pagcou = 0;

            var list = new PageOfItems<HariStyleList>() { PageNumber = pageindex, PageSize = pagesize };
            list.TotalItemCount = _db.t_salinhairstylist
                .Where(a => a.shs_s_uid == uid && a.shs_status == 1).Count();

            cou = list.TotalItemCount;
            pagcou = list.TotalPageCount;
            var beginnum = (pageindex - 1) * pagesize;

            var info = new List<HariStyleList>();
            try
            {
                //var lista = from a in _db.t_salinhairstylist
                //            join b in _db.Wx_Comment on a.shs_h_uid equals b.wc_h_uid
                //            into temp
                //            from b in temp.DefaultIfEmpty()
                //            where a.shs_s_uid == uid
                //            orderby a.shs_exp descending
                //            select new HariStyleList
                //            {
                //                Huid = a.shs_h_uid,
                //                Name = a.shs_name,
                //                Hairprice = a.shs_hairprice,
                //                Exp = a.shs_exp,
                //                Cell = a.shs_cell,
                //                Uid = a.shs_s_uid,
                //                Logo = a.shs_logo
                //            };
                info = _db.t_salinhairstylist
                       .Where(a => a.shs_s_uid == uid && a.shs_status == 1)
                       .OrderByDescending(a => a.shs_exp)
                       .Skip(beginnum)
                       .Take(pagesize)
                        .Select(a => new HariStyleList()
                        {
                            Huid = a.shs_h_uid,
                            Name = a.shs_name,
                            Hairprice = a.shs_hairprice,
                            Exp = a.shs_exp,
                            Cell = a.shs_cell,
                            Uid = a.shs_s_uid,
                            Logo = a.shs_logo,
                            status = a.shs_status,
                            score = (from b in _db.wx_comment where b.wc_h_uid == a.shs_h_uid select b.wc_score).Average() != null ? (from b in _db.wx_comment where b.wc_h_uid == a.shs_h_uid && a.shs_status == 1 select b.wc_score).Average() : 0
                        }).ToList();
            }
            catch (Exception ex)
            {

                _err.AddError("GetHariStyleListList", ex.Message);
            }
            return info;
        }

        /// <summary>
        /// 根据沙龙uid 获取发型师id
        /// </summary>
        /// <param name="uid"></param> 
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public string GetHariStyleListID(Guid uid)
        {
            string str = "";
            var info = new List<HariStyleList>();
            try
            {
                info = _db.t_salinhairstylist.Where(a => a.shs_s_uid == uid && a.shs_status == 1)
                              .Select(a => new HariStyleList()
                              {
                                  Huid = a.shs_h_uid
                              }).ToList();
            }
            catch (Exception ex)
            {

                _err.AddError("GetHariStyleListList", ex.Message);
            }
            if (info != null && info.Count > 0)
            {
                for (int i = 0; i < info.Count; i++)
                {
                    str += info[i].Huid + ";";
                }
                if (str.Length > 0)
                {
                    str = str.Substring(0, str.Length - 1);
                }
            }
            return str;
        }

        /// <summary>
        /// 修改发型师别名信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateHariStyleList(HariStyleList mod)
        {
            int count = 0;
            var model = _db.t_salinhairstylist.Find(mod.Huid);
            try
            {
                model.shs_logo = mod.Logo;
                model.shs_hairprice = mod.Hairprice;
                model.shs_name = mod.Name;
                model.shs_exp = mod.Exp;
                model.shs_s_uid = mod.Uid;
                model.shs_status = mod.status;
                _db.Entry(model).State = EntityState.Modified;
                count = _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _db.Entry(model).Reload();
                _err.AddError("UpdateLogoInfo", ex.Message);
            }
            return count;
        }

        /// <summary>
        /// 添加 发型师别名信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddHariStyleList(HariStyleList mod)
        {

            int count = 0;
            var model = new t_salinhairstylist()
            {
                shs_exp = mod.Exp,
                shs_hairprice = mod.Hairprice == null ? "" : mod.Hairprice,
                shs_h_uid = mod.Huid,
                shs_logo = mod.Logo == null ? "" : mod.Logo,
                shs_cell = mod.Cell == null ? "" : mod.Cell,
                shs_s_uid = mod.Uid,
                shs_name = mod.Name == null ? "" : mod.Name,
                shs_status = mod.status,
                shs_sort = (int)mod.score,
            };
            try
            {
                _db.t_salinhairstylist.Add(model);
                _db.Configuration.ValidateOnSaveEnabled = false;
                count = _db.SaveChanges();
                _db.Configuration.ValidateOnSaveEnabled = true;

            }
            catch (Exception ex)
            {
                _db.t_salinhairstylist.Remove(model);
                _err.AddError("AddHariStyleList", ex.Message);
            }
            return count;
        }


        /// <summary>
        /// 删除 发型师别名信息
        /// </summary>
        /// <param name="Uid"></param>
        /// <returns></returns>
        public int DelHariStyleList(Guid Uid)
        {

            int count = 0;
            try
            {
                var model = _db.t_salinhairstylist.Find(Uid);
                _db.t_salinhairstylist.Remove(model);
                return _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _err.AddError("AddHariStyleList", ex.Message);
            }
            return count;
        }
        #endregion

    }


    /// <summary>
    // 创建人  ： JiangSL
    // 创建标识： 2015/10/13 17:38
    /// </summary>
    public partial interface IHairListService : IBaseService<t_salinhairstylist>
    {

        /// <summary>
        /// 根据手机号获取Oracle中t_hairstylist表中的数据
        /// </summary>
        /// <param name="cell">手机号</param>
        /// <returns></returns>
        O_T_Hairstylist GetT_hairstylistInfoByCell_Oracle(string cell);


        /// <summary>
        /// 根据手机号获取Server中t_hairstylist表中的数据
        /// </summary>
        /// <param name="cell">手机号</param>
        /// <returns></returns>
        HariStyleList GetT_SalinHairStylistByCell_Server(string cell, string S_UID = null);

        /// <summary>
        /// 获取沙龙绑定发型师所发布的图片
        /// 排序按照人气+普通的形式 只去前100张
        /// </summary>
        /// <param name="shs_h_uid">发型师H_UID</param>
        /// <returns></returns>
        O_T_Blog GetBlogBySalinHairStylist(string H_UID);


        /// <summary>
        /// 根据salon uid 获取美发师信息
        /// </summary>
        /// <param name="whereLamda"></param>
        /// <returns></returns>
        List<HariStyleList> GetT_SalinHairStylistByLamda(System.Linq.Expressions.Expression<Func<t_salinhairstylist, bool>> whereLamda);


        /// <summary>
        /// 根据salon uid 获取美发师信息
        /// </summary>
        /// <param name="uid">SALON uid</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">一页X条</param>
        /// <returns></returns>
        PageOfItems<HariStyleList> GetHairStyList(Guid uid, int pageIndex, int pageSize, int type = 1);

    }

    /// <summary>
    // 创建人  ： JiangSL
    // 创建标识： 2015/10/13 17:38
    /// </summary>
    public partial class HairListService : BaseService<t_salinhairstylist>, IHairListService
    {
        /// <summary>
        /// 根据手机号获取Oracle中t_hairstylist表中的数据
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public O_T_Hairstylist GetT_hairstylistInfoByCell_Oracle(string cell)
        {
            string aescell = EncryptionStrings.EncryptResult("cell=" + cell); //加密参数
            string str = WebRequestGetOrPost.Post("/api/weisalon/gethairinfo", aescell).TrimEnd('"').TrimStart('"');
            str = EncryptionStrings.DecryptParameter(str);//解密参数
            O_T_Hairstylist hair = JsonConvert.DeserializeObject<O_T_Hairstylist>(str);
            return hair;
        }

        /// <summary>
        /// 根据手机号获取Server中t_hairstylist表中的数据
        /// </summary>
        /// <param name="cell">手机号</param>
        /// <returns></returns>
        public HariStyleList GetT_SalinHairStylistByCell_Server(string cell, string S_UID = null)
        {
            if (string.IsNullOrEmpty(cell))
            {
                return null;
            }
            IQueryable<t_salinhairstylist> model = null;
            if (S_UID != null)
            {
                model = _db.t_salinhairstylist.Where(c => c.shs_cell == cell);
            }
            else
            {
                model = _db.t_salinhairstylist.Where(c => c.shs_cell == cell && c.shs_status == 1);
            }

            if (model != null)
            {
                return model.Select(a => new HariStyleList()
                         {
                             Cell = a.shs_cell,
                             Exp = a.shs_exp,
                             Hairprice = a.shs_hairprice,
                             Huid = a.shs_h_uid,
                             Logo = a.shs_logo,
                             Name = a.shs_name,
                             Uid = a.shs_s_uid,
                             status = a.shs_status,
                             Flag = (_db.to_employee.Where(c => c.oe_b_uid == a.shs_h_uid).FirstOrDefault() == null ? false : true)
                         }).FirstOrDefault();
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 获取沙龙绑定发型师所发布的图片
        /// 排序按照人气+普通的形式 只去前100张
        /// </summary>
        /// <param name="shs_h_uid">发型师H_UID</param>
        /// <returns></returns>
        public O_T_Blog GetBlogBySalinHairStylist(string parm)
        {
            string aescell = EncryptionStrings.EncryptResult(parm); //加密参数
            string str = WebRequestGetOrPost.Post("/api/weisalon/gethairweibolist", aescell).TrimEnd('"').TrimStart('"');
            str = EncryptionStrings.DecryptParameter(str);//解密参数
            O_T_Blog blog = JsonConvert.DeserializeObject<O_T_Blog>(str);
            return blog;
        }



        public List<HariStyleList> GetT_SalinHairStylistByLamda(System.Linq.Expressions.Expression<Func<t_salinhairstylist, bool>> whereLamda)
        {
            IQueryable<t_salinhairstylist> list = _db.t_salinhairstylist.Where(whereLamda);
            List<HariStyleList> hairlist = new List<HariStyleList>();
            if (list.Count() > 0)
            {
                hairlist = list.Select(a => new HariStyleList
                {
                    Cell = a.shs_cell,
                    Exp = a.shs_exp,
                    Hairprice = a.shs_hairprice,
                    Huid = a.shs_h_uid,
                    Logo = a.shs_logo,
                    Name = a.shs_name,
                    Uid = a.shs_s_uid,
                    status = a.shs_status
                }).ToList();
            }

            return hairlist;
        }

        #region IHairListService 成员


        public PageOfItems<HariStyleList> GetHairStyList(Guid uid, int pageIndex, int pageSize, int type = 1)
        {
            Func<t_salinhairstylist, bool> func1 = a => a.shs_status == 1 && a.shs_s_uid == uid;
            if (type != 1)
            {
                func1 = a => a.shs_s_uid == uid;
            }
            var list = new PageOfItems<HariStyleList>()
            {
                PageNumber = pageIndex,
                PageSize = pageSize,
                TotalItemCount = _db.t_salinhairstylist.Count(func1)
            };

            var beginnum = (pageIndex - 1) * pageSize;

            var info = _db.t_salinhairstylist
                .Where(a => a.shs_status == 1 && a.shs_s_uid == uid)
                .OrderBy(a => a.shs_sort)
                .Select(a => new HariStyleList
                {
                    Cell = a.shs_cell,
                    Exp = a.shs_exp,
                    Hairprice = a.shs_hairprice,
                    Huid = a.shs_h_uid,
                    Logo = a.shs_logo,
                    Name = a.shs_name,
                    Uid = a.shs_s_uid,
                    status = a.shs_status,
                    Flag = (_db.to_employee.Where(c => c.oe_b_uid == a.shs_h_uid).FirstOrDefault() == null ? false : true)
                })
                .Skip(beginnum)
                .Take(pageSize)
                ;


            list.AddRange(info);
            return list;

        }

        #endregion
    }
}
