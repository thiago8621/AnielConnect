using System.ComponentModel.DataAnnotations;


namespace Aniel.Connect.Model.enumerado
{
    public enum EnumOperadores
    {     
        [Display(Name = "MaiorQue", ResourceType = typeof(Resources.RsCadastros))]
        MaiorQue = 0,
        [Display(Name = "MenorQue", ResourceType = typeof(Resources.RsCadastros))]
        MenorQue = 1,
        [Display(Name = "MaiorouIgual", ResourceType = typeof(Resources.RsCadastros))]
        MaiorouIgual = 2,
        [Display(Name = "MenorouIgual", ResourceType = typeof(Resources.RsCadastros))]
        MenorouIgual = 3,
        [Display(Name = "Igual", ResourceType = typeof(Resources.RsCadastros))]
        Igual = 4
    }
}

     
