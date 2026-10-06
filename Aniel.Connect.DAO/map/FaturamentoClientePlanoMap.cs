using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class FaturamentoClientePlanoMap : ClassMapBase<FaturamentoClientePlanos>
    {
        public FaturamentoClientePlanoMap() : base()
        {
            Table("TB_FATURAMENTO_CLIENTE_PLANO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_FATURAMENTO_CLIENTE_PLANO");

            References(x => x.Faturamento_Cliente, "ID_FATURAMENTO_CLIENTE").ForeignKey("FK_FAT_CLIENTE_PLANO_FAT");
            References(x => x.PlanoProduto, "ID_PLANO_PRODUTO").ForeignKey("FK_FAT_CLIENTE_PLANO_PP");

            Map(x => x.Valor_Plano, "VALOR_PLANO");
            Map(x => x.Valor_Cliente, "VALOR_CLIENTE");
            Map(x => x.Valor_Instalacao, "VALOR_INSTALACAO");
            Map(x => x.Desconto_Progressivo_Valor, "DESCONTO_PROGRESSIVO_VALOR");
            Map(x => x.Desconto_Progressivo_Perc, "DESCONTO_PROGRESSIVO_PERC");
            Map(x => x.Multa_Carencia, "MULTA_CARENCIA");
        }
    }
}
