using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum EnumInd_InscricaoEstadual
    {
        [Display(Name = "CONTRIBUINTE_ICMS", ResourceType = typeof(Resources.RsCadastros))]
        Contribuinte_ICMS = 1,
        [Display(Name = "CONTRIBUINTE_ISENTO_IE", ResourceType = typeof(Resources.RsCadastros))]
        Contribuintes_Isento_IE = 2,
        [Display(Name = "NAO_CONTRIBUINTE", ResourceType = typeof(Resources.RsCadastros))]
        Nao_Contribuinte = 9,
    }
}
