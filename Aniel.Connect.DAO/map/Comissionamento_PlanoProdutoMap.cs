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
    public class Comissionamento_PlanoProdutoMap : ClassMapBase<Comissionamento_PlanoProduto>
    {

        public Comissionamento_PlanoProdutoMap() : base()
        {
            Table("TB_COMISSAO_PLANO_PRODUTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_COMISSAO_PLANO_PRODUTO");

            References(x => x.Comissionamento, "ID_COMISSIONAMENTO").ForeignKey("FK_COMISS_PP_COMISSIONAMENTO");
            References(x => x.Plano, "ID_PLANO").ForeignKey("FK_COMISS_PP_PLANO");
            References(x => x.Produto, "ID_PRODUTO").ForeignKey("FK_COMISS_PP_PRODUTO");

            
            Map(x => x.Unidade, "UNIDADE").Length(10);
            Map(x => x.Quantidade, "QUANTIDADE");

        }


    }
}
