using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum ONU_Tipo_Identificacao
    {
        [Display(Name = "MAC", ResourceType = typeof(Resources.RsCadastros))]
        MAC = 1,
        [Display(Name = "SERIAL", ResourceType = typeof(Resources.RsCadastros))]
        LOID = 2
    }
}
