using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Status_SLA_Atendimento
    {
        [Display(Name = "DENTRO_PRAZO", ResourceType = typeof(Resources.RsCadastros))]
        Dentro_Prazo = 0,
        [Display(Name = "ATENCAO", ResourceType = typeof(Resources.RsCadastros))]
        Atencao = 1,
        [Display(Name = "VENCIMENTO_PROXIMO", ResourceType = typeof(Resources.RsCadastros))]
        Vencimento_Proximo = 2,
        [Display(Name = "RISCO_PERDA_PRAZO", ResourceType = typeof(Resources.RsCadastros))]
        Risco_Perda_Prazo = 3,
        [Display(Name = "PRAZO_VENCIDO", ResourceType = typeof(Resources.RsCadastros))]
        Prazo_Vencido = 4
    }
}
