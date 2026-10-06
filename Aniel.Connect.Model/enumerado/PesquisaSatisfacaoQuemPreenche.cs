using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum PesquisaSatisfacaoQuemPreenche
    {
        [Display(Name = "CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Cliente = 0,
        [Display(Name = "ATENDENTE", ResourceType = typeof(Resources.RsCadastros))]
        Atendente = 1
    }
}
