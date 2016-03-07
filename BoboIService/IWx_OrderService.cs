using Boboframework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboIService
{
    public interface IWx_OrderService : IBaseService<wx_order>
    {

    }
    public class Wx_OrderService : BaseService<wx_order>, IWx_OrderService
    {

    }
}
