using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Localidade_Anatel
    {
        [Display(Name = "ALDEIA_INDIGENA", ResourceType = typeof(Resources.RsCadastros))]
        Aldeia_indigena = 0,
        [Display(Name = "ARQUIPELAGO", ResourceType = typeof(Resources.RsCadastros))]
        arquipelago = 1,
        [Display(Name = "SEDE_MUNICIPIO", ResourceType = typeof(Resources.RsCadastros))]
        Sede_Municipio = 2,
        [Display(Name = "PROJETO_ASSENTAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Projeto_Assentamento = 3,
        [Display(Name = "REGIAO_REMOTA", ResourceType = typeof(Resources.RsCadastros))]
        Regiao_Remota = 4,
        [Display(Name = "LOCALIDADE_RURAL", ResourceType = typeof(Resources.RsCadastros))]
        Localidade_Rural = 5,
        [Display(Name = "LOCALIDADE_UBARNA", ResourceType = typeof(Resources.RsCadastros))]
        Localidade_Ubarna = 6,

    }
}
