using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Periodicidade_ExecucaoEnum
    {
        Hora,
        [Display(Name = "DIARIAMENTE", ResourceType = typeof(Resources.RsCadastros))]
        Dia,
        [Display(Name = "SEMANALMENTE", ResourceType = typeof(Resources.RsCadastros))]
        Semana,
        [Display(Name = "MENSALMENTE", ResourceType = typeof(Resources.RsCadastros))]
        Mes
    }
}
