using Sinapse.Model.enumerados;
using Sinapse.Model;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ModelLog.nhibernate
{
    public class LogFormaPagamentoCliente : LogGenerico<Int64>
    {
        public LogFormaPagamentoCliente()
        {
            
        }
        public virtual Int64? Id_Forma_Pagamento_Cliente { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual Int64? Id_Forma_Pagamento { get; set; }
        public virtual Int64? Id_Banco { get; set; }
        public virtual String Conta_Corrente { get; set; }
        public virtual String Agencia { get; set; }
        public virtual String Digito { get; set; }
        public virtual Int64? Id_Vencimento { get; set; }
        public virtual String Dia_Vencimento { get; set; }
        public virtual Int32? Forma_Envio { get; set; }
        public virtual Int64? Id_Endereco_Cobranca { get; set; }
        public virtual Boolean End_Cobranca_Igual_Principal { get; set; }
        public virtual Int32 Status { get; set; }
        public virtual Boolean Origem_Mobile { get; set; }
        public virtual Boolean Alterado { get; set; }
        public virtual Int32? StatusValidoCartao { get; set; }

        #region Mobile
        public virtual String Token_Cartao { get; set; }
        #endregion
    }
}
