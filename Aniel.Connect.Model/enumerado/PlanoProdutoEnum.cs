using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum PlanoProdutoEnum
    {
        [Display(Name = "TXT_PLANO", ResourceType = typeof(Resources.RsCadastros))]
        Plano = 1,
        [Display(Name = "PRODUTO", ResourceType = typeof(Resources.RsCadastros))]
        Produto = 2,
    }
}
