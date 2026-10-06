using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Campanha_PlanoProduto : BaseModel
    {
        public virtual String Nome { get; set; }
        public virtual Plano Plano { get; set; }
        public virtual Int64? Id_Plano_Produto { get; set; }
        public virtual Int64 Parcela_Instalacao { get; set; }
                
        public virtual Atividade_Decisoria? Validade_Plano_Produto { get; set; }
        public virtual DateTime? Data_Validade { get; set; }        
        public virtual Produto Produto { get; set; }
        public virtual Campanha Campanha { get; set; }
        public virtual String Novo_Nome_Plano_Produto { get; set; }
        public virtual Double Valor_Plano_Produto { get; set; }
        public virtual Double Valor_Instalacao { get; set; }
        public virtual PlanoProdutoEnum? IsPlanProd { get; set; }

    }

}
