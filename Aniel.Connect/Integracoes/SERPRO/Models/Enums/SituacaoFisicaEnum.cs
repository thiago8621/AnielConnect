using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Integracoes.SERPRO.Models.Enums
{
    public enum SituacaoFisicaEnum : int
    {
        [Display(Name = "REGULAR")]
        Regular = 0,
        [Display(Name = "SUSPENSA")]
        Suspensa = 2,
        [Display(Name = "TITULAR FALECIDO")]
        TitularFalecido = 3,
        [Display(Name = "PENDENTE REGULARIZAÇÃO")]
        PendenteRegularizacao = 4,
        [Display(Name = "CANCELADA POR MULTIPLICIDADE")]
        CanceladaMultiplicidade = 5,
        [Display(Name = "NULA")]
        Nula = 8,
        [Display(Name = "CANCELADA OFICIO")]
        CanceladaOficio = 9,
        [Display(Name = "NÃO IDENTIFICADO")]
        NaoIdentificado = 999
    }
}