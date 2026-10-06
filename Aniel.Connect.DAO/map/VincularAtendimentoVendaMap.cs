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
    public class VincularAtendimentoVendaMap : ClassMapBase<VincularAtendimentoVenda>
    {
        public VincularAtendimentoVendaMap() : base()
        {
            Table("TB_VINC_ATENDE_VENDA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ID");
            Map(x => x.Venda, "ID_VENDA");
            Map(x => x.Atendimento, "ID_ATENDIMENTO");
                    
        }
    }
}
