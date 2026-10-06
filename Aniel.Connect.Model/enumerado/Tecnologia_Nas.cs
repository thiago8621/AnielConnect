using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tecnologia_Nas
    {
        [Display(Name = "MIKROTIK", ResourceType = typeof(Resources.RsCadastros))]
        Mikrotik = 0,
        [Display(Name = "CISCO", ResourceType = typeof(Resources.RsCadastros))]
        Cisco = 1,
        [Display(Name = "OTHER", ResourceType = typeof(Resources.RsCadastros))]
        other = 2
    }
}
