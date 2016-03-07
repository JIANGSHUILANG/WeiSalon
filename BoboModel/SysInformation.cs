using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboModel
{
    /// <summary>
    /// 系统公告
    /// </summary>
    public class SysInformation : BaseDtoModel
    {
        public int sy_id { get; set; }
        public string sy_title { get; set; }
        public string sy_content { get; set; }
        public System.DateTime sy_date { get; set; }
        public int sy_status { get; set; }
    }
}
