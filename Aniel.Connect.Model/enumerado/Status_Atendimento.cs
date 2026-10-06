using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Status_Atendimento
    {
        [Display(Name = "A_INICIAR", ResourceType = typeof(Resources.RsCadastros))]
        A_Iniciar = 0,
        [Display(Name = "EM_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Em_Atendimento = 5,
        //[Display(Name = "EM_ANDAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        //Em_Andamento = 1,
        [Display(Name = "AGUARDANDO", ResourceType = typeof(Resources.RsCadastros))]
        Aguardando = 2,
        [Display(Name = "CANCELADO", ResourceType = typeof(Resources.RsCadastros))]
        Cancelado = 3,
        [Display(Name = "CONCLUIDO", ResourceType = typeof(Resources.RsCadastros))]
        Concluido = 4,
        

    }
}
