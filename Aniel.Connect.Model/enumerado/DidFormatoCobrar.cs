using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum DidFormatoCobrar
    {
        [Display(Name = "CIRCUITO", ResourceType = typeof(Resources.RsCadastros))]
        Circuito = 0,
        [Display(Name = "DID", ResourceType = typeof(Resources.RsCadastros))]
        Did = 1,
        
    }
}