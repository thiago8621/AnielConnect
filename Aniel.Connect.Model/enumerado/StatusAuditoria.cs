using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum StatusAuditoria
    {
        [Display(Name = "REPROVADO", ResourceType = typeof(Resources.RsCadastros))]
        Reprovado = 0,
        [Display(Name = "APROVADO", ResourceType = typeof(Resources.RsCadastros))]
        Aprovado = 1,
        [Display(Name = "AGUARDANDO_AVALIACAO", ResourceType = typeof(Resources.RsCadastros))]
        AguardandoAvaliacao = 2,

    }
}
