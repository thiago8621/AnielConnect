using System.ComponentModel.DataAnnotations;


namespace Aniel.T5.Model.enumerado
{
    public enum Enum_Tipo_Sla_
    {
        [Display(Name = "ABERTURA_ENUM", ResourceType = typeof(Resources.RsCadastros))]
        Abertura,
        [Display(Name = "AGENDAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Agendamento
    }
}
