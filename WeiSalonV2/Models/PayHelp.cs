/*----------------------------------------------------------------
// 莫笛思
// 版权所有。
//
// 文件名：PayHelp.cs
// 功能描述：
// 
// 创建人  ： KK
// 创建标识： 2015/11/9 14:00:48
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
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WeiSalonV2.Models
{
    /// <summary>
    /// 微信支付用
    /// </summary>
    public class PayHelp
    {
        //public readonly string Token = "febe829e3cb74c26b2d3e63c0c0c2bac";
        //公众号的唯一标识
        public readonly string _appid = ConfigurationManager.AppSettings["wxappID"];//微信appid        
        public readonly string _appsecret = ConfigurationManager.AppSettings["wxappsecret"];//微信appsecret 

        public string GetAccesstoken()
        {
            var token = CacheManage.GetCache("qqtoken");
            try
            {
                if (token == null || string.IsNullOrEmpty(token.ToString()))
                {
                    token = gettoken("1");
                    CacheManage.SetCacheMinutes("qqtoken", token, 90);
                }
            }
            catch
            {
                token = gettoken("1");
                CacheManage.SetCacheMinutes("qqtoken", token, 90);
            }
            return token.ToString();
        }

        public string gettoken(string type)
        {
            string token = "";
            string appId = _appid;
            string appsecret = _appsecret;
            var url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appId + "&secret=" + appsecret;
            var responseBody = Tools.StartRequest(url, "RequestService.GetAccesstoken", "GET");
            var dic = JsonTools.JsonToObject<Dictionary<string, object>>(responseBody);
            try
            {
                token = dic["access_token"].ToString();
            }
            catch (Exception ex)
            {
                token = "";
                //_log.InsertErrorInfo("RequestService.GetAccesstoken", ex.Message);
            }
            return token;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>    
        public string wx_get_token(string HtmlUrl)
        {
            //string token = "";
            //Tools.Logger("链接:" + HtmlUrl);
            string ticket = "";
            string appId = _appid;
            string nonce_str = "";
            string timestamp = "";
            string urlsha1 = "";
            string signature = "";
            var token = GetAccesstoken(); //获取缓存中的token 
            try
            {
                ticket = CacheManage.GetCache("wx_ticket").ToString();
            }
            catch
            {
                //获取token
                //获取jsapi_ticket  
                ticket = getticket(token.ToString());
                CacheManage.SetCacheMinutes("wx_ticket", ticket, 90);
            }
            nonce_str = "123456789123456789123456789"; //getRandStringEx(32);   //随机数      
            timestamp = ConvertDateTimeInt(DateTime.Now).ToString();
            string string1 = "jsapi_ticket=" + ticket + "&noncestr=" + nonce_str + "&timestamp=" + timestamp + "&url=" + HtmlUrl;
            signature = SHA1_Hash(string1);
            T_nonce_str = nonce_str;
            T_timestamp = timestamp;
            T_signature = signature;
            return "{\"appId\":\"" + appId + "\",\"nonceStr\":\"" + nonce_str + "\",\"timestamp\":\"" + timestamp + "\",\"signature\":\"" + signature + "\"}";
        }

        //public string T_appid = "";
        public string T_nonce_str = "";
        public string T_timestamp = "";
        public string T_signature = "";

        public string getticket(string token)
        {
            string ticket = "";
            const string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?";
            string at = url + "access_token=" + token + "&type=jsapi";
            var request = (HttpWebRequest)WebRequest.Create(at);
            request.Method = "get";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            var response = (HttpWebResponse)request.GetResponse();
            var stream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
            string responseBody = stream.ReadToEnd();
            stream.Close();
            response.Close();
            try
            {
                var dic = JSONHelper.JSONToObject<Dictionary<string, object>>(responseBody);
                if (dic != null)
                {
                    ticket = dic["ticket"].ToString();
                }
            }
            catch (Exception ex)
            {
            }
            return ticket;
        }

        /// <summary>  
        /// DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time"> DateTime时间格式</param>  
        /// <returns>Unix时间戳格式</returns>   
        public int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }


        //SHA1     
        public string SHA1_Hash(string str_sha1_in)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_sha1_in = UTF8Encoding.Default.GetBytes(str_sha1_in);
            byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
            string str_sha1_out = BitConverter.ToString(bytes_sha1_out);
            str_sha1_out = str_sha1_out.Replace("-", "");
            return str_sha1_out.ToLower();
        }

        private string PostXml(string url, string strPost)
        {
            string result = "";
            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Method = "POST";
            objRequest.ContentType = "text/xml";//提交xml 
            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(strPost);
            }
            catch (Exception e)
            {
                return "PostXml_err:" + e.Message;
            }
            finally
            {
                myWriter.Close();
            }

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                sr.Close();
            }
            return result;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns></returns>
        public string Md5Encrypt(string input)
        {
            System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider();
            var data = Encoding.UTF8.GetBytes(input);
            var encs = md5.ComputeHash(data);
            return BitConverter.ToString(encs).Replace("-", "").ToUpper();
        }

    }


    /// <summary>
    /// 微信授权获取信息用
    /// </summary>
    public class WeiXin_OAuth2
    {
        public static readonly string Token = "4099BF67085A4750E415F20C6A861634";
        //公众号的唯一标识
        public static readonly string _appid = ConfigurationManager.AppSettings["wxappID"];//微信appid
        public static readonly string _appsecret = ConfigurationManager.AppSettings["wxappsecret"];//微信appsecret
        //joycut用
        public static readonly string _appid_joycut = ConfigurationManager.AppSettings["wxappID_joycut"];//微信appid        
        public static readonly string _appsecret_joycut = ConfigurationManager.AppSettings["wxappsecret_joycut"];//微信appsecret 
        /// <summary>
        /// 第一步获取授权地址
        /// </summary>
        /// <param name="url">返回地址</param>
        /// <param name="state">返回参数</param>
        /// <returns>授权地址</returns>
        public static string GetUrl(string url, string state, string type)
        {
            //公众号的唯一标识
            string appid = type == "1" ? _appid : _appid_joycut;
            //应用授权作用域，
            //snsapi_base （不弹出授权页面，直接跳转，只能获取用户openid），
            //snsapi_userinfo （弹出授权页面，可通过openid拿到昵称、性别、所在地。并且，即使在未关注的情况下，只要用户授权，也能获取其信息）
            //string scope = "snsapi_userinfo";
            string scope = "snsapi_base";
            //授权后重定向的回调链接地址
            //url = HttpUtility.UrlEncode(url);
            string returl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + appid + "&redirect_uri=" + url + "&response_type=code&scope=" + scope + "&state=" + state + "#wechat_redirect";
            return returl;
        }
        /// <summary>
        /// 第一步获取授权地址
        /// </summary>
        /// <param name="url">返回地址</param>
        /// <param name="state">返回参数</param>
        /// <returns>授权地址</returns>
        public static string GetUrl2(string url, string state, string type)
        {
            //公众号的唯一标识
            string appid = type == "1" ? _appid : _appid_joycut;
            //应用授权作用域，
            //snsapi_base （不弹出授权页面，直接跳转，只能获取用户openid），
            //snsapi_userinfo （弹出授权页面，可通过openid拿到昵称、性别、所在地。并且，即使在未关注的情况下，只要用户授权，也能获取其信息）
            string scope = "snsapi_userinfo";
            //string scope = "snsapi_base";
            //授权后重定向的回调链接地址
            //url = HttpUtility.UrlEncode(url);
            string returl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + appid + "&redirect_uri=" + url + "&response_type=code&scope=" + scope + "&state=" + state + "#wechat_redirect";
            return returl;
        }

        /// <summary>
        /// 第二步获取授权
        /// </summary>
        /// <param name="CODE">第一步获取到的CODE（每次不同,5分钟失效）</param>
        /// <returns>微信用户信息</returns>
        public static string Getaccess(string CODE, string type)
        {
            const string url = "https://api.weixin.qq.com/sns/oauth2/access_token?";
            string appid = type == "1" ? _appid : _appid_joycut;
            string appsecret = type == "1" ? _appsecret : _appsecret_joycut;
            //MapKey
            string at = url + "appid=" + appid + "&secret=" + appsecret + "&code=" + CODE + "&grant_type=authorization_code";
            string responseBody = "";
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(at);
                request.Method = "get";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Accept = "*/*";
                var response = (HttpWebResponse)request.GetResponse();
                var stream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                responseBody = stream.ReadToEnd();
                stream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                return "responseBody_err:" + ex.Message;
            }
            string ACCESS_TOKEN = "";
            string OPENID = "";
            string Info = "";
            string err = "";
            try
            {
                var dic = JsonTools.JsonToObject<Dictionary<string, object>>(responseBody);
                if (dic != null)
                {
                    ACCESS_TOKEN = dic["access_token"].ToString();
                    //try
                    //{
                    //    CacheManage.SetCacheMinutes("token", dic["access_token"].ToString(), 100);
                    //}
                    //catch { }
                    OPENID = dic["openid"].ToString();
                    //根据授权信息获取用户详细信息
                    //Info = GetInfo(ACCESS_TOKEN, OPENID);
                }
                else
                {
                    return "获取失败";
                }
            }
            catch (Exception ex)
            {
                err = "  ----------************----------" + ex.Message;
                return err;
            }
            //return Info;
            return OPENID;
        }

        /// <summary>
        /// 第二步获取授权
        /// </summary>
        /// <param name="CODE">第一步获取到的CODE（每次不同,5分钟失效）</param>
        /// <returns>微信用户信息</returns>
        public static string GetaccessInfo(string CODE, string type)
        {
            const string url = "https://api.weixin.qq.com/sns/oauth2/access_token?";
            string appid = type == "1" ? _appid : _appid_joycut;
            string appsecret = type == "1" ? _appsecret : _appsecret_joycut;
            //MapKey
            string at = url + "appid=" + appid + "&secret=" + appsecret + "&code=" + CODE + "&grant_type=authorization_code";
            string responseBody = "";
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(at);
                request.Method = "get";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Accept = "*/*";
                var response = (HttpWebResponse)request.GetResponse();
                var stream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                responseBody = stream.ReadToEnd();
                stream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                return "responseBody_err:" + ex.Message;
            }
            string ACCESS_TOKEN = "";
            string OPENID = "";
            string Info = "";
            string err = "";
            try
            {
                var dic = JsonTools.JsonToObject<Dictionary<string, object>>(responseBody);
                if (dic != null)
                {
                    ACCESS_TOKEN = dic["access_token"].ToString();
                    //try
                    //{
                    //    CacheManage.SetCacheMinutes("token", dic["access_token"].ToString(), 100);
                    //}
                    //catch { }
                    OPENID = dic["openid"].ToString();
                    //根据授权信息获取用户详细信息
                    Info = GetInfo(ACCESS_TOKEN, OPENID);
                }
                else
                {
                    return "获取失败";
                }
            }
            catch (Exception ex)
            {
                err = "  ----------************----------" + ex.Message;
                return err;
            }
            return Info;
            //return OPENID;
        }

        /// <summary>
        /// 第三步获取微信用户信息
        /// </summary>
        /// <param name="ACCESS_TOKEN">第二步获取到的access_token</param>
        /// <param name="OPENID">第二步获取到的openid（微信账号唯一标识）</param>
        /// <returns></returns>
        public static string GetInfo(string ACCESS_TOKEN, string OPENID)
        {
            const string url = "https://api.weixin.qq.com/sns/userinfo?";

            //MapKey
            string at = url + "access_token=" + ACCESS_TOKEN + "&openid=" + OPENID + "&lang=zh_CN";
            string responseBody = "";
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(at);
                request.Method = "get";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Accept = "*/*";
                var response = (HttpWebResponse)request.GetResponse();
                var stream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                responseBody = stream.ReadToEnd();
                stream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                responseBody = "  ----------" + ex.Message + "----------";
            }
            if (responseBody.Trim() == "")
            {
                responseBody = "未获取到值";
            }
            return responseBody;
        }
    }
}