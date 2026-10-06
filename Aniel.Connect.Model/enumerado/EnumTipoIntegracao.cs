using System.ComponentModel.DataAnnotations;


namespace Aniel.Connect.Model.enumerado
{
    public enum EnumTipoIntegracao
    {
        [Display(Name = "CONSULTA_CREDITO", ResourceType = typeof(Resources.RsCadastros))]
        CONSULTA_CREDITO = 0,
        [Display(Name = "CONSULTA_PENDENCIA_ENDERECO", ResourceType = typeof(Resources.RsCadastros))]
        CONSULTA_PENDENCIA_ENDERECO = 1, 
        [Display(Name = "CONSULTA_PENDENCIA_FINANCEIRA", ResourceType = typeof(Resources.RsCadastros))]
        CONSULTA_PENDENCIA_FINANCEIRA = 2,
        [Display(Name = "COMERCIAL", ResourceType = typeof(Resources.RsCadastros))]
        COMERCIAL = 4,
        [Display(Name = "OUTROS", ResourceType = typeof(Resources.RsCadastros))]
        OUTROS = 3,
        [Display(Name = "PROVISIONAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        PROVISIONAMENTO = 5,
        [Display(Name = "COMERCIAL_AGRUPADOR", ResourceType = typeof(Resources.RsCadastros))]
        COMERCIAL_AGRUPADOR = 6,
        [Display(Name = "EXCLUSIVIDADE_PLANO", ResourceType = typeof(Resources.RsCadastros))]
        EXCLUSIVIDADE_PLANO = 7,
    }
}