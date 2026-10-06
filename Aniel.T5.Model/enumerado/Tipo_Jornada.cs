using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum Tipo_Jornada
    {
        [Display(Name = "HORARIOS_FIXOS", ResourceType = typeof(Resources.RsPontoEletronico))]
        Horarios_Fixos = 1,
        [Display(Name = "HORARIO_FLEXIVEL", ResourceType = typeof(Resources.RsPontoEletronico))]
        Horario_Flexivel = 2
    }
}
