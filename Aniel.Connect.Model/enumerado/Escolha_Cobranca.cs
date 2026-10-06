using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Aniel.Connect.Model.enumerado
{
    public enum Escolha_Cobranca
    {
        [Display(Name = "COBRANCA_AUTOMATICA", ResourceType = typeof(Resources.RsCadastros))]
        Cobranca_Automatica = 0,
        [Display(Name = "AGUARDA_AVALIACAO", ResourceType = typeof(Resources.RsCadastros))]
        Aguarda_Avaliacao = 1
    }

   
}

