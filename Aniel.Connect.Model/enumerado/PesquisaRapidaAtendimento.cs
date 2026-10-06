using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum PesquisaRapidaAtendimento
    {
        [Display(Name = "PROTOCOLO", ResourceType = typeof(Resources.RsCadastros))]
        Protocolo = 1,
        [Display(Name = "CATEGORIA_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Categoria_Atendimento = 2,
        [Display(Name = "TIPO_ATEDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Tipo_Atendimento = 3, 
        [Display(Name = "CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Cliente = 4,
        [Display(Name = "ORGANIZACAO", ResourceType = typeof(Resources.RsCadastros))]
        Organizacao = 5,
        [Display(Name = "RESPONSAVEL", ResourceType = typeof(Resources.RsCadastros))]
        Responsavel = 6,
        [Display(Name = "SETOR", ResourceType = typeof(Resources.RsCadastros))]
        Setor = 7,
        [Display(Name = "ASSUNTO", ResourceType = typeof(Resources.RsCadastros))]
        Assunto = 8
    }
}
