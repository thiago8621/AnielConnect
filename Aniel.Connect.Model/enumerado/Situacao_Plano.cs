using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Situacao_plano
    {
        [Display(Name = "INATIVO", ResourceType = typeof(Resources.RsCadastros))]
        Inativo = 0,
        [Display(Name = "ATIVO", ResourceType = typeof(Resources.RsCadastros))]
        Ativo = 1
    }
}
