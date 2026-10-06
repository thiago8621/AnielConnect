using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum WebHook_Type
    {
        [Display(Name = "SEGUNDAVIA_BOLETO", ResourceType = typeof(Resources.RsCadastros))]
        SegundaVia_Boleto = 1,
        [Display(Name = "ATIVAR_PLANO", ResourceType = typeof(Resources.RsCadastros))]
        AtivarPlano = 2,
        [Display(Name = "BLOQUEARPLANO", ResourceType = typeof(Resources.RsCadastros))]
        BloquearPlano = 3,
        [Display(Name = "DESBLOQUEARPLANO", ResourceType = typeof(Resources.RsCadastros))]
        DesbloquearPlano = 4,
        [Display(Name = "CANCELARPLANO", ResourceType = typeof(Resources.RsCadastros))]
        CancelarPlano = 5
    }
}
