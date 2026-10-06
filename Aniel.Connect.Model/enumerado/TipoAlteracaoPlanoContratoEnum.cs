using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum TipoAlteracaoPlanoContratoEnum
    {
        [Display(Name = "UPGRADE", ResourceType = typeof(Resources.RsCadastros))]
        upgrade = 0,
        [Display(Name = "DOWNGRADE", ResourceType = typeof(Resources.RsCadastros))]
        downgrade = 1
    }
}
