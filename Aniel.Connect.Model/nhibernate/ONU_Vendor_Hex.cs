using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class ONU_Vendor_Hex : BaseModel
    {
        public virtual string Vendor { get; set; }
        public virtual string Hexadecimal { get; set; }
        public virtual string Descricao { get; set; }
    }
}
