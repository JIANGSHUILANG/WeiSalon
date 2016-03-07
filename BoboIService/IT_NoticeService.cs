using Boboframework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboIService
{
    public interface IT_NoticeService : IBaseService<t_notice>
    {
    }
    public class T_NoticeService : BaseService<t_notice>, IT_NoticeService
    {
    }
}
