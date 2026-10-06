using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.enumerado
{
    public enum Status_CobrancaAniel
    {
        [Display(Name = "AUTOMATICA", ResourceType = typeof(Resources.RsCadastros))]
        Automatica = 0,
        [Display(Name = "AGUARDANDO_AVALIAVACAO_COBRANCA", ResourceType = typeof(Resources.RsCadastros))]
        Aguardando_Avaliacao_Cobranca = 1,
        [Display(Name ="APROVADO",ResourceType =typeof(Resources.RsCadastros))]
        Aprovado= 2,
        [Display(Name = "REPROVADO", ResourceType = typeof(Resources.RsCadastros))]
        Reprovado = 3,
    }
}
