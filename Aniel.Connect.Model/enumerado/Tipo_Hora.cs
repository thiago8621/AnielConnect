using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinapse.Model.enumerados
{
    public enum Tipo_HORA
    {
        [Display(Name = "HORAS_CORRIDAS", ResourceType = typeof(Resources.RsCadastros))]
        Horas_Corridas = 0,
        [Display(Name = "HORAS_UTEIS", ResourceType = typeof(Resources.RsCadastros))]
        Horas_Uteis = 1
    }
}