using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum ModeloPainel_Data
    {
        [Display(Name = "DIA_ATUAL", ResourceType = typeof(Resources.RsCadastros))]
        DIA_ATUAL = 0,
        [Display(Name = "SEMANA_ATUAL", ResourceType = typeof(Resources.RsCadastros))]
        SEMANA_ATUAL = 1,
        [Display(Name = "ULTIMOS_7_DIAS", ResourceType = typeof(Resources.RsCadastros))]
        ULTIMOS_7_DIAS = 2,
        [Display(Name = "MES_ATUAL", ResourceType = typeof(Resources.RsCadastros))]
        MES_ATUAL = 3,
        [Display(Name = "ULTIMOS_30_DIAS", ResourceType = typeof(Resources.RsCadastros))]
        ULTIMOS_30_DIAS = 4,
        [Display(Name = "ULTIMOS_3_MESES", ResourceType = typeof(Resources.RsCadastros))]
        ULTIMOS_3_MESES = 5
    }
}