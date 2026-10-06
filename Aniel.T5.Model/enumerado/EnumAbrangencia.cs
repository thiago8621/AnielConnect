using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum Enum_Abrangencia
    {
        [Display(Name = "INTERNACIONAL", ResourceType = typeof(Resources.RsCadastros))]
        Internacional = 0,
        [Display(Name = "NACIONAL", ResourceType = typeof(Resources.RsCadastros))]
        Nacional = 1,
        [Display(Name = "MUNICIPAL", ResourceType = typeof(Resources.RsCadastros))]
        Municipal = 2,
        [Display(Name = "ESTADUAL", ResourceType = typeof(Resources.RsCadastros))]
        Estadual = 3
    }
}
