using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.enumerado
{
    public enum FormatoDisparoAutomaticoRoteirizacao
    {
        
        [Display(Name = "UMA_VEZ", ResourceType = typeof(Resources.RsCadastros))]
        UMA_VEZ = 0,
        [Display(Name = "DIARIAMENTE", ResourceType = typeof(Resources.RsCadastros))]
        DIARIAMENTE = 1,
        [Display(Name = "SEMANALMENTE", ResourceType = typeof(Resources.RsCadastros))]
        SEMANALMENTE = 2,
        [Display(Name = "MENSALMENTE", ResourceType = typeof(Resources.RsCadastros))]
        MENSALMENTE = 3
    }
}
