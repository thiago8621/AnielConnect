using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum Dashboard_Gestao_Atendimento
    {
        [Display(Name = "EVOLUCAO_TMA", ResourceType = typeof(Resources.RsCadastros))]
        Evolucao_TMA = 0,
        [Display(Name = "NIVEL_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        Nivel_Servico = 1,
        [Display(Name = "TAXA_ABANDONO", ResourceType = typeof(Resources.RsCadastros))]
        Taxa_Abandono = 2,
        [Display(Name = "EVOLUCAO_TAXA_ABANDONO", ResourceType = typeof(Resources.RsCadastros))]
        Evolucao_Taxa_Abandono = 3,
        
    }
}