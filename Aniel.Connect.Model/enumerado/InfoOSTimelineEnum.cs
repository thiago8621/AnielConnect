using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum InfoOSTimelineEnum
    {   
            [Display(Name = "SEGMENTO", ResourceType = typeof(Resources.RsCadastros))]
            SEGMENTO = 0,
            [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
            CATEGORIA_CLIENTE = 1,
            [Display(Name = "TIPO_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
            TIPO_SERVICO = 2,
            [Display(Name = "NATUREZA_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
            NATUREZA_SERVICO = 3
        
    }

}
