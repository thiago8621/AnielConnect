using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.enumerado
{
    public enum Criterio_RoteirizacaoTipoAniel
    {

        [Display(Name = "AREA", ResourceType = typeof(Resources.RsCadastros))]
        Area = 0,

        [Display(Name = "DISTANCIA", ResourceType = typeof(Resources.RsCadastros))]
        Distancia = 1,

        [Display(Name = "HABILIDADES", ResourceType = typeof(Resources.RsCadastros))]
        Habilidades = 2,

        [Display(Name = "SLA_EXECUCAO", ResourceType = typeof(Resources.RsCadastros))]
        SLA_Execucao = 3,

    }
}
