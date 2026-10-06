using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Faixas_Mensalidades : BaseModel
    {
        public virtual Int16? Parcela_Ini { get; set; }
        public virtual Int16? Parcela_Fin { get; set; }
        public virtual Double? Percentual_Desconto { get; set; }
        public virtual Double? Valor_Desconto { get; set; }

        public virtual Plano Plano { get; set; }
        public virtual Produto Produto { get; set; }
    }
}
