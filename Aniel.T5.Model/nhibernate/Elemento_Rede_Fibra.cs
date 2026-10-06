using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Aniel.Connect.Model.nhibernate;

namespace Aniel.T5.Model.nhibernate
{
    public class Elemento_Rede_Fibra : BaseModel
    {
        public virtual Elemento_Rede Elemento_Cabo { get; set; }
        public virtual long numLoose { get; set; }
        public virtual long numFibra { get; set; }
        public virtual Int64? Cliente_Plano_Prod { get; set; }
        public virtual long? Ponta_A { get; set; }
        public virtual long? Conexao_Ponta_A { get; set; }
        public virtual long? Ponta_B { get; set; }
        public virtual long? Conexao_Ponta_B { get; set; }
    }
}
