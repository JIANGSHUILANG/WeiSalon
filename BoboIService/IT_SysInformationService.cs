using Boboframework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboIService
{
    /// <summary>
    /// 沙龙系统消息
    /// </summary>
    public partial interface IT_SysInformationService : IBaseService<t_sysinformation>
    {
    }
    public partial class T_SysInformationService : BaseService<t_sysinformation>, IT_SysInformationService
    {
    }
}
