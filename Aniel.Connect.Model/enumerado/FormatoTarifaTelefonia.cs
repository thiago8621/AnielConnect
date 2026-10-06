using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum FormatoTarifaTelefonia
    {
        [Display(Name = "BRASIL", ResourceType = typeof(Resources.RsCadastros))]
        Brasil = 0,
        [Display(Name = "E164", ResourceType = typeof(Resources.RsCadastros))]
        E164 = 1,
        
    }
}