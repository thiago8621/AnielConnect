using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.enumerado
{
    public enum Origem_Documento_Producao
    {
        [Display(Name = "CONNECT", ResourceType = typeof(Resources.RsBase))]
        Connect = 0,
        [Display(Name = "OLA_CLIENTE", ResourceType = typeof(Resources.RsBase))]
        Ola_Cliente = 1,
        [Display(Name = "PREMIER", ResourceType = typeof(Resources.RsBase))]
        Premier = 2
    }
}
