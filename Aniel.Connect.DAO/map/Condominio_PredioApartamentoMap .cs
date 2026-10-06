using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class Condominio_PredioApartamentoMap : ClassMapBase<Condominio_PredioApartamento>
    {
        public Condominio_PredioApartamentoMap() : base()
        {
            Table("TB_COND_PR_APARTAMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_COND_PR_APARTAMENTO");
            References(u => u.Bloco, "ID_BLOCO").ForeignKey("FK_COND_PR_BLOCO");
            Map(x => x.Descricao, "DESCRICAO").Index("IDX_APARTAMENTO_COND_PR").Length(100);           
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();            
        }
    }
}
