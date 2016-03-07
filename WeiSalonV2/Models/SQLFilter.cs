/*----------------------------------------------------------------
// 莫笛思
// 版权所有。
//
// 文件名：SQLFilter.cs
// 功能描述：
// 
// 创建人  ： KK
// 创建标识： 2015/10/16 10:12:04
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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace WeiSalonV2.Models
{
    /// <summary>
    /// 过滤字符串
    /// </summary>
    public class SQLFilter
    {
        /// <summary>
        /// 说明：sql非法字符过滤
        /// 作者：李天文
        /// 日期：2010-8-31
        /// </summary>
        /// <param name="str">要过滤的非法字符</param>
        /// <returns>string</returns>
        public static string ReplaceStr(string str)
        {
            //把字符串转换成小写并且前后加上空格
            str = " " + str + " ";

            //需要过滤的字符串
            string[] lst = { "'", "\"", " or ", "&"," select "," update ", " delete ", " insert ", "count(", " drop ", "truncate",
                       "asc(","mid(","char(","xp_cmdshell","exec master","net localgroup administrators"," and ","net user",};

            //计数器
            int count = 0;

            //验证字符串中是否包含非法字符，并自增计数器
            for (int i = 0; i < lst.Length; i++)
            {
                if (str.IndexOf(lst[i].ToString()) > -1)
                {
                    count = count + 1;
                }
            }

            //替换非法字符
            if (count > 0)
            {
                for (int i = 0; i < lst.Length; i++)
                {
                    str = str.Replace(lst[i].ToString(), "");
                }
            }
            else
            {
                return str.Trim();
            }
            return ReplaceStr(str);
        }

        /// <summary>
        /// 说明：把全角空格转换成半角并且转换为小写
        /// 作者：李天文
        /// 日期：2010-8-31
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换为半角的字符串</returns>
        public static string ToDBC(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                char[] c = str.ToCharArray();
                for (int i = 0; i < c.Length; i++)
                {
                    if (c[i] == 12288)
                    {
                        c[i] = (char)32;
                    }
                }

                return new string(c).ToLower();
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// 说明：过滤所有html标签
        /// 作者：李天文
        /// 日期：2010-8-31
        /// </summary>
        /// <param name="strHtml">要过滤的字符串</param>
        /// <returns>string</returns>
        public static string CutHTML(string strHtml)
        {
            if (string.IsNullOrEmpty(strHtml))
            {
                return "";
            }
            string strOutput = strHtml.Replace("&nbsp;", "");

            System.Text.RegularExpressions.Regex regex1 = new System.Text.RegularExpressions.Regex(@"<.+?>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            strOutput = regex1.Replace(strOutput, "");

            return strOutput;

        }

        /// <summary>
        /// 过滤html标签
        /// </summary>
        /// <param name="strHtml">要过滤的字符串</param>
        /// <returns>string</returns>
        public static string CutHTML0(string strHtml)
        {
            if (string.IsNullOrEmpty(strHtml))
            {
                return "";
            }
            string[] aryReg ={  
          @"<script[^>]*?>.*?</script>",  
          @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",  
          @"([\r\n])[\s]+",  
          @"&(quot|#34);",  
          @"&(amp|#38);",  
          @"&(lt|#60);",  
          @"&(gt|#62);",    
          @"&(nbsp|#160);",    
          @"&(iexcl|#161);",  
          @"&(cent|#162);",  
          @"&(pound|#163);",  
          @"&(copy|#169);",  
          @"&#(\d+);",  
          @"-->",  
          @"<!--.*\n"  
         };

            string[] aryRep =   {  
             "",  
             "",  
             "",  
             "\"",  
             "&",  
             "<",  
             ">",  
             "   ",  
             "\xa1",//chr(161),  
             "\xa2",//chr(162),  
             "\xa3",//chr(163),  
             "\xa9",//chr(169),  
             "",  
             "\r\n",  
             ""  
            };

            //string newReg = aryReg[0];
            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, aryRep[i]);
            }
            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");
            return strOutput;
        }

        /// <summary>
        /// 说明：把字符串中的特殊字符过滤掉
        /// 作者：李天文
        /// 日期：2010-8-31
        /// </summary>
        /// <param name="str">要过滤的字符串</param>
        /// <returns></returns>
        public static string ReplaceUnStr(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            string[] aryReg ={   
                                  "&amp;",    
                                  "&nbsp;",    
                                  "&iexcl;",  
                                  "&cent;",  
                                  "&pound;",  
                                  "&copy;",   
                              };
            for (int i = 0; i < aryReg.Length; i++)
            {
                str = str.Replace(aryReg[i], "");
            }
            return str;

        }


        /// <summary>
        /// 说明：对字符串中特殊字符解码
        /// 作者：李天文
        /// 日期：2010-8-31
        /// </summary>
        /// <param name="str">要解码的字符串</param>
        /// <returns></returns>
        public static string StrDeCode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            str = ReplaceUnStr(str);

            string[] enstr = { 
                                 "&gt;", 
                                 "&lt;" ,
                                 "&quot;"
                             };
            string[] destr = { 
                                 ">",
                                 "<" ,
                                 "\""
                             };
            for (int i = 0; i < enstr.Length; i++)
            {
                str = str.Replace(enstr[i], destr[i]);
            }
            return str;
        }


        /// <summary>
        /// 说明：对字符串中特殊字符编码
        /// 作者：李天文
        /// 日期：2010-8-31
        /// </summary>
        /// <param name="str">要编码的字符串</param>
        /// <returns></returns>
        public static string StrEnCode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            str = ReplaceUnStr(str);
            string[] enstr = { 
                                 "&gt;", 
                                 "&lt;" ,
                                 "&quot;"
                             };
            string[] destr = { 
                                 ">",
                                 "<" ,
                                 "\""
                             };
            for (int i = 0; i < enstr.Length; i++)
            {
                str = str.Replace(destr[i], enstr[i]);
            }
            return str;
        }

        /// <summary>
        /// 说明：过滤html标签
        /// 作者：李天文
        /// 日期：2010-8-31
        /// </summary>
        /// <param name="html">要过滤的字符串</param>
        /// <returns>过滤后的字符串</returns>
        public static string checkStr(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return "";
            }

            System.Text.RegularExpressions.Regex regex1 = new System.Text.RegularExpressions.Regex(@"<script[\s\S]+</script*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex2 = new System.Text.RegularExpressions.Regex(@" href *= *[\s\S]*script*:", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //    System.Text.RegularExpressions.Regex regex3 = new System.Text.RegularExpressions.Regex(@" on[\s\S]*=", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex4 = new System.Text.RegularExpressions.Regex(@"<iframe[\s\S]+</iframe*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex5 = new System.Text.RegularExpressions.Regex(@"<frameset[\s\S]+</frameset*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex6 = new System.Text.RegularExpressions.Regex(@"\<img[^\>]+\>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex7 = new System.Text.RegularExpressions.Regex(@"\<[img|IMG][^\>]+\>\w*\<\/(img|IMG)\>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex8 = new System.Text.RegularExpressions.Regex(@"\<a[^\>]+\>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex9 = new System.Text.RegularExpressions.Regex(@"\<[a|A][^\>]+\>\w*\<\/(a|A)\>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex10 = new System.Text.RegularExpressions.Regex(@"    *", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            html = regex10.Replace(html, "");
            html = regex9.Replace(html, ""); //过滤frameset 
            html = regex7.Replace(html, ""); //过滤frameset 
            html = regex1.Replace(html, ""); //过滤<script></script>标记 
            html = regex2.Replace(html, ""); //过滤href=javascript: (<A>) 属性 
            //     html = regex3.Replace(html, " _disibledevent="); //过滤其它控件的on...事件 
            html = regex4.Replace(html, ""); //过滤iframe 
            html = regex5.Replace(html, ""); //过滤frameset 
            html = regex6.Replace(html, ""); //过滤frameset 
            html = regex7.Replace(html, ""); //过滤frameset 
            html = regex8.Replace(html, ""); //过滤frameset 

            return html;
        }


        /// <summary>
        /// 说明：除img标签外，去掉所有html标签
        /// 作者：李天文
        /// 日期：2010-11-8
        /// </summary>
        /// <param name="html">要过滤的字符串</param>
        /// <returns></returns>
        public static StringBuilder RetainImg(string html)
        {
            StringBuilder str = new StringBuilder();
            string pat = @"<img\s.*?\s?src\s*=\s*['|""]?([^\s'""]+).*?>";
            Regex r = new Regex(pat, RegexOptions.Compiled);
            Match m = r.Match(html.ToLower());
            string result = string.Empty;
            while (m.Success)
            {
                System.Text.RegularExpressions.Group g = m.Groups[1];

                if (g.ToString().IndexOf("/xheditor/xheditor_emot") < 0)
                {
                    str.Append(g).Append(",");
                }

                m = m.NextMatch();
            }
            return str;
        }


        /// <summary>
        /// 说明：过滤html标签，保留img和字体标签
        /// 作者：李天文
        /// 日期：2010-8-31
        /// </summary>
        /// <param name="str">要过滤的字符串</param>
        /// <returns>过滤后的字符串</returns>
        public static string RetainImgFont(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return "";
            }

            Regex regex1 = new System.Text.RegularExpressions.Regex(@"<script[\s\S]+</script*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Regex regex2 = new System.Text.RegularExpressions.Regex(@" href *= *[\s\S]*script*:", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //    Regex regex3 = new System.Text.RegularExpressions.Regex(@" on[\s\S]*=", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Regex regex4 = new System.Text.RegularExpressions.Regex(@"<iframe[\s\S]+</iframe*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Regex regex5 = new System.Text.RegularExpressions.Regex(@"<frameset[\s\S]+</frameset*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Regex regex6 = new System.Text.RegularExpressions.Regex(@"<a.*?>|</a>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Regex regex7 = new System.Text.RegularExpressions.Regex(@"<(\/)?pre[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Regex regex8 = new Regex(@"<v:[^\>]+>|</v:[^\>]+>", RegexOptions.IgnoreCase);//过滤复制页面文件时的<v>和</v>标签  <v:.+?>|</v:.+?>
            html = regex1.Replace(html, ""); //过滤<script></script>标记 
            html = regex2.Replace(html, ""); //过滤href=javascript: (<a>) 属性 
            //   html = regex3.Replace(html, " _disibledevent="); //过滤其它控件的on...事件 
            html = regex4.Replace(html, ""); //过滤iframe 
            html = regex5.Replace(html, ""); //过滤frameset
            html = regex6.Replace(html, "");
            html = regex7.Replace(html, "");//过滤pre标签
            html = regex8.Replace(html, "");
            return html;
        }

        /// <summary>
        /// 说明：过滤html标签，保留img和字体标签
        /// 作者：李天文
        /// 日期：2010-8-31
        /// </summary>
        /// <param name="str">要过滤的字符串</param>
        /// <returns>过滤后的字符串</returns>
        public static string RetainImgFont2(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return "";
            }

            Regex regex1 = new System.Text.RegularExpressions.Regex(@"<script[\s\S]+</script*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Regex regex2 = new System.Text.RegularExpressions.Regex(@" href *= *[\s\S]*script*:", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //    Regex regex3 = new System.Text.RegularExpressions.Regex(@" on[\s\S]*=", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Regex regex4 = new System.Text.RegularExpressions.Regex(@"<iframe[\s\S]+</iframe*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Regex regex5 = new System.Text.RegularExpressions.Regex(@"<frameset[\s\S]+</frameset*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //   Regex regex6 = new System.Text.RegularExpressions.Regex(@"<a.*?>|</a>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Regex regex7 = new System.Text.RegularExpressions.Regex(@"<(\/)?pre[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Regex regex8 = new Regex(@"<v:[^\>]+>|</v:[^\>]+>", RegexOptions.IgnoreCase);//过滤复制页面文件时的<v>和</v>标签  <v:.+?>|</v:.+?>
            html = regex1.Replace(html, ""); //过滤<script></script>标记 
            html = regex2.Replace(html, ""); //过滤href=javascript: (<a>) 属性 
            //   html = regex3.Replace(html, " _disibledevent="); //过滤其它控件的on...事件 
            html = regex4.Replace(html, ""); //过滤iframe 
            html = regex5.Replace(html, ""); //过滤frameset
            //     html = regex6.Replace(html, "");
            html = regex7.Replace(html, "");//过滤pre标签
            html = regex8.Replace(html, "");
            return html;
        }

        /// <summary> 
        /// 说明：判断号码是联通，移动，电信中的哪个,在使用本方法前，请先验证号码的合法性
        /// 作者：李天文
        /// 日期：2010-8-31
        /// </summary>
        /// <param name="mobile">要判断的号码</param>
        /// <returns>返回相应类型：1代表联通；2代表移动；3代表电信；0未知</returns>
        public static int GetMobileType(string mobile)
        {
            //中国移动134.135.136.137.138.139.150.151.152.157.158.159.187.188 ,147(数据卡)
            //中国联通130.131.132.155.156.185.186
            //中国电信133.153.180.189
            //CDMA 133,153

            //联通
            string sPattern1 = "^(130|131|132|185|186|155|156)[0-9]{8}";

            //移动
            string sPattern2 = "^(135|136|137|138|139|134|147|150|151|152|154|157|158|159|181|182|183|184|187|188)[0-9]{7,8}";

            //电信
            string sPattern3 = "^(100|133|153|180|189|177|170)[0-9]{8}";

            if (Regex.IsMatch(mobile, sPattern1))
            {
                return 1;
            }
            else if (Regex.IsMatch(mobile, sPattern2))
            {
                return 2;
            }
            else if (Regex.IsMatch(mobile, sPattern3))
            {
                return 3;
            }
            return 0;
        }


        /// <summary>
        /// 说明：波波搜索过滤敏感符号
        /// 作者：李天文
        /// 日期：2010-8-31
        /// </summary>
        /// <param name="str">要过滤的字符串</param>
        /// <returns>过滤后的字符串</returns>
        public static string BoboSearchFilterStr(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            str = str.Replace(",", "");
            str = str.Replace("(", "");
            str = str.Replace(")", "");
            str = str.Replace("'", "");
            str = str.Replace("\\", "");
            str = str.Replace("&", "");
            str = str.Replace("-", "");
            str = str.Replace("%", "");
            str = str.Replace(">", "");
            str = str.Replace("<", "");
            str = str.Replace("=", "");
            str = str.Replace("|", "");
            str = str.Replace("~", "");
            str = str.Replace("*", "");
            str = str.Replace("$", "");
            str = str.Replace(";", "");
            return str;
        }


        /// <summary>
        /// 说明：验证是否为敏感字符
        /// 作者：李天文
        /// 日期：2010-8-31
        /// </summary>
        /// <param name="str">要验证的字符串</param>
        /// <returns>返回true为敏感字符</returns>
        //public static bool ValidateStr1(string str)
        //{
        //    bool bl = false;
        //    string[] strlst;
        //    string sql = "select si_info from t_seninfo where si_status=1";
        //    try
        //    {
        //        DataTable dt = DbHelperOra.Query(sql).Tables[0];
        //        //填充敏感词数组
        //        strlst = new string[dt.Rows.Count];
        //        if (dt.Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                strlst[i] = dt.Rows[i]["si_info"].ToString();
        //            }
        //        }

        //        str = str.ToLower();
        //        //不过滤空格的情况
        //        //验证字符串
        //        if (IsChina(str))//字符串为汉字
        //        {
        //            for (int i = 0; i < strlst.Length; i++)
        //            {
        //                if (str.LastIndexOf(strlst[i]) > -1)
        //                {
        //                    return true;
        //                }
        //            }
        //        }
        //        else//字符串不是汉字
        //        {
        //            for (int i = 0; i < strlst.Length; i++)
        //            {
        //                if (strlst[i].ToLower().Equals(str))
        //                {
        //                    return true;
        //                }
        //            }
        //        }

        //        //过滤空格的情况
        //        //验证字符串
        //        str = str.Replace(" ", "");
        //        str = str.Replace("　", "");
        //        if (IsChina(str))//字符串为汉字
        //        {
        //            for (int i = 0; i < strlst.Length; i++)
        //            {
        //                if (str.LastIndexOf(strlst[i]) > -1)
        //                {
        //                    return true;
        //                }
        //            }
        //        }
        //        else//字符串不是汉字
        //        {
        //            for (int i = 0; i < strlst.Length; i++)
        //            {
        //                if (strlst[i].ToLower().Equals(str))
        //                {
        //                    return true;
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        int erid;
        //        //EventLog.ErrorLog("ValidateStr", ex.Message, out erid);
        //    }
        //    return bl;
        //}


        /// <summary>
        /// 说明：判断是否为汉字
        /// 作者：李天文
        /// 日期：2010-8-31
        /// </summary>
        /// <param name="CString">要验证的字符串</param>
        /// <returns></returns>
        public static bool IsChina(string CString)
        {
            bool BoolValue = false;
            for (int i = 0; i < CString.Length; i++)
            {
                if (Convert.ToInt32(Convert.ToChar(CString.Substring(i, 1))) < Convert.ToInt32(Convert.ToChar(128)))
                {
                    BoolValue = false;
                }
                else
                {
                    return BoolValue = true;
                }
            }
            return BoolValue;
        }


        /// <summary>
        /// 说明：过滤IMG标签
        /// 作者：李天文
        /// 日期：2010-12-21
        /// </summary>
        /// <param name="text">要过滤的文本</param>
        /// <returns></returns>
        public static string FilterImg(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            Regex regex1 = new Regex(@"\<(img|IMG)[^\>]+\>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Regex regex2 = new Regex(@"\<(img|IMG)[^\>]+\>\w*\<\/(img|IMG)\>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Regex regex3 = new Regex(@"\<(embed|EMBED)[^\>]+\>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Regex regex4 = new Regex(@"\<(embed|EMBED)[^\>]+\>\w*\<\/(embed|EMBED)\>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            text = regex1.Replace(text, "");
            text = regex2.Replace(text, "");
            text = regex3.Replace(text, "");
            text = regex4.Replace(text, "");
            return text;
        }

        /// <summary>
        /// 获得指定网址的HTML代码
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public string getHtmlByUrl(string Url)
        {
            HttpWebRequest hr = (HttpWebRequest)HttpWebRequest.Create(Url);
            hr.KeepAlive = false;//不是永久性连接
            HttpWebResponse hr2 = (HttpWebResponse)hr.GetResponse();
            Stream s = hr2.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string str = "";
            while (sr.Peek() != -1)
                str += sr.ReadLine();
            return str;
        }

    }
}