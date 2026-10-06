using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class EquipeVendaMap : ClassMapBase<EquipeVenda>
    {
        public EquipeVendaMap()
        {
            Table("TB_EQUIPE_VENDA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_EQUIPE_VENDA");

            Map(u => u.Descricao, "DESCRICAO");

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

            #region Mobile
            HasMany(x => x.Supervisores).KeyColumn("ID_SUPERVISOR").Cascade.All().Inverse();
            #endregion

        }
    }
}
