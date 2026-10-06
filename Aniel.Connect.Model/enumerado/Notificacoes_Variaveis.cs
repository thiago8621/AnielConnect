using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum Notificacoes_Variaveis
    {
        [Display(Name = "VAR_GABINETE", ResourceType = typeof(Resources.RsCadastros))]
        Gabinete,
        [Display(Name = "VAR_GEM_PORT", ResourceType = typeof(Resources.RsCadastros))]
        GemPort,

        [Display(Name = "VAR_IP_GERENCIA", ResourceType = typeof(Resources.RsCadastros))]
        IpGerencia,
        [Display(Name = "VAR_LOGIN_ONU_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        LoginOnuCliente,

        [Display(Name = "VAR_NOME", ResourceType = typeof(Resources.RsCadastros))]
        Nome,
    }
}
