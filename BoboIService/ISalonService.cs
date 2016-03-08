using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BoboModel;
using System.Data;
using Boboframework;
using Boboframework.Data;
using System.Linq.Expressions;
using System.Collections;
using System.Data.Entity;

namespace BoboIService
{
    #region ZWDong
    public partial interface ISalonService : IBaseService<t_salon>
    {
        /// <summary>
        /// 添加 沙龙（测试用）
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        int AddSalon(SalonSimple mod);

        /// <summary>
        /// 获取美发店信息(简单信息)
        /// </summary>
        /// <param name="uid">美发店UID</param>
        /// <returns></returns>
        SalonSimple GetSalonSimpleInfo(Guid uid);

        /// <summary>
        /// 登陆验证 并且获取美发店信息(简单信息)
        /// </summary>
        /// <param name="loginID">手机号</param>
        /// <param name="PWD">密码</param>
        /// <returns></returns>
        SalonSimple GetLoginInfo(string loginID, string PWD);

        /// <summary>
        /// 注册验证 判断手机号是否注册过
        /// </summary>
        /// <param name="loginID">手机号</param>
        /// <returns></returns>
        SalonSimple CHKInfo(string loginID);

        /// <summary>
        /// 添加 沙龙环境图等
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        int AddAllSalon(AllSalon mod);

        /// <summary>
        /// 修改沙龙环境图等
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="photo"></param>
        /// <param name="sta"></param>
        /// <returns>返回-1为密码错误</returns>
        int UpdateImageInfo(string uid, string photo, int sta);

        /// <summary>
        /// 修改logo
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="logo"></param>
        /// <returns></returns>
        int UpdateLogoInfo(string uid, string logo, string[] strs = null);

        /// <summary>
        /// 修改status
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        int UpdatestatusInfo(string uid, int status);

        /// <summary>
        /// 获取美发店信息(完整信息)
        /// </summary>
        /// <param name="uid">美发店UID</param>
        /// <param name="type">图片类型 1 营业执照 2 正门 3 环境照 4 发型价格  0全部</param>
        /// <returns></returns>
        List<AllSalon> GetSalonInfo(Guid uid, int type);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <param name="pwdnew"></param>
        /// <returns>返回-1为密码错误</returns>
        int UpdateSimpleInfo(string uid, string pwd, string pwdnew);


        /// <summary>
        /// 管理员账号 获取沙龙信息表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">一页X条</param>
        /// <returns></returns>
        PageOfItems<SalonSimple> GetSalonList(int pageIndex, int pageSize, string saleName = null, Func<IQueryable<t_salemanager>> func = null);


        /// <summary>
        /// 根据uid 获取沙龙资料
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        List<OneSalonInfo> GetSalonByUid(Guid uid);
    }

    public partial class SalonService : BaseService<t_salon>, ISalonService
    {
        private readonly mysqkbobohairEntitesContainer _db = DbContextManager.DefaultInstance;
        private readonly IErrorService _err = new ErrorService();



        /// <summary>
        /// 添加 沙龙
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public int AddSalon(SalonSimple mod)
        {
            int count = 0;
            var model = new t_salon()
            {
                s_cell = mod.Cell,
                s_date = mod.Date,
                s_email = mod.Email,
                s_kind = mod.Kind,
                s_logo = mod.Logo,
                s_nickname = mod.Nickname,
                s_pwd = mod.Pwd,
                s_status = mod.Status,
                s_uid = mod.Uid,
                s_address = mod.Address,
                s_linkname = mod.LinkName,
                s_summary = mod.Summary,
                s_lat = mod.Lat,
                s_lon = mod.Lon,
                s_price = mod.Price,
                s_wxname = mod.Wxname,
                s_businessdate = mod.Businessdate,
                s_recommend = mod.Recommend
            };
            try
            {
                _db.t_salon.Add(model);
                return _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _db.t_salon.Remove(model);
                _err.AddError("AddSalon", ex.Message);
            }
            return count;
        }

        /// <summary>
        /// 从新提交 沙龙
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public int UpdateRegSalon(SalonSimple mod)
        {
            var model = _db.t_salon.Find(mod.Uid);
            int count = 0;
            try
            {
                model.s_cell = mod.Cell;
                model.s_date = mod.Date;
                model.s_email = mod.Email;
                model.s_kind = mod.Kind;
                model.s_logo = mod.Logo;
                model.s_nickname = mod.Nickname;
                model.s_pwd = mod.Pwd;
                model.s_status = mod.Status;
                model.s_address = mod.Address;
                model.s_linkname = mod.LinkName;
                model.s_summary = mod.Summary;
                model.s_lat = mod.Lat;
                model.s_lon = mod.Lon;
                model.s_price = mod.Price;
                model.s_wxname = mod.Wxname;
                model.s_businessdate = mod.Businessdate;
                _db.Entry(model).State = EntityState.Modified;
                count = _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _db.Entry(model).Reload();
                _err.AddError("AddSalon", ex.Message);
            }
            return count;
        }

        /// <summary>
        /// 添加 沙龙环境图等
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public int AddAllSalon(AllSalon mod)
        {

            var model = new t_salonimageinfo()
            {
                si_kind = mod.SiKind,
                si_photo = mod.Photo,
                si_s_uid = mod.Suid,
                si_status = 1
            };
            int count = 0;
            try
            {
                _db.t_salonimageinfo.Add(model);
                return _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _db.t_salonimageinfo.Remove(model);
                _err.AddError("AddSalon", ex.Message);
            }
            return count;
        }


        /// <summary>
        /// 修改沙龙环境图等
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="photo"></param>
        /// <param name="sta"></param>
        /// <returns>返回-1为密码错误</returns>
        public int UpdateImageInfo(string uid, string photo, int sta)
        {
            int count = 0;
            var model = _db.t_salonimageinfo.Find(int.Parse(uid));
            try
            {
                model.si_photo = photo;
                model.si_status = sta;
                _db.Entry(model).State = EntityState.Modified;
                count = _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _db.Entry(model).Reload();
                _err.AddError("UpdateImageInfo", ex.Message);
            }
            return count;
        }

        /// <summary>
        /// 获取美发店信息(简单信息)
        /// </summary>
        /// <param name="uid">美发店UID</param>
        /// <returns></returns>
        public SalonSimple GetSalonSimpleInfo(Guid uid)
        {
            var info = new List<SalonSimple>();
            try
            {
                info = _db.t_salon.Where(a => a.s_uid == uid && a.s_status != 0)
                    .Select(a => new SalonSimple()
                    {
                        Nickname = a.s_nickname,
                        Date = a.s_date,
                        Cell = a.s_cell,
                        Email = a.s_email,
                        Kind = a.s_kind,
                        Logo = a.s_logo,
                        Status = a.s_status,
                        Address = a.s_address,
                        Summary = a.s_summary,
                        LinkName = a.s_linkname,
                        Uid = a.s_uid,
                        Lat = a.s_lat,
                        Lon = a.s_lon,
                        Price = a.s_price,
                        Opendate = a.s_opendate,
                        Businessdate = a.s_businessdate,
                        Wxname = a.s_wxname
                    }).ToList();


            }
            catch (Exception ex)
            {
                _err.AddError("GetSalonSimpleInfo", ex.Message);
            }

            return info.FirstOrDefault();
        }

        /// <summary>
        /// 登陆验证 并且获取美发店信息(简单信息)
        /// </summary>
        /// <param name="loginID">手机号</param>
        /// <param name="PWD">密码</param>
        /// <returns></returns>
        public SalonSimple GetLoginInfo(string loginID, string PWD)
        {

            var info = new List<SalonSimple>();
            try
            {
                info = _db.t_salon.Where(a => a.s_email == loginID && a.s_pwd == PWD.ToLower() && a.s_status != 0)
                    .Select(a => new SalonSimple()
                    {
                        Nickname = a.s_nickname,
                        Date = a.s_date,
                        Cell = a.s_cell,
                        Email = a.s_email,
                        Kind = a.s_kind,
                        Logo = a.s_logo,
                        Status = a.s_status,
                        Address = a.s_address,
                        Summary = a.s_summary,
                        LinkName = a.s_linkname,
                        Businessdate = a.s_businessdate,
                        Uid = a.s_uid,
                        Lat = a.s_lat,
                        Lon = a.s_lon,
                        Opendate = a.s_opendate,
                        Price = a.s_price,
                        Wxname = a.s_wxname
                    }).ToList();
            }
            catch (Exception ex)
            {
                _err.AddError("GetSalonSimpleInfo", ex.Message);
            }

            return info.FirstOrDefault();
        }


        /// <summary>
        /// 注册验证 判断手机号是否注册过
        /// </summary>
        /// <param name="Cell">手机号</param>
        /// <param name="loginID">登陆帐号</param>
        /// <returns></returns>
        public SalonSimple CHKInfo(string loginID)
        {
            var info = new List<SalonSimple>();
            try
            {
                info = _db.t_salon.Where(a => a.s_email == loginID)
                    .Select(a => new SalonSimple()
                    {
                        Email = a.s_email,
                        Uid = a.s_uid,
                        Cell = a.s_cell
                    }).ToList();
            }
            catch (Exception ex)
            {
                _err.AddError("CHKInfo", ex.Message);
            }

            return info.FirstOrDefault();
        }

        /// <summary>
        /// 修改status
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UpdatestatusInfo(string uid, int status)
        {
            int count = 0;
            var model = _db.t_salon.Find(new Guid(uid));
            try
            {
                model.s_status = status;
                if (status == 4 || status == 5)
                {
                    model.s_status = 4;
                    model.s_opendate = status == 4 ? DateTime.Now.AddYears(1) : DateTime.Now.AddDays(7);
                    if (status == 4)
                    {
                        model.s_kind = 1;
                    }
                }
                _db.Entry(model).State = EntityState.Modified;
                count = _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _db.Entry(model).Reload();
                _err.AddError("UpdatestatusInfo", ex.Message);
            }
            return count;
        }

        /// <summary>
        /// 修改logo
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="logo"></param>
        /// <returns></returns>
        public int UpdateLogoInfo(string uid, string logo, string[] strs = null)
        {
            int count = 0;
            var model = _db.t_salon.Find(uid);
            try
            {
                model.s_logo = logo;
                if (strs != null)
                {
                    model.s_cell = strs[0];
                    model.s_email = strs[1];
                    model.s_linkname = strs[2];
                    model.s_nickname = strs[3];
                }
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
        /// 修改密码
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="pwd"></param>
        /// <param name="pwdnew"></param>
        /// <returns>返回-1为密码错误</returns>
        public int UpdateSimpleInfo(string uid, string pwd, string pwdnew)
        {
            int count = 0;
            var model = _db.t_salon.Find(uid);
            try
            {
                if (model.s_pwd != pwd)
                {
                    return -1;
                }
                model.s_pwd = pwdnew;
                _db.Entry(model).State = EntityState.Modified;
                count = _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _db.Entry(model).Reload();
                _err.AddError("UpdateSimpleInfo", ex.Message);
            }
            return count;
        }

        /// <summary>
        /// 获取美发店信息(完整信息)
        /// </summary>
        /// <param name="uid">美发店UID</param>
        /// <param name="type">图片类型 1 营业执照 2 正门 3 环境照 4 发型价格  0全部</param>
        /// <returns></returns>
        public List<AllSalon> GetSalonInfo(Guid uid, int type)
        {

            var list = new List<AllSalon>();
            try
            {
                if (type == 0)
                {
                    list = (from a in _db.t_salon
                            join b in _db.t_salonimageinfo
                                on a.s_uid equals b.si_s_uid
                            where b.si_status != 0 && a.s_uid == uid
                            select new AllSalon
                            {
                                Nickname = a.s_nickname,
                                Address = a.s_address,
                                Summary = a.s_summary,
                                LinkName = a.s_linkname,
                                Date = a.s_date,
                                Cell = a.s_cell,
                                Businessdate = a.s_businessdate,
                                Email = a.s_email,
                                Kind = a.s_kind,
                                Logo = a.s_logo,
                                Status = a.s_status,
                                Uid = a.s_uid,
                                Opendate = a.s_opendate,
                                Id = b.si_id,
                                Photo = b.si_photo,
                                SiKind = b.si_kind,
                                Lat = a.s_lat,
                                Lon = a.s_lon,
                                Price = a.s_price,
                                Wxname = a.s_wxname
                            }).ToList();

                }
                else
                {
                    list = (from a in _db.t_salon
                            join b in _db.t_salonimageinfo
                                on a.s_uid equals b.si_s_uid
                            where b.si_status != 0 && b.si_kind == type && a.s_uid == uid
                            select new AllSalon
                            {
                                Nickname = a.s_nickname,
                                Address = a.s_address,
                                Summary = a.s_summary,
                                LinkName = a.s_linkname,
                                Date = a.s_date,
                                Cell = a.s_cell,
                                Email = a.s_email,
                                Kind = a.s_kind,
                                Logo = a.s_logo,
                                Opendate = a.s_opendate,
                                Businessdate = a.s_businessdate,
                                Status = a.s_status,
                                Uid = a.s_uid,
                                Id = b.si_id,
                                Photo = b.si_photo,
                                SiKind = b.si_kind,
                                Lat = a.s_lat,
                                Lon = a.s_lon,
                                Price = a.s_price,
                                Wxname = a.s_wxname
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                _err.AddError("GetSalonInfo", ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 管理员账号 获取沙龙信息表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">一页X条</param>
        /// <returns></returns>
        public PageOfItems<SalonSimple> GetSalonList(int pageIndex, int pageSize, string saleName = null, Func<IQueryable<t_salemanager>> func = null)
        {
            IQueryable<SalonSimple> salons = null;
            IQueryable<t_salon> info = null;
            var list = new PageOfItems<SalonSimple>()
            {
                PageNumber = pageIndex,
                PageSize = pageSize,
                TotalItemCount = (saleName == null || saleName == "") ? _db.t_salon.Count() : _db.t_salemanager.Where(c => c.sm_name.Contains(saleName)).Count()
            };
            var beginNum = (pageIndex - 1) * pageSize;

            info = saleName == null ? _db.t_salon.OrderByDescending(a => a.s_date).Skip(beginNum).Take(pageSize) : _db.t_salon;

            if (func != null)
            {

                salons = from a in info
                         join b in func() on a.s_uid equals b.sm_s_uid
                         into pro
                         from x in pro.DefaultIfEmpty()
                         where ((saleName == null || saleName == "") ? true : x.sm_name.Contains(saleName))
                         select new SalonSimple
                         {
                             Uid = a.s_uid,
                             Cell = a.s_cell,
                             Nickname = a.s_nickname,
                             Logo = a.s_logo,
                             Opendate = a.s_opendate,
                             Status = a.s_status,
                             Businessdate = a.s_businessdate,
                             LinkName = a.s_linkname,
                             LastTime = x.sm_editdate,
                             Sale_Id = x.sm_id,
                             SaleName = x.sm_name,
                             Recommend = a.s_recommend
                         };
            }
            else
            {
                salons = info.Select(a => new SalonSimple
                {
                    Uid = a.s_uid,
                    Cell = a.s_cell,
                    Nickname = a.s_nickname,
                    Logo = a.s_logo,
                    Opendate = a.s_opendate,
                    Status = a.s_status,
                    Businessdate = a.s_businessdate,
                    LinkName = a.s_linkname
                });
            }
            list.AddRange(saleName == null ? salons.ToList() : salons.OrderByDescending(c => c.LastTime).Skip(beginNum).Take(pageSize).ToList());
            return list;
        }




        public List<OneSalonInfo> GetSalonByUid(Guid uid)
        {
            var list = new List<OneSalonInfo>();
            //            var info = _db.t_salon
            //                .Where(a => a.s_uid == uid)
            //                .Select(a => new SalonSimple()
            //                {
            //                    Uid = a.s_uid,
            //                    Cell = a.s_cell,
            //                    Nickname = a.s_nickname,
            //                    Logo = a.s_logo,
            //                    Status = a.s_status,
            //                    LinkName = a.s_linkname,
            //                    Email = a.s_email,
            //                    Date = a.s_date,
            //                    Kind = a.s_kind,
            //                    Address = a.s_address,
            //                    Summary = a.s_summary,
            //                    Wxname = a.s_wxname,
            //                })
            //                .ToList();
            var info = (from a in _db.t_salon
                        join b in _db.t_salonimageinfo
                            on a.s_uid equals b.si_s_uid
                        where b.si_status == 1 && a.s_uid == uid
                        select new OneSalonInfo()
                        {
                            Uid = a.s_uid,
                            Cell = a.s_cell,
                            Nickname = a.s_nickname,
                            Logo = a.s_logo,
                            Status = a.s_status,
                            LinkName = a.s_linkname,
                            Email = a.s_email,
                            Date = a.s_date,
                            Kind = a.s_kind,
                            Address = a.s_address,
                            Summary = a.s_summary,
                            Wxname = a.s_wxname,
                            Photo = b.si_photo,
                            SiKind = b.si_kind
                        }).ToList();


            list.AddRange(info);
            return list;
        }

    }
    #endregion

    #region JSLang
    public partial interface ISalonService : IBaseService<t_salon>
    {
        /// <summary>
        /// 实时统计，月度汇总
        /// </summary>
        /// <param name="UID">沙龙ID</param>
        /// <param name="sbwhere">实时统计的条件，月度汇总的条件</param>
        /// <returns></returns>
        IQueryable<StatisticsMonthSum> RealTimeStatisticsMonthSummarizing(string UID, StringBuilder sbwhere);
    }
    public partial class SalonService : BaseService<t_salon>, ISalonService
    {
        /// <summary>
        /// 实时统计，月度汇总
        /// </summary>
        /// <param name="UID">沙龙ID</param>
        /// <param name="sbwhere">实时统计的条件，月度汇总的条件</param>
        /// <returns></returns>
        public IQueryable<StatisticsMonthSum> RealTimeStatisticsMonthSummarizing(string UID, StringBuilder sbwhere)
        {
            StringBuilder Sb_detail = new StringBuilder();
            #region MyRegion

            //            Sb_detail.AppendFormat(@"select shs_name as HairName,shs_h_uid as HairUid, max(shs_logo) as HairLog,COUNT(w1) AS Month_HairCount,COUNT(w2) as Today_HairCount,COUNT(w3)  as Week_HairCount,sum(p1) as Month_Price,sum(p2) as Today_Price,sum(p3) as Week_Price from (
            //select shs_name,shs_logo,wo_date,wo_orderstatus,shs_h_uid,
            //case  when  ({0}) then COUNT(wo_orderstatus) end w1 ,
            //case  when  ({1}) then sum(wo_price) end p1 ,
            //case  when (DATEDIFF(DAY,wo_date,getdate())=0) then  COUNT(wo_orderstatus) end w2,
            //case  when (DATEDIFF(DAY,wo_date,getdate())=0) then  sum(wo_price) end p2,
            //case  when  (DATEDIFF(WEEK,wo_date,getdate())=0) then  COUNT(wo_orderstatus) end w3,
            //case  when  (DATEDIFF(WEEK,wo_date,getdate())=0) then  sum(wo_price) end p3
            //from
            //(select * from t_salinhairstylist
            //where shs_s_uid='{2}' and shs_status=1) b
            //left join
            //(select * from Wx_Order
            //where wo_orderstatus=2 ) a
            //on a.wo_h_uid=b.shs_h_uid
            //group by shs_name,shs_logo,wo_orderstatus,wo_date,shs_h_uid) c
            //group by shs_name,shs_h_uid", sbwhere.ToString(), sbwhere.ToString(), UID); 
            #endregion

            Sb_detail.AppendFormat(@"select shs_name as HairName,shs_h_uid as HairUid, max(shs_logo) as HairLog,COUNT(w1) AS Month_HairCount,COUNT(w2) as Today_HairCount,COUNT(w3)  as Week_HairCount,sum(p1) as Month_Price,sum(p2) as Today_Price,sum(p3) as Week_Price from (
            select shs_name,shs_logo,wo_date,wo_orderstatus,shs_h_uid,
            case  when  ({0}) then COUNT(wo_orderstatus) end w1 ,
            case  when  ({1}) then sum(wo_price) end p1 ,
            case  when (DATEDIFF(DAY,wo_date,getdate())=0) then  COUNT(wo_orderstatus) end w2,
            case  when (DATEDIFF(DAY,wo_date,getdate())=0) then  sum(wo_price) end p2,
            case  when  (DATEDIFF(WEEK,wo_date,getdate())=0) then  COUNT(wo_orderstatus) end w3,
            case  when  (DATEDIFF(WEEK,wo_date,getdate())=0) then  sum(wo_price) end p3
            from
            (select * from t_salinhairstylist
            where shs_s_uid='{2}' and shs_status=1) b
            left join
            (select * from Wx_Order
            where wo_orderstatus=2 and wo_s_uid='{3}') a
            on a.wo_h_uid=b.shs_h_uid
            group by shs_name,shs_logo,wo_orderstatus,wo_date,shs_h_uid) c
            group by shs_name,shs_h_uid", sbwhere.ToString(), sbwhere.ToString(), UID, UID);

            var staticsmothsum = _db.Database.SqlQuery<StatisticsMonthSum>(Sb_detail.ToString()).AsQueryable();
            return staticsmothsum;
        }
    }
    #endregion
}
