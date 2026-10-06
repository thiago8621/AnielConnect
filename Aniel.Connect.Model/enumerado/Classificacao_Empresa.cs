using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Classificacao_Organizacao
    {
        [Display(Name = "GRANDE", ResourceType = typeof(Resources.RsCadastros))]
        Grande = 1,
        [Display(Name = "MEDIA", ResourceType = typeof(Resources.RsCadastros))]
        Media = 2,
        [Display(Name = "PEQUENA", ResourceType = typeof(Resources.RsCadastros))]
        Pequena = 3
    }
}
