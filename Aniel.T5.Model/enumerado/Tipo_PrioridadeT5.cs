using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_PrioridadeT5
    {
        [Display(Name = "ALTA", ResourceType = typeof(Resources.RsCadastros))]
        Alta = 0,
        [Display(Name = "MEDIA", ResourceType = typeof(Resources.RsCadastros))]
        Media = 1,
        [Display(Name = "BAIXA", ResourceType = typeof(Resources.RsCadastros))]
        Baixa = 2,
        [Display(Name = "NAOENCONTRADO", ResourceType = typeof(Resources.RsCadastros))]
        NaoEncontrado = 3

    }
}
