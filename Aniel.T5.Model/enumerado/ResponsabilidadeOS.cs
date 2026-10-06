using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.enumerado
{
    public enum ResponsabilidadeOS
    {
        
        [Display(Name = "LIVRE", ResourceType = typeof(Resources.RsCadastros))]
        LIVRE = 0,
        [Display(Name = "BACKOFFICE", ResourceType = typeof(Resources.RsCadastros))]
        COP = 1,
        [Display(Name = "TECNICO", ResourceType = typeof(Resources.RsCadastros))]
        TECNICO = 2
    }
}
