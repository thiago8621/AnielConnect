using Aniel.Connect.Model.enumerado;
using System;
using Sinapse.Model.nhibernate;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.Model.nhibernate
{
    public class Motivo_Status_Tipo_Atendimento : BaseModel
    {
        public virtual Motivo_Status_Atendimento Motivo_Status_Atendimento { get; set; }
        public virtual Status_Atendimento Status_Motivo { get; set; }
        public virtual Tipo_Atendimento Tipo_Atendimento { get; set; }

    }
}
