using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Enum_Status_Chat
    {
        [Display(Name = "ONLINE", ResourceType = typeof(Resources.RsBase))]
        Online = 0,
        [Display(Name = "EM_ATENDIMENTO", ResourceType = typeof(Resources.RsBase))]
        Em_Atendimento = 1,
        [Display(Name = "OFFLINE", ResourceType = typeof(Resources.RsBase))]
        Offline = 2
    }
}
