using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class SupervisorEquipeVendaMap : ClassMapBase<SupervisorEquipeVenda>
    {
        public SupervisorEquipeVendaMap() : base()
        {
            Table("TB_EQUIPE_VENDA_SUPERVISOR");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_EQUIPEVENDASUPERVISOR");
            
            References(u => u.Supervisor, "ID_SUPERVISOR").ForeignKey("FK_SUPERVISOR");
            References(u => u.EquipeVenda, "ID_EQUIPE_VENDA").ForeignKey("FK_SUPERV_EQUIPE_VENDA");
        }
    }
}
