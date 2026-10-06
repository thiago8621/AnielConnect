using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum TipoChecklist
    {
        [Display(Name = "SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        Servico = 0,
        [Display(Name = "COMERCIAL", ResourceType = typeof(Resources.RsCadastros))]
        Comercial = 1
    }
}
