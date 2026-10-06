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
    public class Plano_Produto_CompMap : ClassMapBase<Plano_Produto_Comp>
    {
        public Plano_Produto_CompMap() : base()
        {
            Table("TB_PLANO_PRODUTOS_COMP");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PLANO_PRODUTOS_COMP");

            Map(x => x.ValorFixo, "VALOR_FIXO");
            Map(x => x.ValorPercentual, "VALOR_PERCENTUAL");
            Map(x => x.ValorTotal, "VALOR_TOTAL");

            References(x => x.Plano, "ID_PLANO").ForeignKey("FK_PLANO_PRODUTOS_COMP");
            References(x => x.ProdutosComp, "ID_PRODUTOS_COMP").ForeignKey("FK_PRODUTO_COMP");
        }
    }
}
