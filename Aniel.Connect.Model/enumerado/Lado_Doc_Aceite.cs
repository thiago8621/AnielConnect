using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Lado_Doc_Aceite
    {
        [Display(Name = "FRENTE", ResourceType = typeof(Resources.RsCadastros))]
        Frente = 0,
        [Display(Name = "VERSO", ResourceType = typeof(Resources.RsCadastros))]
        Verso = 1,
        [Display(Name = "UNICO", ResourceType = typeof(Resources.RsCadastros))]
        Unico = 2,
    }
}
