using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Cliente_PlanoProduto_Parcela : BaseModel
    {
        public virtual Cliente_PlanoProduto PlanoProduto { get; set; }

        public virtual Int32? Parcela { get; set; }
        public virtual Int32? TotParcelas { get; set; }
        public virtual Double? ValorParcela { get; set; }
        public virtual Double? ValorTotal { get; set; }

        public virtual Faturamento_Cliente Faturamento { get; set; }
    }
}
