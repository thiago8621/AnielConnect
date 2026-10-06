using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum Tipo_Intervalo_Jornada
    {
        [Display(Name = "INTERVALO_FIXO", ResourceType = typeof(Resources.RsPontoEletronico))]
        Intervalo_Fixo = 1,
        [Display(Name = "INTERVALO_FLEXIVEL", ResourceType = typeof(Resources.RsPontoEletronico))]
        Intervalo_Flexivel = 2
    }
}
