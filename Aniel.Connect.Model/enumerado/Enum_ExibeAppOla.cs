using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum EnumExibeAppOla
    {
        [Display(Name = "SIM", ResourceType = typeof(Resources.RsCadastros))]
        Não =  0,
        [Display(Name = "NAO", ResourceType = typeof(Resources.RsCadastros))]
        Sim = 1
      
    }
}
