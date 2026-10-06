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
    public class Motivo_Aprovacao_Resticao : BaseModel
    {     
        public virtual String Descricao { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual EnumMotivo_Aprovacao_Restricao Tipo_Restricao { get; set; }       
        public virtual EnumExibeAppOla ExibeAppOla { get; set; }
    }
}
