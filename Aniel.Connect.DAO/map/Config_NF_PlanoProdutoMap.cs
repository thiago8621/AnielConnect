using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class Config_NF_PlanoProdutoMap : ClassMapBase<Config_NF_PlanoProduto>
    {
        public Config_NF_PlanoProdutoMap() : base()
        {
            Table("TB_CONFIG_NF_PLANOPRODUTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CONFIG_NF_PLANOPRODUTO");

            References(x => x.Configuracao_Tributaria, "ID_CONFIGURACAO_TRIBUTARIA").ForeignKey("FK_CONFIG_NF_CONFIG_TRIBUTARIA");

            Map(x => x.ValorDiscriminado, "VALOR_DISCRIMINADO");
            Map(x => x.PercentualDiscriminado, "PERCENTUAL_DISCRIMINADO");
            Map(x => x.ModeloNotaFiscal, "MODELONOTAFISCAL").CustomType<Modelo_NotaFiscal>();
            Map(x => x.ValorDiscriminadoNf, "VALORDISCRIMINADONF");

            References(x => x.Plano, "ID_PLANO").ForeignKey("FK_CONFIG_NF_PLANO");
            References(x => x.Produto, "ID_PRODUTO").ForeignKey("FK_CONFIG_NF_PRODUTO");

            References(x => x.Empresa, "ID_EMPRESA").ForeignKey("FK_CONFIG_NF_EMPRESA");
            References(x => x.Filial, "ID_FILIAL").ForeignKey("FK_CONFIG_NF_FILIAL");
            References(x => x.ProdutosComp, "ID_PRODUTOS_COMP").ForeignKey("FK_CONFIG_NF_PRODUTOS_COMP");

            Map(x => x.Instalacao, "INSTALACAO");
        }
    }
}
