using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum PesquisaSatisfacaoCategoriaCaixaTexto
    {
        [Display(Name = "SUGESTAO", ResourceType = typeof(Resources.RsCadastros))]
        Sugestao = 0,
        [Display(Name = "ELOGIO", ResourceType = typeof(Resources.RsCadastros))]
        Elogio = 1,
        [Display(Name = "CRITICA", ResourceType = typeof(Resources.RsCadastros))]
        Critica = 2
    }
}
