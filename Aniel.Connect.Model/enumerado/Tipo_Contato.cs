using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Contato
    {
        [Display(Name = "EMAIL", ResourceType = typeof(Resources.RsCadastros))]
        Email = 1,
        [Display(Name = "FACEBOOK", ResourceType = typeof(Resources.RsCadastros))]
        Facebook = 2,
        [Display(Name = "OUTROS", ResourceType = typeof(Resources.RsCadastros))]
        Outros = 0

    }
}
