using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Sinapse.Model;

namespace Aniel.T5.Model.nhibernate
{

    public class Tipo_ElementoCaderno : BaseModel
    {
        public virtual String Unidade_Custo{ get; set; }

        public virtual String Codigo { get; set; }
        public virtual Tipo_Elemento_Rede Tipo { get; set; }
    }



}
