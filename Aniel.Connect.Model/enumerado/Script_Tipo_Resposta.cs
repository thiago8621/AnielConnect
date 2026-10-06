using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Script_Tipo_Resposta
    {
        [Display(Name = "TEXTO", ResourceType = typeof(Resources.RsBase))]
        Texto = 0,
        [Display(Name = "VALOR", ResourceType = typeof(Resources.RsBase))]
        Valor = 1,
        [Display(Name = "DATA", ResourceType = typeof(Resources.RsBase))]
        Data = 2,
        [Display(Name = "UNICA_ESCOLHA", ResourceType = typeof(Resources.RsBase))]
        Unica_Escolha = 3,
        [Display(Name = "MULTIPLA_ESCOLHA", ResourceType = typeof(Resources.RsBase))]
        Multipla_Escolha = 4
    }
}
