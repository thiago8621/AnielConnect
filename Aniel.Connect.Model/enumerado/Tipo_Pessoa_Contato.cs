using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Pessoa_Contato
    {
        [Display(Name = "TITULAR", ResourceType = typeof(Resources.RsCadastros))]
        Titular = 0,
        [Display(Name = "OUTROS", ResourceType = typeof(Resources.RsCadastros))]
        Outros = 1
    }
}
