using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Pool_Ip_Endereco : BaseModel
    {
        public virtual String Ip { get; set; }
        public virtual Pool_Ip_Range_StatusEnum Status_Ip { get; set; }
        public virtual LoginRadius Login { get; set; }
        public virtual Pool_Ip Pool_Ip { get; set; }
    }
}
