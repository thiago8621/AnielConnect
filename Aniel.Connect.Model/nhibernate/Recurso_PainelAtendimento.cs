using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Recurso_PainelAtendimento : BaseModel
    {
        public virtual String Valor { get; set; }
        public virtual Modelo_PainelAtendimento Modelo_PainelAtendimento { get; set; }
        public virtual Recurso_PainelAtendimentoEnum Recurso{ get; set; }
    }
}
