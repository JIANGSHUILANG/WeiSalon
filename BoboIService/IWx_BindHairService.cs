using Boboframework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboIService
{
    public interface IWx_BindHairService : IBaseService<wx_bindhair>
    {
    }
    public class Wx_BindHairService : BaseService<wx_bindhair>, IWx_BindHairService
    { 
    
    }
}
