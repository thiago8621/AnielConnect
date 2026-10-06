using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_Forma_Pagamento
    {
        [Display(Name = "DEBITO_CONTA", ResourceType = typeof(Resources.RsFinanceiro))]
        Debito_Conta = 0,
        [Display(Name = "BOLETO", ResourceType = typeof(Resources.RsFinanceiro))]
        Boleto = 1,
        [Display(Name = "CARTAO_DEBITO", ResourceType = typeof(Resources.RsFinanceiro))]
        Cartao_Debito = 2,
        [Display(Name = "DINHEIRO", ResourceType = typeof(Resources.RsFinanceiro))]
        Dinheiro = 3,
        [Display(Name = "CARNE", ResourceType = typeof(Resources.RsFinanceiro))]
        Carne = 4,
        [Display(Name = "CARTAO_CREDITO", ResourceType = typeof(Resources.RsFinanceiro))]
        Cartao_Credito = 5,
        [Display(Name = "CHEQUE", ResourceType = typeof(Resources.RsFinanceiro))]
        Cheque = 6,
        [Display(Name = "VINCULADO_PLANO_CLIENTE", ResourceType = typeof(Resources.RsFinanceiro))]
        Vinculado_Plano_Cliente = 7
    }
}
