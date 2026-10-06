using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum TipoDiscagem
    {
        [Display(Name = "SCM", ResourceType = typeof(Resources.RsCadastros))]
        SCM = 0,
        [Display(Name = "STFC", ResourceType = typeof(Resources.RsCadastros))]
        STFC = 1,
        [Display(Name = "ROTA_DINAMICA", ResourceType = typeof(Resources.RsCadastros))]
        Dinamico = 2
    }
}
