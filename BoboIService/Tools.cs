using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboIService
{
    public class Tools
    {
        /// <summary>
        /// 美发店审核状态枚举
        /// </summary>
        public enum SalonStatus
        {
            注销 = 0,
            申请注册 = 1,
            未通过 = 2,
            通过未付款 = 3,
            上线 = 4
        }
    }
}
