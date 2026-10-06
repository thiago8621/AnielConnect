using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Doc_Termo_Anexo
    {
        [Display(Name = "RG", ResourceType = typeof(Resources.RsCadastros))]
        RG = 0,

        [Display(Name = "CNH", ResourceType = typeof(Resources.RsCadastros))]
        CNH = 1,

        [Display(Name = "SELFIE", ResourceType = typeof(Resources.RsCadastros))]
        SELFIE = 2,
    }
}
