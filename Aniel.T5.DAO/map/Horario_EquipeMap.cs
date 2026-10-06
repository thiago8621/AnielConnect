using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class Horario_EquipeMap : ClassMapBase<Horario_Equipe>
    {
        public Horario_EquipeMap() : base() 
        {
            Table("TB_HORARIO_EQUIPE_CONNECT");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_HORARIO_EQUIPE_CONNECT");

            References(x => x.Equipe, "EQUIPE")
            .ForeignKey("FK_HORARIO_EQUIPE_EQUIPE")
            .Index("IDX_EQUIPE_APENASNO_DATA")
            .Index("IDX_EQUIPE_DATA")
            .Index("IDX_EQUPE_DATA_APENAS_NO_DIA")
            .Index("IDX_HORARIO_EQUIPE_DATADESC")
            .Index("TB_HORARIO_EQUIPE_CONNECT_IDX3");


            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

            References(x => x.Horario, "ID_HORARIO").ForeignKey("FK_HORARIO_EQUIPE_HORARIO");

            Map(x => x.Data, "DATA")
            .Index("IDX_APENAS_NO_DIA_DATA")
            .Index("IDX_DATA_EQUIPE_CONNECT_DESC")
            .Index("IDX_EQUIPE_APENASNO_DATA")
            .Index("IDX_EQUIPE_DATA")
            .Index("IDX_EQUPE_DATA_APENAS_NO_DIA")
            .Index("IDX_HORARIO_EQUIPE_DATADESC")
            .Index("TB_HORARIO_EQUIPE_CONNECT_IDX2")
            .Index("TB_HORARIO_EQUIPE_CONNECT_IDX3");

            Map(x => x.ApenasNoDia, "APENAS_NO_DIA")
            .Index("IDX_APENAS_NO_DIA_DATA")
            .Index("IDX_EQUIPE_APENASNO_DATA")
            .Index("IDX_EQUPE_DATA_APENAS_NO_DIA")
            .Index("TB_HORARIO_EQUIPE_CONNECT_IDX3");

            Map(x => x.Indisponivel, "INDISPONIVEL");
            References(x => x.PerfilAgendamento, "ID_PERFIL_AGENDAMENTO").ForeignKey("FK_HORARIO_PERFIL_AGEND");
        }
    }
}
