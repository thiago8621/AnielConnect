using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum EventoArmazenamento
    {

        /// <summary>
        /// STRING_VAZIA: Usado para enum removido ou desativado
        /// </summary>
        [Display(Name = "PLANO", ResourceType = typeof(Resources.RsCadastros))]
        PLANO = 0,

        [Display(Name = "BOLETO", ResourceType = typeof(Resources.RsCadastros))]
        BOLETO = 1,

        [Display(Name = "CPF_CNPJ", ResourceType = typeof(Resources.RsCadastros))]
        CPF_CNPJ = 2

    }
}
