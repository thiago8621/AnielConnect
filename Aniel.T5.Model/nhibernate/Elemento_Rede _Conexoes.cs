using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;

namespace Aniel.T5.Model.nhibernate
{
 
    public class Elemento_Rede_Conexoes : BaseModel
    {
        public virtual Elemento_Rede_Fibra Elemento_Origem { get; set; }
        public virtual Elemento_Rede Elementos_Destino { get; set; }

    }

}
