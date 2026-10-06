using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class PerfilAgendamentoAreaMap : ClassMapBase<PerfilAgendamentoArea>
    {
        public PerfilAgendamentoAreaMap() : base()
        {
            Table("TB_PERFIL_AGENDAMENTO_AREA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PERFIL_AGENDAMENTO_AREA");
            Map(p => p.Escritorio, "COD_ESCRITORIO").Length(4);
            Map(p => p.Area, "COD_AREA").Length(4);
            Map(p => p.Localidade, "LOCALIDADE").Length(30);
            Map(p => p.Nivel, "NIVEL");
            Map(p => p.Todas_Localidades, "TODAS_LOCALIDADES");
            Map(p => p.Todas_Areas, "TODAS_AREAS");

            References(p => p.PerfilAgendamento, "ID_PERFIL_AGENDAMENTO").ForeignKey("FK_PERFIL_AGEND_AREA");
        }
    }
}
