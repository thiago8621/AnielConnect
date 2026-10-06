using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Regime_Anatel
    {
        [Display(Name = "REGIMEPUBLICO", ResourceType = typeof(Resources.RsCadastros))]
        Publico = 1,
        [Display(Name = "PUBLICOPRIVADO", ResourceType = typeof(Resources.RsCadastros))]
        PublicoPrivado = 2,
    }
}
