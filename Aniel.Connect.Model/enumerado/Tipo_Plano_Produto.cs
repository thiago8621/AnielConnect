using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Plano_Produto
    {
        [Display(Name = "TXT_PLANO", ResourceType = typeof(Resources.RsCadastros))]
        Plano = 0,
        [Display(Name = "PRODUTO", ResourceType = typeof(Resources.RsCadastros))]
        Produto = 1,
    }
}
