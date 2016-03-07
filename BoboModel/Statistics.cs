using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboModel
{

    public class StatisticsMonthSum
    {
        public string HairUid { get; set; }
        public string HairName { get; set; }
        public string HairLog { get; set; }
        public Nullable<int> HairStatus { get; set; }
        public Nullable<int> OrderStatus { get; set; }
        public Nullable<double> OrderPrice { get; set; }
        public Nullable<int> HairCount { get; set; }
        public Nullable<DateTime> Orderbookingdate { get; set; }
        public Nullable<int> Today_HairCount { get; set; }
        public Nullable<double> Today_Price { get; set; }
        public Nullable<int> Week_HairCount { get; set; }
        public Nullable<double> Week_Price { get; set; }
        public Nullable<int> Month_HairCount { get; set; }
        public Nullable<double> Month_Price { get; set; }
        public Nullable<int> AddCustomer { get; set; }
        public Nullable<int> LostCustomer { get; set; }
    }
    public class ComparerStatisticsMonthSum : IEqualityComparer<StatisticsMonthSum>
    {
        public bool Equals(StatisticsMonthSum x, StatisticsMonthSum y)
        {
            return x.HairUid == y.HairUid;
         
        }

        public int GetHashCode(StatisticsMonthSum obj)
        {
            return obj.HairUid.GetHashCode();
        }
    }

    public class AjaxEchartsParms
    {
        // 0：失败 1:成功  
        public int Status { get; set; }
        public string Msg { get; set; }
        public Dictionary<string, dynamic> Info { get; set; }
    }
}
