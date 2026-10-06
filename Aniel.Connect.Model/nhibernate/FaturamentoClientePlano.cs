using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class FaturamentoClientePlanos : BaseModel
    {
        public virtual Faturamento_Cliente Faturamento_Cliente { get; set; }
        public virtual Cliente_PlanoProduto PlanoProduto { get; set; }
        public virtual double? Valor_Plano { get; set; }
        public virtual double? Valor_Cliente { get; set; }
        public virtual double? Valor_Instalacao { get; set; }
        public virtual double? Desconto_Progressivo_Valor { get; set; }
        public virtual double? Desconto_Progressivo_Perc { get; set; }
        public virtual double? Multa_Carencia { get; set; }
    }
}
