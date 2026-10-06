using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class PlanoProdutoAdicional : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual Plano Id_Plano { get; set; }
        public virtual Produto Id_Produto { get; set; }
        //public virtual Tipo_PlanoProduto Tipo_PlanoProduto { get; set; }
        public virtual Int64? Id_Plano_Produto_Adicional { get; set; }

    }
}
