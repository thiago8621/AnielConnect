using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.enumerado
{
    public enum TipoCarregamentoBateria
    {
        [Display(Name = "USB", ResourceType = typeof(Resources.RsCadastros))]
        Usb = 0,
        [Display(Name = "AC", ResourceType = typeof(Resources.RsCadastros))]
        Ac = 1,
        [Display(Name = "WIRELESS", ResourceType = typeof(Resources.RsCadastros))]
        Wireless = 2,
        [Display(Name = "SEM_CARREGAR", ResourceType = typeof(Resources.RsCadastros))]
        Sem_Carregar = 3
    }
}
