using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_EnderecoCliente
    {
        [Display(Name = "INSTALACAO", ResourceType = typeof(Resources.RsCadastros))]
        Instalacao = 1,
        [Display(Name = "COBRANCA", ResourceType = typeof(Resources.RsCadastros))]
        Cobranca = 2,
        [Display(Name = "PRINCIPAL", ResourceType = typeof(Resources.RsCadastros))]
        Principal = 3,
        [Display(Name = "PROSPECTO", ResourceType = typeof(Resources.RsCadastros))]
        Prospecto = 4
    }
}
