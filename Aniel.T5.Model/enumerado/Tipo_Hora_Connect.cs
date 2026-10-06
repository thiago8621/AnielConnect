using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum Tipo_Hora_Connect
    {
        [Display(Name = "HORARIO_AGENDAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Horario_Agendamento = 0,
        [Display(Name = "HORARIO_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Horario_Atendimento = 1,
        [Display(Name = "HORARIO_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        Horario_Servico = 2
    }
}
