using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace Aniel.T5.Model.enumerado
{
    public enum EquipeStatusTipo
    {
        [Display(Name = "DISPONIVEL", ResourceType = typeof(Resources.RsCadastros))]
        Disponivel = 0,
        [Display(Name = "AUSENTE", ResourceType = typeof(Resources.RsCadastros))]
        Ausente = 1,
        [Display(Name = "EM_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Em_Atendimento = 2,
        [Display(Name = "EM_DESLOCAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Em_Deslocamento = 3,
        [Display(Name = "INDISPONIVEL", ResourceType = typeof(Resources.RsCadastros))]
        Em_Intervalo = 4,  
        [Display(Name = "INDISPONIVEL",  ResourceType = typeof(Resources.RsCadastros))]
        Indisponivel = 5,
        [Display(Name = "SEM_STATUS", ResourceType = typeof(Resources.RsCadastros))]
        Sem_Status = 6,
        [Display(Name = "ATENDIMENTO_FORA_HORARIO", ResourceType = typeof(Resources.RsCadastros))]
        Atendimento_Fora_Horario = 7,
        [Display(Name = "FORA_HORARIO_TRABALHO", ResourceType = typeof(Resources.RsCadastros))]
        Fora_Horario_Trabalho = 8,
        [Display(Name = "SEM_AGENDA", ResourceType = typeof(Resources.RsCadastros))]
        Sem_Agenda = 9
    }
}
