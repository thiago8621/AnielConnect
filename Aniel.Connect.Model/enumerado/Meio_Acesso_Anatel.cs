using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Meio_Acesso_Anatel
    {
        [Display(Name = "CABO_COAXIAL", ResourceType = typeof(Resources.RsCadastros))]
        Cabo_Coaxial = 0,
        [Display(Name = "CABO_METALICO", ResourceType = typeof(Resources.RsCadastros))]
        Cabo_Metalico = 1,
        [Display(Name = "SATELITE", ResourceType = typeof(Resources.RsCadastros))]
        Satelite = 2,
        [Display(Name = "RADIO", ResourceType = typeof(Resources.RsCadastros))]
        Radio = 3,
        [Display(Name = "FIBRA", ResourceType = typeof(Resources.RsCadastros))]
        Fibra =4 
    }
}
