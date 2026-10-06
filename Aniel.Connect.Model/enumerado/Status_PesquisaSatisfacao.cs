using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Aniel.Connect.Model.enumerado
{
    public enum Status_PesquisaSatisfacao
    {

        [Display(Name = "TODAS", ResourceType = typeof(Resources.RsCadastros))]
        Todas = 2,

        [Display(Name = "ATIVAS", ResourceType = typeof(Resources.RsCadastros))]
        Ativas = 1,

        [Display(Name = "INATIVAS", ResourceType = typeof(Resources.RsCadastros))]
        Inativas = 0,

      


    }

 
}

