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
    public class Coluna_PainelAtendimento : BaseModel
    {
        
        public virtual Modelo_PainelAtendimento Modelo_PainelAtendimento { get; set; }
        public virtual Coluna_PainelAtendimentoEnum Coluna { get; set; }

    }
}
