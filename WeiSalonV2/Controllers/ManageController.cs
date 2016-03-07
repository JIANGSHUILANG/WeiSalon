using AutoMapper;
using BoboCommon;
using Boboframework;
using Boboframework.Data;
using BoboIService;
using BoboModel;
using Newtonsoft.Json;
using Qiniu.IO;
using Qiniu.RS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WeiSalonV2.Models;

namespace WeiSalonV2.Controllers
{
    public class ManageController : Controller
    {
        //
        // GET: /Manage/
        readonly string _baseSalonurl = "http://" + System.Web.HttpContext.Current.Request.Url.Authority + "/SalonWeb/index.html#salon/"; //"http://192.168.6.6:84/";
        readonly string _baseFaxinshiurl = "http://" + System.Web.HttpContext.Current.Request.Url.Authority + "/SalonWeb/index.html#allfxs/";
        readonly string _baseAllworksurl = "http://" + System.Web.HttpContext.Current.Request.Url.Authority + "/SalonWeb/index.html#allworks/";
        readonly string _adminCell = ConfigurationManager.AppSettings["AdminCell"];
        IT_SysInformationService sysfinfoservice = new T_SysInformationService();
        ISalonService salonservice = new SalonService();
        IWx_OrderService wxorderservice = new Wx_OrderService();
        ICustomerManageService customerservice = new CustomerManageService();
        IOrderService orderservice = new OrderService();
        IWx_MemberService wx_memberservice = new Wx_MemberService();
        IWx_BindHairService wx_bindhairservice = new Wx_BindHairService();
        IT_HairOffService t_hairoffservice = new T_HairOffService();
        IHairListService hair = new HairListService();
        IT_NoticeService noticeservice = new T_NoticeService();
        ISaleManagerService salemanagerservice = new SaleManagerService();
        ISalonImageService salonimageservice = new SalonImageService();
        public string ValAdmin()
        {
            if (string.IsNullOrEmpty(manage.UID.ToString()))
            {
                return "false";
            }
            if (_adminCell.Contains(manage.User.Cell))
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }

        public ManageController()
        {
            ViewBag.admim = ValAdmin();
        }


        //
        // GET: /manage/
        /// <summary>
        /// 后台管理首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(manage.UID.ToString()))
            {
                Response.Redirect("/home/index");
                return View();
            }
            Response.Redirect("/Manage/SalonSystemMessages");
            return View();
        }


        #region 修改密码

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdatePwd()
        {
            return View();
        }

        /// <summary>
        /// 修改密码
        /// </summary>   
        /// <param name="PWD"></param>
        /// <param name="PWDnew"></param>
        /// <returns></returns>
        public string Pwd(string PWD, string PWDnew)
        {
            if (string.IsNullOrEmpty(manage.UID.ToString()))
            {
                return "-1";
            }
            if (string.IsNullOrEmpty(PWD))
            {
                return "密码不能为空";
            }
            if (string.IsNullOrEmpty(PWDnew))
            {
                return "密码不能为空";
            }
            //解密参数值
            PWDnew = WeiSalonV2.Models.AESHelper.DecryptString(PWDnew);
            PWD = WeiSalonV2.Models.AESHelper.DecryptString(PWD);
            //去掉参数中的转移字符
            PWDnew = new string((from c in PWDnew.ToCharArray() where !char.IsControl(c) select c).ToArray());
            PWD = new string((from c in PWD.ToCharArray() where !char.IsControl(c) select c).ToArray());
            if (string.IsNullOrEmpty(PWD))
            {
                return "密码不能为空";
            }
            if (string.IsNullOrEmpty(PWDnew))
            {
                return "密码不能为空";
            }
            SalonService salon = new SalonService();
            int cou = salon.UpdateSimpleInfo(manage.UID.ToString(), MD5.MDString(PWD), MD5.MDString(PWDnew));
            if (cou == -1)
            {
                return "密码错误";
            }
            return cou.ToString();
        }


        #endregion

        #region 获取验证码

        public ActionResult CheckImg()
        {
            CodeVaildate codeimage = new CodeVaildate();
            string code = codeimage.CreateVerifyCode();
            Session["code"] = code;
            byte[] butter = codeimage.CreateImageOnPage(code);
            return File(butter, "image/jpeg");
        }

        #endregion

        #region  获取7牛 token

        /// <summary>
        /// 获取7牛 token
        /// </summary>
        /// <returns></returns>
        private string GetToken()
        {
            Qiniu.Conf.Config.ACCESS_KEY = "svBSIqScEwfWhLW5mJf62-t7pFLE-qm5Ofe8-YMP";
            Qiniu.Conf.Config.SECRET_KEY = "AnwF8pDDTqIhSRGU3tC2MsjCldHqgsS_B6FG5DYc";
            var policy = new PutPolicy("bobosquad", 3600);
            return policy.Token();
        }
        #endregion

        #region MyRegion
        /// <summary>
        /// 发型师设置
        /// </summary>
        /// <returns></returns>
        public ActionResult hairlistmanager(string ind, string id)
        {
            if (string.IsNullOrEmpty(ind))
            {
                ind = "1";
            }
            int pagesize = 10;
            int pageindex = 1;
            if (!string.IsNullOrEmpty(ind))
            {
                try
                {
                    pageindex = int.Parse(ind);
                }
                catch
                { }
            }
            int rowcount = 0;
            int pagecount = 0;
            string content = null;
            HairListService hair = new HairListService();
            List<HariStyleList> lis = new List<HariStyleList>();
            try
            {
                lis = hair.GetHariStyleListList(manage.UID, pageindex, pagesize, out rowcount, out pagecount);
                content = "<tr>";
                for (int i = 0; i < lis.Count; i++)
                {
                    string bbuid = lis[i].Huid.ToString();
                    string firststyle = i == 0 ? "style='margin-left: 15px;'" : "";
                    string logo = lis[i].Logo;
                    content += "<td class='table_cell product'><div class='haird'" +
                        firststyle + "><div class='box'><img src='" +
                        logo + "' class='js_msgSenderAvatar'></div><div class='Description'>" +
                       lis[i].Name + "</div><div class='Description'><span class='btn btn_input btn_primary' style='min-width: 70px;'><button type='button'style='padding: 0;'onclick=Cancel('" +
                       lis[i].Huid + "')>取消关联</button></span> <a class='btn btn_input btn_primary' style='min-width: 70px;' href='/manage/hairlistmanager/" + ind + "/" +
                        lis[i].Huid + "' class='avatar'>个人信息</a></div></div></td>";
                    if (i % 3 == 0 && i != 0 && i != lis.Count - 1)
                    {
                        content += "</tr><tr>";
                    }
                }
                content += "</tr>";
            }
            catch
            { }
            string pagecontent = null;
            string first = pageindex == 1 ? "#" : "";
            string prv = pageindex == 1 ? "1#" : (pageindex - 1).ToString();
            string next = pageindex == pagecount ? pagecount + "#" : (pageindex + 1).ToString();
            string end = pageindex == pagecount ? "#" : "";
            if (pagecount > 1)
            {
                pagecontent = "<span class='page_nav_area'><a href='/manage/hairlistmanager/" + first + "'class='btn page_first'>首页</a> <a href='/manage/hairlistmanager/" + prv + "'class='btn page_prev'><i class='arrow'></i></a> <span class='page_num'><label>" + pageindex + "</label><span class='num_gap'> /</span> <label>" + pagecount + "</label></span> <a href='/manage/hairlistmanager/" + next + "'class='btn page_next'><i class='arrow'></i></a> <a href='/manage/hairlistmanager/" + pagecount.ToString() + end + "'class='btn page_last'>末页</a> </span> ";
            }
            ViewBag.img = "/Images/bphoto.png";
            if (!string.IsNullOrEmpty(id))
            {
                HariStyleList mo = new HariStyleList();
                mo = hair.GetHariStyleListInfo(new Guid(id));
                ViewBag.uid = id;
                ViewBag.cell = mo.Cell;
                ViewBag.Exp = mo.Exp;
                ViewBag.price = mo.Hairprice;
                ViewBag.Name = mo.Name;
                ViewBag.img = mo.Logo;
            }
            ViewBag.content = content;
            ViewBag.id = id;
            ViewBag.pagecontent = pagecontent;
            ViewBag.Token = GetToken();
            ViewBag.slname = manage.User.Name;
            return View();
        }

        #endregion


        /// <summary>
        /// 获取微信二维码(临时二维码 新加接口)
        /// </summary>
        /// <returns></returns>                    
        private string Getticket(string huid)
        {
            if (string.IsNullOrEmpty(manage.UID.ToString()))
            {
                return null;
            }
            ///API地址
            string url = "api/redbobo/GetTempQrCode";
            ///API参数
            string paramstr = "uid=" + huid;
            //获取返回值(Json)
            string responseBody0 = WebRequestGetOrPost.GetResponseResult(url, paramstr);
            return responseBody0;
        }

        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        private string TwoCode_(string UID)
        {
            if (string.IsNullOrEmpty(manage.UID.ToString()))
            {
                return null;
            }
            string code_return = string.Empty;
            string temp = Getticket(UID);
            TowCode code = JsonConvert.DeserializeObject<TowCode>(temp);
            if (code.status == 1)
            {
                code_return = code.info.url;
            }
            return code_return;
        }




        #region 沙龙基本信息展示以及修改


        public ActionResult BasicInfo()
        {
            string str = Request.RawUrl;
            if (string.IsNullOrEmpty(manage.UID.ToString()))
            {
                Response.Redirect("/home/index");
                return View();
            }
            if (Session["MsgSaveBasicInfo"] != null)
            {
                ViewData["Msg"] = Session["MsgSaveBasicInfo"];
                Session.Remove("MsgSaveBasicInfo");
            }
            else
                ViewData.Remove("Msg");
            ViewBag.titleshow = "沙龙基本信息";
            ViewBag.uid = _baseSalonurl + manage.UID;
            ViewBag.code = TwoCode_(manage.UID.ToString());
            //获取沙龙信息
            var salon_Model = salonservice.LoadEntities(c => c.s_uid == manage.UID && c.s_status == 4).FirstOrDefault();
            var salonimagemodel = salonimageservice.LoadEntities(c => c.si_s_uid == manage.UID && c.si_status == 1 && c.si_kind == 5).FirstOrDefault();
            if (salonimagemodel == null)
            {
                ViewBag.imagehairstatus = 0;
            }
            else
            {
                ViewBag.imagehairstatus = 1;
            }
            var salon = new AllSalon()
            {
                Address = salon_Model.s_address,
                Cell = salon_Model.s_cell,
                Suid = salon_Model.s_uid,
                Status = salon_Model.s_status,
                SiKind = salon_Model.s_kind,
                Price = salon_Model.s_price,
                Nickname = salon_Model.s_nickname,
                Email = salon_Model.s_email,
                Pwd = salon_Model.s_pwd,
                LinkName = salon_Model.s_linkname,
                Summary = salon_Model.s_summary,
                Wxname = salon_Model.s_wxname,
                Businessdate = salon_Model.s_businessdate,
                ImageUrl = salonimagemodel == null ? "" : salonimagemodel.si_photo
            };


            ViewBag.Token = GetToken();
            return View(salon);
        }

        /// <summary>
        /// 移除发型师背景图
        /// </summary>
        [HttpPost]
        public int RemoveHairBackImage()
        {
            bool flag = false;
            var salonimagemodel = salonimageservice.LoadEntities(c => c.si_s_uid == manage.UID && c.si_status == 1 && c.si_kind == 5).FirstOrDefault();
            if (salonimagemodel != null)
            {
                salonimagemodel.si_status = 0;
                try
                {
                    flag = salonimageservice.UpdateEntity(salonimagemodel);
                }
                catch { }
            }
            return flag ? 0 : 1;
        }


        /// <summary>
        /// 基本信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveBasicInfo()
        {
            if (string.IsNullOrEmpty(manage.UID.ToString()))
            {
                Response.Redirect("/home/index");
                return View();
            }
            #region Request Form

            string s_summary = Request.Form["summary"];
            string name = Request.Form["name"];
            string low_price = Request.Form["low_price"];
            string hight_price = Request.Form["hight_price"];
            string cell = Request.Form["cell"];
            string add = Request.Form["add"];
            string lng = Request.Form["lng"];
            string lat = Request.Form["lat"];
            string wxname = Request.Form["wxname"];
            string start_time = Request.Form["start_time"];
            string end_time = Request.Form["end_time"];
            #endregion


            t_salon salon = salonservice.LoadEntities(c => c.s_uid == manage.UID).FirstOrDefault();
            if (salon != null)
            {
                salon.s_summary = s_summary;
                salon.s_nickname = name;
                salon.s_price = low_price + "-" + hight_price;
                salon.s_cell = cell;
                salon.s_address = add;
                salon.s_lon = lng == null ? "0" : lng.ToString();
                salon.s_lat = lat == null ? "0" : lat.ToString();
                salon.s_wxname = wxname == null ? "" : wxname;
                salon.s_businessdate = start_time + "~" + end_time;
            }
            bool Flag = salonservice.UpdateEntity(salon);
            if (Flag)
                Session["MsgSaveBasicInfo"] = "修改成功";
            else
                Session["MsgSaveBasicInfo"] = "修改失败";
            return RedirectToAction("BasicInfo");
            #region MyRegion


            //string logo = Request.Form["logo"];
            //string idimgmd = Request.Form["idimgmd"];
            //string imgmd = Request.Form["imgmd"];
            //string id1 = Request.Form["id1"];
            //string pohot1 = Request.Form["pohot1"];
            //string id2 = Request.Form["id2"];
            //string pohot2 = Request.Form["pohot2"];
            //string id3 = Request.Form["id3"];
            //string pohot3 = Request.Form["pohot3"];
            //string name = Request.Form["name"];
            //string linkname = Request.Form["linkname"];
            //string cell = Request.Form["cell"];
            //string email = Request.Form["email"];
            //string[] strs = { cell, email, linkname, name };
            //if (string.IsNullOrEmpty(manage.UID))
            //{
            //    return "-1";
            //}
            //int cou = 0;
            //SalonService salon = new SalonService();
            //cou += salon.UpdateLogoInfo(manage.UID, logo, strs);
            //if (string.IsNullOrEmpty(idimgmd))
            //{
            //    AllSalon alls = new AllSalon();
            //    alls.SiKind = 2;
            //    alls.Suid = manage.UID;
            //    alls.Sistatus = 1;
            //    alls.Photo = imgmd;
            //    salon.AddAllSalon(alls);
            //}
            //else
            //{
            //    cou += salon.UpdateImageInfo(idimgmd, imgmd, 1);
            //}
            //cou += salon.UpdateImageInfo(id1, pohot1, 1);
            //cou += salon.UpdateImageInfo(id2, pohot2, 1);
            //cou += salon.UpdateImageInfo(id3, pohot3, 1);
            //cou.ToString() 
            #endregion

        }

        public JsonResult UpdatePassword(string oldpassword, string password)
        {
            string Msg = "请勿输入空值";
            if (!string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(oldpassword))
            {
                oldpassword = MD5.MDString(oldpassword);
                var model = salonservice.LoadEntities(c => c.s_uid == manage.UID && c.s_pwd == oldpassword).FirstOrDefault();
                if (model == null)
                    Msg = "请输入正确的原始密码!";
                else
                {
                    model.s_pwd = MD5.MDString(password);
                    Msg = salonservice.UpdateEntity(model) ? "修改成功" : "修改失败";
                }
            }
            bool flag = Msg == "修改成功" ? true : false;
            return Json(new { flag = flag, Msg = Msg }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 发型师信息，发型师绑定，解除绑定

        /// <summary>
        /// 发型师信息
        /// </summary>
        /// <returns></returns>
        public ActionResult FaxinshiInfos(int pageIndex = 1, int pageSize = 4)
        {
            if (string.IsNullOrEmpty(manage.UID.ToString()))
            {
                Response.Redirect("/home/index");
                return View();
            }

            ViewBag.uid = _baseFaxinshiurl + manage.UID;
            ViewBag.salon = TwoCode_(manage.UID.ToString());

            int TotalCount = 0;
            if (Session["FaxinshiInfotemp"] != null)
            {
                var model = JsonConvert.DeserializeObject<HariStyleList>(Session["FaxinshiInfotemp"].ToString());
                Session.Remove("FaxinshiInfotemp");
                if (model != null)
                {
                    model.Msg = TwoCode_(model.Huid.ToString());
                    ViewData["Model"] = model;
                }
            }
            else
            {
                PageOfItems<HariStyleList> list = new PageOfItems<HariStyleList>();
                PageOfItems<HariStyleList> Cachelist = CacheManage.GetCache<PageOfItems<HariStyleList>>("Page_HairList_" + pageIndex + "_" + manage.UID);
                if (Cachelist != null)
                {
                    list = Cachelist;

                }
                else
                {
                    list = hair.GetHairStyList(manage.UID, pageIndex, pageSize);
                    for (int i = 1; i <= list.TotalPageCount; i++)
                    {
                        var templist = hair.GetHairStyList(manage.UID, i, pageSize);
                        CacheManage.SetCache<PageOfItems<HariStyleList>>("Page_HairList_" + i + "_" + manage.UID, templist);
                    }
                }

                TotalCount = list.TotalPageCount;

                foreach (var item in list)
                {
                    item.Msg = TwoCode_(item.Huid.ToString());
                }
                ViewData["Models"] = list;
            }
            ViewBag.FXSUid = manage.UID;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageCount = TotalCount;
            return View();
        }

        //搜索发型师
        [HttpPost]
        public ActionResult FaxinshiInfos(string txttel)
        {
            if (string.IsNullOrEmpty(manage.UID.ToString()))
            {
                Response.Redirect("/home/index");
                return View();
            }
            IEmployeeService employee = new EmployeeService();
            HariStyleList salonHair = new HariStyleList();
            if (!string.IsNullOrWhiteSpace(txttel))
            {
                O_T_Hairstylist oracle_hair = hair.GetT_hairstylistInfoByCell_Oracle(txttel);//根据手机号查找Oracle中t_hairstylist 发型师
                if (oracle_hair != null && oracle_hair.status == 1)
                {
                    salonHair.Cell = txttel;
                    salonHair.Exp = oracle_hair.info.mysign == null ? "" : oracle_hair.info.mysign.Length < 10 ? oracle_hair.info.mysign : oracle_hair.info.mysign.Substring(0, 10) + "......";
                    salonHair.Hairprice = oracle_hair.info.price2;
                    salonHair.Huid = new Guid(oracle_hair.info.uid); //发型师 id
                    salonHair.Logo = oracle_hair.info.logo;
                    salonHair.Name = oracle_hair.info.name;
                    var localbobo = hair.LoadEntities(c => c.shs_h_uid == new Guid(oracle_hair.info.uid.Trim())).FirstOrDefault();
                    if (localbobo != null)
                    {
                        salonHair.Uid = localbobo.shs_s_uid;
                        salonHair.status = localbobo.shs_status;
                    }
                    //false:没开通接单功能    true:已开通接单功能
                    salonHair.Flag = employee.LoadEntities(c => c.oe_b_uid == new Guid(oracle_hair.info.uid.Trim())).FirstOrDefault() == null ? false : true;
                }
                else
                {
                    salonHair.status = -1023; //没有匹配此手机号码的发型师
                }

            }
            Session["FaxinshiInfotemp"] = JsonConvert.SerializeObject(salonHair);
            return RedirectToAction("FaxinshiInfos", new { JsonsalonHair = Server.UrlEncode(JsonConvert.SerializeObject(salonHair)) });
        }

        /// <summary>
        /// 绑定,取消绑定发型师
        /// </summary>
        public ActionResult BindOrUnBindFaxinshiInfos(string cell, int insert_delete = 1, string imageurl = "")
        {
            string manage_uid = manage.UID.ToString();
            if (string.IsNullOrEmpty(manage_uid))
            {
                Response.Redirect("/home/index");
                return View();
            }
            //insert_delete=1 绑定   insert_delete=0 解除绑定
            HariStyleList salonHair = new HariStyleList();
            bool Flag = false;  //Flag=true绑定成功则绑定失败

            if (string.IsNullOrWhiteSpace(cell))
                return null;

            HariStyleList salon = hair.GetT_SalinHairStylistByCell_Server(cell, manage_uid); //根据手机号查找T_SalinHairStylist中是否存在绑定的发型师
            if (salon != null)
            {
                salon.status = insert_delete;
                salon.Uid = new Guid(manage_uid);
                salon.Logo = imageurl;
                HairOff_Entry(salon.Huid.ToString(), insert_delete);
                Flag = hair.UpdateHariStyleList(salon) > 0; //表示已经绑定沙龙
            }
            else
            {
                O_T_Hairstylist boboHair = hair.GetT_hairstylistInfoByCell_Oracle(cell); ////根据手机号查找Oracle中t_hairstylist 发型师
                if (boboHair != null && boboHair.status == 1)//表示此手机号绑定过Oracle中t_hairstylist 发型师
                {
                    salonHair.Cell = cell;
                    salonHair.Exp = boboHair.info.mysign == null ? "" : boboHair.info.mysign;
                    salonHair.Hairprice = boboHair.info.price2;
                    salonHair.Huid = new Guid(boboHair.info.uid);
                    salonHair.Logo = boboHair.info.logo;
                    salonHair.Name = boboHair.info.name;
                    salonHair.Uid = new Guid(manage_uid);
                    salonHair.status = 1;
                    salonHair.score = 0;
                    HairOff_Entry(salonHair.Huid.ToString(), 1);
                    Flag = hair.AddHariStyleList(salonHair) > 0;//绑定发型师  EF上下文需保证线程内唯一 
                }
            }



            #region 删除发型师列表缓存
            var templist = hair.GetHairStyList(new Guid(manage_uid), 1, 4, 2);
            for (int i = 1; i <= templist.TotalPageCount; i++)
            {
                CacheManage.RemoveCache("Page_HairList_" + i + "_" + manage_uid);
            }
            #endregion
            CacheManage.RemoveCache("SearchHotHair" + manage_uid);

            return RedirectToAction("FaxinshiInfos");
        }

        /// <summary>
        /// 发型师辞职与入职
        /// </summary>
        /// <param name="h_uid">发型师 ID</param>
        /// <param name="status">0：辞职  1：入职</param>

        private void HairOff_Entry(string h_uid, int status)
        {
            t_hairoff off = new t_hairoff();
            off.ho_h_uid = new Guid(h_uid);
            off.ho_status = status;
            off.ho_s_uid = manage.UID;
            off.ho_date = DateTime.Now;
            t_hairoffservice.AddEntity(off);
        }
        #endregion

        #region 离职发型师
        public ActionResult DimissionHairstylist(int pageIndex = 1)
        {
            int pageSize = 10;
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            var hairs = hair.LoadEntities(c => true); //所有发型师  && c.shs_status == 0
            var offhairs = t_hairoffservice.LoadEntities(c => c.ho_s_uid == manage.UID);
            var temp = (from c in hairs
                        join o in offhairs on c.shs_h_uid equals o.ho_h_uid
                        orderby c.shs_h_uid, o.ho_date
                        select new HariStyleList()
                        {
                            Huid = c.shs_h_uid,
                            Logo = c.shs_logo,
                            Name = c.shs_name,
                            ho_status = o.ho_status,
                            ho_date = o.ho_date
                        }).ToList();

            List<HariStyleList> templist = new List<HariStyleList>();
            for (int i = 0; i < temp.Count; i++)
            {
                HariStyleList hair_ = new HariStyleList();
                if (temp[i].ho_status == 0)
                {
                    hair_.Huid = temp[i].Huid;
                    hair_.Logo = temp[i].Logo;
                    hair_.Name = temp[i].Name;
                    hair_.enterdate = temp[i - 1].ho_date;
                    hair_.offdate = temp[i].ho_date;
                    templist.Add(hair_);
                }
            }
            var list = GetPage<HariStyleList>(templist.AsQueryable(), pageIndex, pageSize);
            return View(list);
        }
        #endregion

        #region 顾客管理
        //头像/名称 来源 关注的发型师 手机号 生日等备注信息 状态(关注/已取消 根据我的发型师 发型师绑定表 Wx_BindHair 字段 wb_status 0 解绑 1 绑定)
        public ActionResult CustomerManage(int pageIndex = 1)
        {

            int pageSize = 10;
            var templist = customerservice.GetCustomerInfo(manage.UID.ToString());

            var list = GetPage<CustomerManageInfo>(templist, pageIndex, pageSize);
            list.ForEach(c =>
            {
                c.Customer_Remark = string.IsNullOrWhiteSpace(c.Customer_Remark) ? (string.IsNullOrWhiteSpace(c.Customer_Cell) ? "暂无" : c.Customer_Cell) : c.Customer_Remark;
            });
            return View(list);
        }

        //修改备注信息
        public bool updateRemark(string remark, int customer_id = 0)
        {
            bool flag = false; //状态为0：修改失败  1：修改成功          
            if (customer_id != 0)
            {
                var model = customerservice.LoadEntities(c => c.cm_id == customer_id).FirstOrDefault();
                if (model != null)
                    model.cm_remarks1 = remark; flag = customerservice.UpdateEntity(model);
            }
            return flag;
        }
        #endregion

        #region 订单管理

        public ActionResult TodayOrder(int pageIndex = 1)
        {
            int pageSize = 10;
            IQueryable<OrderManageInfo> templist = orderservice.GetOrderManage(manage.UID.ToString(), 1);
            var list = GetPage<OrderManageInfo>(templist, pageIndex, pageSize);
            return View(list);
        }
        //店名 发型师名 顾客名字 预约时间 下单时间 金额 评论
        public ActionResult AllOrder(int pageIndex = 1)
        {
            int pageSize = 10;
            IQueryable<OrderManageInfo> templist = orderservice.GetOrderManage(manage.UID.ToString(), 2);
            var list = GetPage<OrderManageInfo>(templist, pageIndex, pageSize);
            return View(list);
        }

        private PageOfItems<T> GetPage<T>(IQueryable<T> templist, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            PageOfItems<T> list = orderservice.PageOfItemBySelfModel<T, DateTime>(pageIndex, pageSize, templist, c => true, null, false);
            int TotalCount = 0;
            if (list != null)
                TotalCount = list.TotalPageCount;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageCount = TotalCount;
            return list;
        }

        #endregion

        #region 作品集

        /// <summary>
        /// 作品集
        /// </summary>
        /// <returns></returns>
        public ActionResult Showreel()
        {
            if (string.IsNullOrEmpty(manage.UID.ToString()))
            {
                Response.Redirect("/home/index");
                return View();
            }
            ViewBag.showreel = "作品集";
            ViewBag.allworkurl = _baseAllworksurl + manage.UID;
            #region MyRegion
            //IHairListService hair = new HairListService();
            //var list = hair.LoadEntities(c => c.shs_s_uid.Trim() == manage.UID.Trim());
            //if (list.Count() > 0)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    sb.Append("uid=");
            //    foreach (var item in list)
            //    {
            //        sb.AppendFormat("{0};", item.shs_h_uid);
            //    }
            //    var blog = hair.GetBlogBySalinHairStylist(sb.ToString()); //获取接口返回的数据
            //    var blogInfoList = blog.info.list; //接口数据的 list 集合信息 list
            //    var OfItemBlogs = hair.PageOfItemApi<BlogInfo, int>(pageIndex, pageSize, blogInfoList, c => true, null);
            //    if (blog.status == 1 && blog.info != null)
            //    {
            //        ViewData["HairList"] = OfItemBlogs;
            //        ViewBag.page = "/Manage/Showreel/(p)";
            //    }
            //} 
            #endregion
            return View();
        }
        #endregion

        #region 系统公告管理

        #region 系统公告展示
        public ActionResult SalonSystemMessages(int pageIndex = 1)
        {
            IQueryable<SysInformation> templists = sysfinfoservice.LoadEntities<SysInformation>(c => true);
            int TotalCount = 0;
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            int pageSize = 10;
            Expression<Func<t_sysinformation, bool>> wherelamda = c => c.sy_status == 1;
            Expression<Func<t_sysinformation, DateTime>> orderbylamda = c => c.sy_date.Value;
            PageOfItems<t_sysinformation> list = sysfinfoservice.PageOfItemsLoadPageEntites<DateTime>(pageIndex, pageSize, wherelamda, orderbylamda, false);
            Mapper.CreateMap<t_sysinformation, SysInformation>();
            PageOfItems<SysInformation> newlist = Mapper.Map<PageOfItems<SysInformation>>(list);
            TotalCount = list.TotalPageCount;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageCount = TotalCount;
            return View(newlist);
        }

        #endregion

        #region 系统公告详情
        public ActionResult SystemInfo()
        {
            int id;
            SysInformation newsysinfomation = new SysInformation();
            string sy_id = Request.QueryString["sy_id"];
            if (!string.IsNullOrWhiteSpace(sy_id))
            {
                if (int.TryParse(sy_id, out id))
                {
                    var sysinfomation = sysfinfoservice.LoadEntities(c => c.sy_id == id && c.sy_status == 1).FirstOrDefault();
                    if (sysinfomation != null)
                    {
                        Mapper.CreateMap<t_sysinformation, SysInformation>();
                        newsysinfomation = Mapper.Map<SysInformation>(sysinfomation);
                    }
                }
            }
            return View(newsysinfomation);
        }
        #endregion

        #endregion

        #region 沙龙公告管理

        /// <summary>
        /// 沙龙公告列表
        /// </summary>
        public ActionResult ShowSalonNotice(int pageIndex = 1)
        {
            if (Session["MsgShowSalonNotice"] != null)
            {
                ViewBag.msg = Session["MsgShowSalonNotice"];
                Session.Remove("MsgShowSalonNotice");
            }
            else
            {
                ViewBag.msg = "";
            }
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            int pageSize = 10;
            IQueryable<t_notice> templist = noticeservice.LoadEntities(c => c.tn_status == 1);
            PageOfItems<t_notice> entitylist = noticeservice.PageOfItemBySelfModel<t_notice, DateTime>(pageIndex, pageSize, templist, c => true, c => c.tn_date.Value, false);
            Mapper.CreateMap<t_notice, Notice>();
            var list = Mapper.Map<PageOfItems<Notice>>(entitylist);
            int TotalCount = 0;
            if (entitylist != null)
                TotalCount = entitylist.TotalPageCount;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageCount = TotalCount;
            return View(list);
        }
        /// <summary>
        /// 沙龙公告添加展示
        /// </summary>
        public ActionResult AddSalonNotice()
        {
            return View();
        }
        /// <summary>
        /// 沙龙公告添加操作
        /// </summary>
        [HttpPost]
        public ActionResult AddSalonNotice(string content, string status = "1")
        {
            t_notice notice = new t_notice();
            bool flag = true;
            if (!string.IsNullOrWhiteSpace(content) && !string.IsNullOrWhiteSpace(status))
            {
                notice.tn_date = DateTime.Now;
                notice.tn_s_uid = manage.UID;
                notice.tn_content = content;
                notice.tn_status = Convert.ToInt32(status);
                try
                {
                    noticeservice.AddEntity(notice);
                }
                catch (Exception ex)
                {
                    flag = false;
                }
            }

            string Msg = flag ? "公告添加成功" : "公告添加失败";
            Session["MsgShowSalonNotice"] = Msg;
            return RedirectToAction("ShowSalonNotice");
        }
        /// <summary>
        /// 沙龙公告删除操作
        /// </summary>
        public ActionResult DeleteNotice(string id)
        {
            bool flag = false;
            if (!string.IsNullOrWhiteSpace(id))
            {
                int notice_id = 0;
                if (int.TryParse(id, out notice_id))
                {
                    var model = noticeservice.LoadEntities(c => c.tn_id == notice_id).FirstOrDefault();
                    flag = noticeservice.DeleteEntity(model);
                }
            }
            string Msg = flag ? "删除成功" : "删除失败";
            Session["MsgShowSalonNotice"] = Msg;
            return RedirectToAction("ShowSalonNotice");
        }

        #endregion




        #region 统计

        #region 沙龙实时统计 -->>今日预约   本周累计    本月累计

        public ActionResult RealTimeStatistics(int pageIndex = 1)
        {
            int pageSize = 5;
            StringBuilder WHERESQL = new StringBuilder();
            WHERESQL.Append("DATEDIFF(MONTH,wo_date,getdate())=0");
            PageOfItems<StatisticsMonthSum> temp = null;
            string UID = manage.UID.ToString();

            //查出所有人的数据
            var list = salonservice.RealTimeStatisticsMonthSummarizing(UID, WHERESQL);
            if (list != null)
            {
                var templist = list.AsQueryable();
                temp = salonservice.PageOfItemBySelfModel<StatisticsMonthSum, int>(pageIndex, pageSize, templist, c => true);
                //今日
                ViewBag.today_allcount = templist.Sum(c => (c.Today_HairCount == null ? 0 : c.Today_HairCount));
                ViewBag.today_price = templist.Sum(c => (c.Today_Price == null ? 0 : c.Today_Price));
                //本周
                ViewBag.week_allcount = templist.Sum(c => (c.Week_HairCount == null ? 0 : c.Week_HairCount));
                ViewBag.week_price = templist.Sum(c => (c.Week_Price == null ? 0 : c.Week_Price));
                //本月
                ViewBag.month_allcount = templist.Sum(c => (c.Month_HairCount == null ? 0 : c.Month_HairCount));
                ViewBag.month_price = templist.Sum(c => (c.Month_Price == null ? 0 : c.Month_Price));


                ViewBag.pageIndex = pageIndex;
                ViewBag.pageCount = temp.TotalPageCount;
            }
            return View(temp);
        }

        #endregion

        #region 沙龙月度汇总-->>预约单数   定金收入   流失客人   新增客人（按绑定，解除绑定来统计）
        public ActionResult SalonMonthStatistics(string date_start, string date_end, int pageIndex = 1)
        {
            ValAdmin();
            string UID = manage.UID.ToString();
            var temphairlist = hair.LoadEntities(c => c.shs_s_uid == new Guid(UID) && c.shs_status == 1);
            //IQueryable<Wx_BindHair> tempbind = null;
            StringBuilder TEMP = new StringBuilder();
            int pageSize = 5;
            int thissame = 1;
            int totalcount = 0;
            //int lost = 0;  //新增客户
            //int add = 0;  //流失客户

            #region 拼接日期字符

            if (!string.IsNullOrWhiteSpace(date_start) && !string.IsNullOrWhiteSpace(date_start))
            {

                string[] starttime = date_start.Split('-');
                string[] endtime = date_end.Split('-');
                DateTime timestart = new DateTime(Convert.ToInt32(starttime[0]), Convert.ToInt32(starttime[1]), 1);
                if (starttime.Equals(endtime))
                {
                    thissame = Convert.ToInt32(endtime[1]) + 1;
                }
                else
                {
                    thissame = Convert.ToInt32(endtime[1]);
                }

                DateTime timeend = new DateTime(Convert.ToInt32(endtime[0]), thissame, 1);
                //tempbind = wx_bindhairservice.LoadEntities(c => c.wb_date >= timestart && c.wb_date <= timeend);
                TEMP.AppendFormat("{0}|{1}", timestart, timeend);
                CacheManage.SetCache<StringBuilder>("date_where", TEMP);
                ViewBag.start_time = starttime[0] + "-" + starttime[1];
                ViewBag.end_time = endtime[0] + "-" + endtime[1];
            }
            else
            {
                TEMP = CacheManage.GetCache("date_where") as StringBuilder;
                if (TEMP == null)
                {
                    TEMP = new StringBuilder();
                    ViewBag.start_time = DateTime.Now.Year + "-" + DateTime.Now.Month;
                    ViewBag.end_time = ((DateTime.Now.Month + 1) > 12 ? DateTime.Now.Year + 1 : DateTime.Now.Year) + "-" + ((DateTime.Now.Month + 1) > 12 ? 1 : 12);
                    TEMP.AppendFormat("{0}|{1}", new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString(), new DateTime((DateTime.Now.Month + 1) > 12 ? DateTime.Now.Year + 1 : DateTime.Now.Year, (DateTime.Now.Month + 1) > 12 ? 1 : 12, 1).ToString());
                    //tempbind = wx_bindhairservice.LoadEntities(c => c.wb_date.Year == DateTime.Now.Year && c.wb_date.Month == DateTime.Now.Month);
                }
                else
                {

                    ViewBag.start_time = Convert.ToDateTime(TEMP.ToString().Split('|')[0]).Year + "-" + Convert.ToDateTime(TEMP.ToString().Split('|')[0]).Month;
                    ViewBag.end_time = Convert.ToDateTime(TEMP.ToString().Split('|')[1]).Year + "-" + Convert.ToDateTime(TEMP.ToString().Split('|')[1]).Month;
                    DateTime timestart = new DateTime(Convert.ToDateTime(TEMP.ToString().Split('|')[0]).Year, Convert.ToDateTime(TEMP.ToString().Split('|')[0]).Month, 1);
                    DateTime timeend = new DateTime(Convert.ToDateTime(TEMP.ToString().Split('|')[1]).Year, Convert.ToDateTime(TEMP.ToString().Split('|')[1]).Month, 1);
                    //tempbind = wx_bindhairservice.LoadEntities(c => c.wb_date >= timestart && c.wb_date <= timeend);
                }
            }

            #endregion

            StringBuilder WHERESQL = new StringBuilder();
            WHERESQL.AppendFormat("wo_date BETWEEN '{0}' AND '{1}'", TEMP.ToString().Split('|')[0], TEMP.ToString().Split('|')[1]);
            var list = salonservice.RealTimeStatisticsMonthSummarizing(UID, WHERESQL).ToList(); //按照月份查出来的数据
            ViewBag.sum_pirce = list.Sum(c => (c.Month_Price == null ? 0 : c.Month_Price)); //总定金收入
            ViewBag.sum_count = list.Sum(c => (c.Month_HairCount == null ? 0 : c.Month_HairCount));//总预约单数

            #region 新增客人  流失客人统计

            //foreach (var item in temphairlist)
            //{
            //    var tempadd = tempbind.Where(c => c.wb_h_uid.Contains(item.shs_h_uid) && c.wb_status == 1).Count();
            //    add += tempadd;
            //    var templost = tempbind.Where(c => c.wb_h_uid.Contains(item.shs_h_uid) && c.wb_status == 0).Count(); ;
            //    lost += templost;
            //    var addmodel = list.Where(c => c.HairUid.Contains(item.shs_h_uid)).FirstOrDefault(); //每位发型师新增客人
            //    addmodel.AddCustomer = tempadd; ;
            //    var lostmodel = list.Where(c => c.HairUid.Contains(item.shs_h_uid)).FirstOrDefault();//每位发型师流失客人
            //    lostmodel.LostCustomer = templost;

            //}
            #endregion

            var pagelist = salonservice.PageOfItemBySelfModel<StatisticsMonthSum, int>(pageIndex, pageSize, list.AsQueryable(), c => true);
            if (pagelist != null)
            {
                totalcount = pagelist.TotalPageCount;
            }



            ViewBag.pageIndex = pageIndex;
            ViewBag.pageCount = totalcount;
            //ViewBag.lost = lost;
            //ViewBag.add = add;
            return View(pagelist);
        }
        #endregion

        //Echarts图片请求
        [HttpPost]
        public JsonResult GetgraphRealTimeStatistics(string id)
        {
            AjaxEchartsParms result = new AjaxEchartsParms();
            Dictionary<string, dynamic> dic = new Dictionary<string, dynamic>();
            List<wx_order> wxorder_list = new List<wx_order>();
            Expression<Func<wx_order, bool>> expression = null;
            try
            {
                int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); //获取本月共有多少天
                //int year = DateTime.Now.Year;    //当前年
                int month = DateTime.Now.Month;  //当前月
                DateTime timestart = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day);
                DateTime timeend = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                if (id == "0")
                {

                    var models = hair.LoadEntities(c => c.shs_s_uid == manage.UID && c.shs_status == 1); //加载沙龙绑定的所有发型师
                    var temp = wxorderservice.LoadEntities(c => c.wo_orderstatus == 2 && c.wo_date >= timestart && c.wo_date <= timeend);//加载预定订单
                    wxorder_list = (from m in models
                                    from t in temp
                                    where m.shs_h_uid.Equals(t.wo_h_uid)
                                    select t).ToList();
                }
                else
                {
                    expression = c => c.wo_h_uid == new Guid(id) && c.wo_orderstatus == 2 && c.wo_date >= timestart && c.wo_date <= timeend;
                    wxorder_list = wxorderservice.LoadEntities(expression).ToList(); //获取预约单的发型师订单
                }
                for (int i = 1; i <= days; i++)
                {
                    var templist = wxorder_list.Where(c => c.wo_date.Value.Day == i);
                    result.Status = 1;
                    result.Msg = "成功";
                    result.Info = dic;

                    if (templist.Count() > 0)
                    {
                        dic.Add(string.Format("{0}月{1}日", month, i.ToString()), new { all_count = templist.Count(), all_price = templist.Sum(c => c.wo_price) });
                    }

                }
            }
            catch
            {
                result.Status = 0;
                result.Msg = "查询失败";
            }
            string jsondata = JsonConvert.SerializeObject(result);
            return Json(jsondata.ToLower(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region MyRegion

        ///// <summary>
        ///// 中奖记录
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult winningrec(string page)
        //{
        //    int pagesize = 20;
        //    int pageindex = 1;
        //    if (!string.IsNullOrEmpty(page))
        //    {
        //        try
        //        {
        //            pageindex = int.Parse(page);
        //        }
        //        catch
        //        { }
        //    }
        //    int rowcount = 0;
        //    int pagecount = 0;
        //    ViewBag.ban = ban;
        //    return View();
        //}

        ///// <summary>
        ///// 扫码回流数据
        ///// </summary>
        ///// <param name="page"></param>
        ///// <returns></returns>
        //public ActionResult Backflow()
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        Response.Redirect("/manage/LoginEnd"); return View();
        //    }
        //    BackflowService bak = new BackflowService();
        //    ScanrecService sca = new ScanrecService();
        //    Backflow bakmol = new SalonModel.Backflow();
        //    string dts = "";
        //    for (int i = 6; i >= 0; i--)
        //    {
        //        DateTime dt = DateTime.Now.AddDays(-i);
        //        string dy = dt.Date.ToString().Split(' ')[0];
        //        dts += "'" + dy + "',";
        //    }
        //    dts = dts.Substring(0, dts.Length - 1);
        //    string va = bak.GetSevenDayInfo(manage.UID, 0);
        //    string va1 = bak.GetSevenDayInfo(manage.UID, 1);
        //    string sa = sca.GetSevenDayInfo(manage.UID);
        //    string cou1 = sca.GetScanrecCou(manage.UID).Split('|')[0];
        //    string cou2 = bak.GetBackflowCou(manage.UID, 0);
        //    string cou3 = bak.GetBackflowCou(manage.UID, 1);
        //    ViewBag.cou1 = cou1;
        //    ViewBag.cou2 = cou2;
        //    ViewBag.cou3 = cou3;
        //    ViewBag.dts = dts;
        //    ViewBag.va = va;
        //    ViewBag.va1 = va1;
        //    ViewBag.sa = sa;
        //    ViewBag.ban = ban;
        //    return View();
        //}

        ///// <summary>
        ///// 沙龙信息修改
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult Salon()
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        Response.Redirect("/manage/LoginEnd"); return View();
        //    }
        //    SalonService salon = new SalonService();
        //    //获取沙龙信息
        //    List<AllSalon> alllist = new List<AllSalon>();
        //    alllist = salon.GetSalonInfo(manage.UID, 3);
        //    string logo = "";
        //    string name = "";
        //    string photo1 = "";
        //    string photo2 = "";
        //    string photo3 = "";
        //    string imgmd = "";
        //    string id1 = "";
        //    string id2 = "";
        //    string id3 = "";
        //    string idimgmd = "";
        //    if (alllist != null && alllist.Count > 0)
        //    {
        //        logo = alllist[0].Logo;
        //        name = alllist[0].Nickname;
        //        photo1 = alllist[0].Photo;
        //        photo2 = alllist[1].Photo;
        //        photo3 = alllist[2].Photo;
        //        id1 = alllist[0].Id.ToString();
        //        id2 = alllist[1].Id.ToString();
        //        id3 = alllist[2].Id.ToString();
        //    }
        //    alllist = salon.GetSalonInfo(manage.UID, 2);
        //    if (alllist != null && alllist.Count > 0)
        //    {
        //        imgmd = alllist[0].Photo;
        //        idimgmd = alllist[0].Id.ToString();
        //    }
        //    ViewBag.name = name;
        //    ViewBag.logo = logo;
        //    ViewBag.id1 = id1;
        //    ViewBag.id2 = id2;
        //    ViewBag.id3 = id3;
        //    ViewBag.idimgmd = idimgmd;
        //    ViewBag.photo1 = photo1;
        //    ViewBag.photo2 = photo2;
        //    ViewBag.photo3 = photo3;
        //    ViewBag.imgmd = imgmd;
        //    ViewBag.ban = ban;
        //    ViewBag.Token = GetToken();
        //    return View();
        //}


        ///// <summary>
        ///// 修改店面图片
        ///// </summary>
        ///// <param name="logo"></param>
        ///// <param name="id1"></param>
        ///// <param name="pohot1"></param>
        ///// <param name="id2"></param>
        ///// <param name="pohot2"></param>
        ///// <param name="id3"></param>
        ///// <param name="pohot3"></param>
        ///// <returns></returns>
        //public string UpdateImage(string logo, string idimgmd, string imgmd, string id1, string pohot1, string id2, string pohot2, string id3, string pohot3)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        return "-1";
        //    }
        //    int cou = 0;
        //    SalonService salon = new SalonService();
        //    cou += salon.UpdateLogoInfo(manage.UID, logo);
        //    if (string.IsNullOrEmpty(idimgmd))
        //    {
        //        AllSalon alls = new AllSalon();
        //        alls.SiKind = 2;
        //        alls.Suid = manage.UID;
        //        alls.Sistatus = 1;
        //        alls.Photo = imgmd;
        //        salon.AddAllSalon(alls);
        //    }
        //    else
        //    {
        //        cou += salon.UpdateImageInfo(idimgmd, imgmd, 1);
        //    }
        //    cou += salon.UpdateImageInfo(id1, pohot1, 1);
        //    cou += salon.UpdateImageInfo(id2, pohot2, 1);
        //    cou += salon.UpdateImageInfo(id3, pohot3, 1);
        //    return cou.ToString();
        //}


        //private const string userName = "sdk_hairbobo";
        //private const string userPwd = "18518880";
        ///// <summary>
        ///// 发送手机短信
        ///// </summary>
        ///// <param name="content">短信内容</param>
        ///// <param name="cell">手机号</param>
        ///// <returns>返回发送状态，大于零成功，小于零失败</returns>
        //public static int SendMessageByJZ(string content, string cell)
        //{
        //    int result = 0;
        //    try
        //    {
        //        IService.Sms.BusinessService client = new IService.Sms.BusinessServiceClient();
        //        var sbmb = new IService.Sms.sendBatchMessageBody
        //        {
        //            account = userName,
        //            password = userPwd,
        //            destmobile = cell,
        //            msgText = content
        //        };

        //        var req = new IService.Sms.sendBatchMessage(sbmb);
        //        var resp = client.sendBatchMessage(req);
        //        IService.Sms.sendBatchMessageResponseBody body = resp.Body;
        //        result = body.sendBatchMessageReturn > 0 ? 1 : 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        int temp;
        //    }
        //    return result;
        //}

        ///// <summary>
        ///// 修改作品状态
        ///// </summary>
        ///// <param name="uid">作品ID</param>
        ///// <param name="sta">是否显示</param>
        ///// <param name="top">是否在发型师页面显示</param>
        ///// <returns></returns>
        //public string updateart(string uid, string sta, string top)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        return "-1";
        //    }
        //    if (string.IsNullOrEmpty(uid))
        //    {
        //        return "作品参数错误";
        //    }
        //    if (string.IsNullOrEmpty(sta))
        //    {
        //        return "作品状态错误";
        //    }
        //    if (string.IsNullOrEmpty(top))
        //    {
        //        return "发型师作品状态错误";
        //    }
        //    try
        //    {
        //        ///API地址
        //        string url = "";
        //        ///API参数
        //        string paramstr = "kind=updateTopStatusById&bgid=" + uid + "&top=" + top + "&status=" + sta;
        //        //获取返回值(Json)
        //        string responseBody0 = Common.GetResponseResult(url, paramstr).ToLower();
        //        //解析Json字符串
        //        var dic = Common.JSONToObject<Dictionary<string, object>>(responseBody0);
        //        string status = dic["status"].ToString(); //返回状态 
        //        if (status == "1")
        //        {
        //            return "1";
        //        }
        //    }
        //    catch
        //    {
        //        return "0";
        //    }
        //    return "0";
        //}

        ///// <summary>
        ///// 作品设置
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult artwork(string ind, string ind2)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        Response.Redirect("/manage/LoginEnd");
        //    }
        //    if (string.IsNullOrEmpty(ind))
        //    {
        //        ind = "1";
        //    }
        //    if (string.IsNullOrEmpty(ind2))
        //    {
        //        ind2 = "1";
        //    }
        //    try
        //    {
        //        string val = GetHtml(ind, ind2, "-1", "1", 1);
        //        string[] vals = val.Split('|');
        //        string val2 = GetHtml(ind, ind2, "-1", "0", 0);
        //        string[] vals2 = val2.Split('|');
        //        ViewBag.val = vals[0];
        //        ViewBag.fen = vals[1];
        //        ViewBag.val2 = vals2[0];
        //        ViewBag.fen2 = vals2[1];
        //    }
        //    catch { }
        //    ViewBag.ban = ban;
        //    ViewBag.ind = ind;
        //    ViewBag.ind2 = ind2;
        //    return View();
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="ind"></param>
        ///// <param name="ind2"></param>
        ///// <param name="top"></param>
        ///// <param name="status"></param>
        ///// <param name="t"></param>
        ///// <returns></returns>
        //public string GetHtml(string ind, string ind2, string top, string status, int t)
        //{
        //    int pagesize1 = 20;
        //    int pageindex1 = 1; if (t == 0)
        //    {
        //        if (!string.IsNullOrEmpty(ind))
        //        {
        //            try
        //            {
        //                pageindex1 = int.Parse(ind);
        //            }
        //            catch
        //            { }
        //        }
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(ind2))
        //        {
        //            try
        //            {
        //                pageindex1 = int.Parse(ind2);
        //            }
        //            catch
        //            { }
        //        }
        //    }
        //    HairListService hise = new HairListService();
        //    string str = "";
        //    try
        //    {
        //        str = hise.GetHariStyleListID(manage.UID);
        //    }
        //    catch
        //    { }
        //    int rowcount1 = 0;
        //    int pagecount1 = 0;
        //    ///API地址
        //    string url1 = "";
        //    ///API参数
        //    //string paramstr1 = "kind=getWorksPager&uid=" + manage.UID + 
        //    string paramstr1 = "kind=getWorksPager&uid=" + str +
        //        "&top=" + top + "&status=" + status + "&pagesize=" + pagesize1 + "&pageindex=" + ind;
        //    //获取返回值(Json)
        //    string responseBody1 = Common.GetResponseResult(url1, paramstr1).ToLower();
        //    //解析Json字符串
        //    var dic1 = Common.JSONToObject<Dictionary<string, object>>(responseBody1);
        //    string status1 = dic1["status"].ToString(); //返回状态
        //    string count1 = null;
        //    if (status1 == "1")
        //    {
        //        rowcount1 = int.Parse(dic1["status"].ToString()); //返回状态 
        //        pagecount1 = int.Parse(dic1["pagecount"].ToString()); //返回状态 
        //        ArrayList info1 = (ArrayList)dic1["info"];//返回用户信息合集
        //        rowcount1 = info1.Count;//获取详细信息对象的数量 
        //        count1 = "<tr>";
        //        for (int i = 0; i < rowcount1; i++)
        //        {
        //            Dictionary<string, object> DictionaryInfo = (Dictionary<string, object>)info1[i];
        //            string bgid = DictionaryInfo["bgid"].ToString();//作品ID    
        //            string bguid = DictionaryInfo["bguid"].ToString();//作者UID
        //            string bgname = DictionaryInfo["bgname"].ToString();//作者名称
        //            string bgtitle = DictionaryInfo["bgtitle"].ToString();//作品标题
        //            string bgcontent = DictionaryInfo["bgcontent"].ToString();//作品描述
        //            string bgimage = DictionaryInfo["bgimage"].ToString();//作品图片
        //            count1 += " <div class=\"haird\"><div class=\"box\"> " +
        //                      " <img src=\"" + imgpath + bgimage + "\" class=\"js_msgSenderAvatar\"> " +
        //                      "<br /></div><div class='Description'><span class=\"btn btn_input btn_primary \"onclick=\"if(confirm('确定要修改?')){changeStatusd('" + bgid + "'," + (t.ToString() == "0" ? "1" : "0") + ");}\"> " +
        //                     (t.ToString() == "0" ? "展示" : "取消展示") + "</span></div></div>";
        //            if (i % 3 == 0 && i != 0 && i != rowcount1 - 1)
        //            {
        //                count1 += "</tr><tr>";
        //            }
        //        }
        //        count1 += "</tr>";
        //    }
        //    string pagecontent1 = null;
        //    string first1 = pageindex1 == 1 ? "" : "";
        //    string prv1 = pageindex1 == 1 ? "1" : (pageindex1 - 1).ToString();
        //    string next1 = pageindex1 == pagecount1 ? pagecount1 + "" : (pageindex1 + 1).ToString();
        //    string end1 = pageindex1 == pagecount1 ? "" : "";
        //    if (pagecount1 > 1)
        //    {
        //        if (t == 0)
        //        {
        //            pagecontent1 = "<span class='page_nav_area'><a href='/manage/artwork/" + first1 + "/" + ind2 + "'class='btn page_first'>首页</a> <a href='/manage/artwork/" + prv1 + "/" + ind2 + "'class='btn page_prev'><i class='arrow'></i></a> <span class='page_num'><label>" + pageindex1 + "</label><span class='num_gap'> /</span> <label>" + pagecount1 + "</label></span> <a href='/manage/artwork/" + next1 + "/" + ind2 + "'class='btn page_next'><i class='arrow'></i></a> <a href='/manage/artwork/" + pagecount1.ToString() + end1 + "/" + ind2 + "'class='btn page_last'>末页</a> </span> ";
        //        }
        //        else
        //        {
        //            pagecontent1 = "<span class='page_nav_area'><a href='/manage/artwork/" + ind + "/" + first1 + "'class='btn page_first'>首页</a> <a href='/manage/artwork/" + ind + "/" + prv1 + "'class='btn page_prev'><i class='arrow'></i></a> <span class='page_num'><label>" + pageindex1 + "</label><span class='num_gap'> /</span> <label>" + pagecount1 + "</label></span> <a href='/manage/artwork/" + ind + "/" + next1 + "'class='btn page_next'><i class='arrow'></i></a> <a href='/manage/artwork/" + ind + "/" + pagecount1.ToString() + end1 + "'class='btn page_last'>末页</a> </span> ";
        //        }
        //    }
        //    return count1 + "|" + pagecontent1;
        //}

        ///// <summary>
        ///// 发型师作品设置
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult HairWorks(string ind, string id, string ind2)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        Response.Redirect("/manage/LoginEnd");
        //    }
        //    if (string.IsNullOrEmpty(ind))
        //    {
        //        ind = "1";
        //    }
        //    if (string.IsNullOrEmpty(ind2))
        //    {
        //        ind2 = "1";
        //    }
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        id = "1";
        //    }
        //    int pagesize = 10;
        //    int pageindex = 1;
        //    if (!string.IsNullOrEmpty(ind))
        //    {
        //        try
        //        {
        //            pageindex = int.Parse(ind);
        //        }
        //        catch
        //        { }
        //    }
        //    int rowcount = 0;
        //    int pagecount = 0;
        //    string content = null;
        //    string count1 = null;
        //    string pagecontent = null;
        //    string pagecontent1 = null;
        //    HairListService hise = new HairListService();
        //    List<HariStyleList> lis = new List<HariStyleList>();
        //    try
        //    {
        //        lis = hise.GetHariStyleListList(manage.UID, pageindex, pagesize, out rowcount, out pagecount);
        //        content = "<tr>";
        //        for (int i = 0; i < lis.Count; i++)
        //        {
        //            content += "<td class='table_cell product'><div class='haird'><div class='box'><img src='" + lis[i].Logo + "' class='js_msgSenderAvatar'></div><div class='Description'>" + lis[i].Name + "</div><div class='Description'><span class='btn btn_input btn_primary '><button type='button'style='padding: 0;'onclick=\"showinfo('" + lis[i].Huid + "')\">查看作品</button></span></div></div></td>";
        //        }
        //        content += "</tr>";

        //        string first = pageindex == 1 ? "" : "";
        //        string prv = pageindex == 1 ? "1" : (pageindex - 1).ToString();
        //        string next = pageindex == pagecount ? pagecount + "" : (pageindex + 1).ToString();
        //        string end = pageindex == pagecount ? "" : "";
        //        if (pagecount > 1)
        //        {
        //            pagecontent = "<span class='page_nav_area'><a href='/manage/HairWorks/" + first + "/" + ind2 +
        //                "'class='btn page_first'>首页</a> <a href='/manage/HairWorks/" + prv + "/" + ind2 + "'class='btn page_prev'><i class='arrow'></i></a> <span class='page_num'><label>" + pageindex +
        //                "</label><span class='num_gap'> /</span> <label>" + pagecount + "</label></span> <a href='/manage/HairWorks/" + ind + "/" + next +
        //                "'class='btn page_next'><i class='arrow'></i></a> <a href='/manage/HairWorks/" + pagecount.ToString() + end + "/" +
        //                 ind2 + "'class='btn page_last'>末页</a> </span> ";
        //        }

        //        if (id != "1")
        //        {

        //            if (!string.IsNullOrEmpty(ind2))
        //            {
        //                try
        //                {
        //                    pageindex = int.Parse(ind2);
        //                }
        //                catch
        //                { }
        //            }
        //            int rowcount1 = 0;
        //            int pagecount1 = 0;
        //            ///API地址
        //            string url1 = "";
        //            ///API参数
        //            //string paramstr1 = "kind=getWorksPager&uid=" + manage.UID + 
        //            string paramstr1 = "kind=getWorksPager&uid=" + id + "&top=-1&status=-1&pagesize=" + pagesize + "&pageindex=" + ind2;//+
        //            //    "&top=" + top + "&status=" + status + "&pagesize=" + pagesize1 + "&pageindex=" + ind;
        //            //获取返回值(Json)
        //            string responseBody1 = Common.GetResponseResult(url1, paramstr1).ToLower();
        //            //解析Json字符串
        //            var dic1 = Common.JSONToObject<Dictionary<string, object>>(responseBody1);
        //            string status1 = dic1["status"].ToString(); //返回状态
        //            if (status1 == "1")
        //            {
        //                rowcount1 = int.Parse(dic1["status"].ToString()); //返回状态 
        //                pagecount1 = int.Parse(dic1["pagecount"].ToString()); //返回状态 
        //                ArrayList info1 = (ArrayList)dic1["info"];//返回用户信息合集
        //                rowcount1 = info1.Count;//获取详细信息对象的数量 
        //                count1 = "<tr>";
        //                for (int i = 0; i < rowcount1; i++)
        //                {
        //                    Dictionary<string, object> DictionaryInfo = (Dictionary<string, object>)info1[i];
        //                    string bgid = DictionaryInfo["bgid"].ToString();//作品ID    
        //                    string bguid = DictionaryInfo["bguid"].ToString();//作者UID
        //                    string bgname = DictionaryInfo["bgname"].ToString();//作者名称
        //                    string bgtitle = DictionaryInfo["bgtitle"].ToString();//作品标题
        //                    string bgcontent = DictionaryInfo["bgcontent"].ToString();//作品描述
        //                    string bgimage = DictionaryInfo["bgimage"].ToString();//作品图片  
        //                    string title_status = DictionaryInfo["top"].ToString() == "0" ? "设为展示作品" : "取消展示";
        //                    string bg_status = DictionaryInfo["top"].ToString() == "0" ? "2" : "0";
        //                    count1 += " <div class=\"haird\"><div class=\"box\"> " +
        //                              " <img src=\"" + imgpath + bgimage + "\" class=\"js_msgSenderAvatar\"> " +
        //                              "<br /></div><div class='Description'><span class=\"btn btn_input btn_primary \"onclick=\"if(confirm('确定要修改?')){changeStatusd('" + bgid + "'," + bg_status + ");}\"> " +
        //                              title_status + "</span></div></div>";
        //                    if (i % 3 == 0 && i != 0 && i != rowcount1 - 1)
        //                    {
        //                        count1 += "</tr><tr>";
        //                    }
        //                }
        //                count1 += "</tr>";
        //            }
        //            string first1 = pageindex == 1 ? "" : "";
        //            string prv1 = pageindex == 1 ? "1" : (pageindex - 1).ToString();
        //            string next1 = pageindex == pagecount1 ? pagecount + "" : (pageindex + 1).ToString();
        //            string end1 = pageindex == pagecount ? "" : "";
        //            if (pagecount1 > 1)
        //            {
        //                pagecontent1 = "<span class='page_nav_area'><a href='/manage/HairWorks/" + ind + "/" + id + "/" + first1 +
        //                    "'class='btn page_first'>首页</a> <a href='/manage/HairWorks/" + ind + "/" + id + "/" + prv1 + "'class='btn page_prev'><i class='arrow'></i></a> <span class='page_num'><label>" + pageindex +
        //                    "</label><span class='num_gap'> /</span> <label>" + pagecount1 + "</label></span> <a href='/manage/HairWorks/" + ind + "/" + id + "/" + next1 +
        //                    "'class='btn page_next'><i class='arrow'></i></a> <a href='/manage/HairWorks/" + ind + "/" + id + "/" +
        //                    pagecount1.ToString() + end1 + "'class='btn page_last'>末页</a> </span> ";
        //            }
        //        }
        //    }
        //    catch { }

        //    ViewBag.ind = ind;
        //    ViewBag.ind2 = ind2;
        //    ViewBag.id = id;
        //    ViewBag.ban = ban;
        //    ViewBag.content1 = count1;
        //    ViewBag.pagecontent1 = pagecontent1;
        //    ViewBag.content = content;
        //    ViewBag.pagecontent = pagecontent;
        //    return View();
        //}

        ///// <summary>
        ///// 二维码
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult QRCode()
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        Response.Redirect("/manage/LoginEnd");
        //    }
        //    ViewBag.ban = ban;
        //    ViewBag.ImageUrl = ConfigurationManager.AppSettings["Url"];
        //    ViewBag.uid = manage.UID;
        //    return View();
        //}


        ///// <summary>
        ///// 沙龙添加发型师
        ///// </summary>
        ///// <param name="Cell"></param>
        ///// <param name="Exp"></param>
        ///// <param name="Hairprice"></param>
        ///// <param name="Huid"></param>
        ///// <param name="Logo"></param>
        ///// <param name="Name"></param>  
        ///// <param name="code"></param>
        ///// <returns></returns>
        //public string Addhair(string Cell, string Exp, string Hairprice, string Huid, string Logo, string Name, string code)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        return "-1";
        //    }
        //    if (string.IsNullOrEmpty(Cell))
        //    {
        //        return "手机号码不能为空";
        //    }
        //    if (string.IsNullOrEmpty(Hairprice))
        //    {
        //        return "剪发价格不能为空";
        //    }
        //    if (string.IsNullOrEmpty(Huid))
        //    {
        //        return "发型师不能为空";
        //    }
        //    if (string.IsNullOrEmpty(Logo))
        //    {
        //        return "头像不能为空";
        //    }
        //    if (string.IsNullOrEmpty(Name))
        //    {
        //        return "发型师昵称不能为空";
        //    }
        //    if (string.IsNullOrEmpty(code))
        //    {
        //        return "验证码不能为空";
        //    }

        //    SmsModelService smos = new SmsModelService();
        //    SmsModel smm = new SmsModel();
        //    try
        //    {
        //        smm = smos.GetSmsModelList(Cell, string.Format(SmsDefult, code));
        //        if (smm == null)
        //        {
        //            return "验证码验证失败";
        //        }
        //        else
        //        {
        //            //验证成功
        //        }
        //    }
        //    catch
        //    {
        //        return "验证码验证失败";
        //    }
        //    HariStyleList hamo = new HariStyleList();
        //    HairListService hise = new HairListService();
        //    string val = "";
        //    try
        //    {
        //        hamo.Cell = Cell;
        //        hamo.Exp = Exp;
        //        hamo.Hairprice = Exp;
        //        hamo.Huid = Huid;
        //        hamo.Logo = Logo;
        //        hamo.Name = Name;
        //        hamo.Uid = manage.UID;
        //        val = hise.AddHariStyleList(hamo).ToString();
        //    }
        //    catch
        //    {
        //        return "0";
        //    }
        //    if (val == "1")
        //    {
        //        try
        //        {
        //            //添加成功后短信失效
        //            smos.UpdateSmsModel(smm).ToString();
        //        }
        //        catch
        //        {
        //            return "0";
        //        }
        //    }
        //    return val;
        //}


        ///// <summary>
        ///// 修改发型师
        ///// </summary>
        ///// <param name="Cell"></param>
        ///// <param name="Exp"></param>
        ///// <param name="Hairprice"></param>
        ///// <param name="Huid"></param>
        ///// <param name="Logo"></param>
        ///// <param name="Name"></param>
        ///// <returns></returns>
        //public string Updatehair(string Cell, string Exp, string Hairprice, string Huid, string Logo, string Name)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        return "-1";
        //    }
        //    if (string.IsNullOrEmpty(Cell))
        //    {
        //        return "手机号码不能为空";
        //    }
        //    if (string.IsNullOrEmpty(Hairprice))
        //    {
        //        return "剪发价格不能为空";
        //    }
        //    if (string.IsNullOrEmpty(Huid))
        //    {
        //        return "发型师不能为空";
        //    }
        //    if (string.IsNullOrEmpty(Logo))
        //    {
        //        return "头像不能为空";
        //    }
        //    if (string.IsNullOrEmpty(Name))
        //    {
        //        return "发型师昵称不能为空";
        //    }

        //    HariStyleList hamo = new HariStyleList();
        //    HairListService hise = new HairListService();
        //    try
        //    {
        //        hamo.Cell = Cell;
        //        hamo.Exp = Exp;
        //        hamo.Hairprice = Exp;
        //        hamo.Huid = Huid;
        //        hamo.Logo = Logo;
        //        hamo.Name = Name;
        //        hamo.Uid = manage.UID;
        //        return hise.UpdateHariStyleList(hamo).ToString();
        //    }
        //    catch
        //    {
        //        return "0";
        //    }
        //    return "";
        //}


        ///// <summary>
        ///// 取消发型师关联
        ///// </summary>
        ///// <param name="cell"></param>
        ///// <returns></returns>
        //public string Cancelhair(string uid)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        return "-1";
        //    }
        //    if (string.IsNullOrEmpty(uid))
        //    {
        //        return "0";
        //    }
        //    HairListService hise = new HairListService();
        //    try
        //    {
        //        return hise.DelHariStyleList(uid).ToString();
        //    }
        //    catch
        //    {
        //        return "-2";
        //    }
        //    return "0";
        //}

        ///// <summary>
        ///// 验证手机号码是否存在
        ///// </summary>
        ///// <param name="cell"></param>
        ///// <returns></returns>
        //public string YanFa(string cell)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        return "-1";
        //    }
        //    if (string.IsNullOrEmpty(cell))
        //    {
        //        //手机号码为空
        //        return "0";
        //    }
        //    string h_uid = "";
        //    string name = "";
        //    try
        //    {
        //        ///API地址
        //        string url = "";
        //        ///API参数
        //        string paramstr = "kind=hairinfoForWeiSalon&cell=" + cell;
        //        //获取返回值(Json)
        //        string responseBody0 = Common.GetResponseResult(url, paramstr).ToLower();
        //        //解析Json字符串
        //        var dic = Common.JSONToObject<Dictionary<string, object>>(responseBody0);
        //        string status = dic["status"].ToString(); //返回状态 
        //        if (status == "1")
        //        {
        //            //有返回数据
        //            string ram = GetRandom();
        //            SmsModel smo = new SmsModel();
        //            SmsModelService smos = new SmsModelService();
        //            smo.Cell = cell;
        //            smo.Content = string.Format(SmsDefult, ram);
        //            smo.Date = DateTime.Now;
        //            smo.Status = 1;
        //            int cou = smos.AddSmsModel(smo);
        //            if (cou > 0)
        //            {
        //                //短信记录已经添加
        //                //发送短信
        //                cou = 1;// SendMessageByJZ(smo.Content, smo.Cell);
        //                if (cou > 0)
        //                {
        //                    //短信已经发送
        //                    ArrayList info = (ArrayList)dic["info"];//返回用户信息合集
        //                    cou = info.Count;//获取详细信息对象的数量 
        //                    Dictionary<string, object> DictionaryInfo = new Dictionary<string, object>();
        //                    if (cou > 0)
        //                    {
        //                        //获取到信息
        //                        DictionaryInfo = (Dictionary<string, object>)info[0];
        //                        h_uid = DictionaryInfo["h_uid"].ToString();// 
        //                        name = DictionaryInfo["h_nickname"].ToString();//
        //                    }
        //                    else
        //                    {
        //                        //发型师详情获取失败
        //                        return "-2";
        //                    }
        //                }
        //                else
        //                {
        //                    //短信发送失败
        //                    return "-5";
        //                }
        //            }
        //            else
        //            {
        //                //短信记录添加失败
        //                return "-3";
        //            }
        //        }
        //        else
        //        {
        //            //未获取到发型师信息
        //            return "-4";
        //        }
        //    }
        //    catch
        //    {
        //        //未知错误
        //        return "-6";
        //    }
        //    string ret = h_uid + "|" + name;
        //    return ret;
        //}

        ///// <summary>
        ///// 生成6位随机数 可0开头
        ///// </summary>
        ///// <returns></returns>
        //public string GetRandom()
        //{
        //    string val = "";
        //    Random ra = new Random();
        //    for (int i = 0; i < 6; i++)
        //    {
        //        val += ra.Next(0, 9).ToString();
        //    }
        //    return val;
        //}




        ///// <summary>
        ///// 沙龙套餐设置
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult salonpackage(string ind, string id)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        Response.Redirect("/manage/LoginEnd");
        //    }
        //    if (string.IsNullOrEmpty(ind))
        //    {
        //        ind = "1";
        //    }
        //    int pagesize = 10;
        //    int pageindex = 1;
        //    if (!string.IsNullOrEmpty(ind))
        //    {
        //        try
        //        {
        //            pageindex = int.Parse(ind);
        //        }
        //        catch
        //        { }
        //    }
        //    int rowcount = 0;
        //    int pagecount = 0;
        //    string content = null;
        //    SalonPackageService salser = new SalonPackageService();
        //    List<SalonPackage> lis = new List<SalonPackage>();
        //    lis = salser.GetSalonPackageList(manage.UID, pageindex, pagesize, out rowcount, out pagecount);
        //    if (lis != null && lis.Count > 0)
        //    {
        //        for (int i = 0; i < lis.Count; i++)
        //        {

        //            string sid = lis[i].Id.ToString();
        //            content += "<tr><td>";
        //            content += lis[i].Dorder.ToString();
        //            content += "</td><td>";
        //            content += lis[i].Title.ToString();
        //            content += "</td><td>";
        //            content += lis[i].Content.ToString();
        //            content += "</td><td>";
        //            content += lis[i].Price.ToString();
        //            content += "</td><td>";
        //            content += "<img src=\"" + lis[i].Image + "\" class=\"JQ_img\" style=\"width: 80px;\" />";
        //            content += "<div class=\"JQ_div\" style=\"position: absolute; margin-left: -30px; display: none;\">";
        //            content += "<img src=\"" + lis[i].Image + "\"/>";
        //            content += "</div>";
        //            content += "</td><td class=\"table_cell product\"><a class='btn btn_input btn_primary' href='/manage/salonpackage/" + ind + "/" + sid +
        //                        "'>更改套餐</a> <a class='btn btn_input btn_primary' href='javascript:void(0);' onclick=del(" + sid + ")>删除套餐</a></td></tr>";
        //        }
        //    }
        //    ViewBag.Image = "/Images/bphoto.png";
        //    SalonPackage sal = new SalonPackage();
        //    if (id != null)
        //    {
        //        sal = salser.GetSalonPackageInfo(id);
        //        if (sal != null)
        //        {
        //            ViewBag.Image = sal.Image;
        //            ViewBag.Price = sal.Price;
        //            ViewBag.Title = sal.Title;
        //            ViewBag.salContent = sal.Content;
        //            ViewBag.Dorder = sal.Dorder;
        //            ViewBag.salId = sal.Id;
        //        }
        //    }
        //    ViewBag.ind = ind;
        //    ViewBag.content = content;
        //    ViewBag.ban = ban;
        //    ViewBag.uid = manage.UID;
        //    ViewBag.Token = GetToken();
        //    return View();
        //}



        ///// <summary>
        ///// 沙龙审核详情
        ///// </summary>
        ///// <returns></returns>
        //public string UPdatemanage(string id, string sta)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        return "-1";
        //    }
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        return "-2";
        //    }
        //    try
        //    {
        //        SalonService salser = new SalonService();
        //        return salser.UpdatestatusInfo(id, int.Parse(sta)).ToString();
        //    }
        //    catch
        //    { }
        //    return "0";
        //}


        ///// <summary>
        ///// 沙龙审核详情
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult manageCoInfo(string id)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        Response.Redirect("/manage/LoginEnd");
        //        return null;
        //    }
        //    if (AdminCell.IndexOf(manage.User.Cell) > -1)
        //    {
        //    }
        //    else
        //    {
        //        Response.Redirect("/manage/Scanrec");
        //        return null;
        //    }
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        Response.Redirect("/manage/manageCo");
        //        return null;
        //    }

        //    SalonService salser = new SalonService();
        //    List<AllSalon> lis = new List<AllSalon>();
        //    ViewBag.ban = ban;
        //    lis = salser.GetSalonInfo(id, 3);//环境照
        //    ViewBag.img1 = lis[0].Photo;
        //    ViewBag.img2 = lis[1].Photo;
        //    ViewBag.img3 = lis[2].Photo;
        //    lis = salser.GetSalonInfo(id, 1);//营业执照
        //    try
        //    {
        //        lis[0].Address = lis[0].Address.Replace("|", "");
        //    }
        //    catch
        //    { }
        //    return View(lis[0]);
        //}

        ///// <summary>
        ///// 沙龙审核设置
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult manageCo(string page)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        Response.Redirect("/manage/LoginEnd");
        //        return null;
        //    }
        //    if (AdminCell.IndexOf(manage.User.Cell) > -1)
        //    {
        //    }
        //    else
        //    {
        //        Response.Redirect("/manage/Scanrec");
        //        return null;
        //    }
        //    if (string.IsNullOrEmpty(page))
        //    {
        //        page = "1";
        //    }
        //    int pagesize = 10;
        //    int pageindex = 1;
        //    if (!string.IsNullOrEmpty(page))
        //    {
        //        try
        //        {
        //            pageindex = int.Parse(page);
        //        }
        //        catch
        //        { }
        //    }
        //    int rowcount = 0;
        //    int pagecount = 0;
        //    string content = null;
        //    string pagecontent1 = null;
        //    SalonService salser = new SalonService();
        //    List<SalonSimple> lis = new List<SalonSimple>();
        //    lis = salser.GetSalonList(-1, pageindex, pagesize, out rowcount, out pagecount);
        //    if (lis != null && lis.Count > 0)
        //    {
        //        for (int i = 0; i < lis.Count; i++)
        //        {

        //            string sid = lis[i].Uid.ToString();
        //            content += "<tr><td>";
        //            content += lis[i].Nickname.ToString();
        //            content += "</td><td>";
        //            content += lis[i].Cell.ToString();
        //            content += "</td><td>";
        //            content += lis[i].LinkName.ToString();
        //            content += "</td><td>";
        //            content += lis[i].Address.ToString().Replace("|", "");
        //            content += "</td><td style=\"word-break:break-all;\">";
        //            //content += Server.UrlDecode(lis[i].Summary.ToString());  
        //            content += lis[i].Summary.ToString();
        //            string sta = "";
        //            switch (lis[i].Status)
        //            {
        //                case 0:
        //                    sta = "帐号禁用";
        //                    break;
        //                case 1:
        //                    sta = "审核中";
        //                    break;
        //                case 2:
        //                    sta = "审核未通过";
        //                    break;
        //                case 3:
        //                    sta = "审核已通过未付款";
        //                    break;
        //                case 4:
        //                    sta = "审核已通过已付款";
        //                    break;
        //            }
        //            content += "</td><td>";
        //            content += sta;
        //            content += "</td><td class=\"table_cell product\"><a class='btn btn_input btn_primary' href='/manage/manageCoInfo/" + sid +
        //                        "'>查看详情</a></td></tr>";
        //        }
        //        string first1 = pageindex == 1 ? "" : "";
        //        string prv1 = pageindex == 1 ? "1" : (pageindex - 1).ToString();
        //        string next1 = pageindex == pagecount ? pagecount + "" : (pageindex + 1).ToString();
        //        string end1 = pageindex == pagecount ? "" : "";
        //        if (pagecount > 1)
        //        {
        //            pagecontent1 = "<span class='page_nav_area'><a href='/manage/manageCo/" + first1 +
        //                "'class='btn page_first'>首页</a> <a href='/manage/manageCo/" + prv1 + "'class='btn page_prev'><i class='arrow'></i></a> <span class='page_num'><label>" + pageindex +
        //                "</label><span class='num_gap'> /</span> <label>" + pagecount + "</label></span> <a href='/manage/manageCo/" + next1 +
        //                "'class='btn page_next'><i class='arrow'></i></a> <a href='/manage/manageCo/" +
        //                pagecount.ToString() + end1 + "'class='btn page_last'>末页</a> </span> ";
        //        }
        //    }
        //    ViewBag.ind = page;
        //    ViewBag.content = content;
        //    ViewBag.fen = pagecontent1;
        //    ViewBag.ban = ban;
        //    ViewBag.uid = manage.UID;
        //    ViewBag.Token = GetToken();
        //    return View();
        //}

        ///// <summary>
        ///// 沙龙订单
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult manageOrder(string page)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        Response.Redirect("/manage/LoginEnd");
        //        return null;
        //    }
        //    if (AdminCell.IndexOf(manage.User.Cell) > -1)
        //    {
        //    }
        //    else
        //    {
        //        Response.Redirect("/manage/Scanrec");
        //        return null;
        //    }
        //    if (string.IsNullOrEmpty(page))
        //    {
        //        page = "1";
        //    }
        //    int pagesize = 10;
        //    int pageindex = 1;
        //    if (!string.IsNullOrEmpty(page))
        //    {
        //        try
        //        {
        //            pageindex = int.Parse(page);
        //        }
        //        catch
        //        { }
        //    }
        //    int rowcount = 0;
        //    int pagecount = 0;
        //    string content = null;
        //    string pagecontent = null;
        //    OrderService ords = new OrderService();
        //    List<Order> lis = new List<Order>();
        //    lis = ords.GetOrderList(pageindex, pagesize, out rowcount, out pagecount);
        //    if (lis != null && lis.Count > 0)
        //    {
        //        for (int i = 0; i < lis.Count; i++)
        //        {

        //            string sid = lis[i].Id.ToString();
        //            content += "<tr><td>";
        //            content += lis[i].Orid.ToString();
        //            content += "</td><td>";
        //            content += lis[i].Name.ToString();
        //            content += "</td><td>";
        //            content += lis[i].Money.ToString();
        //            content += "</td><td>";
        //            content += lis[i].Date.ToString();
        //            content += "</td><td>";
        //            content += lis[i].Paydate.ToString() == lis[i].Date.ToString() ? "" : lis[i].Paydate.ToString();
        //            content += "</td><td>";
        //            string sta = "";
        //            switch (lis[i].Status)
        //            {
        //                case 0:
        //                    sta = "未支付";
        //                    break;
        //                case 1:
        //                    sta = "已支付";
        //                    break;
        //            }
        //            content += sta;
        //            content += "</td>";
        //            //content += "</td><td class=\"table_cell product\"><a class='btn btn_input btn_primary' href='/manage/manageCoInfo/" + sid +
        //            //            "'>查看详情</a></td></tr>";
        //        }
        //        //string first1 = pageindex == 1 ? "" : "";
        //        //string prv1 = pageindex == 1 ? "1" : (pageindex - 1).ToString();
        //        //string next1 = pageindex == pagecount ? pagecount + "" : (pageindex + 1).ToString();
        //        //string end1 = pageindex == pagecount ? "" : "";
        //        //pagecontent1 = "<span class='page_nav_area'><a href='/manage/manageOrder/" + first1 +
        //        //    "'class='btn page_first'>首页</a> <a href='/manage/manageOrder/" + prv1 + "'class='btn page_prev'><i class='arrow'></i></a> <span class='page_num'><label>" + pageindex +
        //        //    "</label><span class='num_gap'> /</span> <label>" + pagecount + "</label></span> <a href='/manage/manageOrder/" + next1 +
        //        //    "'class='btn page_next'><i class='arrow'></i></a> <a href='/manage/manageOrder/" +
        //        //    pagecount.ToString() + end1 + "'class='btn page_last'>末页</a> </span> ";
        //        //string pagecontent = null;
        //        string first = pageindex == 1 ? "#" : "";
        //        string prv = pageindex == 1 ? "1#" : (pageindex - 1).ToString();
        //        string next = pageindex == pagecount ? pagecount + "#" : (pageindex + 1).ToString();
        //        string end = pageindex == pagecount ? "#" : "";
        //        if (pagecount > 1)
        //        {
        //            pagecontent = "<span class='page_nav_area'><a href='/manage/manageOrder/" +
        //                first + "'class='btn page_first'>首页</a> <a href='/manage/manageOrder/" +
        //                prv + "'class='btn page_prev'><i class='arrow'></i></a> <span class='page_num'><label>" +
        //                pageindex + "</label><span class='num_gap'> /</span> <label>" +
        //                pagecount + "</label></span> <a href='/manage/manageOrder/" + next +
        //                "'class='btn page_next'><i class='arrow'></i></a> <a href='/manage/manageOrder/"
        //                + pagecount.ToString() + end + "'class='btn page_last'>末页</a> </span> ";
        //        }
        //    }
        //    ViewBag.ind = page;
        //    ViewBag.content = content;
        //    ViewBag.fen = pagecontent;
        //    ViewBag.ban = ban;
        //    ViewBag.uid = manage.UID;
        //    ViewBag.Token = GetToken();
        //    return View();
        //}

        //public ActionResult Logout()
        //{
        //    if (manage.UID != null && manage.UID != "")
        //    {
        //        if (Session["W_B_UID"] != null && new Guid(Session["W_B_UID"].ToString()) != "" && new Guid(Session["W_B_UID"].ToString()) == manage.UID)
        //        {
        //            Session.Remove("W_B_UID");
        //        }
        //        FormsAuthentication.SignOut();
        //    }
        //    Response.Redirect("/Home/index");
        //    return View();
        //}

        ///// <summary>
        ///// 添加套餐
        ///// </summary>
        ///// <returns></returns>
        //public string AddSalonPackage(string Title, string Dorder, string Content, string Price, string Image)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        return "-1";
        //    }
        //    double dupr = 0.00f;
        //    int intd = 0;
        //    if (string.IsNullOrEmpty(Title))
        //    {
        //        return "套餐标题不能为空";
        //    }
        //    if (string.IsNullOrEmpty(Dorder))
        //    {
        //        return "套餐排序不能为空";
        //    }
        //    else
        //    {
        //        try
        //        {
        //            intd = int.Parse(Price);
        //        }
        //        catch
        //        {
        //            return "套餐排序格式不正确";
        //        }
        //    }
        //    if (string.IsNullOrEmpty(Price))
        //    {
        //        return "套餐价格不能为空";
        //    }
        //    else
        //    {
        //        try
        //        {
        //            dupr = double.Parse(Price);
        //        }
        //        catch
        //        {
        //            return "套餐价格不正确";
        //        }
        //    }
        //    if (string.IsNullOrEmpty(Image))
        //    {
        //        return "套餐图片不能为空";
        //    }
        //    SalonPackageService salser = new SalonPackageService();
        //    SalonPackage salp = new SalonPackage();
        //    salp.Title = Title;
        //    salp.Status = 1;
        //    salp.Price = dupr;
        //    salp.Image = Image;
        //    salp.Dorder = intd;
        //    salp.Date = DateTime.Now;
        //    salp.Content = Content;
        //    salp.Suid = manage.UID;
        //    int cou = salser.AddSalonPackage(salp);
        //    return cou.ToString();
        //}


        ///// <summary>
        ///// 修改套餐
        ///// </summary>
        ///// <returns></returns>
        //public string UpdateSalonPackage(string id, string Title, string Dorder, string Content, string Price, string Image)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        return "-1";
        //    }
        //    double dupr = 0.00f;
        //    int intd = 0;
        //    int intid = 0;
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        return "套餐序号不能为空";
        //    }
        //    else
        //    {
        //        try
        //        {
        //            intid = int.Parse(id);
        //        }
        //        catch
        //        {
        //            return "套餐序号格式不正确";
        //        }
        //    }
        //    if (string.IsNullOrEmpty(Title))
        //    {
        //        return "套餐标题不能为空";
        //    }
        //    if (string.IsNullOrEmpty(Dorder))
        //    {
        //        return "套餐排序不能为空";
        //    }
        //    else
        //    {
        //        try
        //        {
        //            intd = int.Parse(Price);
        //        }
        //        catch
        //        {
        //            return "套餐排序格式不正确";
        //        }
        //    }
        //    if (string.IsNullOrEmpty(Price))
        //    {
        //        return "套餐价格不能为空";
        //    }
        //    else
        //    {
        //        try
        //        {
        //            dupr = double.Parse(Price);
        //        }
        //        catch
        //        {
        //            return "套餐价格不正确";
        //        }
        //    }
        //    if (string.IsNullOrEmpty(Image))
        //    {
        //        return "套餐图片不能为空";
        //    }
        //    SalonPackageService salser = new SalonPackageService();
        //    SalonPackage salp = new SalonPackage();
        //    salp.Id = intid;
        //    salp.Title = Title;
        //    salp.Status = 1;
        //    salp.Price = dupr;
        //    salp.Image = Image;
        //    salp.Dorder = intd;
        //    salp.Content = Content;
        //    int cou = salser.UpdateSalonPackage(salp);
        //    return cou.ToString();
        //}

        ///// <summary>
        ///// 删除套餐
        ///// </summary>
        ///// <returns></returns>
        //public string DelSalonPackage(string id)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        return "-1";
        //    }
        //    int intid = 0;
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        return "套餐序号不能为空";
        //    }
        //    else
        //    {
        //        try
        //        {
        //            intid = int.Parse(id);
        //        }
        //        catch
        //        {
        //            return "套餐序号格式不正确";
        //        }
        //    }
        //    SalonPackageService salser = new SalonPackageService();
        //    SalonPackage salp = new SalonPackage();
        //    salp.Id = intid;
        //    salp.Status = 0;
        //    int cou = salser.UpdateSalonPackage(salp);
        //    return cou.ToString();
        //}



        ///// <summary>
        ///// 扫码记录
        ///// </summary>
        ///// <param name="page"></param>
        ///// <returns></returns>
        //public ActionResult Scanrec(string page)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        Response.Redirect("/manage/LoginEnd"); return View();
        //    }
        //    ScanrecService sca = new ScanrecService();
        //    string cou = sca.GetScanrecCou(manage.UID);
        //    string couz = cou.Split('|')[0];
        //    string couj = cou.Split('|')[1];

        //    int pagesize = 10;
        //    int pageindex = 1;
        //    if (!string.IsNullOrEmpty(page))
        //    {
        //        try
        //        {
        //            pageindex = int.Parse(page);
        //        }
        //        catch
        //        { }
        //    }
        //    int rowcount = 0;
        //    int pagecount = 0;
        //    List<Scanrec> lis = new List<SalonModel.Scanrec>();
        //    lis = sca.GetScanrecInfo(manage.UID, pageindex, pagesize, out rowcount, out pagecount);
        //    string content = "";
        //    for (int i = 0; i < lis.Count; i++)
        //    {
        //        content += "<tr>";
        //        DateTime date = new DateTime();
        //        date = DateTime.Parse(lis[i].Date.ToString());
        //        content += "<td class='table_cell product'>" + lis[i].Code + "</td><td class='table_cell product'>" + date.ToString("yyyy年MM月dd日HH时mm分ss秒") + "</td></tr>";
        //    }
        //    string pagecontent = null;
        //    string first = pageindex == 1 ? "#" : "";
        //    string prv = pageindex == 1 ? "1#" : (pageindex - 1).ToString();
        //    string next = pageindex == pagecount ? pagecount + "#" : (pageindex + 1).ToString();
        //    string end = pageindex == pagecount ? "#" : "";
        //    if (pagecount > 1)
        //    {
        //        pagecontent = "<span class='page_nav_area'><a href='/manage/Scanrec/" + first + "'class='btn page_first'>首页</a> <a href='/manage/Scanrec/" + prv + "'class='btn page_prev'><i class='arrow'></i></a> <span class='page_num'><label>" + pageindex + "</label><span class='num_gap'> /</span> <label>" + pagecount + "</label></span> <a href='/manage/Scanrec/" + next + "'class='btn page_next'><i class='arrow'></i></a> <a href='/manage/Scanrec/" + pagecount.ToString() + end + "'class='btn page_last'>末页</a> </span> ";
        //    }
        //    ViewBag.content = content;
        //    ViewBag.pagecontent = pagecontent;
        //    ViewBag.ban = ban;
        //    ViewBag.couz = couz;
        //    ViewBag.couj = couj;
        //    return View();
        //}

        ///// <summary>
        ///// 投诉建议记录
        ///// </summary>
        ///// <param name="page"></param>
        ///// <returns></returns>
        //public ActionResult Tou(string page)
        //{
        //    if (string.IsNullOrEmpty(manage.UID))
        //    {
        //        Response.Redirect("/manage/LoginEnd"); return View();
        //    }
        //    AdviceService sca = new AdviceService();
        //    int pagesize = 10;
        //    int pageindex = 1;
        //    if (!string.IsNullOrEmpty(page))
        //    {
        //        try
        //        {
        //            pageindex = int.Parse(page);
        //        }
        //        catch
        //        { }
        //    }
        //    int rowcount = 0;
        //    int pagecount = 0;
        //    List<Advice> lis = new List<SalonModel.Advice>();
        //    lis = sca.GetAdviceList(manage.UID, pageindex, pagesize, out rowcount, out pagecount);
        //    string content = "";
        //    for (int i = 0; i < lis.Count; i++)
        //    {
        //        content += "<tr>";
        //        DateTime date = new DateTime();
        //        date = DateTime.Parse(lis[i].Date.ToString());
        //        content += "<td class='table_cell product'>" + lis[i].Content + "</td><td class='table_cell product'>";
        //        content += (lis[i].Kind == 2 ? "建议" : "投诉");
        //        content += "</td><td class='table_cell product'>" + lis[i].Cell + "</td><td class='table_cell product'>" + date.ToString("yyyy年MM月dd日HH时mm分ss秒") + "</td>";
        //        content += "</tr>";
        //    }
        //    string pagecontent = null;
        //    string first = pageindex == 1 ? "#" : "";
        //    string prv = pageindex == 1 ? "1#" : (pageindex - 1).ToString();
        //    string next = pageindex == pagecount ? pagecount + "#" : (pageindex + 1).ToString();
        //    string end = pageindex == pagecount ? "#" : "";
        //    if (pagecount > 1)
        //    {
        //        pagecontent = "<span class='page_nav_area'><a href='/manage/Tou/" + first + "'class='btn page_first'>首页</a> <a href='/manage/Tou/" + prv + "'class='btn page_prev'><i class='arrow'></i></a> <span class='page_num'><label>" + pageindex + "</label><span class='num_gap'> /</span> <label>" + pagecount + "</label></span> <a href='/manage/Tou/" + next + "'class='btn page_next'><i class='arrow'></i></a> <a href='/manage/Tou/" + pagecount.ToString() + end + "'class='btn page_last'>末页</a> </span> ";
        //    }
        //    ViewBag.content = content;
        //    ViewBag.pagecontent = pagecontent;
        //    ViewBag.ban = ban;
        //    return View();
        //}


        #endregion


        #region 审核

        #region  管理员审核美发店，搜索参数：SaleName

        public ActionResult ChecksSalon(int pageIndex = 1, int pageSize = 10, string SaleName = null)
        {

            if (string.IsNullOrEmpty(manage.UID.ToString()))
            {
                Response.Redirect("/home/index");
                return View();
            }
            if (ViewBag.admim == "false")
            {
                return null;
            }

            if (Session["AddOrUpdateSalesclerk"] != null)
            {
                ViewBag.message = Session["AddOrUpdateSalesclerk"]; Session.Remove("AddOrUpdateSalesclerk");
            }
            else
            {
                ViewBag.msg = "";
            }

            var salonService = new SalonService();



            var list = salonService.GetSalonList(pageIndex, pageSize, SaleName, () =>
            {
                Expression<Func<t_salemanager, bool>> exp = null;
                exp = c => true;
                return salemanagerservice.LoadEntities(exp);
            });

            if (!string.IsNullOrWhiteSpace(SaleName))
                ViewBag.page = string.Format("/Manage/ChecksSalon?pageIndex=(p)&pageSize={0}&SaleName={1}", pageSize, SaleName);
            else
                ViewBag.page = "/Manage/ChecksSalon/(p)";
            return View(list);
        }



        #endregion

        #region  管理员审核美发店 查看详情
        public ActionResult SalonInfo(string uid)
        {
            if (string.IsNullOrEmpty(manage.UID.ToString()))
            {
                Response.Redirect("/home/index");
                return View();
            }
            if (ViewBag.admim == "false")
            {
                return null;
            }
            var list = salonservice.GetSalonByUid(new Guid(uid));

            return View(list);
        }
        #endregion

        /// <summary>
        /// 添加或修改销售人员
        /// </summary>
        /// <param name="id">销售人员表ID</param>
        /// <param name="insert_update">0:添加  1：修改</param>
        /// <returns></returns>
        public ActionResult AddOrUpdateSalesclerk(string s_id, string saleName = null)
        {
            bool flag = false;
            if (!string.IsNullOrEmpty(s_id))
            {
                var tempmodel = salemanagerservice.LoadEntities(c => c.sm_s_uid == new Guid(s_id)).FirstOrDefault();
                Mapper.CreateMap<t_salemanager, SaleManager>();
                Mapper.CreateMap<SaleManager, t_salemanager>();
                try
                {
                    saleName = Server.UrlDecode(saleName ?? "");
                    if (tempmodel == null) //添加
                    {
                        SaleManager manager = new SaleManager()
                        {
                            sm_date = DateTime.Now,
                            sm_editdate = DateTime.Now,
                            sm_name = saleName,
                            sm_oldname = string.Empty,
                            sm_s_uid = s_id
                        };
                        salemanagerservice.AddEntity(Mapper.Map<t_salemanager>(manager));
                        flag = true;
                    }
                    else  //修改
                    {
                        tempmodel.sm_oldname = tempmodel.sm_name;
                        tempmodel.sm_name = saleName;
                        tempmodel.sm_editdate = DateTime.Now;
                        flag = salemanagerservice.UpdateEntity(tempmodel);
                    }
                }
                catch
                {
                    flag = false;
                }
            }

            Session["AddOrUpdateSalesclerk"] = flag ? "更改销售人员姓名成功 !" : "更改销售人员姓名失败 !";

            return RedirectToAction("ChecksSalon");
        }

        #endregion

        /// <summary>
        /// 根据UID 获取美发店图片信息
        /// </summary>
        /// <returns></returns>
        public ActionResult ImageInfo()
        {
            if (string.IsNullOrEmpty(manage.UID.ToString()))
            {
                Response.Redirect("/manage/LoginEnd");
            }
            ISalonImage service = new SalonImageInfo();
            var info = service.GetImagesInfo(manage.UID);
            ViewBag.Token = GetToken();
            return View(info);
        }


        /// <summary>
        /// 图片上传至七牛
        /// </summary>
        /// <returns></returns>
        public string SaveIamgeAlone(string image, int kind)
        {
            var result = string.Empty;
            try
            {
                var token = GetToken();
                PutExtra extra = new PutExtra();
                Qiniu.IO.IOClient client = new IOClient();
                var salonimagemodel = new t_salonimageinfo();
                var date = DateTime.Now;
                var key = date.Year.ToString() + (date.Month + 1).ToString() + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() + date.Second.ToString() + date.Millisecond.ToString() + ".jpg";
                var butter = GetQiNiuBase64(image);
                Stream stream = new MemoryStream(butter);
                PutRet ret = client.Put(token, key, stream, extra);
                if (!string.IsNullOrWhiteSpace(ret.key))
                {
                    salonimagemodel = salonimageservice.LoadEntities(c => c.si_s_uid == manage.UID && c.si_kind == 5 && c.si_status == 1).FirstOrDefault();
                    if (salonimagemodel == null)
                    {
                        var tempmodel = new t_salonimageinfo();
                        tempmodel.si_status = 1;
                        tempmodel.si_kind = kind;
                        tempmodel.si_s_uid = manage.UID;
                        tempmodel.si_photo = "/" + ret.key;
                        salonimageservice.AddEntity(tempmodel);
                    }
                    else
                    {
                        salonimagemodel.si_photo = "/" + ret.key;
                        salonimagemodel.si_kind = kind;
                        salonimageservice.UpdateEntity(salonimagemodel);
                    }
                }
                result = string.Format("http://bobosquad.qiniudn.com/{0}?imageView2/0/w/236/h/177", ret.key);
            }
            catch { }
            return result;
        }

        //获取图片的base64码
        protected byte[] GetQiNiuBase64(string imagefile)
        {
            if (string.IsNullOrWhiteSpace(imagefile))
                return null;
            if (imagefile.Length <= 50)
            {
                return null;
            }
            var length = imagefile.IndexOf(','); //过滤 js 生成 Base64 前缀
            var temp = imagefile.Substring(length + 1);

            byte[] buttertilte = Convert.FromBase64String(temp);
            return buttertilte;
        }

    }


}
