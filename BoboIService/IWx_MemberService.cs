using Boboframework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboIService
{
    public interface IWx_MemberService : IBaseService<wx_member>
    {
    }
    public class Wx_MemberService : BaseService<wx_member>,IWx_MemberService
    {
    }
}
