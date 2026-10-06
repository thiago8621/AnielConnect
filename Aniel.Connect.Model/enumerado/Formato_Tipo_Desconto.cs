using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Aniel.Connect.Model.enumerado
{
    public enum Formato_Tipo_Desconto
    {
        [Display(Name = "ACRESCIMO", ResourceType = typeof(Resources.RsCadastros))]
        Acrescimo = 0,
        [Display(Name = "DESCONTO", ResourceType = typeof(Resources.RsCadastros))]
        Desconto = 1
    }
}

