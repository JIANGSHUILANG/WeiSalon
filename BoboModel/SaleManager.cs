using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboModel
{
   public class SaleManager:BaseDtoModel
    {
        public int sm_id { get; set; }
        public string sm_s_uid { get; set; }
        public string sm_name { get; set; }
        public System.DateTime sm_date { get; set; }
        public string sm_oldname { get; set; }
        public Nullable<System.DateTime> sm_editdate { get; set; }
    }
}
