using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class PerfilAgendamentoMap : ClassMapBase<PerfilAgendamento>
    {
        public PerfilAgendamentoMap() : base()
        {
            Table("TB_PERFIL_AGENDAMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PERFIL_AGENDAMENTO");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            HasMany(x => x.Areas).KeyColumn("ID_PERFIL_AGENDAMENTO").Cascade.All().Inverse();
            HasMany(x => x.Naturezas).KeyColumn("ID_PERFIL_AGENDAMENTO").Cascade.All().Inverse();

        }
    }
}
