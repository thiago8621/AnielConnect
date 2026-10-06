using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum AcoesProvaVidaTermoEnum
    {
        [Display(Name = "UP", ResourceType = typeof(Resources.RsCadastros))]
        Up = 0,
        [Display(Name = "DOWN", ResourceType = typeof(Resources.RsCadastros))]
        Down = 1,
        [Display(Name = "LEFT", ResourceType = typeof(Resources.RsCadastros))]
        Left = 2,
        [Display(Name = "RIGHT", ResourceType = typeof(Resources.RsCadastros))]
        Right = 3,
        [Display(Name = "SMILE", ResourceType = typeof(Resources.RsCadastros))]
        Smile = 4,
        [Display(Name = "BLINK", ResourceType = typeof(Resources.RsCadastros))]
        Blink = 5
    }
}
