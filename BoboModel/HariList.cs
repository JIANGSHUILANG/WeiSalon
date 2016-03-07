using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboModel
{
    /// <summary>
    /// 沙龙发型师信息表
    /// </summary>
    public class HariStyleList
    {
        /// <summary>
        /// 发型师ID
        /// </summary>
        public Guid Huid { get; set; }

        /// <summary>
        /// 沙龙发型师昵称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 剪发价格
        /// </summary>
        public string Hairprice { get; set; }

        /// <summary>
        /// 擅长标签
        /// </summary>
        public string Exp { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// 沙龙ID
        /// </summary>
        public Guid? Uid { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Cell { get; set; }

        /// <summary>
        /// 沙龙是否绑定了该发型师
        /// </summary>
        public bool Flag { get; set; }

        public int? status { get; set; }

        public double? score { get; set; }

        public string Msg { get; set; }

        public string salonname { get; set; }

        public string salonlogo { get; set; }
        /// <summary>
        /// 绑定时间
        /// </summary>
        public Nullable<DateTime> enterdate { get; set; }

        /// <summary>
        /// 解除绑定时间
        /// </summary>
        public Nullable<DateTime> offdate { get; set; }

        public Nullable<DateTime> ho_date { get; set; }

        public Nullable<int> ho_status { get; set; }
    }
}
