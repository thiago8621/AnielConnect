using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.enumerado
{
    public enum ClassificacaoMaterial
    {
        [Display(Name = "INSTALACAO", ResourceType = typeof(Resources.RsCadastros))] 
        Instalacao = 1,
        [Display(Name = "DEFEITO", ResourceType = typeof(Resources.RsCadastros))]
        Defeito = 2,
        [Display(Name = "DEF_ESTOQUE", ResourceType = typeof(Resources.RsCadastros))]
        Def_Estoque = 3,
        [Display(Name = "RETIRADA", ResourceType = typeof(Resources.RsCadastros))]
        Retirada = 4,
        [Display(Name = "REAPROVEITADO", ResourceType = typeof(Resources.RsCadastros))]
        Reaproveitado = 5,
        [Display(Name = "SUCATA", ResourceType = typeof(Resources.RsCadastros))]
        Sucata = 6
    }

    
}
