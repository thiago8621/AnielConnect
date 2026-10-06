using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum TipoApiSISOP
    {
        [Display(Name = "REDE_FISICA", ResourceType = typeof(Resources.RsCadastros))]
        Rede_Fisica = 1,
        [Display(Name = "REDE_LOGICA", ResourceType = typeof(Resources.RsCadastros))]
        Rede_Logica = 2,
        [Display(Name = "GERADOR_SOLAR", ResourceType = typeof(Resources.RsCadastros))]
        Gerador_Solar = 3,
        [Display(Name = "ESCOLA_CONECTADA", ResourceType = typeof(Resources.RsCadastros))]
        Escola_Conectada = 4
    }
}
