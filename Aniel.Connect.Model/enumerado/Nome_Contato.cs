using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum Nome_Contato
    {
        [Display(Name = "NOME_WHATSAPP", ResourceType = typeof(Resources.RsCadastros))]
        Nome_Whatsapp = 0,
        [Display(Name = "NUMERO_WHATSAPP", ResourceType = typeof(Resources.RsCadastros))]
        Numero_Whatsapp = 1,
        [Display(Name = "APELIDO", ResourceType = typeof(Resources.RsCadastros))]
        Apelido = 2
    }
}
