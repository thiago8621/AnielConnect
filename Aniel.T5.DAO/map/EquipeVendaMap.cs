using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class EquipeVendaMap : ClassMapBase<EquipeVenda>
    {
        public EquipeVendaMap() : base()
        {
            Table("TB_EQUIPE_VENDA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_EQUIPE_VENDA");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            References(x => x.funcionario, "ID_FUNCIONARIO").ForeignKey("FK_FUNCIONARIO_ID");        
        }
    }
}