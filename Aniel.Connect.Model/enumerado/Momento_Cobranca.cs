using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum Momento_Cobranca
    {
        [Display(Name = "BAIXA_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        Baixa_Servico = 0,
        [Display(Name = "FECHAMENTO_FATURAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Fechamento_Faturamento = 1
    }
}
