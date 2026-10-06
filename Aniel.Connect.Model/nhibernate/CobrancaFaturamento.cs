using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class CobrancaFaturamento : BaseModel
    {
        public virtual string Id_Boleto { get; set; }
        public virtual string Nosso_Numero { get; set; }
        public virtual double desconto { get; set; }
        public virtual double juros { get; set; }
        public virtual double multa { get; set; }
        public virtual double valor { get; set; }
        public virtual DateTime vencimento { get; set; }

        public virtual DateTime? DataHora_Registro { get; set; }

        public virtual string instrucoesCaixa { get; set; }

        public virtual string linhaDigitavel { get; set; }
        public virtual string referencia { get; set; }
        public virtual string Link_Boleto { get; set; }
        public virtual string html { get; set; }
        public virtual byte[] pdf { get; set; }
        public virtual Filial recebedor { get; set; }
        public virtual Cliente pagador { get; set; }
        public virtual Faturamento_Cliente FaturamentoCliente { get; set; }

        public virtual string Caminho { get; set; }

        public virtual string Valor_Total_Boletos { get; set; }

        public virtual string Quantidade_Registros { get; set; }
        public virtual long? IdGateWay { get; set; }
        public virtual long? Id_AssinaturaGateway { get; set; }
        public virtual long? Id_PlanoGateway { get; set; }
        /// <summary>
        /// Propriedade temporaria utilizada no processar faturamento
        /// </summary>
        public virtual bool ComErroFaturamento { get; set; }
        public virtual Cobranca_StatusPagamento Status { get; set; }
    }
}
