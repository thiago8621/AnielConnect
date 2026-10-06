using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Modo_Cobranca_Instalacao
    {
        [Display(Name = "NA_HABILITACAO", ResourceType = typeof(Resources.RsCadastros))]
        Na_Habilitacao = 0,
        [Display(Name = "FINALIZAR_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        Finalizar_Venda = 1,
        [Display(Name = "FECHAMENTO_FATURA", ResourceType = typeof(Resources.RsCadastros))]
        Fechamento_Fatura = 2
        //[Display(Name = "DEPOIS_PRIMEIRO_FATURAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        //Depois_Primeiro_Faturamento = 1     
    }
}
