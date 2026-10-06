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
    public class Motivo_Suspensao_ContratualMap : ClassMapBase<Motivo_Suspensao_Contratual>
    {
        public Motivo_Suspensao_ContratualMap() : base()
        {
            Table("TB_MOTIVO_SUSPENSAO_CONTRATUAL");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_MOTIVO_SUSPENSAO_CONTRATUAL");

            Map(x => x.Motivo, "MOTIVO").Length(300);

            Map(x => x.Motivo_Para, "MOTIVO_PARA").CustomType<Status_Motivo_Para>();

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

            Map(x => x.Observacao_Motivo_Suspensao_Plano_Cliente_Outros, "OBSERVACAO_MOTIVO");
        }
    }
}
