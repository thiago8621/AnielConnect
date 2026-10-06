using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Status_Motivo
    {
        [Display(Name = "AGUARDANDO", ResourceType = typeof(Resources.RsCadastros))]
        Aguardando = 0,
        [Display(Name = "CANCELADO", ResourceType = typeof(Resources.RsCadastros))]
        Cancelado = 1,
        [Display(Name = "CONCLUIDO", ResourceType = typeof(Resources.RsCadastros))]
        Concluido = 2
    }
}
