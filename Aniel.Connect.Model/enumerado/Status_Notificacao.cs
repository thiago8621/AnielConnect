using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Status_Notificacao
    {
        [Display(Name = "LIDO", ResourceType = typeof(Resources.RsCadastros))]
        Lido = 0,
        [Display(Name = "NÃO LIDO", ResourceType = typeof(Resources.RsCadastros))]
        NaoLido = 1,
    }
}
