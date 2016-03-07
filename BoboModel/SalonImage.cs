using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboModel
{
    public class SalonImage
    {
        public int Id { get; set; }
        public Nullable<System.Guid> Suid { get; set; }
        public string Photo { get; set; }
        public int? Kind { get; set; }
        public int? Status { get; set; }
    }
}
