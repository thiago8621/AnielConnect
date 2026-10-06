using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum ConsultasRestricaoVenda
    {
        [Display(Name = "CONSULTA_VIABILIDADE", ResourceType = typeof(Resources.RsCadastros))]
        ConsultaViabilidade = 1,
        [Display(Name = "CONSULTA_PENDENCIA_FINANCEIRA", ResourceType = typeof(Resources.RsCadastros))]
        ConsultaPendenciaFinanceira = 2,
        [Display(Name = "CONSULTA_PENDENCIA_ENDERECO", ResourceType = typeof(Resources.RsCadastros))]
        ConsultaPendenciaEndereco = 3,
        [Display(Name = "CONSULTA_CREDITO", ResourceType = typeof(Resources.RsCadastros))]
        ConsultaCredito = 4
    }
}
