using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Status_Credito
    {
        [Display(Name = "CANCELADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Cancelado = 0,
        [Display(Name = "PENDENTE", ResourceType = typeof(Resources.RsFinanceiro))]
        Pendente = 1,
        [Display(Name = "APROVADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Aprovado = 2,
        [Display(Name = "PROCESSADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Processado = 3,
    }
}
