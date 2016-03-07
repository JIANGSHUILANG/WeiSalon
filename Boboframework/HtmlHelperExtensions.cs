using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace Boboframework
{

    public static class HtmlHelperExtensions
    {

        #region 图片扩展
        private const string ImageServer = "http://img.hairbobo.com";
        private const string QiniuServer = "http://bobosquad.qiniudn.com";

        public static MvcHtmlString ResizeImage(this HtmlHelper htmlHelper, string src, int width, int height, string alt = "", object htmlAttributes = null)
        {
            src = htmlHelper.ResizeImageUrl(src, width, height).ToString();
            return htmlHelper.Image(src, alt, 0, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString ResizeImageUrl(this HtmlHelper htmlHelper, string src, int width, int height)
        {
            src = string.Format("{0}?width={1}&height={2}", src, width, height);
            return new MvcHtmlString(src);
        }

        public static MvcHtmlString Image(this HtmlHelper htmlHelper, string src, int type, string alt = "", object htmlAttributes = null)
        {
            return htmlHelper.Image(src, alt, type, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString Image(this HtmlHelper htmlHelper, string src, string alt, int type, IDictionary<string, object> htmlAttributes)
        {
            //src = type == 0 ? ImageServer + src : QiniuServer + src + "?imageView2/0/w/100/h/100";

            switch (type)
            {
                case 0:
                    src = ImageServer + src;
                    break;
                case 1:
                    src = QiniuServer + src + "?imageView2/0/w/100/h/100";
                    break;
            }

            var url = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            string imageUrl = url.Content(src);
            var imageTag = new TagBuilder("img");

            if (!string.IsNullOrEmpty(imageUrl))
                imageTag.MergeAttribute("src", imageUrl);

            if (!string.IsNullOrEmpty(alt))
                imageTag.MergeAttribute("alt", alt);

            imageTag.MergeAttributes(htmlAttributes, true);

            if (imageTag.Attributes.ContainsKey("alt") && !imageTag.Attributes.ContainsKey("title"))
                imageTag.MergeAttribute("title", imageTag.Attributes["alt"] ?? "");

            return MvcHtmlString.Create(imageTag.ToString(TagRenderMode.SelfClosing));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="src"></param>
        /// <param name="type">0 默认网站  1 七牛</param>
        /// <returns></returns>
        public static string ImageUrl(this HtmlHelper htmlHelper, string src, int type = 0)
        {

            return type == 0 ? ImageServer + src : QiniuServer + src + "?imageView2/0/w/100/h/100";
        }
        #endregion


        #region 链接ActionUrl

        /// <summary>
        /// 生成Url
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="actionn"></param>
        /// <param name="controller"></param>
        /// <param name="routedata"></param>
        /// <returns></returns>
        public static string ActionUrl(this HtmlHelper htmlHelper, string actionn, object routedata = null)
        {
            var url = htmlHelper.ActionUrl(actionn, "", new RouteValueDictionary(routedata));
            return url;
        }
        /// <summary>
        /// 生成Url
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="actionn"></param>
        /// <param name="controller"></param>
        /// <param name="routedata"></param>
        /// <returns></returns>
        public static string ActionUrl(this HtmlHelper htmlHelper, string actionn, string controller = null, object routedata = null)
        {
            var url = htmlHelper.ActionUrl(actionn, controller, new RouteValueDictionary(routedata));
            return url;
        }
        /// <summary>
        /// 生成Url
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="actionn"></param>
        /// <param name="controller"></param>
        /// <param name="routedata"></param>
        /// <returns></returns>
        public static string ActionUrl(this HtmlHelper htmlHelper, string actionn, string controller, RouteValueDictionary routedata)
        {
            routedata = routedata ?? new RouteValueDictionary();
            if (routedata.All(a => a.Key.ToLowerInvariant() != "area"))
            {
                var area = htmlHelper.ViewContext.RouteData.DataTokens["area"] as string;
                if (area != null)
                {
                    area = area.ToLowerInvariant();
                    routedata.Add("area", area);
                }
            }
            RouteCollection routeCollection = htmlHelper.RouteCollection;
            var requestContext = htmlHelper.ViewContext.RequestContext;
            var url = UrlHelper.GenerateUrl(null, actionn, controller, routedata, routeCollection, requestContext, true);
            return url;
        }
        #endregion

    }
}

