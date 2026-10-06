using System;

namespace Aniel.Connect.Models
{
    public class PainelProdutividadeModels
    {
    }
    public class ListaProdutividadeModels
    {
        public ListaProdutividadeModels()
        {
        }

        //Producao
        public String monitor { get; set; }
        public String supervisor { get; set; }
        public String tecnico { get; set; }
        public String grupoArea { get; set; }
        public Int64? osprodutiva { get; set; }
        public Int64? osimprodutiva { get; set; }
        public Int64? osTotal { get; set; }
        public String osporcentagemProducao { get; set; }
        public String osprecoProducao { get; set; }
        //Revisita
        public Int64? quantidadeRevisita { get; set; }
        public Int64? revisitaTotal { get; set; }
        public String porcentagemRevisita { get; set; }
        public String precoRevisita { get; set; }
        //BaixaProdutiva
        public Int64? baixaProdutivaPrazo { get; set; }
        public Int64? baixaImprodutivaPrazo { get; set; }
        public Int64? baixaPrazoTotal { get; set; }
        public String porcentagemBaixaNoPrazo { get; set; }
        public String precoBaixaTEC { get; set; }
        //Vistoria
        public Int64? vistoriaAprovada { get; set; }
        public Int64? vistoriaReprovada { get; set; }
        public Int64? vistoriaNaoFeitas { get; set; }
        public Int64? vistoriaTotal { get; set; }
        public Int64? vistoriasFechadas { get; set; }
        public String porcentagemVistoria { get; set; }
        public String precoVistoria { get; set; }
        //SLA
        public Int64? dentroPrazoSLA { get; set; }
        public Int64? foraPrazoSLA { get; set; }
        public String porcentagemSLA { get; set; }
        public Int64? totalSLA { get; set; }
        public String precoSLA { get; set; }
        //Total
        public String totalPrecoSomado { get; set; }
    }
    public class ProdutividadeDataTableRequest
    {
        public String TipoSelecionado { get; set; }
        public String PessoaAgrupamento { get; set; }
        public String ContratoValor { get; set; }
        public String ProjetoValor { get; set; }
        public String GrupoArea { get; set; }
        public String DataInicio { get; set; }
        public String DataFim { get; set; }
        public String TipoData { get; set; }
    }
    public class PainelProdutividadeFiltro
    {
        public String DataInicial { get; set; }
        public String DataFinal { get; set; }
        public String AgroupId { get; set; }
        public String ContratoId { get; set; }
        public String ProjetoId { get; set; }
        public String GrupoAreaId { get; set; }
    }
}


