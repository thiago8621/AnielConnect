using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum SecaoConnectPerfilEnum
    {
        [Display(Name = "ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        ATENDIMENTO = 0,
        [Display(Name = "COMERCIAL", ResourceType = typeof(Resources.RsCadastros))]
        COMERCIAL = 1,
        [Display(Name = "SERVICOS", ResourceType = typeof(Resources.RsCadastros))]
        SERVICOS = 2,
        [Display(Name = "GESTAO_CONEXOES", ResourceType = typeof(Resources.RsCadastros))]
        GESTAO_CONEXOES = 3
    }
    
    public enum TipoUsuarioEnum
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
        [Display(Name = "CONVIDADO", ResourceType = typeof(Resources.RsCadastros))]
        CONVIDADO = 11,
    }


}
