using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class CamposObrigatoriosAtendimento : BaseModel
    {
        public virtual CamposObrigatoriosAtendimentoEnum Campo { get; set; }
        public virtual String CampoString { get; set; }
        public virtual Boolean Bloqueado { get; set; }
        public virtual Boolean Obrigatorio { get; set; }

    }
}
