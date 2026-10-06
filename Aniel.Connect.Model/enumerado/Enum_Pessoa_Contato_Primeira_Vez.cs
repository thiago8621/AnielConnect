using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Enum_Pessoa_Contato_Primeira_Vez
    {

        [Display(Name = "PROSPECTO", ResourceType = typeof(Resources.RsCadastros))]
        Prospect = 0,
        [Display(Name = "CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Cliente = 1,

    }
}
