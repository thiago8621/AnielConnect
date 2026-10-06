using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class DetalhesHistoricoVenda_LOG
    {
        /*Propriedades para CLIENTE_PLANO_PRODUTO*/
        public virtual String DescPlano_Combo { get; set; }
        public virtual String Valor_Real { get; set; }
        public virtual Boolean FlagPlano { get; set; }
        public virtual String Valor_Instalacao_Real { get; set; }
        public virtual String Nr_Parcelas_Instalacao { get; set; }
        public virtual String Valor_Entrada_Instalacao { get; set; }
        public virtual String Valor_Instalacao_Final { get; set; }
        public virtual String Dias_Teste_Gratis { get; set; }
        public virtual String Vigencia_Contrato { get; set; }
        public virtual String Modelo_Contrato { get; set; }
        public virtual String  DescFormaPagto { get; set; }
        public virtual String DescEnderecoInstalacao { get; set; }

        /*Propriedades para FORMA_PAGAMENTO_CLIENTE*/
        public virtual String DescFormaPagtoCliente { get; set; }
        public virtual String DescBanco { get; set; }
        public virtual String DescAgencia { get; set; }
        public virtual String DescConta { get; set; }
        public virtual String Digito { get; set; }
        public virtual String Dia_Vencimento { get; set; }
        public virtual String FormaEnvioFatura { get; set; }
        public virtual String DescEnderecoCobranca { get; set; }
    }
}