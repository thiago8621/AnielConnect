using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum Formato_Plano
    {
        [Display(Name = "NACIONAL", ResourceType = typeof(Resources.RsCadastros))]
        Nacional = 1,
        [Display(Name = "INTERNACIONAL", ResourceType = typeof(Resources.RsCadastros))]
        Internacional = 2,
        
    }
}