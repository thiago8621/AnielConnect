using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Coluna_PainelServico : BaseModel
    {
        public virtual Modelo_PainelServico Modelo_PainelServico { get; set; }
        public virtual Coluna_PainelServicoEnum Coluna { get; set; }
    }
}
