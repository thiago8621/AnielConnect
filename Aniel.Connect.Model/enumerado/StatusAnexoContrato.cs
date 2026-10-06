using Resources;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum StatusAnexoContrato
    {
        [Display(Name = "PENDENTE", ResourceType = typeof(RsCadastros))]
        Pendente = 0,
        [Display(Name = "APROVADO", ResourceType = typeof(RsCadastros))]
        Aprovado = 1,
        [Display(Name = "REPROVADO", ResourceType = typeof(RsCadastros))]
        Reprovado = 2
    }
}
