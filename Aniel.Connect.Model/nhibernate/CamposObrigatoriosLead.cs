using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class CamposObrigatoriosLead : BaseModel
    {
        public virtual CamposObrigatoriosLeadEnum Campo { get; set; }
        public virtual Boolean Bloqueado { get; set; }
        public virtual Boolean Obrigatorio { get; set; }
    }
}
