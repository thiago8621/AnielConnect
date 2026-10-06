using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum Enum_Forma_Compensacao
    {
        [Display(Name = "DIARIA", ResourceType = typeof(Resources.RsPontoEletronico))]
        Diaria = 0,
        [Display(Name = "MENSAL", ResourceType = typeof(Resources.RsPontoEletronico))]
        Mensal = 1
    }
}
