using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class PlanoProdutoAniel : BaseModel
    {
        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }

        public virtual Tipo_PlanoProduto Tipo { get; set; }

        public virtual Status_PlanoProduto Status { get; set; }
    }
}
