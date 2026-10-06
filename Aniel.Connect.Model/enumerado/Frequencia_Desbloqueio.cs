using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Frequencia_Desbloqueio
    {
        [Display(Name = "QUINZENAL", ResourceType = typeof(Resources.RsCadastros))]
        QUINZENAL = 15,
        [Display(Name = "MENSAL", ResourceType = typeof(Resources.RsCadastros))]
        Mensal = 30,
        [Display(Name = "BIMESTRAL", ResourceType = typeof(Resources.RsCadastros))]
        Bimestral = 60,
        [Display(Name = "TRIMESTRAL", ResourceType = typeof(Resources.RsCadastros))]
        Trimestral = 90,
        [Display(Name = "SEMESTRAL", ResourceType = typeof(Resources.RsCadastros))]
        Semestral = 180,
        [Display(Name = "ANUAL", ResourceType = typeof(Resources.RsCadastros))]
        Anual = 360        
    }
}
