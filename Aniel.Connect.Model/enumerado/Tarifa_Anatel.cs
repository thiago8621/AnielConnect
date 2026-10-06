using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tarifa_Anatel
    {
        [Display(Name = "ATB", ResourceType = typeof(Resources.RsCadastros))]
        ATB = 0,
        [Display(Name = "FATB", ResourceType = typeof(Resources.RsCadastros))]
        FATB = 1,
    }
}
