using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum CampanhaUtilizadaPor
    {
        [Display(Name = "VENDEDOR", ResourceType = typeof(Resources.RsCadastros))]
        Vendedor = 0,
        [Display(Name = "API", ResourceType = typeof(Resources.RsCadastros))]
        Api = 1,
        [Display(Name = "AMBOS", ResourceType = typeof(Resources.RsCadastros))] 
        Ambos = 2,
    }
}
