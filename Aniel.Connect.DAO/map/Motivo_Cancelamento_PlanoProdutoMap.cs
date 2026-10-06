using Sinapse.DAO.map;
using Aniel.Connect.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class Motivo_Cancelamento_PlanoProdutoMap : ClassMapBase<Motivo_Cancelamento_PlanoProduto>
    {
        public Motivo_Cancelamento_PlanoProdutoMap() : base()
        {
            Table("TB_MOTIVO_CANC_PLANOPRODUTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_MOTIVO_CANCEL_PLANOPRODUTO");

            Map(x => x.Motivo, "MOTIVO").Length(100);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
