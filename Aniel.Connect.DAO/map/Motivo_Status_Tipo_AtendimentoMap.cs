
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
    public class Motivo_Status_Tipo_AtendimentoMap : ClassMapBase<Motivo_Status_Tipo_Atendimento>
    {
        public Motivo_Status_Tipo_AtendimentoMap() : base()
        {
            Table("TB_MOTIVO_TIPO_STATUS_ATEND");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_MOTIVO_TIPO_STATUS_ATEN");
            References(u => u.Tipo_Atendimento, "ID_TIPO_ATENDIMENTO").ForeignKey("FK_TIPO_ATENDIMENTO");
            References(u => u.Motivo_Status_Atendimento, "ID_MOTIVO_STATUS_ATENDIMENTO").ForeignKey("FK_MOTIVO_STATUS_ATENDIMENTO");
            Map(x => x.Status_Motivo, "STATUS_ATENDIMENTO").CustomType<Status_Atendimento>();
        }
    }
}
