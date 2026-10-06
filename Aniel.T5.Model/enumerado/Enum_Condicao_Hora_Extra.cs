using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum Enum_Condicao_Hora_Extra
    {

        [Display(Name = "ATE_APOS", ResourceType = typeof(Resources.RsPontoEletronico))]
        Ate_Apos = 0,
        [Display(Name = "TODA_HR_EX_REALIZADA", ResourceType = typeof(Resources.RsPontoEletronico))]
        Toda_HR_EX_Realizada = 1
    }
}
