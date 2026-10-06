using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Visibilidade
    {
        [Display(Name = "PRIVADO", ResourceType = typeof(Resources.RsCadastros))]
        Privado = 0,
        [Display(Name = "PUBLICO", ResourceType = typeof(Resources.RsCadastros))]
        Publico = 1
    }
}
