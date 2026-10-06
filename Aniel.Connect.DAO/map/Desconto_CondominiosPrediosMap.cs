using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class Desconto_CondominiosPrediosMap : ClassMapBase<Desconto_CondominiosPredios>
    {
        public Desconto_CondominiosPrediosMap() : base()
        {
            Table("TB_DESCONTO_CONDOMINIOSPRED");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_DESCONTO_CONDOMINIOSPRED");
            Map(x => x.Cond_Descricao, "DESCRICAO");
            References(x => x.CondominioPredio, "ID_CONDOMINIO").ForeignKey("FK_CONDOMINIOSPRED_CONDOMINIO");
            References(x => x.Desconto, "ID_DESCONTO").ForeignKey("FK_CONDOMINIOSPRED_DESCONTO");
        }
    }
}
