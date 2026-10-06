using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Contato_Cliente
    {
        
        [Display(Name = "TELEFONE_FIXO", ResourceType = typeof(Resources.RsCadastros))]
        TelefoneFixo = 1,
        [Display(Name = "TELEFONE_CELULAR", ResourceType = typeof(Resources.RsCadastros))]
        TelefoneCelular = 2,
        Email = 3,
    }
}
