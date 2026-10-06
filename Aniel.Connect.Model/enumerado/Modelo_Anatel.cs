using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Modelo_Anatel
    {
        [Display(Name = "MODELO_SCM", ResourceType = typeof(Resources.RsCadastros))]
        Modelo_SCM = 1,
        [Display(Name = "MODELO_SMP", ResourceType = typeof(Resources.RsCadastros))]
        Modelo_SMP = 2,
        [Display(Name = "MODELO_STFC", ResourceType = typeof(Resources.RsCadastros))]
        Modelo_STFC = 3,
        [Display(Name = "TV_ASSINATURA", ResourceType = typeof(Resources.RsCadastros))]
        TV_Assinatura = 4,
    }
}
