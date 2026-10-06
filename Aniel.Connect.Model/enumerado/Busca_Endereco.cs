using System.ComponentModel.DataAnnotations;


namespace Aniel.Connect.Model.enumerado
{
    public enum Busca_Endereco
    {
        [Display(Name = "ENDERECO_LOGRADOURO", ResourceType = typeof(Resources.RsCadastros))]
        Endereco_Logradouro = 1,
        [Display(Name = "CONDOMINIO_PREDIO", ResourceType = typeof(Resources.RsCadastros))]
        Condominio_Predio = 2,
    }

 
}

