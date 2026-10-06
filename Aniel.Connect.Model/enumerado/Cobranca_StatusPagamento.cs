using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum Cobranca_StatusPagamento
    {
        [Display(Name = "AGUARDANDO", ResourceType = typeof(Resources.RsFinanceiro))]
        Aguardando = 0,
        [Display(Name = "CANCELADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Cancelado = 1,
        [Display(Name = "CONTESTADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Contestado = 2,
        [Display(Name = "ANALISE", ResourceType = typeof(Resources.RsFinanceiro))]
        Analise = 3,
        [Display(Name = "ESTORNADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Estornado = 4,
        [Display(Name = "RECEBIDO", ResourceType = typeof(Resources.RsFinanceiro))]
        Recebido = 5,
        [Display(Name = "RECEBIDOMANUAL", ResourceType = typeof(Resources.RsFinanceiro))]
        RecebidoManual = 6,
        [Display(Name = "RECUSADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Recusado = 7,
        [Display(Name = "VENCIDO", ResourceType = typeof(Resources.RsFinanceiro))]
        Vencido = 8,
        [Display(Name = "NEGOCIADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Negociado = 9
    }
}
