using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Cobranca_Realizada
    {
        [Display(Name = "BOLETO", ResourceType = typeof(Resources.RsCadastros))]
        Boleto = 0,
        [Display(Name = "FORMA_PAGAMENTO_VINCULADO", ResourceType = typeof(Resources.RsCadastros))]
        Forma_Pagmento_Vinculado = 1
    }
}
