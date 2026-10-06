using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Atendimento_Zona
    {
        [Display(Name = "URBANO", ResourceType = typeof(Resources.RsCadastros))]
        Urbano = 0,
        [Display(Name = "RURAL", ResourceType = typeof(Resources.RsCadastros))]
        Rural = 1
    }
}
