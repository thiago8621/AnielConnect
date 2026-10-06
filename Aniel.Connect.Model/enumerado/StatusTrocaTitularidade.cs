using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace Aniel.Connect.Model.enumerado
{
    public enum StatusTrocaTitularidade : int
    {      
        [Display(Name = "ANALISE_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Analise_Cliente = 1,
        [Display(Name = "CANCELADO", ResourceType = typeof(Resources.RsCadastros))]
        Cancelado = 2,
        [Display(Name = "APROVADO", ResourceType = typeof(Resources.RsCadastros))]
        Aprovado = 3,
        [Display(Name = "REPROVADO", ResourceType = typeof(Resources.RsCadastros))]
        Reprovado = 4,
        Todos = 99
    }
}
