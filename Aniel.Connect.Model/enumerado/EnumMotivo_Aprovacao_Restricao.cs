
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum EnumMotivo_Aprovacao_Restricao
    {
        [Display(Name = "VIABILIDADE", ResourceType = typeof(Resources.RsCadastros))]
        Viabilidade = 0,
        [Display(Name = "PEDENCIA_FINANCEIRA", ResourceType = typeof(Resources.RsCadastros))]
        Pendencia_Financeira = 1,
        [Display(Name = "PEDENCIA_ENDERECO", ResourceType = typeof(Resources.RsCadastros))]
        Pendencia_Endereco = 2,
        [Display(Name = "CREDITO", ResourceType = typeof(Resources.RsCadastros))]
        Credito = 3,


    }
}
