using Aniel.Connect.Model;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class Cliente_PlanoProduto_ParcelaMap : ClassMapBase<Cliente_PlanoProduto_Parcela>
    {
        public Cliente_PlanoProduto_ParcelaMap() : base()
        {
            Table("TB_CLIENTE_PLANO_PRODUTO_PARC");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CLIENTE_PLANO_PRODUTO_PARC");

            Map(x => x.Parcela, "PARCELA");
            Map(x => x.TotParcelas, "TOT_PARCELAS");
            Map(x => x.ValorParcela, "VALOR_PARCELA");
            Map(x => x.ValorTotal, "VALOR_TOTAL");

            References(x => x.PlanoProduto, "ID_PLANO_PRODUTO").ForeignKey("FK_CLIENTE_PPPARC_PP");

            References(x => x.Faturamento, "ID_FATURAMENTO_CLIENTE").ForeignKey("FK_CLIENTE_PPPARC_FATCLI");
        }
    }
}
