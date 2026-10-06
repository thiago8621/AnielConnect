using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

        public enum Tipo_Faturamento
        {
            [Display(Name = "INTEGRAL", ResourceType = typeof(Resources.RsFinanceiro))]
            Integral = 0,

            [Display(Name = "PARCIAL", ResourceType = typeof(Resources.RsFinanceiro))]
            Parcial = 1,

            [Display(Name = "AVULSO", ResourceType = typeof(Resources.RsFinanceiro))]
            Avulso = 2
        }
    
}