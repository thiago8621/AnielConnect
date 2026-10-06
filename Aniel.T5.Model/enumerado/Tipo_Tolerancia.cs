using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.enumerado
{
    public enum Tipo_Tolerancia
    {
        [Display(Name = "ULTRAPASSAR_TOLERANCIA_BATIDA", ResourceType = typeof(Resources.RsPontoEletronico))]
        Tolerancia_Batida = 1,
        [Display(Name = "ULTRAPASSAR_TOLERANCIA_DIARIA", ResourceType = typeof(Resources.RsPontoEletronico))]
        Tolerancia_Diaria = 2,
        [Display(Name = "CONSIDERAR_AMBAS", ResourceType = typeof(Resources.RsPontoEletronico))]
        Considerar_Ambas = 3
    }
}
