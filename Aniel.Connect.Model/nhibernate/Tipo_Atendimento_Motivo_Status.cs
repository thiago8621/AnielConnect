using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Tipo_Atendimento_Motivo_Status : BaseModel
    {
        public virtual Motivo_Status_Atendimento Motivo { get; set; }

        public virtual Tipo_Atendimento Tipo_Atendimento { get; set; }    
    }
}
