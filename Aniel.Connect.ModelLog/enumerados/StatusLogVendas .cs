using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.ModelLog
{
    public enum StatusLogVendas
    {
        [Display(Name = "CONSULTA_CREDITO", ResourceType = typeof(Resources.RsCadastros))]
        CONSULTA_CREDITO = 0,
        [Display(Name = "NEGATIVAR ", ResourceType = typeof(Resources.RsCadastros))]
        NEGATIVAR = 1,
        [Display(Name = "RETIRAR_NEGATIVACAO", ResourceType = typeof(Resources.RsCadastros))]
        RETIRAR_NEGATIVACAO = 2,

    }
}
