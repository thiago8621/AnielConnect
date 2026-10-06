using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Desconto_Captacao
    {
        [Display(Name = "CONTRATANTE", ResourceType = typeof(Resources.RsCadastros))]
        Contratante = 0,

        [Display(Name = "QUEMINDICOU", ResourceType = typeof(Resources.RsCadastros))]
        QuemIndicou = 1,

        [Display(Name = "AMBOS", ResourceType = typeof(Resources.RsCadastros))]
        Ambos = 2,
    }
}
