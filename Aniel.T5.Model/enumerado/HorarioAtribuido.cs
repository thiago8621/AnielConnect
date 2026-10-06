using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum HorarioAtribuido
    {
        [Display(Name = "ANIEL_CONNECT", ResourceType = typeof(Resources.RsCadastros))]
        AnielConnect = 0,
        [Display(Name = "ANIEL_POINT", ResourceType = typeof(Resources.RsCadastros))]
        AnielPoint = 1
    
    }
}
