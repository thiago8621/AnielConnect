using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class PerfilAgendamentoNaturezaMap : ClassMapBase<PerfilAgendamentoNatureza>
    {
        public PerfilAgendamentoNaturezaMap() : base()
        {
            Table("TB_PERFIL_AGENDAMENTO_NATUREZA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_AGENDNATUREZA");
                        
            References(u => u.Natureza, "ID_NATUREZA").ForeignKey("FK_AGEND_NATUREZA");
            References(u => u.PerfilAgendamento, "ID_PERFIL_AGENDAMENTO").ForeignKey("FK_PERFIL_AGEND_NATU");

        }
    }
}
