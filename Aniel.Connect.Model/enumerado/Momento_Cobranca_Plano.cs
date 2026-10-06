using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum Momento_Cobranca_Plano
    {
        [Display(Name = "HABILITACAO_PLANO_PRODUTO", ResourceType = typeof(Resources.RsCadastros))]
        Habilitacao_Plano_Produto = 1,
        [Display(Name = "FINALIZAR_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        Finalizar_Venda = 2,
        [Display(Name = "FECHAMENTO_FATURAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Fechamento_Faturamento = 3
    }
}
