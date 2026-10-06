using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class ComboPlanoProdutoMap : ClassMapBase<ComboPlanoProduto>
    {
        public ComboPlanoProdutoMap()
        {
            Table("TB_COMBO_PLANO_PRODUTO");

            Id(p => p.Id, "ID").GeneratedBy.Sequence("GN_COMBO_PLAN_PROD");

            References(p => p.Combo, "ID_COMBO").ForeignKey("FK_COMBO_PLAN_PROD_COMBO").Not.LazyLoad();
            References(p => p.Plano, "ID_PLANO").ForeignKey("FK_COMBO_PLAN_PROD_PLANO").Not.LazyLoad();
            References(p => p.Produto, "ID_PRODUTO").ForeignKey("FK_COMBO_PLAN_PROD_PRODUTO").Not.LazyLoad();
            
            Map(p => p.Valor, "VALOR");
            
            Map(p => p.CobrarInstalacao, "COBRAR_INSTALACAO");
            Map(p => p.ValorInstalacao, "VALOR_INSTALACAO");
        }
    }
}
