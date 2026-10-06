using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Conexao_ONU
    {
        [Display(Name = "PPPOE", ResourceType = typeof(Resources.RsCadastros))]
        PPPoE = 3,
        [Display(Name = "BRIDGE", ResourceType = typeof(Resources.RsCadastros))]
        BRIDGE = 0,
        [Display(Name = "DHCP", ResourceType = typeof(Resources.RsCadastros))]
        DHCP = 1,
        [Display(Name = "IP_ESTATICO", ResourceType = typeof(Resources.RsCadastros))]
        IP_ESTATICO = 2
    }
}
