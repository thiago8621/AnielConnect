using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class ProdutoMap : ClassMap<Produto>
    {
        public ProdutoMap() 
        {
            Table("TB_PRODUTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("Plano_Produto");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Codigo, "CODIGO").Length(20);
            Map(x => x.Detalhes, "DETALHES").Length(5000);

            Map(x => x.Status_Inicial, "STATUS_INICIAL").CustomType<Status_PlanoProduto>();
            Map(x => x.Situacao, "SITUACAO").CustomType<Situacao_Produto>();
            Map(x => x.ValorProduto, "VALORPRODUTO");
            Map(x => x.ValorProdutoFinal, "VALORPRODUTOFINAL");

            Map(x => x.AtivarValidade, "ATIVAR_VALIDADE");
            Map(x => x.Data_Validade, "DATA_VALIDADE");

            Map(x => x.OsPremier, "OS_PREMIER");
            Map(x => x.Nome_Venda, "Nome_Venda").Length(100);
            Map(x => x.Tem_Pro_Rata, "TEM_PRO_RATA");


            Map(x => x.Pro_Rata_Habilitacao, "PRO_RATA_HABILITACAO");
            Map(x => x.Pro_Rata_Interrupcao, "PRO_RATA_INTERRUPCAO");
            Map(x => x.Pro_Rata_Cancelamento, "PRO_RATA_CANCELAMENTO");

            Map(x => x.EmitirContratoProduto, "EMITE_CONTRATO");
            References(x => x.ModeloContratoProduto, "ID_CONTRATO").ForeignKey("FK_PRODUTO_MODELO_C");

            References(x => x.Conta_Financeira,"ID_CONTA_FINANCEIRA").ForeignKey("FK_PRODUTO_CF");
            Map(x => x.FrequenciaCobrancaProduto, "FREQUENCIA_COBRANCA").CustomType<Frequencia_Cobranca>();
            Map(x => x.ComprovantePagamentoProduto, "COMPROVANTE_PAGAMENTO").CustomType<Comprovante_Pagamento>();

            References(x => x.Cfop, "ID_CFOP").ForeignKey("FK_PRODUTO_CFOP");
            Map(x => x.FormatoCobrancaProduto, "FORMATO_COBRANCA").CustomType<Formato_Cobranca>();
            References(x => x.Modelo_Recibo, "ID_MODELO_RECIBO").ForeignKey("FK_PRODUTO_MODELO_RECIBO");

            Map(x => x.DescontoProgressivoProduto, "DESCONTO_PROGRESSIVO");
            Map(x => x.Percentual_Desconto, "PERCENTUAL_DESCONTO");
            Map(x => x.MensalidadeInicialProduto, "MENSALIDADE_INICIAL");
            Map(x => x.MensalidadeFinalProduto, "MENSALIDADE_FINAL");
            Map(x => x.PercentualDescontoProgressivo, "PERCENTUAL_DESC_PROGRESSIVO");
            Map(x => x.ValorDescontoProgressivo, "VALOR_DESC_PROGRESSIVO");
            References(x => x.FormaPagamentoCob, "ID_FORMAPAG_COB").ForeignKey("FK_PRODUTO_FORMA"); ;
            Map(x => x.Id_Tipo_Servico, "ID_TIPO_SERVICO");
            Map(x => x.Id_Subtipo_Servico, "ID_SUBTIPO_SERVICO");
            Map(x => x.Tipo_Produto, "TIPO_PRODUTO").CustomType<Tipo_Produto>();
            Map(x => x.Momento_Cobranca, "MOMENTO_COBRANCA").CustomType<Momento_Cobranca>();
            Map(x => x.Cobranca_Realizada, "COBRANCA_REALIZADA").CustomType<Cobranca_Realizada>();
            Map(x => x.Plano_Comercializavel, "PLANO_COMERCIALIZAVEL");
            Map(x => x.Dias_Vencimento, "DIAS_VENCIMENTO");

            Map(x => x.UsaPlanoAdicional, "USA_PLANO_ADICIONAL");

            #region Dados da Instalação

            Map(x => x.CobrancaInstalacao, "COBRANCA_INSTALACAO");
            Map(x => x.ValorInstalacao, "VALOR_INSTALACAO");
            References(x => x.ContaFinanceiraInstalacao, "ID_CONTA_FINANCEIRA_INSTALACAO").ForeignKey("FK_PRODUTO_CF_INSTALACAO");
            Map(x => x.MomentoCobrancaInstalacao, "MOMENTO_COBRANCA_INSTALACAO").CustomType<Momento_Cobranca>();
            Map(x => x.ComprovantePagamentoInstalacao, "COMPROVANTE_PGTO_INSTALACAO").CustomType<Comprovante_Pagamento>();
            Map(x => x.OSTrocaEndereco, "OS_TROCA_ENDERECO");

            #endregion

            HasMany(x => x.Config_NF_PlanoProduto).KeyColumn("ID_PRODUTO").Cascade.All().Inverse();
            HasMany(x => x.Faixas_Mensalidades).KeyColumn("ID_PRODUTO").Cascade.All().Inverse();

            Map(x => x.UtilizaVendaAgroup, "UTILIZA_VENDA_AGRUPADA");
            Map(x => x.ValorVendaAgrupada, "VALOR_VENDA_AGRUPADA");
        }
    }
}
