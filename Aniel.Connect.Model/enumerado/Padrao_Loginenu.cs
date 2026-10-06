using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Padrao_loginenu
    {
        [Display(Name = "PRIMEIRONOME", ResourceType = typeof(Resources.RsCadastros))]
        PrimeiroNome = 0,
        [Display(Name = "ULTIMONOME", ResourceType = typeof(Resources.RsCadastros))]
        UltimoNome = 1,
        [Display(Name = "CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        Contrato = 2,
        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsCadastros))]
        Codigo = 3,
        [Display(Name = "EMAIL", ResourceType = typeof(Resources.RsCadastros))]
        Email = 4,
        [Display(Name = "CPF_CNPJ", ResourceType = typeof(Resources.RsCadastros))]
        Cpf_Cnpj = 5,
        [Display(Name = "UF", ResourceType = typeof(Resources.RsCadastros))]
        Uf = 6,
        [Display(Name = "CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        Cidade = 7,
        [Display(Name = "CONTRATO_PLANO_PRODUTO", ResourceType = typeof(Resources.RsCadastros))]
        ContratoPlanoProduto = 8,
    }
}
