using Boboframework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboIService
{
    public interface IT_HairOffService : IBaseService<t_hairoff>
    { }
    public partial class T_HairOffService : BaseService<t_hairoff>, IT_HairOffService
    { }
}
