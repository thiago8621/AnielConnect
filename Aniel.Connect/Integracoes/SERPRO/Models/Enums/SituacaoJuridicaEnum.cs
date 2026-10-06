using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Integracoes.SERPRO.Models.Enums
{
    public enum SituacaoJuridicaEnum : int
    {
        [Display(Name = "NULA")]
        Nula = 1,
        [Display(Name = "ATIVA")]
        Ativa = 2,
        [Display(Name = "SUSPENSA")]
        Suspensa = 3,
        [Display(Name = "INAPTA")]
        Inapta = 4,
        [Display(Name = "ATIVA NÃO REGULAR")]
        AtivaNaoRegular = 5,
        [Display(Name = "BAIXADA")]
        Baixada = 8,
        [Display(Name = "NÃO IDENTIFICADO")]
        NaoIdentificado = 999
    }
}