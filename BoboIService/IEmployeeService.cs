using Boboframework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboIService
{
    public interface IEmployeeService : IBaseService<to_employee>
    {

    }

    public class EmployeeService : BaseService<to_employee>, IEmployeeService
    { 
    
    }
}
