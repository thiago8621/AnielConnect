using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Formato_Cobranca
    {
        [Display(Name = "PRE_PAGO", ResourceType = typeof(Resources.RsCadastros))]
        Pre_Pago = 0,
        [Display(Name = "POS_PAGO", ResourceType = typeof(Resources.RsCadastros))]
        Pos_Pago = 1        
    }
}
