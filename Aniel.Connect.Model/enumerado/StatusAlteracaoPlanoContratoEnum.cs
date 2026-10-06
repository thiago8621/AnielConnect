using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum StatusAlteracaoPlanoContratoEnum
    {
        [Display(Name = "ABERTO", ResourceType = typeof(Resources.RsCadastros))]
        aberto = 0,
        [Display(Name = "APROVADO", ResourceType = typeof(Resources.RsCadastros))]
        aprovado = 1,
        [Display(Name = "REPROVADO", ResourceType = typeof(Resources.RsCadastros))]
        reprovado = 2
    }
}
