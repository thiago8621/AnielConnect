using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Status_Conta_Receber
    {
        [Display(Name = "ABERTO", ResourceType = typeof(Resources.RsCadastros))]
        Aberto = 0,
        [Display(Name = "BAIXADO", ResourceType = typeof(Resources.RsCadastros))]
        Baixado = 1,        
        [Display(Name = "VENCIDO", ResourceType = typeof(Resources.RsCadastros))]
        Vencido = 2,
        [Display(Name = "CANCELADO", ResourceType = typeof(Resources.RsCadastros))]
        Cancelado = 3,
        [Display(Name = "NEGOCIADO", ResourceType = typeof(Resources.RsCadastros))]
        Negociado = 4,
        [Display(Name = "TODOS", ResourceType = typeof(Resources.RsFinanceiro))]
        Todos = 5,
        [Display(Name = "RECUSADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Recusado = 6
    }
}
