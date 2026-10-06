using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Identificacao_Cliente
    {

        [Display(Name = "SEQUENCIAL", ResourceType = typeof(Resources.RsCadastros))]
        Sequencial = 0,
        [Display(Name = "INFORMADO_CADASTRO_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Numero_Contrato = 1,


    }
}
