using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Canal_Comunicacao
    {
        [Display(Name = "WHATSAPP", ResourceType = typeof(Resources.RsCadastros))]
        Whatsapp = 0,

        [Display(Name = "TELEGRAM", ResourceType = typeof(Resources.RsCadastros))]
        Telegram = 1,

        [Display(Name = "MESSENGER", ResourceType = typeof(Resources.RsCadastros))]
        Messenger = 2,

        [Display(Name = "INSTAGRAM", ResourceType = typeof(Resources.RsCadastros))]
        Instagram = 3,

        [Display(Name = "SKYPE", ResourceType = typeof(Resources.RsCadastros))]
        Skype = 4,

    }
}
