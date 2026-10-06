using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum Enum_Status_Solicitacao_Ajuste
    {
        [Display(Name = "AGUARDANDO_AVALIACAO", ResourceType = typeof(Resources.RsPontoEletronico))]
        AGUARDANDO_AVALIACAO = 0,

        [Display(Name = "APROVADO", ResourceType = typeof(Resources.RsPontoEletronico))]
        APROVADO = 1,

        [Display(Name = "REPROVADO", ResourceType = typeof(Resources.RsPontoEletronico))]
        REPROVADO = 2
    }
}
