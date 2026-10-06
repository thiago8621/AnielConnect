using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Cliente
    {
        [Display(Name = "CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Cliente = 0,        
        [Display(Name = "PROSPECTO", ResourceType = typeof(Resources.RsCadastros))]
        Prospecto = 1,
    }
}
