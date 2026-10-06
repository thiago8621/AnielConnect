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
    public class Agente_Tipo_atendimento : BaseModel
    {
        public virtual Perfil_Agente Agente { get; set; }
        public virtual Tipo_Atendimento Tipo_atendimento { get; set; }
    }
}
