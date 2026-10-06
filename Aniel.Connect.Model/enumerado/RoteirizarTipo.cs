using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum RoteirizarTipo
    {
        [Display(Name = "ROTEIRIZAR_LATLONG", ResourceType = typeof(Resources.RsCadastros))]
        ROTEIRIZAR_LATLONG = 1,
        [Display(Name = "ROTEIRIZAR_CAIXA", ResourceType = typeof(Resources.RsCadastros))]
        ROTEIRIZAR_CAIXA = 2,
        [Display(Name = "ROTEIRIZAR_GEOLOCALIZACAO", ResourceType = typeof(Resources.RsCadastros))]
        ROTEIRIZAR_GEOLOCALIZACAO = 3

    }
}
