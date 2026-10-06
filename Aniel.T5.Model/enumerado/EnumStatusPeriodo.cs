using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum Enum_Status_Periodo
    {
        [Display(Name = "FECHADO", ResourceType = typeof(Resources.RsCadastros))]
        Fechado = 0,
        [Display(Name = "ABERTO", ResourceType = typeof(Resources.RsCadastros))]
        Aberto = 1

    }
}