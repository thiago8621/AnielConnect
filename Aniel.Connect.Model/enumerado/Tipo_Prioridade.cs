using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Prioridade
    {
        [Display(Name = "ALTA", ResourceType = typeof(Resources.RsCadastros))]
        Alta = 0,
        [Display(Name = "MEDIA", ResourceType = typeof(Resources.RsCadastros))]
        Media = 1,
        [Display(Name = "BAIXA", ResourceType = typeof(Resources.RsCadastros))]
        Baixa = 2

    }
}
