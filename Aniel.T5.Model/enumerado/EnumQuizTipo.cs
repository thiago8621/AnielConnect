using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum EnumQuizTipo
    {
        [Display(Name = "TEXTO", ResourceType = typeof(Resources.RsCadastros))]
        Texto = 0,
        [Display(Name = "VALOR", ResourceType = typeof(Resources.RsCadastros))]
        Valor = 1,
        [Display(Name = "DATA", ResourceType = typeof(Resources.RsCadastros))]
        Data = 2,
        [Display(Name = "FOTO", ResourceType = typeof(Resources.RsCadastros))]
        Foto = 3,
        [Display(Name = "UNICA_ESCOLHA", ResourceType = typeof(Resources.RsCadastros))]
        UnicaEscolha = 4,
        [Display(Name = "MULTIPLA_ESCOLHA", ResourceType = typeof(Resources.RsCadastros))]
        MultiplaEscolha = 5,
        [Display(Name = "ASSINATURA", ResourceType = typeof(Resources.RsCadastros))]
        Assinatura = 6,
        [Display(Name = "CODIGO_BARRA", ResourceType = typeof(Resources.RsCadastros))]
        CodigoBarra = 7,
        [Display(Name = "ASSINATURA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        AssinaturaCliente = 8
    }
}
