using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Recibo : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual String Conteudo { get; set; }
    }
}
