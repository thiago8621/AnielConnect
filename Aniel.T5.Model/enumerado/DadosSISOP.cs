using System.ComponentModel.DataAnnotations;

namespace Aniel.T5.Model.enumerado
{
    public enum DadosSISOP
    {
        [Display(Name = "DATA", ResourceType = typeof(Resources.RsCadastros))]
        Data = 1,
        [Display(Name = "FOTO_FACHADA", ResourceType = typeof(Resources.RsCadastros))]
        Foto_Fachada = 2,
        [Display(Name = "VELOCIDADE", ResourceType = typeof(Resources.RsCadastros))]
        Velocidade = 3,
        [Display(Name = "TECNOLOGIA", ResourceType = typeof(Resources.RsCadastros))]
        Tecnologia = 4,
        [Display(Name = "QUANTIDADE_EQUIPAMENTOS", ResourceType = typeof(Resources.RsCadastros))]
        Quantide_Equipamentos = 5,
        [Display(Name = "LATITUDE", ResourceType = typeof(Resources.RsCadastros))]
        Latitude = 6,
        [Display(Name = "LONGITUDE", ResourceType = typeof(Resources.RsCadastros))]
        Longitude = 7       

    }
}
