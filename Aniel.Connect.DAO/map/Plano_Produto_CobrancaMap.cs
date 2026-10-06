using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
   public class Plano_Produto_CobrancaMap : ClassMapBase<Plano_Produto_Cobranca>
    {
        public Plano_Produto_CobrancaMap() : base() {
            Table("TB_PLANO_PRODUTO_COBRANCA");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PLANO_PRODUTO_COBRANCA");
            References(m => m.Plano, "ID_PLANO").ForeignKey("FK_PLANO_COBRANCA");
            References(m => m.Produto, "ID_PRODUTO").ForeignKey("FK_PRODUTO_COBRANCA");
            References(m => m.Cobranca_Servico, "ID_COBRANCA_SERVICO").ForeignKey("FK_PLANO_PRODUTO_COBRANCA");

        }
    }
}
