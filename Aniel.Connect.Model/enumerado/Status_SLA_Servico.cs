using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Status_SLA_Servico
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
    public enum Status_SLA_Horas_Servico
    {
        [Display(Name = "DENTRO_PRAZO_HORAS", ResourceType = typeof(Resources.RsCadastros))]
        Dentro_Prazo = 0,
        [Display(Name = "ATENCAO_HORAS", ResourceType = typeof(Resources.RsCadastros))]
        Atencao = 1,
        [Display(Name = "VENCIMENTO_PROXIMO_HORAS", ResourceType = typeof(Resources.RsCadastros))]
        Vencimento_Proximo = 2,
        [Display(Name = "RISCO_PERDA_PRAZO_HORAS", ResourceType = typeof(Resources.RsCadastros))]
        Risco_Perda_Prazo = 3,
        [Display(Name = "SLA_VENCIDO", ResourceType = typeof(Resources.RsCadastros))]
        Prazo_Vencido = 4,
        [Display(Name = "FECHADO_PRAZO_HORAS", ResourceType = typeof(Resources.RsCadastros))]
        Fechado_Prazo_Horas = 5,
        [Display(Name = "FECHADO_FORA_PRAZO", ResourceType = typeof(Resources.RsCadastros))]
        FECHADO_FORA_PRAZO = 6,
        [Display(Name = "SEM_INDICADOR_DE_SLA", ResourceType = typeof(Resources.RsCadastros))]
        SEM_INDICADOR_DE_SLA = 7
    }
}
