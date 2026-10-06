using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Sistema
    {
        [Display(Name = "CONNECT", ResourceType = typeof(Resources.RsCadastros))]
        Connect    = 0,
        [Display(Name = "EVA", ResourceType = typeof(Resources.RsCadastros))]
        EVA        = 1,
        [Display(Name = "TOUCH", ResourceType = typeof(Resources.RsCadastros))]
        TOUCH      = 2,
        [Display(Name = "POINT", ResourceType = typeof(Resources.RsCadastros))]
        Point      = 3,
        [Display(Name = "INNOVATION", ResourceType = typeof(Resources.RsCadastros))]
        Innovation = 4,
        [Display(Name = "OLA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        OlaCliente = 5
    }
}
