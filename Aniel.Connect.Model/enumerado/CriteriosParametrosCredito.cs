using System.ComponentModel.DataAnnotations;


namespace Aniel.Connect.Model.enumerado
{
    public enum CriteriosParametrosCredito
    {
        [Display(Name = "SCORE_POSITIVO", ResourceType = typeof(Resources.RsCadastros))]
        SCORE_POSITIVO = 1,
        [Display(Name = "ULTIMA_DATA_PENDENCIA", ResourceType = typeof(Resources.RsCadastros))]
        ULTIMA_DATA_PENDENCIA = 2,
        [Display(Name = "VALOR_TOTAL_DEBITO", ResourceType = typeof(Resources.RsCadastros))]
        VALOR_TOTAL_DEBITO = 3,
        [Display(Name = "PROTESTO", ResourceType = typeof(Resources.RsCadastros))]
        PROTESTO = 4,
    }
}

     
