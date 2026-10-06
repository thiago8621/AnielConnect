using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class CriteriosParamCredito : BaseModel
    {

        public virtual String Criterio { get; set; }
        public virtual String Operador { get; set; }
        public virtual Int64 Valor { get; set; }
        public virtual Int64 Pontos { get; set; }
        public virtual Boolean Eliminatorio { get; set; }

        public virtual ParametrosConsultaCredito Id_ParametrosConsultaCredito { get; set; }

    }
}
