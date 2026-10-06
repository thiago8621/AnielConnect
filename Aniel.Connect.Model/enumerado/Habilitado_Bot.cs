using System.ComponentModel.DataAnnotations;

namespace Sinapse.Model.enumerados
{
    public enum Habilitado_Bot
    {
        [Display(Name = "NAO", ResourceType = typeof(Resources.RsCadastros))]
        Inativo = 0,
        [Display(Name = "SIM", ResourceType = typeof(Resources.RsCadastros))]
        Ativo = 1
    }
}
