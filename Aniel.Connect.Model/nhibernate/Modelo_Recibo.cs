using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using System;
using Sinapse.Model.nhibernate;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Modelo_Recibo : BaseModel
    {
        public virtual String Nome_Arquivo { get; set; }
        public virtual String Extensao_Arquivo { get; set; }
        public virtual String Descricao { get; set; }

        public virtual Status_Default? Status { get; set; }
    }
}
