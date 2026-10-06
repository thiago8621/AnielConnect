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
    public class Tipo_Atendimento_Motivo_StatusMap : ClassMapBase<Tipo_Atendimento_Motivo_Status>
    {
        public Tipo_Atendimento_Motivo_StatusMap() : base()
        {
            Table("TB_TIPO_ATEND_MOTIVO_STATUS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_TIPO_ATEND_MOTIVO_STATUS");

            References(x => x.Tipo_Atendimento, "ID_TIPO_ATENDIMENTO").ForeignKey("FK_TIPO_ATEND_TIPO_ATEND");
            References(x => x.Motivo, "ID_MOTIVO_STATUS").ForeignKey("FK_TIPO_ATEND_TIPO_ATEND");
        }
    }
}
