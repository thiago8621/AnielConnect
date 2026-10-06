using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class SLA_Atendimento_PlanoProdutoMap : ClassMapBase<SLA_Atendimento_PlanoProduto>
    {
        public SLA_Atendimento_PlanoProdutoMap() : base()
        {
            Table("TB_SLA_ATENDIMENTO_PLAN_PROD");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_SLA_ATENDIMENTO_PLAN_PROD");
          
            References(x => x.SLA_Atendimento, "ID_SLA_ATENDIMENTO").ForeignKey("FK_SLA_ATENDIMENTO_PP_SLA_ATENDIMENTO");
            References(x => x.Plano, "ID_PLANO").ForeignKey("FK_SLA_ATENDIMENTO_PP_PLANO");
            References(x => x.Produto, "ID_PRODUTO").ForeignKey("FK_SLA_ATENDIMENTO_PP_PRODUTO");
            
        }
    }
}
