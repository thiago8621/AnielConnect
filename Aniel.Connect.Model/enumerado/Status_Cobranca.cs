using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Status_Cobranca
    {
        [Display(Name = "AUTOMATICA", ResourceType = typeof(Resources.RsCadastros))]
        Automatica = 0,
        [Display(Name = "AGUARDANDO_AVALIAVACAO_COBRANCA", ResourceType = typeof(Resources.RsCadastros))]
        Aguardando_Avaliacao_Cobranca = 1,
        [Display(Name ="APROVADO",ResourceType =typeof(Resources.RsCadastros))]
        Aprovado=2,
        [Display(Name = "REPROVADO", ResourceType = typeof(Resources.RsCadastros))]
        Repovado = 3
    }
}
