/*----------------------------------------------------------------
// 莫笛思
// 版权所有。
//
// 文件名：Tools.cs
// 功能描述：
// 
// 创建人  ： KK
// 创建标识： 2015/11/9 14:03:49
// 
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Security;

namespace WeiSalonV2.Models
{
    public class Tools
    {
        private const string Token = "febe829e3cb74c26b2d3e63c0c0c2bac";


        /// <summary>
        /// 返回验证信息
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="echostr"></param>
        /// <returns></returns>
        public static bool ValWeixin(string signature, string timestamp, string nonce, string echostr)
        {
            string[] arry = { Token, timestamp, nonce };
            Array.Sort(arry);
            string wxStr = string.Join("", arry);
            wxStr = FormsAuthentication.HashPasswordForStoringInConfigFile(wxStr, "SHA1");
            try
            {
                if (wxStr != null && wxStr.ToLower() == signature)
                {
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                //Log.InsertErrorInfo("Tools.ValWeixin", ex.Message);
                return true;
            }
        }


        /// <summary>
        /// 发起微信请求
        /// </summary>
        /// <param name="url">请求链接</param>
        /// <param name="interfaceName">接口名字</param>
        /// <param name="method">请求方式</param>
        /// <returns></returns>
        public static string StartRequest(string url, string interfaceName, string method)
        {
            //            var model = new Wx_Request()
            //            {
            //                wr_date = DateTime.Now,
            //                wr_post = url,
            //                wr_interfacename = interfaceName
            //            };
            // Service.AddRequest(model);
            var responseBody = string.Empty;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Accept = "*/*";
                var response = (HttpWebResponse)request.GetResponse();
                var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                responseBody = stream.ReadToEnd();
                stream.Close();
                response.Close();
                //                dic = JsonTools.JsonToObject<Dictionary<string, object>>(responseBody);
            }
            catch (Exception ex)
            {
                //Log.InsertErrorInfo("string.StartRequest----" + interfaceName, ex.Message);
            }
            return responseBody;
        }


        /// <summary>
        /// 带Json 请求
        /// </summary>
        /// <param name="url">url请求</param>
        /// <param name="json">json数据</param>
        /// <param name="interfaceName">请求接口名字</param>
        /// <param name="method">请求方式</param>
        /// <returns></returns>
        public static Dictionary<string, object> StartRequest(string url, string json, string interfaceName, string method)
        {
            var dic = new Dictionary<string, object>();
            //            var model = new Wx_Request()
            //            {
            //                wr_date = DateTime.Now,
            //                wr_post = url,
            //                wr_interfacename = interfaceName
            //            };
            //            Service.AddRequest(model);
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Accept = "*/*";
                byte[] data = Encoding.UTF8.GetBytes(json);
                var outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();

                var response = (HttpWebResponse)request.GetResponse();
                var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string responseBody = stream.ReadToEnd();
                stream.Close();
                response.Close();

                dic = JsonTools.JsonToObject<Dictionary<string, object>>(responseBody);
                //return responseBody;
            }
            catch (Exception ex)
            {
                //Log.InsertErrorInfo("dic.StartRequest----" + interfaceName, ex.Message);
            }

            return dic;
        }



        /// <summary>
        /// 带Json https 安全请求支付接口 返回xml
        /// </summary>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <param name="interfaceName"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string StartRequestForXml(string url, string json, string interfaceName, string method)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                string cert = @"D:\apiclient_cert.p12";
                string password = ConfigurationManager.AppSettings["mch_id"];
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                X509Certificate cer = new X509Certificate(cert, password);
                request.ClientCertificates.Add(cer);

                request.Method = method;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Accept = "*/*";
                byte[] data = Encoding.UTF8.GetBytes(json);
                var outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();

                var response = (HttpWebResponse)request.GetResponse();
                var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string responseBody = stream.ReadToEnd();
                stream.Close();
                response.Close();

                return responseBody;
            }
            catch (Exception ex)
            {
                //Log.InsertErrorInfo("xml.StartRequest----" + interfaceName, ex.Message);
            }
            return "";
        }

        /// <summary>
        /// 微信ssl策略判断
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }


        /// <summary>
        /// 根据字母a到z的顺序把参数排序
        /// </summary>
        /// <param name="dicArrayPre">排序前的参数组</param>
        /// <returns>排序后的参数组</returns>
        public static string SortPara(Dictionary<string, string> dicArrayPre, out SortedDictionary<string, string> dicTemp)
        {
            dicTemp = new SortedDictionary<string, string>(dicArrayPre);

            var prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicTemp)
            {
                prestr.Append(temp.Key + "=" + temp.Value + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();

        }

        /// <summary>
        /// 生成时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimestamp()
        {
            var ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static string Md5(string pwd)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(pwd, "md5");
        }


        /// <summary>
        /// 文本记录日志
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="type"></param>
        public static void Logger(string content)
        {
            try
            {
                var path = @"D:\Achievement\log\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                using (var sw = !File.Exists(path) ? File.CreateText(path) : File.AppendText(path))
                {
                    sw.WriteLine("时间" + DateTime.Now + ":" + content);
                    sw.WriteLine("-------------------------------------");
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (Exception ex)
            {
                //Log.InsertErrorInfo("Tools.Logger", ex.Message);
            }

        }


        /// <summary>
        /// 返回IP
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            try
            {
                return HttpContext.Current.Request.Headers["Cdn-Src-Ip"] ?? HttpContext.Current.Request.UserHostAddress;
            }
            catch
            {
                return "255.255.255.255";
            }
        }



        /// <summary>
        /// 报警EMAIL
        /// </summary>
        /// <param name="content"></param>
        public static void SendEmali(string content)
        {
            string strHost = "smtp.exmail.qq.com"; //STMP服务器地址
            string strAccount = "mail@hairbobo.com"; //SMTP服务帐号
            string strPwd = "METIS)(*123"; //SMTP服务密码
            string strFrom = "mail@hairbobo.com"; //发送方邮件地址

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network; //指定电子邮件发送方式
            smtpClient.Host = strHost;//指定SMTP服务器
            smtpClient.Credentials = new NetworkCredential(strAccount, strPwd); //用户名和密码


            Encoding encoding = Encoding.UTF8;
            string senderDisplayName = "波波网"; //这个配置的是发件人的要显示在邮件的名称   
            MailAddress mailfrom = new MailAddress(strFrom, senderDisplayName, encoding); //发件人邮箱地址，名称，编码UTF8   
            MailAddress mailto = new MailAddress("cloud@hairbobo.com", "cloud@hairbobo.com", encoding); //收件人邮箱地址，名称，编码UTF8 

            MailMessage mailMessage = new MailMessage(mailfrom, mailto);
            mailMessage.Subject = "我的发型师企业支付报警"; //主题
            mailMessage.Body = content; //内容
            mailMessage.BodyEncoding = Encoding.UTF8; //正文编码
            mailMessage.IsBodyHtml = true; //设置为HTML格式
            mailMessage.Priority = MailPriority.High; //优先级

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                //Log.InsertErrorInfo("SendEmali", ex.Message);
            }
        }
    }
}