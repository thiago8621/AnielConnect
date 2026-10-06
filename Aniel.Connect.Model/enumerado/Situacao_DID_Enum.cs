using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Aniel.Connect.Model.enumerado
{
    public enum Situacao_DID
    {

        [Display(Name = "DISPONIVEL", ResourceType = typeof(Resources.RsCadastros))]
        Disponivel = 0,

        [Display(Name = "INDISPONIVEL", ResourceType = typeof(Resources.RsCadastros))]
        Indisponivel = 1,

        [Display(Name = "AGUARDANDO_PORTABILIDADE", ResourceType = typeof(Resources.RsCadastros))]
        Aguardando_Portabilidade = 2,

        [Display(Name = "RESERVADO", ResourceType = typeof(Resources.RsCadastros))]
        Resevado = 3,

        [Display(Name = "VENDA_APROVADA", ResourceType = typeof(Resources.RsCadastros))]
        Vendido = 4,

        [Display(Name = "QUARENTENA", ResourceType = typeof(Resources.RsCadastros))]
        Quarentena = 5
    }
}
