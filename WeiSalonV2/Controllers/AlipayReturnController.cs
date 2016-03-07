using BoboIService;
using BoboModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WeiSalon.Alipay;
using WeiSalonV2.Models;

namespace WeiSalonV2.Controllers
{
    public class AlipayReturnController : Controller
    {
        //
        // GET: /AlipayReturn/
        [HttpPost]
        public ActionResult NotifyUrl()
        {
            var tbsign = Request.Form["sign"];
            var notifyid = Request.Form["notify_id"];
            var status = Request.Form["trade_status"];
            var orderid = Request.Form["out_trade_no"];
            var payid = Request.Form["trade_no"];
            string retval = "tbsign=" + tbsign + ";notifyid=" + notifyid + ";status=" + status + ";orderid=" + orderid + ";payid=" + payid;
            try
            {
                if (status == "TRADE_SUCCESS")
                {
                    var aArry = GetRequestPost(0);
                    var notify = new Notify();
                    bool verifyResult = notify.Verify(aArry, notifyid, tbsign, "0");
                    if (verifyResult)
                    {
                        try
                        {
                            OrderService ords = new OrderService();
                            Order od = new Order();
                            od = ords.GetOrderInfo(orderid);
                            if (od != null && od.Suid.ToString() != "")
                            {
                                int cou = ords.UpdateOrderstatus(od.Id, 1);
                                if (cou > 0)
                                {
                                    SalonService salon = new SalonService();
                                    cou = salon.UpdatestatusInfo(od.Suid.ToString(), 4);
                                    Common.Logger(retval + ";cou=" + cou + (cou > 0 ? "" : "-沙龙表修改失败"));
                                }
                                else
                                {
                                    Common.Logger(retval + ";cou=" + cou + "-订单表修改失败");
                                }
                            }
                            else
                            {
                                Common.Logger(retval + "-未取到订单信息");
                            }
                        }
                        catch (Exception ex)
                        {
                            Common.Logger("AlipayReturnController-NotifyUrl1``````" + retval + ";error=" + ex.Message);
                        }
                    }
                    else
                    {
                        Common.Logger("AlipayReturnController-NotifyUrl-消息不合法``````" + retval);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Logger("AlipayReturnController-NotifyUrl2``````" + retval + ";error=" + ex.Message);
            }
            Response.Write("success");
            return View();
        }


        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public Dictionary<string, string> GetRequestPost()
        {
            int i = 0;
            var sArray = new Dictionary<string, string>();
            NameValueCollection coll;
            coll = Request.Form;
            String[] requestItem = coll.AllKeys;
            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }
            return sArray;
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <param name="type">0 支付宝 1 微信</param> 
        /// <returns>request回来的信息组成的数组</returns>
        private SortedDictionary<string, string> GetRequestPost(int type)
        {
            int i = 0;
            var sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            try
            {
                coll = type == 0 ? Request.Form : Request.QueryString;
                var requestItem = coll.AllKeys;
                for (i = 0; i < requestItem.Length; i++)
                {
                    sArray.Add(requestItem[i], coll[requestItem[i]]);
                }
                sArray.Remove("sign");
                if (type == 0)
                {
                    sArray.Remove("sign_type");
                }
            }
            catch (Exception ex)
            {
            }
            return sArray;
        }

        public ActionResult ReturnUrl()
        {
            int cou = 0;
            //System.IO.StreamWriter sw = new System.IO.StreamWriter(Server.MapPath(".") + "\\" + "ReturnUrl.txt");
            //sw.WriteLine("ReturnUrl");
            //sw.Flush();
            //sw.Close();
            //获取支付宝POST过来通知消息
            SortedDictionary<string, string> parameter = Unity.GetRequestGet();
            if (parameter.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                var verifyResult = aliNotify.Verify(parameter, parameter["notify_id"], parameter["sign"], "1");
                if (verifyResult)//验证成功
                {
                    var out_trade_no = parameter["out_trade_no"];     //商户订单号
                    var trade_no = parameter["trade_no"];         //支付宝交易号
                    var trade_status = parameter["trade_status"];     //交易状态
                    var price = parameter["total_fee"];        //金额
                    if (trade_status == "TRADE_FINISHED" || trade_status == "TRADE_SUCCESS")
                    {
                        cou = 1;
                        //----------------Jiang----------------
                        ISalonService salon = new SalonService();

                        if (Session["W_B_UID"] != null)
                        {
                            string UID = Session["W_B_UID"].ToString();
                            int Insert = 0;
                            try
                            {
                                Insert = salon.UpdatestatusInfo(UID, 4);//支付完成后修改用户支付状态  s_status:4
                            }
                            catch { }
                            if (Insert > 0)
                            {
                                SalonSimple salonsimple = salon.GetSalonSimpleInfo(new Guid(UID));
                                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                    1,
                                    salonsimple.Uid.ToString(),
                                    DateTime.Now,
                                    DateTime.Now.AddDays(1),
                                    false,
                                    salonsimple.Cell.Trim() + "$bobo$" + salonsimple.Nickname + "$bobo$" + salonsimple.Logo
                                    );
                                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                                HttpContext.Response.Cookies.Add(authCookie);  //往Cookie中添加用户信息
                                Response.Redirect("/Home/Index");
                            }
                        }
                        //-------------------------------------
                    }
                    else
                    {
                        cou = 2;
                    }
                }
                else
                {
                    cou = 3;
                }
            }
            else
            {
                cou = 4;
            }
            return View(cou);
        }
    }
}
