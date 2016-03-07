using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoboModel
{
    /// <summary>
    /// 创建人  ： JiangSL
    // 创建标识： 2015/10/14 09:52
    ///Oracle发型师信息表实体类
    /// </summary>
    [Serializable]
    public class O_T_Hairstylist
    {
        public O_T_Hairstylist() { }
        public int status { get; set; }
        public string msg { get; set; }
        public Info info { get; set; }
        public Extdata extdata { get; set; }
    }

    public class Info
    {
        public string uid { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
        public string price2 { get; set; }
        public string mysign { get; set; }
    }

    public class Extdata
    {
        public int? integralchange { get; set; }
    }
}
