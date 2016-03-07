using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WeiSalon.Alipay
{
    public class Unity
    {
        /// <summary>
        /// 支付宝提交订单
        /// </summary>
        /// <param name="orderNo">商户订单号</param>
        /// <param name="orderName">订单名称</param>
        /// <param name="money">付款金额</param>
        /// <param name="orderDesc">订单描述</param>
        /// <returns></returns>     
        public static string Order(string orderNo, string orderName, string money, string orderDesc)
        {
            string hostUrl = ConfigurationManager.AppSettings["Url"];

            //商户订单号
            string out_trade_no = orderNo;
            //订单名称
            string subject = orderName;
            //订单描述
            string body = orderDesc;
            //付款金额
            string total_fee = money;
            //通知页面路
            string notify_url = hostUrl + ConfigurationManager.AppSettings["NotifyUrl"];
            string return_url = hostUrl + ConfigurationManager.AppSettings["ReturnUrl"];
            //支付类型
            string payment_type = "1";
            //卖家支付宝帐户
            string seller_email = "pay@hairbobo.com";
            //商品展示地址
            string show_url = hostUrl;

            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("service", "create_direct_pay_by_user");
            sParaTemp.Add("payment_type", payment_type);
            sParaTemp.Add("notify_url", notify_url);
            sParaTemp.Add("return_url", return_url);
            sParaTemp.Add("seller_email", seller_email);
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("body", body);
            sParaTemp.Add("total_fee", total_fee);
            sParaTemp.Add("show_url", show_url);

            //建立请求
            string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
            return sHtmlText;
        }

        #region 支付宝GET
        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public static SortedDictionary<string, string> GetRequestGet()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = HttpContext.Current.Request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], HttpContext.Current.Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }
        #endregion

        public static string GenerateRequest()
        {
            ////////////////////////////////////////////请求参数////////////////////////////////////////////
            //支付类型
            string payment_type = "1";
            string hostUrl = ConfigurationManager.AppSettings["Url"];

            //必填，不能修改
            //服务器异步通知页面路径
            string notify_url = hostUrl + "Finance/notify_url";
            //需http://格式的完整路径，不能加?id=123这类自定义参数

            //页面跳转同步通知页面路径
            string return_url = hostUrl + "Finance/return_url";
            //需http://格式的完整路径，不能加?id=123这类自定义参数，不能写成http://localhost/

            //卖家支付宝帐户
            string seller_email = "alipay.account";
            //必填

            //商户订单号
            string out_trade_no = "out_trade_no";
            //商户网站订单系统中唯一订单号，必填

            //订单名称
            string subject = "order";
            //必填

            //付款金额
            string price = "100";
            //必填

            //商品数量
            string quantity = "1";
            //必填，建议默认为1，不改变值，把一次交易看成是一次下订单而非购买一件商品

            //物流费用
            string logistics_fee = "0.00";
            //必填，即运费
            //物流类型
            string logistics_type = "EXPRESS";
            //必填，三个值可选：EXPRESS（快递）、POST（平邮）、EMS（EMS）
            //物流支付方式
            string logistics_payment = "BUYER_PAY";
            //必填，两个值可选：SELLER_PAY（卖家承担运费）、BUYER_PAY（买家承担运费）
            //订单描述

            string body = "违章查询接口";
            //商品展示地址
            string show_url = "";
            //需以http://开头的完整路径，如：http://www.xxx.com/myorder.html

            //收货人姓名
            string receive_name = "";
            //如：张三

            //收货人地址
            string receive_address = "";
            //如：XX省XXX市XXX区XXX路XXX小区XXX栋XXX单元XXX号

            //收货人邮编
            string receive_zip = "";
            //如：123456

            //收货人电话号码
            string receive_phone = "";
            //如：0571-88158090

            //收货人手机号码
            string receive_mobile = "";
            //如：13312341234
            
            ////////////////////////////////////////////////////////////////////////////////////////////////
            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("service", "trade_create_by_buyer");
            sParaTemp.Add("payment_type", payment_type);
            sParaTemp.Add("notify_url", notify_url);
            sParaTemp.Add("return_url", return_url);
            sParaTemp.Add("seller_email", seller_email);
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("price", price);
            sParaTemp.Add("quantity", quantity);
            //sParaTemp.Add("logistics_fee", logistics_fee);
            //sParaTemp.Add("logistics_type", logistics_type);
            //sParaTemp.Add("logistics_payment", logistics_payment);
            sParaTemp.Add("body", body);
            sParaTemp.Add("show_url", show_url);
            sParaTemp.Add("receive_name", receive_name);
            sParaTemp.Add("receive_address", receive_address);
            sParaTemp.Add("receive_zip", receive_zip);
            sParaTemp.Add("receive_phone", receive_phone);
            sParaTemp.Add("receive_mobile", receive_mobile);

            //建立请求
            string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
            return sHtmlText;
        }

    }
}