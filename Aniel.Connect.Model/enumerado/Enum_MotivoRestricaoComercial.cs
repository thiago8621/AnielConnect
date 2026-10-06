using System.ComponentModel.DataAnnotations;


namespace Aniel.Connect.Model.enumerado
{
    public enum Enum_MotivoRestricaoComercial
    {
        [Display(Name = "RESTRICAO_CREDITO", ResourceType = typeof(Resources.RsCadastros))]
        RESTRICAO_CREDITO = 0,   
        [Display(Name = "PENDENCIA_ENDERECO", ResourceType = typeof(Resources.RsCadastros))]
        PENDENCIA_ENDERECO = 1,
        [Display(Name = "PENDENCIA_FINANCEIRA", ResourceType = typeof(Resources.RsCadastros))]
        PENDENCIA_FINANCEIRA = 2,
        [Display(Name = "OUTROS", ResourceType = typeof(Resources.RsCadastros))]
        OUTROS = 3
    }
}

     
