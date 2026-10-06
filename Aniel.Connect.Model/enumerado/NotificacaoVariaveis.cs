using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum VariaveisAtendimento
    {
        [Display(Name = "@NOME_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        NOME_CLIENTE = 0,
        [Display(Name = "@PROTOCOLO", ResourceType = typeof(Resources.RsCadastros))]
        PROTOCOLO = 1,
        [Display(Name = "@NUM_OS", ResourceType = typeof(Resources.RsCadastros))]
        NUM_OS = 2,
        [Display(Name = "@NOME_USUARIO_ACAO", ResourceType = typeof(Resources.RsCadastros))]
        NOME_USUARIO_ACAO = 3,
        [Display(Name = "@DATA_HORA", ResourceType = typeof(Resources.RsCadastros))]
        DATA_HORA = 4,

    }
    
    public enum VariaveisComercial
    {
        [Display(Name = "ATENDENTE", ResourceType = typeof(Resources.RsCadastros))]
        ATENDENTE = 0,
        [Display(Name = "SUPERVISOR_ATENDENTE", ResourceType = typeof(Resources.RsCadastros))]
        SUPERVISOR_ATENDENTE = 1,
        [Display(Name = "SUPERVISOR_COMERCIAL", ResourceType = typeof(Resources.RsCadastros))]
        SUPERVISOR_COMERCIAL = 2,
        [Display(Name = "VENDEDOR", ResourceType = typeof(Resources.RsCadastros))]
        VENDEDOR = 3,
        [Display(Name = "MONITOR_DE_EQUIPE", ResourceType = typeof(Resources.RsCadastros))]
        MONITOR_EQUIPE = 4,
        [Display(Name = "SUPERVISOR_EQUIPE", ResourceType = typeof(Resources.RsCadastros))]
        SUPERVISOR_EQUIPE = 5,
        [Display(Name = "TODOS_VENDEDOR", ResourceType = typeof(Resources.RsCadastros))]
        TODOS_VENDEDORES = 6,
        [Display(Name = "TODOS_SUPERVISOR_COMERCIAL", ResourceType = typeof(Resources.RsCadastros))]
        TODOS_SUPERVISOR_COMERCIAL = 7,
        [Display(Name = "TODOS_ATENDENTE", ResourceType = typeof(Resources.RsCadastros))]
        TODOS_ATENDENTE = 8,
        [Display(Name = "TODOS_SUPERVISOR_ATENDENTE", ResourceType = typeof(Resources.RsCadastros))]
        TODOS_SUPERVISOR_ATENDENTE = 9,
        [Display(Name = "TODOS_ATENDENTE_AGENTE", ResourceType = typeof(Resources.RsCadastros))]
        TODOS_ATENDENTE_AGENTE = 10,
    }
    
    public enum VariaveisServico
    {
        [Display(Name = "ATENDENTE", ResourceType = typeof(Resources.RsCadastros))]
        ATENDENTE = 11,
        [Display(Name = "SUPERVISOR_ATENDENTE", ResourceType = typeof(Resources.RsCadastros))]
        SUPERVISOR_ATENDENTE = 12,
    }


}
