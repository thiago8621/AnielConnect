
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum EnumMotivo_Desistencia_Venda
    {
        [Display(Name = "APROVACAO", ResourceType = typeof(Resources.RsCadastros))]
        Aprovacao = 0,
        [Display(Name = "DESISTENCIA", ResourceType = typeof(Resources.RsCadastros))]
        Desistencia = 1

    }
}
