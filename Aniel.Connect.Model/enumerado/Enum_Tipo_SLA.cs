using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum Enum_Tipo_Sla
    {
        [Display(Name = "ABERTURA_ENUM", ResourceType = typeof(Resources.RsCadastros))]
        Abertura,
        [Display(Name = "AGENDAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Agendamento
    }
}
