/*----------------------------------------------------------------
// 莫迪思
// 版权所有。
//
// 文件名：Common.cs
// 功能描述：
// 
// 创建人  ： KK
// 创建标识： 2015/3/11 15:21:52
// 
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//
//----------------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using System.Xml;

namespace WeiSalonV2.Models
{
    public class Common
    {

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
                var path = @"D:\weiSalon\paylog\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
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
            }

        }
        public static string GetToken()
        {
            var json = GetResponseResult("/api/message/QiNiuToken", "");
            var data = JSONToObject<Dictionary<string, object>>(json);

            return (data.ContainsKey("status") && data["status"].ToString() == "1") ? data["info"].ToString() : string.Empty;
        }

        #region Json文本转对象
        /// <summary>
        /// Json文本转对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="jsonText">JSON文本</param>
        /// <returns>指定类型的对象</returns>
        public static T JSONToObject<T>(string jsonText)
        {
            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            try
            {
                return jss.Deserialize<T>(jsonText);
            }
            catch (Exception ex)
            {
                throw new Exception("JSONHelper.JSONToObject():" + ex.Message);
            }
        }

        #endregion

        #region HTTP请求加密/解密字符串
        /// <summary>
        /// Encrpyts the sourceString, returns this result as an Aes encrpyted, BASE64 encoded string  加密
        /// </summary>
        /// <param name="plainSourceStringToEncrypt">a plain, Framework string (ASCII, null terminated)</param>
        /// <param name="passPhrase">The pass phrase.</param>
        /// <returns>
        /// returns an Aes encrypted, BASE64 encoded string
        /// </returns>
        public static string EncryptString(string plainSourceStringToEncrypt, string passPhrase)
        {
            try
            {
                //Set up the encryption objects
                using (var acsp = GetProvider(Encoding.Default.GetBytes(passPhrase)))
                {
                    var sourceBytes = Encoding.UTF8.GetBytes(plainSourceStringToEncrypt);
                    var ictE = acsp.CreateEncryptor();
                    //Set up stream to contain the encryption
                    var msS = new MemoryStream();
                    //Perform the encrpytion, storing output into the stream
                    var csS = new CryptoStream(msS, ictE, CryptoStreamMode.Write);
                    csS.Write(sourceBytes, 0, sourceBytes.Length);
                    csS.FlushFinalBlock();
                    //sourceBytes are now encrypted as an array of secure bytes
                    var encryptedBytes = msS.ToArray(); //.ToArray() is important, don't mess with the buffer
                    //return the encrypted bytes as a BASE64 encoded string
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
            catch (System.Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// Encrpyts the sourceString, returns this result as an Aes encrpyted, BASE64 encoded string  加密
        /// </summary>
        /// <param name="plainSourceStringToEncrypt">a plain, Framework string (ASCII, null terminated)</param>
        /// <param name="passPhrase">The pass phrase.</param>
        /// <returns>
        /// returns an Aes encrypted, BASE64 encoded string
        /// </returns>
        public static string EncryptString2(string plainSourceStringToEncrypt, string passPhrase)
        {
            try
            {
                //Set up the encryption objects
                using (var acsp = GetProvider(Encoding.Default.GetBytes(passPhrase)))
                {
                    acsp.Padding = PaddingMode.PKCS7;
                    acsp.Mode = CipherMode.ECB;
                    var sourceBytes = Encoding.UTF8.GetBytes(plainSourceStringToEncrypt);
                    var ictE = acsp.CreateEncryptor();
                    //Set up stream to contain the encryption
                    var msS = new MemoryStream();
                    //Perform the encrpytion, storing output into the stream
                    var csS = new CryptoStream(msS, ictE, CryptoStreamMode.Write);
                    csS.Write(sourceBytes, 0, sourceBytes.Length);
                    csS.FlushFinalBlock();
                    //sourceBytes are now encrypted as an array of secure bytes
                    var encryptedBytes = msS.ToArray(); //.ToArray() is important, don't mess with the buffer
                    //return the encrypted bytes as a BASE64 encoded string
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
            catch (System.Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// Decrypts a BASE64 encoded string of encrypted data, returns a plain string 解密
        /// </summary>
        /// <param name="base64StringToDecrypt">an Aes encrypted AND base64 encoded string</param>
        /// <param name="passphrase">The passphrase.</param>
        /// <returns>returns a plain string</returns>
        public static string DecryptString(string base64StringToDecrypt, string passphrase)
        {
            try
            {
                //Set up the encryption objects
                using (var acsp = GetProvider(Encoding.Default.GetBytes(passphrase)))
                {
                    var RawBytes = Convert.FromBase64String(base64StringToDecrypt);
                    var ictD = acsp.CreateDecryptor();
                    //RawBytes now contains original byte array, still in Encrypted state
                    //Decrypt into stream
                    var msD = new MemoryStream(RawBytes, 0, RawBytes.Length);
                    var csD = new CryptoStream(msD, ictD, CryptoStreamMode.Read);
                    //csD now contains original byte array, fully decrypted
                    //return the content of msD as a regular string
                    return (new StreamReader(csD)).ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// Decrypts a BASE64 encoded string of encrypted data, returns a plain string 解密
        /// </summary>
        /// <param name="base64StringToDecrypt">an Aes encrypted AND base64 encoded string</param>
        /// <param name="passphrase">The passphrase.</param>
        /// <returns>returns a plain string</returns>
        public static string DecryptString2(string base64StringToDecrypt, string passphrase)
        {
            try
            {
                //Set up the encryption objects
                using (var acsp = GetProvider(Encoding.Default.GetBytes(passphrase)))
                {
                    acsp.Mode = CipherMode.ECB;
                    acsp.Padding = PaddingMode.PKCS7;
                    var RawBytes = Convert.FromBase64String(base64StringToDecrypt);
                    var ictD = acsp.CreateDecryptor();
                    //RawBytes now contains original byte array, still in Encrypted state
                    //Decrypt into stream
                    var msD = new MemoryStream(RawBytes, 0, RawBytes.Length);
                    var csD = new CryptoStream(msD, ictD, CryptoStreamMode.Read);
                    //csD now contains original byte array, fully decrypted
                    //return the content of msD as a regular string
                    return (new StreamReader(csD)).ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        private static AesCryptoServiceProvider GetProvider(byte[] key)
        {
            var result = new AesCryptoServiceProvider();
            result.BlockSize = 128;
            result.KeySize = 128;
            result.Mode = CipherMode.CBC;
            result.Padding = PaddingMode.PKCS7;
            result.GenerateIV();
            result.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            var RealKey = GetKey(key, result);
            result.Key = RealKey;
            // result.IV = RealKey;
            return result;
        }
        //private static byte[] GetKey(byte[] suggestedKey, SymmetricAlgorithm p)
        //{
        //    var kRaw = suggestedKey;
        //    var kList = new List<byte>();
        //    for (var i = 0; i < p.LegalKeySizes[0].MinSize; i += 8)
        //    {
        //        kList.Add(kRaw[(i / 8) % kRaw.Length]);
        //    }
        //    var k = kList.ToArray();
        //    return k;
        //}
        private static byte[] GetKey(byte[] suggestedKey, SymmetricAlgorithm p)
        {
            var kRaw = suggestedKey;
            var kList = new List<byte>();

            for (var i = kRaw.Length - 1; i > -1; i--)
            {
                kList.Add(kRaw[i]);
                if (kList.Count == 16)
                    break;
            }

            var k = kList.ToArray();
            return k;
        }
        #endregion
        #region 密码加密/解密具体实现
        private const string Key64 = "qecvbhhp";//8个字符，64位  
        private const string Iv64 = "bmncvcfh";
        /// <summary>  
        /// 加密  
        /// </summary>  
        /// <param name="str">要加密的字符串</param>  
        /// <returns>string</returns>  
        private static string EnCode(string str)
        {
            var byKey = Encoding.ASCII.GetBytes(Key64);
            var byIV = Encoding.ASCII.GetBytes(Iv64);
            var cryptoProvider = new DESCryptoServiceProvider();
            var i = cryptoProvider.KeySize;
            var ms = new MemoryStream();
            var cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey,
    byIV), CryptoStreamMode.Write);
            var sw = new StreamWriter(cst);
            sw.Write(str);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }
        /// <summary>  
        /// 解密  
        /// </summary>  
        /// <param name="str">要解密的字符串</param>  
        /// <returns>string</returns>  
        private static string DeCode(string str)
        {
            var byKey = Encoding.ASCII.GetBytes(Key64);
            var byIV = Encoding.ASCII.GetBytes(Iv64);
            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(str);
            }
            catch
            {
                return null;
            }
            var cryptoProvider = new DESCryptoServiceProvider();
            var ms = new MemoryStream(byEnc);
            var cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            var sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }
        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        private static string Encrypt(string text, string sKey)
        {
            var des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(text);
            des.Key = Encoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, FormsAuthPasswordFormat.MD5.ToString()).Substring(0, 8));
            des.IV = Encoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, FormsAuthPasswordFormat.MD5.ToString()).Substring(0, 8));
            var ms = new System.IO.MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            var ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }
        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        private static string Decrypt(string text, string sKey)
        {
            var des = new DESCryptoServiceProvider();
            int len;
            len = text.Length / 2;
            var inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = Encoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, FormsAuthPasswordFormat.MD5.ToString()).Substring(0, 8));
            des.IV = Encoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, FormsAuthPasswordFormat.MD5.ToString()).Substring(0, 8));
            var ms = new System.IO.MemoryStream();
            var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }
        #endregion
        #region 密码 加密/解密 外部调用
        /// <summary>
        /// 数据加密
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string JiaMi(string str)
        {
            return Encrypt(EnCode(str), "!@$%^&*");
        }
        /// <summary>
        /// 数据解密
        /// </summary>
        /// <param name="str">要解密的字符串</param>
        /// <returns>解密后的字符串</returns>
        public static string JieMi(string str)
        {
            return DeCode(Decrypt(str, "!@$%^&*"));
        }
        #endregion

        /// <summary>
        /// 预定加密解密的密钥
        /// </summary>
        private static string Bdes = "00200";
        /// <summary>
        /// API基础地址
        /// </summary>
        private static string UrlBase = ConfigurationManager.AppSettings["TestApiUrl"];

        /// <summary>
        /// 调用接口
        /// </summary>
        /// <param name="url">API地址</param>
        /// <param name="param">参数字符串</param>
        /// <returns>返回Json结果</returns>
        public static string GetResponseResult(string url, string param)
        {
            url = UrlBase + url;
            url = url.Trim();
            param = param.Trim();
            string incou = HttpUtility.UrlDecode(param);
            param = Encrypt(incou);
            var valueStr = string.Empty;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "post";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.UseDefaultCredentials = true;
            var para = Encoding.UTF8.GetBytes("action=" + HttpUtility.UrlEncode(param));
            var a1 = DeEncrypt(HttpUtility.UrlEncode(param));
            request.ContentLength = para.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(para, 0, para.Length);
            }
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                var str = stream.ReadToEnd().Replace("\"", "");
                //解密返回的参数
                valueStr =
                      HttpUtility.UrlDecode(Common.DecryptString2(str.Substring(8, str.Length - 32),
                          str.Substring(0, 8) + str.Substring(str.Length - 24) + "00200"));
                stream.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                var webResponse = ex.Response as HttpWebResponse;
                var myread = new StreamReader(webResponse.GetResponseStream());
                var Values = myread.ReadToEnd();
                myread.Close();
            }
            return valueStr;
        }

        /// <summary>
        /// 解密返回的加密字符串
        /// </summary>
        /// <param name="responseBody">加密过后的字符串</param>
        /// <returns>解密后的字符串</returns>
        public static string DeEncrypt(string responseBody)
        {
            responseBody = responseBody.Replace("\"", "");
            string a = responseBody.Substring(8, responseBody.Length - 32);
            string b = responseBody.Substring(0, 8);
            string c = responseBody.Substring(responseBody.Length - 24);
            responseBody = DecryptString2(a, b + c + Bdes);
            responseBody = HttpUtility.UrlDecode(responseBody);
            return responseBody;
        }

        /// <summary>
        /// 加密请求字符串
        /// </summary>
        /// <param name="key">准备加密的字符串</param>
        /// <returns>加密过后的字符串</returns>
        public static string Encrypt(string key)
        {
            var guid = Guid.NewGuid().ToString();
            guid = guid.Replace("-", "");
            return guid.Substring(0, 8) + EncryptString2(key, guid + Bdes) + guid.Substring(8);
        }



        /// <summary>
        /// 除去数组中的空值和签名参数
        /// </summary>
        /// <param name="dicArrayPre">过滤前的参数组</param>
        /// <returns>过滤后的参数组</returns>
        public static Dictionary<string, string> FilterPara(Dictionary<string, string> dicArrayPre)
        {
            var dicArray = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> temp in dicArrayPre)
            {
                if (temp.Key.ToLower() != "sign" && temp.Key.ToLower() != "sign_type" && temp.Value != "" && temp.Value != null)
                {
                    dicArray.Add(temp.Key, temp.Value);
                }
            }

            return dicArray;
        }

        /// <summary>
        /// 根据字母a到z的顺序把参数排序
        /// </summary>
        /// <param name="dicArrayPre">排序前的参数组</param>
        /// <returns>排序后的参数组</returns>
        public static Dictionary<string, string> SortPara(Dictionary<string, string> dicArrayPre)
        {
            var dicTemp = new SortedDictionary<string, string>(dicArrayPre);
            var dicArray = new Dictionary<string, string>(dicTemp);

            return dicArray;
        }

        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="sArray">需要拼接的数组</param>
        /// <returns>拼接完成以后的字符串</returns>
        public static string CreateLinkString(Dictionary<string, string> dicArray)
        {
            var prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append(temp.Key + "=" + temp.Value + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }

        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串，并对参数值做urlencode
        /// </summary>
        /// <param name="sArray">需要拼接的数组</param>
        /// <param name="code">字符编码</param>
        /// <returns>拼接完成以后的字符串</returns>
        public static string CreateLinkStringUrlencode(Dictionary<string, string> dicArray, Encoding code)
        {
            var prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append(temp.Key + "=" + HttpUtility.UrlEncode(temp.Value, code) + "&");
            }

            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);

            return prestr.ToString();
        }

        /// <summary>
        /// 写日志，方便测试（看网站需求，也可以改成把记录存入数据库）
        /// </summary>
        /// <param name="sWord">要写入日志里的文本内容</param>
        public static void LogResult(string sWord)
        {
            string strPath = HttpContext.Current.Server.MapPath("log");
            strPath = strPath + "\\" + DateTime.Now.ToString().Replace(":", "") + ".txt";
            var fs = new StreamWriter(strPath, false, System.Text.Encoding.Default);
            fs.Write(sWord);
            fs.Close();
        }

        /// <summary>
        /// 获取文件的md5摘要
        /// </summary>
        /// <param name="sFile">文件流</param>
        /// <returns>MD5摘要结果</returns>
        public static string GetAbstractToMD5(Stream sFile)
        {
            var md5 = new MD5CryptoServiceProvider();
            var result = md5.ComputeHash(sFile);
            var sb = new StringBuilder(32);
            foreach (byte t in result)
            {
                sb.Append(t.ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取文件的md5摘要
        /// </summary>
        /// <param name="dataFile">文件流</param>
        /// <returns>MD5摘要结果</returns>
        public static string GetAbstractToMD5(byte[] dataFile)
        {
            var md5 = new MD5CryptoServiceProvider();
            var result = md5.ComputeHash(dataFile);
            var sb = new StringBuilder(32);
            foreach (byte t in result)
            {
                sb.Append(t.ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
    }
}