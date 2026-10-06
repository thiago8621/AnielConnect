using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAOLog;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAOLog.map
{
    public class LogFormaPagamentoClienteMap : LogGenericoMap<LogFormaPagamentoCliente, Int64>
    {
        public LogFormaPagamentoClienteMap() : base()
        {
            Table("TB_LOG_FORMA_PAGAMENTO_CLIENTE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOG_FORMA_PAGAMENTO_CLIENTE");

            Map(x => x.Conta_Corrente, "CONTA_CORRENTE").Length(25);
            Map(x => x.Agencia, "AGENCIA").Length(8);
            Map(x => x.Digito, "DIGITO").Length(1);
            Map(x => x.Alterado, "ALTERADO");

            Map(x => x.Id_Vencimento, "ID_VENCIMENTO");
            Map(x => x.Id_Banco, "ID_BANCO");
            Map(x => x.Id_Forma_Pagamento_Cliente, "ID_FORMA_PAGAMENTO_CLIENTE");
            Map(x => x.Id_Forma_Pagamento, "ID_FORMA_PAGAMENTO");
            Map(x => x.Id_Cliente, "ID_CLIENTE");
            
            Map(x => x.Forma_Envio, "FORMA_ENVIO");

            Map(x => x.Dia_Vencimento, "DIA_VENCIMENTO");

            Map(x => x.Id_Endereco_Cobranca, "ID_ENDERECO_COBRANCA");
            Map(x => x.End_Cobranca_Igual_Principal, "END_COBRANCA_IGUAL_PRINCIPAL");

            Map(x => x.Status, "STATUS");
            Map(x => x.StatusValidoCartao, "STATUS_VALIDO_CARTAO");
        }
    }
}
