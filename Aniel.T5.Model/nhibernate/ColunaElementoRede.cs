using Aniel.T5.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class ColunaElementoRede : BaseModel
    {
        public virtual Modelo_Elemento_Rede Modelo_Elemento_Rede { get; set; }
        public virtual ColunaModeloElementoEnum? Coluna { get; set; }
        public virtual bool Obrigatorio { get; set; }
        public virtual Modelo_CampoDinamico CampoDinamco { get; set; }
    }
}
