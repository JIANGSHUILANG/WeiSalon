using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboModel
{
    public class OrderManageInfo : BaseDtoModel
    {
        //沙龙名称
        public string salonname { get; set; }
        //发型师名字
        public string hairname { get; set; }
        //顾客名字
        public string customername { get; set; }
        //下单金额
        public double ordermoney { get; set; }
        //下单时间
        public Nullable<DateTime> ordertime { get; set; }
        //预约时间
        public Nullable<DateTime> bookingdate { get; set; }
        //顾客评论
        public string comment { get; set; }




    }
}
