using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class Categoria_ClienteAniel : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual Tipo_PrioridadeT5? Tipo_Prioridade { get; set; }
        public virtual Status_Default? Status { get; set; }
    }
}
