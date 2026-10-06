using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum Dashboard_Gestao_Comercial
    {
        [Display(Name = "MEDIA_VENDAS", ResourceType = typeof(Resources.RsCadastros))]
        Media_Vendas = 0,
        [Display(Name = "PERCENTUAL_VENDAS", ResourceType = typeof(Resources.RsCadastros))]
        Percentual_Vendas = 1,
        [Display(Name = "COMISSAO_VENDAS", ResourceType = typeof(Resources.RsCadastros))]
        Comissao_Vendas = 2,
        [Display(Name = "TOTAL_VENDAS_PRODUTO", ResourceType = typeof(Resources.RsCadastros))]
        Total_Vendas_Produto_Servico = 3,
        
    }
}