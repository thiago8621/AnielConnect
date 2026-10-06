using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum TipoAtividadeAtendimento
    {
        [Display(Name = "INTERNO", ResourceType = typeof(Resources.RsCadastros))]
        Interno = 0,
        [Display(Name = "PUBLICO", ResourceType = typeof(Resources.RsCadastros))]
        Publico = 1
        
    }
}