using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum Status_Venda
    {
        [Display(Name = "ANALISE_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Analise_Cliente = 0,

        [Display(Name = "AVALIACAO_APROVADA", ResourceType = typeof(Resources.RsCadastros))]
        Avaliação_Aprovada = 2,

        [Display(Name = "AVALIACAO_REPROVADA", ResourceType = typeof(Resources.RsCadastros))]
        Avaliação_Reprovada = 3,

        [Display(Name = "VENDA_CANCELADA", ResourceType = typeof(Resources.RsCadastros))]
        Venda_Cancelada = 4,

        [Display(Name = "AGUARDANDO_AVALIACAO_COMERCIAL", ResourceType = typeof(Resources.RsCadastros))]
        Aguardando_Avaliacao_Comercial = 5,

        [Display(Name = "VENDA_APROVADA", ResourceType = typeof(Resources.RsCadastros))]
        Venda_Aprovada = 6,

        [Display(Name = "VENDA_REPROVADA", ResourceType = typeof(Resources.RsCadastros))]
        Venda_Reprovada = 7,
        
        [Display(Name = "AGUARDANDO_LIBERACAO", ResourceType = typeof(Resources.RsCadastros))]
        Aguardando_Liberacao = 8,
        
        [Display(Name = "LIBERACAO_REPROVADA", ResourceType = typeof(Resources.RsCadastros))]
        liberacao_Reprovada = 9,

        [Display(Name = "LIBERACAO_APROVADA", ResourceType = typeof(Resources.RsCadastros))]
        liberacao_Aprovada = 10,

        [Display(Name = "AGUARDANDO_PAGAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        aguardando_Pagamento = 11,

        [Display(Name = "PAGAMENTO_REJEITADO", ResourceType = typeof(Resources.RsCadastros))]
        pagamento_Rejeitado = 12

    }
}
