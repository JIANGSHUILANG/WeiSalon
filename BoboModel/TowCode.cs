using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboModel
{
    public class TowCode
    {
        public int status { get; set; }
        public string msg { get; set; }
        public TowCodeInfo info { get; set; }
    }

    public class TowCodeInfo
    {
        public string url { get; set; }
    }
}
