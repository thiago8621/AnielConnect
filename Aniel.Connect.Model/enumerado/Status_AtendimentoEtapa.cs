using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Status_AtendimentoEtapa
    {
        [Display(Name = "CANCELADO", ResourceType = typeof(Resources.RsCadastros))]
        Cancelado = 3,
        [Display(Name = "CONCLUIDO", ResourceType = typeof(Resources.RsCadastros))]
        Concluido = 4,
        

    }
}
