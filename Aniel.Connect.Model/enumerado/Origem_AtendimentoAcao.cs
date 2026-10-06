using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Origem_AtendimentoAcao
    {
        
        [Display(Name = "CONNECT", ResourceType = typeof(Resources.RsCadastros))]
        CONNECT = 0,
        [Display(Name = "CHAT", ResourceType = typeof(Resources.RsCadastros))]
        CHAT = 1,
        [Display(Name = "EMAIL", ResourceType = typeof(Resources.RsCadastros))]
        EMAIL = 2,
        [Display(Name = "MANUAL", ResourceType = typeof(Resources.RsCadastros))]
        MANUAL = 3,

    }
}
