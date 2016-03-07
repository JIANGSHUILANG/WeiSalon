using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboModel
{
   public class Notice:BaseDtoModel
    {
        public int tn_id { get; set; }
        public string tn_s_uid { get; set; }
        public string tn_content { get; set; }
        public System.DateTime tn_date { get; set; }
        public int tn_status { get; set; }
    }
}
