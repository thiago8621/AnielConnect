using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class Condominio_PredioBlocoMap : ClassMapBase<Condominio_PredioBloco>
    {
        public Condominio_PredioBlocoMap() : base()
        {
            Table("TB_COND_PR_BLOCO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_COND_PR_BLOCO");
            
            Map(x => x.Descricao, "DESCRICAO").Index("IDX_BLOCO_COND_PR").Length(100);           
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            //HasMany(x => x.Apartamentos).KeyColumn("ID_APARTAMENTO").Cascade.All().Inverse();
            References(u => u.Condominio, "ID_CONDOMINIO").ForeignKey("FK_CONDOMINIO_PREDIO");
        }
    }
}
