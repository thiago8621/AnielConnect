using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum VariaveisConfigEnvioMensagens
    {
        [Display(Name = "VAR_NOME_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        VAR_NOME_CLIENTE,
        [Display(Name = "VAR_CPF", ResourceType = typeof(Resources.RsCadastros))]
        VAR_CPF,
        [Display(Name = "VAR_RG", ResourceType = typeof(Resources.RsCadastros))]
        VAR_RG ,
        [Display(Name = "VAR_NUMERO_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_NUMERO_CONTRATO ,
        [Display(Name = "VAR_NUMERO_OS", ResourceType = typeof(Resources.RsCadastros))]
        VAR_NUMERO_OS ,
        [Display(Name = "VAR_NOME_TECNICO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_NOME_TECNICO ,
        [Display(Name = "VAR_DATA_HORA_AGENDAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DATA_HORA_AGENDAMENTO ,
        [Display(Name = "VAR_MATRICULA_TECNICO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_MATRICULA_TECNICO ,
        [Display(Name = "VAR_PESQUISA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_PESQUISA = 8,
    }
}
