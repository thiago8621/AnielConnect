using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Produto_Anatel
    {
        [Display(Name = "INTERNET", ResourceType = typeof(Resources.RsCadastros))]
        Internet = 0,
        [Display(Name = "LINHA_DEDICADA", ResourceType = typeof(Resources.RsCadastros))]
        Linha_Dedicada = 1,
        [Display(Name = "M2M", ResourceType = typeof(Resources.RsCadastros))]
        M2m = 2,            
        [Display(Name = "OUTROS", ResourceType = typeof(Resources.RsCadastros))]
        Outros = 3,
    }
}
