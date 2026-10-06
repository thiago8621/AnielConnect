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
    public class Motivo_Status_Atendimento : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual Status_Motivo Status_Motivo { get; set; }
    }
}
