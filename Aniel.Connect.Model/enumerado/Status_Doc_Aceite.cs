using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Status_Doc_Aceite
    {
        [Display(Name = "PENDENTE_ACEITE", ResourceType = typeof(Resources.RsCadastros))]
        Pendente_Aceite = 0,
        [Display(Name = "ACEITE_EFETUADO", ResourceType = typeof(Resources.RsCadastros))]
        Aceite_Efetuado = 1,        
        [Display(Name = "APROVADO", ResourceType = typeof(Resources.RsCadastros))]
        Aprovado = 2,
        [Display(Name = "REPROVADO", ResourceType = typeof(Resources.RsCadastros))]
        Reprovado = 3,
    }
}
