using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Momento_Cobra_Instalacao
    {
        [Display(Name = "FINALIZAR_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        Finalizar_Venda = 1,
        [Display(Name = "FECHAMENTO_FATURAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Fechamento_Faturamento = 2,
        [Display(Name = "HABILITACAO_PLANO_PRODUTO", ResourceType = typeof(Resources.RsCadastros))]
        Habilitacao_Plano_Produto = 3
    }
}
