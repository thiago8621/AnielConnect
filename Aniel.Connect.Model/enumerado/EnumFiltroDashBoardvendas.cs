using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum EnumPeriodoFitlroDashVendas
    {
        [Display(Name = "DATA_ATUAL", ResourceType = typeof(Resources.RsCadastros))]
        DataAtual = 0,
        [Display(Name = "ESTA_SEMANA", ResourceType = typeof(Resources.RsCadastros))]
        EstaSemana = 1,
        [Display(Name = "ULTIMOS_7_DIAS", ResourceType = typeof(Resources.RsCadastros))]
        Ultimos7Dias = 2,
        [Display(Name = "ESTE_MES", ResourceType = typeof(Resources.RsCadastros))]
        EsteMes = 3,
        [Display(Name = "ULTIMOS_30_DIAS", ResourceType = typeof(Resources.RsCadastros))]
        Ultimos30Dias = 4,
    }
}
