using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum SituacaoFuncionarioAniel
    {
        [Display(Name = "FUNCIONARIO_ATIVO", ResourceType = typeof(Resources.RsCadastros))]
        Ativo = 0,
        [Display(Name = "FUNCIONARIO_AFASTADO", ResourceType = typeof(Resources.RsCadastros))]
        Afastado = 1,
        [Display(Name = "FUNCIONARIO_DEMITIDO", ResourceType = typeof(Resources.RsCadastros))]
        Demitido = 2,
        [Display(Name = "FUNCIONARIO_FERIAS", ResourceType = typeof(Resources.RsCadastros))]
        Ferias = 3,
        [Display(Name = "FUNCIONARIO_TRANSFERIDO", ResourceType = typeof(Resources.RsCadastros))]
        Transferido = 4,
        [Display(Name = "FUNCIONARIO_FOLGA", ResourceType = typeof(Resources.RsCadastros))]
        Folga = 5,
        [Display(Name = "FUNCIONARIO_ESCALADO", ResourceType = typeof(Resources.RsCadastros))]
        Escalado = 6
    }
}
