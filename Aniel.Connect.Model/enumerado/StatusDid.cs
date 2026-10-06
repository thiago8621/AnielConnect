using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum StatusDid
    {
        [Display(Name = "DISPONIVEL", ResourceType = typeof(Resources.RsCadastros))]
        Disponivel = 1,
        [Display(Name = "INDISPONIVEL", ResourceType = typeof(Resources.RsCadastros))]
        Indisponivel = 2,
        [Display(Name = "AGUARDANDO_PORTABILIDADE", ResourceType = typeof(Resources.RsCadastros))]
        AguardandoPortabilidade = 3,
        [Display(Name = "RESERVADO", ResourceType = typeof(Resources.RsCadastros))]
        Reservado = 4,
        [Display(Name = "VENDIDO", ResourceType = typeof(Resources.RsCadastros))]
        Vendido = 5,
        [Display(Name = "QUARENTENA", ResourceType = typeof(Resources.RsCadastros))]
        Quarentena = 6
    }
}
