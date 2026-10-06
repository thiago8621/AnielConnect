using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Comissionamento_PlanoProduto : BaseModel
    {
        public virtual Comissionamento Comissionamento { get; set; }

        public virtual Plano Plano { get; set; }

        public virtual Produto Produto { get; set; }

        public virtual String Unidade { get; set; }

        public virtual Double? Quantidade { get; set; }
    }
}
