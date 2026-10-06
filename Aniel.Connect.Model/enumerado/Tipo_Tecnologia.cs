using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Tecnologia
    {
        [Display(Name = "TELEFONIA_MOVEL", ResourceType = typeof(Resources.RsCadastros))]
        Telefonia_Movel = 1,
        [Display(Name = "INTERNET", ResourceType = typeof(Resources.RsCadastros))]
        Internet = 2,
        [Display(Name = "TV", ResourceType = typeof(Resources.RsCadastros))]
        TV = 3,
        [Display(Name = "TELEFONIA_FIXO", ResourceType = typeof(Resources.RsCadastros))]
        Telefonia_Fixo = 4,
        [Display(Name = "SERVICOS_ADICIONAIS", ResourceType = typeof(Resources.RsCadastros))]
        Servicos_Adicionais = 5


    }
}
