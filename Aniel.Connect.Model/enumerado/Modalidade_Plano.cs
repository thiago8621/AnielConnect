using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Modalidade_Plano
    {
        [Display(Name = "AICE", ResourceType = typeof(Resources.RsCadastros))]
        AICE = 0,
        [Display(Name = "PLANO_BASICO", ResourceType = typeof(Resources.RsCadastros))]
        Plano_Basico = 1,
        [Display(Name = "PLANO_ALTERNATIVO", ResourceType = typeof(Resources.RsCadastros))]
        Plano_Alternativo = 2,
        [Display(Name = "PLANO_ALTERNATIVO_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        Plano_Alternativo_Servico = 3,
    }
}
