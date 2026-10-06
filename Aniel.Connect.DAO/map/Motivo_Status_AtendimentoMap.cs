
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
    public class Motivo_Status_AtendimentoMap : ClassMapBase<Motivo_Status_Atendimento>
    {
        public Motivo_Status_AtendimentoMap() : base()
        {
            Table("TB_MOTIVO_STATUS_ATENDIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_MOTIVO_STATUS_ATENDIMENTO");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            //Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Status, "STATUS");
            Map(x => x.Status_Motivo, "STATUS_MOTIVO").CustomType<Status_Motivo>();
        }
    }
}
