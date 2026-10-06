using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum TipoCampoGravar : int
    {
        [Display(Name = "TEXTO", ResourceType = typeof(Resources.RsBase))]
        Texto = 0,
        [Display(Name = "INTEIRO", ResourceType = typeof(Resources.RsBase))]
        Inteiro,
        [Display(Name = "NUMERICO", ResourceType = typeof(Resources.RsBase))]
        Numerico,
        [Display(Name = "UNICAESCOLHA", ResourceType = typeof(Resources.RsBase))]
        UnicaEscolha
    }
}
