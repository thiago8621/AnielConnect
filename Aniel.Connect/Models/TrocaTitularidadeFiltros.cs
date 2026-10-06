namespace Aniel.Connect.Models
{
    public class TrocaTitularidadeFiltros
    {

    }

    public enum StatusTrocaTitularidadeModels : int
    {
        Todos = 0,
        Analise_Cliente = 1,
        Cancelado = 2,
        Aprovado = 3,
        Reprovado = 4 
    } 
    
    public enum TipoPessoaTitular : int
    {
        Todos = 99,
        Fisica = 0,
        Fisica_Rural = 1,
        Juridica = 2,
        Juridica_Rural
    }

    public enum EtapaTrocaTitularidade : int
    {
        Todos = 0,
        Dados_Gerais,
        PendenciaFinan_TitularAtual,
        PendenciaFinan_TitularNovo,
        PendenciaEnder_TitularAtual,
        ConsultaCredito_TitularNovo,
        TermoAdesao_TitularNovo
    }
    
    public enum TipoPeriodo : int
    {
        Todos = 0,
        DataCriacao,
        DataAprovacao,
        DataCancelamento
    }
}