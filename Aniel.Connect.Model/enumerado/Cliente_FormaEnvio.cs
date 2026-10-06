using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Cliente_FormaEnvio
    {
        [Display(Name = "EMAIL", ResourceType = typeof(Resources.RsCadastros))]
        Email = 0,
        [Display(Name = "CORREIO", ResourceType = typeof(Resources.RsCadastros))]
        Correio = 1
    }
}
