using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BoboIService;
using BoboModel;
using Qiniu.RS;
using WeiSalonV2.Models;
using System.Configuration;
using WeiSalon.Alipay;


namespace WeiSalonV2.Controllers
{
    public class RegisterController : Controller
    {


        public ActionResult Pay()
        {
            SalonService salon = new SalonService();
            SalonSimple ss = new SalonSimple();
            if (Session["W_B_UID"] != null && Session["W_B_UID"].ToString() != "")
            {
                ss = salon.GetSalonSimpleInfo(new Guid(Session["W_B_UID"].ToString()));
            }
            else
            {
                Response.Redirect("/Home/index");
                return View();
            }
            string sHtmlText = "";
            if (ss.Status != 3)
            {
                Response.Redirect("/Register/Check");
                return View();
            }
            else
            {
                try
                {
                    OrderService ords = new OrderService();
                    Order od = new Order();
                    //根据配置文件获取支付宝交易过期时间
                    string t = ConfigurationManager.AppSettings["Alipay.Timeout"];
                    t = t.Substring(0, t.Length - 1);
                    DateTime dt = DateTime.Now.AddMinutes(-int.Parse(t));
                    od = ords.GetOrderInfo(ss.Uid, dt);
                    if (od == null)
                    {
                        //支付宝支付   
                        od = new Order();
                        od.Suid = ss.Uid;
                        od.Name = ss.Nickname + "支付订单";
                        od.Money = 0.01;
                        od.Orid = GetOrdId();
                        od.Paydate = DateTime.Now;
                        od.Status = 0;
                        od.Date = DateTime.Now;
                        int cou = ords.AddOrder(od);
                        if (cou > 0)
                        {
                            sHtmlText = Unity.Order(od.Orid.ToString(),
                                ConfigurationManager.AppSettings["OrderName"],
                                ConfigurationManager.AppSettings["OrderPrice"]
                                , ConfigurationManager.AppSettings["OrderDetil"]);
                            Response.Write(sHtmlText);
                        }
                    }
                    else
                    {
                        sHtmlText = Unity.Order(od.Orid.ToString(),
                            ConfigurationManager.AppSettings["OrderName"],
                            ConfigurationManager.AppSettings["OrderPrice"]
                            , ConfigurationManager.AppSettings["OrderDetil"]);
                        Response.Write(sHtmlText);
                    }
                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message);
                }
            }
            return View();
        }

        //得到订单号16位
        private string GetOrdId()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmffff");
        }

        public ActionResult Pact()
        {
            return View();
        }

        public ActionResult Check()
        {
            SalonService salon = new SalonService();
            SalonSimple ss = new SalonSimple();
            if (Session["W_B_UID"] != null && Session["W_B_UID"].ToString() != "")
            {
                ss = salon.GetSalonSimpleInfo(new Guid(Session["W_B_UID"].ToString()));
            }
            else
            {
                Response.Redirect("/Home/index");
                return View();
            }
            return View(ss);
        }

        public ActionResult Register(string uid)
        {

            SalonService salon = new SalonService();
            SalonSimple ss = new SalonSimple();
            ViewBag.Token = GetToken();
            if (Session["W_B_UID"] != null && Session["W_B_UID"].ToString() != "")
            {
                ss = salon.GetSalonSimpleInfo(new Guid(Session["W_B_UID"].ToString()));
            }
            if (!string.IsNullOrEmpty(uid))
            {
                ss = salon.GetSalonSimpleInfo(new Guid(uid));
                SalonService salser = new SalonService();
                List<AllSalon> lis = new List<AllSalon>();
                lis = salser.GetSalonInfo(new Guid(uid), 3);//环境照
                ViewBag.img1 = lis[0].Photo;
                ViewBag.img2 = lis[1].Photo;
                ViewBag.img3 = lis[2].Photo;
                lis = salser.GetSalonInfo(new Guid(uid), 1);//营业执照
                ViewBag.img4 = lis[0].Photo;
            }
            return View(ss);
        }

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

        /// <summary>
        /// 判断登录名是否存在
        /// </summary>
        /// <param name="loginID"></param>
        /// <returns></returns>
        public string CHKLoginID(string loginID)
        {
            var salon = new SalonService();
            var sal = new SalonSimple();
            try
            {
                sal = salon.CHKInfo(loginID);
                if (sal != null && sal.Uid.ToString() != "")
                {
                    return "0";
                }
            }
            catch
            {
                return "0";
            }
            return "1";
        }

        /// <summary>
        /// 注销登陆
        /// </summary>
        /// <returns></returns>
        public string LoginOut()
        {
            try
            {
                Session.Remove("W_B_UID");
            }
            catch
            { }
            return "1";
        }

        /// <summary>
        /// 注册
        /// </summary>                                 
        /// <param name="uid"></param>
        /// <param name="loginID">登录名（手机号）</param>
        /// <param name="PWD">密码</param>
        /// <param name="B_Name">沙龙名称</param>
        /// <param name="B_Phone">沙龙电话</param>
        /// <param name="B_LinkRealName">沙龙联系人</param>
        /// <param name="add">沙龙地址</param>
        /// <param name="B_Summary">沙龙简介</param>
        /// <param name="B_Image1">营业执照</param>
        /// <param name="B_Image2">Logo</param>
        /// <param name="B_Image3">环境照</param>
        /// <param name="B_Image4">环境照</param>
        /// <param name="B_Image5">环境照</param>
        /// <returns></returns>
        public string CHKregister(string loginID, string PWD, string B_Name, string B_Phone, string B_LinkRealName, string add, string B_Summary, string B_Image1, string B_Image2, string B_Image3, string B_Image4, string B_Image5, string B_Wxname, string Lng, string Lat, int istemporary, string recommend)
        {
            SalonService salon = new SalonService();
            SalonSimple ss = new SalonSimple();
            if (Session["W_B_UID"] != null && Session["W_B_UID"].ToString() != "")
            {
                ss = salon.GetSalonSimpleInfo(new Guid(Session["W_B_UID"].ToString()));
            }
            if (string.IsNullOrEmpty(loginID))
            {
                return "登录名不能为空";
            }
            if (string.IsNullOrEmpty(PWD))
            {
                return "密码不能为空";
            }
            if (string.IsNullOrEmpty(B_Phone))
            {
                return "电话号码不能为空";
            }
            if (string.IsNullOrEmpty(add))
            {
                return "地址不能为空";
            }
            if (string.IsNullOrEmpty(B_LinkRealName))
            {
                return "联系人不能为空";
            }
            if (string.IsNullOrEmpty(B_Image1))
            {
                return "营业执照不能为空";
            }
            if (string.IsNullOrEmpty(B_Image2))
            {
                return "LOGO不能为空";
            }
            try
            {
                //解密参数值
                loginID = WeiSalonV2.Models.AESHelper.DecryptString(loginID);
                PWD = WeiSalonV2.Models.AESHelper.DecryptString(PWD);
                //去掉参数中的转移字符
                loginID = new string((from c in loginID.ToCharArray() where !char.IsControl(c) select c).ToArray());
                PWD = new string((from c in PWD.ToCharArray() where !char.IsControl(c) select c).ToArray());
                if (string.IsNullOrEmpty(loginID))
                {
                    return "登录名验证失败";
                }
                if (string.IsNullOrEmpty(PWD))
                {
                    return "密码验证失败";
                }
            }
            catch
            {
                return "验证帐号/密码失败";
            }
            SalonSimple sal = new SalonSimple();
            sal.Status = 1;
            sal.Cell = B_Phone.Trim();
            sal.Date = DateTime.Now;
            sal.Email = loginID;
            sal.Pwd = MD5.MDString(PWD);
            sal.Kind = istemporary == 1 ? 0 : 1;
            sal.Logo = B_Image2.Replace("http://bobosquad.qiniudn.com", "");
            sal.Nickname = B_Name;
            sal.Address = add;
            sal.LinkName = B_LinkRealName;
            sal.Summary = SQLFilter.checkStr(SQLFilter.ReplaceUnStr(SQLFilter.ReplaceStr(Server.UrlDecode(B_Summary.Trim()))));
            sal.Lat = Lat;
            sal.Lon = Lng;
            sal.Price = "";
            sal.Businessdate = "10:00~22:00";
            sal.Opendate = null;
            sal.Wxname = string.IsNullOrEmpty(B_Wxname) ? "" : B_Wxname;
            sal.Recommend = string.IsNullOrEmpty(recommend) ? "" : recommend;
            int ADDCou = 0;
            SalonSimple salc = salon.CHKInfo(loginID);
            if (salc == null)
            {
                //不是修改                 
                //未注册过                  
                AllSalon alls = new AllSalon();
                try
                {
                    sal.Uid = Guid.NewGuid();
                    ADDCou = salon.AddSalon(sal);
                    if (ADDCou > 0)
                    {
                        ADDCou = 0;
                        //注册成功 添加环境图等 
                        //添加营业执照图片
                        alls.SiKind = 1;
                        alls.Suid = sal.Uid;
                        alls.Sistatus = 1;
                        alls.Photo = B_Image1.Replace("http://bobosquad.qiniudn.com", "");
                        salon.AddAllSalon(alls);

                        //添加环境图
                        alls = new AllSalon();
                        alls.SiKind = 3;
                        alls.Suid = sal.Uid;
                        alls.Sistatus = 1;
                        alls.Photo = B_Image3.Replace("http://bobosquad.qiniudn.com", "");
                        salon.AddAllSalon(alls);
                        alls.Photo = B_Image4.Replace("http://bobosquad.qiniudn.com", "");
                        salon.AddAllSalon(alls);
                        alls.Photo = B_Image5.Replace("http://bobosquad.qiniudn.com", "");
                        ADDCou = salon.AddAllSalon(alls);
                        if (ADDCou > 0)
                        {
                            Session["W_B_UID"] = sal.Uid.ToString();
                            //所有信息添加成功
                            return "1";
                        }
                        else
                        {
                            return "基本信息添加成功 图片可能添加失败";
                            //基本信息添加成功 图片可能添加失败
                        }
                    }
                    else
                    {
                        return "信息添加失败";
                    }
                }
                catch (Exception ex)
                {
                    return "异常：信息添加失败";
                }
            }
            else
            {
                //防恶意修改
                if (ss.Uid != salc.Uid)
                {
                    return "帐号信息和提交信息不符";
                }
                //修改
                //如果是重新提交 
                sal.Uid = ss.Uid;
                sal.Email = salc.Email;
                ADDCou = salon.UpdateRegSalon(sal);
                if (ADDCou > 0)
                {
                    try
                    {
                        ADDCou = 0;
                        List<AllSalon> lis = new List<AllSalon>();
                        lis = salon.GetSalonInfo(ss.Uid, 1);//营业执照
                        salon.UpdateImageInfo(lis[0].Id.ToString(), B_Image1, 1);
                        lis = salon.GetSalonInfo(ss.Uid, 3);//营业执照
                        salon.UpdateImageInfo(lis[0].Id.ToString(), B_Image3, 1);
                        salon.UpdateImageInfo(lis[1].Id.ToString(), B_Image4, 1);
                        ADDCou = salon.UpdateImageInfo(lis[2].Id.ToString(), B_Image5, 1);
                        return ADDCou.ToString();
                    }
                    catch
                    { }
                }
                else
                {
                    return "信息修改失败";
                }
            }
            return "未知错误";
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="loginID"></param>
        /// <param name="PWD"></param>
        /// <returns></returns>
        public string CHKLogin(string loginID, string PWD)
        {
            if (string.IsNullOrEmpty(loginID))
            {
                return "登录名不能为空";
            }
            if (string.IsNullOrEmpty(PWD))
            {
                return "密码不能为空";
            }
            try
            {
                //解密参数值
                loginID = AESHelper.DecryptString(loginID);
                PWD = AESHelper.DecryptString(PWD);
                //去掉参数中的转移字符
                loginID = new string((from c in loginID.ToCharArray() where !char.IsControl(c) select c).ToArray());
                PWD = new string((from c in PWD.ToCharArray() where !char.IsControl(c) select c).ToArray());
                if (string.IsNullOrEmpty(loginID))
                {
                    return "登录名验证失败";
                }
                if (string.IsNullOrEmpty(PWD))
                {
                    return "密码验证失败";
                }
            }
            catch
            { }
            SalonService salon = new SalonService();
            SalonSimple sal = salon.GetLoginInfo(loginID, FormsAuthentication.HashPasswordForStoringInConfigFile(PWD, "MD5"));
            if (sal.Email == "CCCCCCCCCCCaa")
            {
                sal.Status = 4;
            }
            if (sal == null)
            {
                //return "登陆失败 请检查登录名和密码";
                return "查询失败，或没有查到相关注册信息";
            }
            else
            {
                if (string.IsNullOrEmpty(sal.Uid.ToString()))
                {
                    return "帐号错误";
                }
                else
                {
                    string turl = "";
                    string stat = "1";
                    Session["W_B_UID"] = sal.Uid.ToString();
                    if (sal.Opendate < DateTime.Now)
                    {
                        if (sal.Email != "CCCCCCCCCCCaa")
                        {
                            //如果开通超过1年 那么就是已经过期
                            sal.Status = 3;
                            salon.UpdatestatusInfo(sal.Uid.ToString(), 3);
                        }
                    }
                    switch (sal.Status)
                    {
                        case 0:
                            //帐号禁用
                            turl = "";
                            stat = "登录失败 帐号已被禁用";
                            Session["W_B_UID"] = "";
                            break;
                        case 1:
                            //审核中
                            turl = "/Register/check";
                            break;
                        case 2:
                            //审核未通过 
                            turl = "/Register/check";
                            break;
                        case 3:
                            //审核已通过未付款  
                            turl = "/Register/check";
                            stat = "2";
                            break;
                        case 4:
                            //审核已通过已付款                                                                                                   
                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                1,
                                sal.Uid.ToString(),
                                DateTime.Now,
                                DateTime.Now.AddDays(1),
                                false,
                                sal.Cell.Trim() + "$bobo$" + sal.Nickname + "$bobo$" + sal.Logo
                                );
                            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                            HttpContext.Response.Cookies.Add(authCookie);
                            turl = "/manage/Index";
                            break;
                    }
                    return stat + "|" + turl;
                }
            }
            return "未知错误";
        }

    }
}
