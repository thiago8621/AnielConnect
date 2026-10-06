using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Tributacao
    {
        [Display(Name = "ICMS", ResourceType = typeof(Resources.RsFinanceiro))]
        ICMS = 1,
        [Display(Name = "ISSQN", ResourceType = typeof(Resources.RsFinanceiro))]
        ISSQN = 2,
        [Display(Name = "ISENTO", ResourceType = typeof(Resources.RsFinanceiro))]
        Isento = 3
    }
}
