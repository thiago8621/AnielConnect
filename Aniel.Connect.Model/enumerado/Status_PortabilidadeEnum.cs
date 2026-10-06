using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Aniel.Connect.Model.enumerado
{
    public enum Status_Portabilidade
    {

        [Display(Name = "NAO_PORTADO", ResourceType = typeof(Resources.RsCadastros))]
        NaoPortado = 0,

        [Display(Name = "PORTADO", ResourceType = typeof(Resources.RsCadastros))]
        Portado = 1
    }
}
