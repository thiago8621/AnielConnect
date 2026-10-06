using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.map;

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
            References(u => u.Departamento, "ID_DEPARTAMENTO").ForeignKey("FK_DEPARTAMENTO");

        }
    }
}
