using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum Prioridade_Atendimento
    {

        [Display(Name = "INDIFERENTE", ResourceType = typeof(Resources.RsBase))]
        Indiferente = 3,
        [Display(Name = "ALTO", ResourceType = typeof(Resources.RsBase))]
        Alto = 2,
        [Display(Name = "MEDIO", ResourceType = typeof(Resources.RsBase))]
        Medio = 1,
        [Display(Name = "BAIXO", ResourceType = typeof(Resources.RsBase))]
        Baixo = 0
    }
}
