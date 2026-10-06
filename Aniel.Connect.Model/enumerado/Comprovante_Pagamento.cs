using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Comprovante_Pagamento
    {
        [Display(Name = "NOTA_FISCAL", ResourceType = typeof(Resources.RsCadastros))]
        Nota_Fiscal = 1,
        [Display(Name = "RECIBO", ResourceType = typeof(Resources.RsCadastros))]
        Recibo = 2,
        [Display(Name = "NENHUM", ResourceType = typeof(Resources.RsCadastros))]
        Nenhum = 3
    }
}
