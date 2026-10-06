using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum Tipo_Autenticacao_Login
    {
        [Display(Name = "PPPOE", ResourceType = typeof(Resources.RsCadastros))]
        PPPOE = 1,
        [Display(Name = "IPOE", ResourceType = typeof(Resources.RsCadastros))]
        IPOE = 2
    }
}