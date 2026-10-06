using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.enumerado
{
    public enum Padrao
    {
        [Display(Name = "NAO", ResourceType = typeof(Resources.RsCadastros))]
        Nao = 0,
        [Display(Name = "SIM", ResourceType = typeof(Resources.RsCadastros))]
        Sim = 1,
       
    }
}
