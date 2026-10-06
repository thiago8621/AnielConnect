using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum Contagem_Prazo
    {
        [Display(Name = "HORAS_UTEIS", ResourceType = typeof(Resources.RsCadastros))]
        Horas_Uteis = 0,
        [Display(Name = "HORAS_CORRIDAS", ResourceType = typeof(Resources.RsCadastros))]
        Horas_Corridas = 1,
                
    }
}