using Resources;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum FormaPagamentoCobranca
    {
        [Display(Name = "Formas_Pagamento_EVA", ResourceType = typeof(RsConfiguracao))]
        Formas_Pagamento_EVA = 1,
        [Display(Name = "Forma_Pagamento_Cliente", ResourceType = typeof(RsConfiguracao))]
        Forma_Pagamento_Cliente = 2
    }
}
