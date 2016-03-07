using BoboIService;
using BoboModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeiSalonV2.Models;

namespace WeiSalonV2.Controllers
{
    public class NewSalonController : ApiController
    {
        //
        // GET: /NewSalon/

        public NewSalonController()
        {
        }


        /// <summary>
        /// 获取沙龙信息和环境图
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public string SalonInfo([FromBody] SUID model)
        {
            string value = CodeHelp.GetResults(0, "失败", "");
            try
            {
                if (string.IsNullOrEmpty(model.suid.ToString()))
                {
                    value = CodeHelp.GetResults(0, "失败", "沙龙ID不能为空");
                }
                else
                {
                    SalonSimple salon = new SalonSimple();
                    List<AllSalon> list = new List<AllSalon>();
                    SalonService sa = new SalonService();
                    INoticeService nots = new INoticeService();
                    //取沙龙的基本信息和环境图
                    list = sa.GetSalonInfo(model.suid, 3);
                    if (list != null && list.Count > 0)
                    {
                        if (list[0].Status != 4)
                        {
                            value = CodeHelp.GetResults(0, "无法找到页面，该沙龙暂时不提供此服务。", "沙龙未开通");
                        }
                        else
                        {
                            if (list[0].Opendate < DateTime.Now)
                            {
                                value = CodeHelp.GetResults(0, "无法找到页面，该沙龙暂时不提供此服务。", "沙龙未开通");
                            }
                            else
                            {
                                list.First().Address = list.First().Address.Replace("|", "");
                                Tnotice noice = new Tnotice();
                                noice = nots.GetNotice(model.suid);
                                if (noice != null)
                                {
                                    list.First().noticecontent = noice.content;
                                }
                                else
                                {
                                    list.First().noticecontent = "暂无公告";
                                }
                                value = CodeHelp.GetResults(1, "成功", list).ToLower();
                            }
                        }
                    }
                    else
                    {
                        value = CodeHelp.GetResults(0, "失败", "未取到数据");
                    }
                }
            }
            catch (Exception ex)
            {
                value = CodeHelp.GetResults(0, "失败", ex.Message);
            }
            return value;
        }

        /// <summary>
        /// 获取沙龙昵称和logo
        /// </summary>
        /// <param name="suid"></param>
        /// <returns></returns>
        private string cacheInfo(string suid)
        {
            string info = "";
            try
            {
                info = CacheManage.GetCache("suid:" + suid).ToString();
            }
            catch
            {
                List<AllSalon> list = new List<AllSalon>();
                SalonService sa = new SalonService();
                //取沙龙的基本信息和环境图
                list = sa.GetSalonInfo(new Guid(suid), 3);
                if (list.Any())
                {
                    info = list[0].Nickname + "|*-*|" + list[0].Logo;
                    CacheManage.SetCacheMinutes("suid:" + suid, info, 90);
                }
            }
            return info;
        }

        /// <summary>
        /// 判断沙龙是否到期
        /// </summary>
        /// <param name="suid"></param>
        /// <returns></returns>
        private bool CHKsal(string suid)
        {
            try
            {
                List<AllSalon> list = new List<AllSalon>();
                SalonService sa = new SalonService();
                list = sa.GetSalonInfo(new Guid(suid), 3);
                if (list.Any())
                {
                    return list[0].Opendate > DateTime.Now;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取沙龙发型师信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public string HariStyleListList([FromBody] SUIDPAGE model)
        {
            string value = CodeHelp.GetResults(0, "失败", "");
            if (string.IsNullOrEmpty(model.suid.ToString()))
            {
                value = CodeHelp.GetResults(0, "失败", "沙龙ID不能为空");
            }
            else
            {
                if (!CHKsal(model.suid.ToString()))
                {
                    value = CodeHelp.GetResults(0, "无法找到页面，该沙龙暂时不提供此服务。", "沙龙未开通");
                    return value;
                }
                var str = CacheManage.GetCache("SearchHotHair" + model.suid);
                if (str == null)
                {
                    int pageindex = 1;
                    int pagesize = 100;
                    int rowcount = 0;
                    int pagecount = 0;
                    HairListService hise = new HairListService();
                    List<HariStyleList> list = new List<HariStyleList>();
                    //获取沙龙发型师信息
                    list = hise.GetHariStyleListList(model.suid, pageindex, pagesize, out rowcount, out pagecount);
                    if (list != null && list.Count > 0)
                    {
                        string saloninfo = cacheInfo(model.suid.ToString());
                        if (!string.IsNullOrEmpty(saloninfo))
                        {
                            saloninfo = saloninfo.Replace("|*-*|", "`");
                            string[] info = saloninfo.Split('`');
                            list[0].salonname = info[0];
                            list[0].salonlogo = info[1];
                        }
                        value = CodeHelp.GetResults(1, "成功", list).ToLower();
                        CacheManage.SetCacheMinutes("SearchHotHair" + model.suid, value, 60);
                    }
                    else
                    {
                        value = CodeHelp.GetResults(0, "尚未绑定发型师", "");
                    }
                }
                else
                {
                    try
                    {
                        value = str.ToString();
                    }
                    catch
                    {
                        value = CodeHelp.GetResults(0, "失败", "未取到数据");
                    }
                }
            }
            return value;
        }



        /// <summary>
        /// 作品分页
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="ind"></param>
        /// <returns></returns>
        [HttpPost]
        public string ShowList([FromBody] SUIDPAGE model)
        {
            if (string.IsNullOrEmpty(model.suid.ToString()))
            {
                return "-1";
            }
            //if (string.IsNullOrEmpty(model.pageindex))
            //{
            //    return "-2";
            //}
            //int pageindex = 0;
            //int pagesize = 0;
            //try
            //{
            //    pageindex = int.Parse(model.pageindex);
            //    pagesize = int.Parse(model.pagesize);
            //}
            //catch
            //{
            //    return "-3";
            //}
            if (!CHKsal(model.suid.ToString()))
            {
                return CodeHelp.GetResults(0, "无法找到页面，该沙龙暂时不提供此服务。", "沙龙未开通"); 
            }
            HairListService hise = new HairListService();
            string str = "";
            try
            {
                //获取沙龙下所有发型师的UID
                str = hise.GetHariStyleListID(model.suid);
            }
            catch
            { }
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    return CodeHelp.GetResults(0, "暂无作品", "");
                }
                ///API地址
                string url1 = "/api/weisalon/gethairweibolist";
                ///API参数
                string paramstr1 = "uid=" + str;
                //获取返回值(Json)
                string responseBody1 = Common.GetResponseResult(url1, paramstr1).ToLower();
                //ViewBag.responseBody1 = responseBody1;
                string retvalue = responseBody1.Replace("\r\n", "").Replace(" ", "");
                if (!string.IsNullOrEmpty(retvalue))
                {
                    var data = JSONHelper.JSONToObject<Dictionary<string, object>>(retvalue);
                    if (data.ContainsKey("status") && data["status"].ToString() == "1")
                    {
                        string saloninfo = cacheInfo(model.suid.ToString());
                        if (!string.IsNullOrEmpty(saloninfo))
                        {
                            saloninfo = saloninfo.Replace("|*-*|", "`");
                            string[] info = saloninfo.Split('`');
                            retvalue = retvalue.Replace("\"integralchange\":null", "\"salonname\":\"" + info[0] + "\",\"salonlogo\":\"" + info[1] + "\"");
                        }
                    }
                }
                return retvalue;
            }
            catch
            {
                return CodeHelp.GetResults(0, "失败", "");
            }
        }


        /// <summary>
        /// 获取作品详情
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="ind"></param>
        /// <returns></returns>
        [HttpPost]
        public string getshow([FromBody] BGID model)
        {
            if (string.IsNullOrEmpty(model.bgid))
            {
                return CodeHelp.GetResults(0, "失败", "");
            }
            try
            {
                ///API地址
                string url1 = "/api/weisalon/getweibo";
                ///API参数
                string paramstr1 = "bgid=" + model.bgid;
                //获取返回值(Json)
                string responseBody1 = Common.GetResponseResult(url1, paramstr1).ToLower();
                return responseBody1.Replace("\r\n", "").Replace(" ", "");
            }
            catch
            {
                return CodeHelp.GetResults(0, "失败", "");
            }
        }

        /// <summary>
        /// 获取微信基础授权（JS用）
        /// </summary>
        /// <returns></returns>     
        [HttpPost]
        public string GetWXtoken([FromBody] HTMLURL model)
        {
            PayHelp ph = new PayHelp();
            return ph.wx_get_token(model.htmlurl);
        }
    }
}
