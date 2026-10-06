using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Produto_AnatelSCM
    {
        [Display(Name = "VOZ", ResourceType = typeof(Resources.RsCadastros))]
        Voz = 0,
        [Display(Name = "DADOS", ResourceType = typeof(Resources.RsCadastros))]
        Dados = 1,
        [Display(Name = "VOZ_DADOS", ResourceType = typeof(Resources.RsCadastros))]
        Voz_Dadps = 2,
        [Display(Name = "PONTO_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        Ponto_Servico = 3,
        [Display(Name = "M2M", ResourceType = typeof(Resources.RsCadastros))]
        M2m = 4,
    }
}
