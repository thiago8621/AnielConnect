using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum Comparativo_QuantidadeEnum
    {        
        [Display(Name = "MAIOR_QUE", ResourceType = typeof(Resources.RsBase))]
        Maior_Que = 0,
        [Display(Name = "MENOR_QUE", ResourceType = typeof(Resources.RsBase))]
        Menor_Que = 1, 
        [Display(Name = "MAIOR_QUE_IGUAL", ResourceType = typeof(Resources.RsBase))]
        Maior_Que_ou_Igual = 2,
        [Display(Name = "MENOR_QUE_IGUAL", ResourceType = typeof(Resources.RsBase))]
        Menor_Que_ou_Igual = 3,
        [Display(Name = "IGUAL", ResourceType = typeof(Resources.RsBase))]
        Igual = 4,
        [Display(Name = "ENTRE", ResourceType = typeof(Resources.RsBase))]
        Entre = 5,
        [Display(Name = "TODOS", ResourceType = typeof(Resources.RsBase))]
        Todos = 6
    }
}
