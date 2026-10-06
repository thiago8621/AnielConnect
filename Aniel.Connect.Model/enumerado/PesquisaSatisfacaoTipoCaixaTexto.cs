using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum PesquisaSatisfacaoTipoCaixaTexto
    {
        [Display(Name = "RESPOSTA_CURTA", ResourceType = typeof(Resources.RsCadastros))]
        Resposta_Curta = 0,
        [Display(Name = "RESPOSTA_LONGA", ResourceType = typeof(Resources.RsCadastros))]
        Resposta_Longa = 1
    }
}
