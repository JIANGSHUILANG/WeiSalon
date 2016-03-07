using Boboframework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboIService
{
    public interface ISaleManagerService : IBaseService<t_salemanager> { }
    public class SaleManagerService : BaseService<t_salemanager>, ISaleManagerService { }
}
