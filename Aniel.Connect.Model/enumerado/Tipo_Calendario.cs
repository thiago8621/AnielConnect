using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum Tipo_Calendario
    {
        [Display(Name = "SLA_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        SLA_Atendimento = 1,
        [Display(Name = "SLA_EXECUCAO", ResourceType = typeof(Resources.RsCadastros))]
        SLA_Execucao = 2
    }
}