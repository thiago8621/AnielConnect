using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Bot_Evento_Anexo
    {
        [Display(Name = "IMAGEM", ResourceType = typeof(Resources.RsCadastros))]
        Imagem = 0,

        [Display(Name = "VIDEO", ResourceType = typeof(Resources.RsCadastros))]
        Video = 1,

        [Display(Name = "AUDIO", ResourceType = typeof(Resources.RsCadastros))]
        Audio = 2,

        [Display(Name = "ARQUIVO", ResourceType = typeof(Resources.RsCadastros))]
        Arquivo = 3,

        [Display(Name = "URL_BOT", ResourceType = typeof(Resources.RsCadastros))]
        Url = 4,

    }
}
