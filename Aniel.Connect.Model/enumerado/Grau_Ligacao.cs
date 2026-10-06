using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Grau_Ligacao
    {
        [Display(Name = "FILHO", ResourceType = typeof(Resources.RsCadastros))]
        Filho = 0,
        [Display(Name = "PAI", ResourceType = typeof(Resources.RsCadastros))]
        Pai = 1,
        [Display(Name = "PARENTE", ResourceType = typeof(Resources.RsCadastros))]
        Parente = 2,
        [Display(Name = "VIZINHO", ResourceType = typeof(Resources.RsCadastros))]
        Vizinho = 3,
        [Display(Name = "AMIGO", ResourceType = typeof(Resources.RsCadastros))]
        Amigo = 4
            ,
        [Display(Name = "OUTRO", ResourceType = typeof(Resources.RsCadastros))]
        Outro = 5
            ,
    }
}
