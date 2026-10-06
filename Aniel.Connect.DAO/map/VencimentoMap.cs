using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class VencimentoMap : ClassMapBase<Vencimento>
    {
        public VencimentoMap() : base()
        {
            Table("TB_VENCIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_VENCIMENTO");

            Map(x => x.DiaFechamento, "DIA_FECHAMENTO");
            Map(x => x.DiaVencimento, "DIA_VENCIMENTO");
            Map(x => x.DiaFaturamento, "DIA_FATURAMENTO");
            Map(x => x.GeraFaturamento_Auto, "GERA_FATURAMENTO_AUTO");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

        }

    }
}
