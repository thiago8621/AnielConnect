using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Forma_Pagamento_VencimentoMap : ClassMapBase<Forma_Pagamento_Vencimento>
    {

        public Forma_Pagamento_VencimentoMap() : base()
        {
            Table("TB_FORMA_PAGAMENTO_VENCIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_FORMA_PAGAMENTO_VENCIMENTO");

            References(x => x.Forma_Pagamento, "ID_FORMA_PAGAMENTO").ForeignKey("FK_FPAGTO_FORMPAGTO");
            References(x => x.Vencimento, "ID_VENCIMENTO").ForeignKey("FK_FPAGTO_VENCIMENTO");
        }

    }
}
