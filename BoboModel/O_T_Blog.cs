using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboModel
{
    /// <summary>
    /// 创建人  ： JiangSL
    // 创建标识： 2015/10/15 14:32
    ///Oracle发型师信息表实体类
    ////api/weisalon/gethairweibolist
    /// </summary>
    [Serializable]
    public class O_T_Blog
    {
        public int status { get; set; }
        public string msg { get; set; }
        public BlogInfoList info { get; set; }
        public Extdata extdata { get; set; }

    }

    public class BlogInfoList
    {
        public List<BlogInfo> list { get; set; }
    }

    public class BlogInfo
    {
        public string bgid { get; set; }
        public string bguid { get; set; }
        public string bgtitle { get; set; }
        public string bgcontent { get; set; }
        public string bgdate { get; set; }
        public int bgsource { get; set; }
        public int bgphoto { get; set; }
        public string bgimage { get; set; }
        public int bgscore { get; set; }
        public string logo { get; set; }
        public int imagewidth { get; set; }
        public int imageheight { get; set; }
        public int bgtop { get; set; }
        public int bgfavnum { get; set; }
        public int bgstatus { get; set; }
    }
}
