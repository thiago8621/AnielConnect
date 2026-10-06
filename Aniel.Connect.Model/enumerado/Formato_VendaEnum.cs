using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Aniel.Connect.Model.enumerado
{
    public enum Formato_Venda
    {
        [Display(Name = "AVULSO", ResourceType = typeof(Resources.RsCadastros))]
        Avulso = 0,
        [Display(Name = "COMBO", ResourceType = typeof(Resources.RsCadastros))]
        Combo = 1
    }

}
