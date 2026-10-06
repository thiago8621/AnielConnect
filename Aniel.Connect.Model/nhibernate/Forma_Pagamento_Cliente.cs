using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Forma_Pagamento_Cliente : BaseModel
    {
        public virtual Cliente Cliente { get; set; }
        public virtual Forma_Pagamento Forma_Pagamento { get; set; }
        public virtual Banco Banco { get; set; }
        public virtual String Conta_Corrente { get; set; }
        public virtual String Agencia { get; set; }
        public virtual String Digito { get; set; }
        public virtual Vencimento Vencimento { get; set; }
        public virtual String Dia_Vencimento { get; set; }
        public virtual Cliente_FormaEnvio? Forma_Envio { get; set; }
        public virtual Endereco_Cliente Endereco_Cobranca { get; set; }
        public virtual Boolean End_Cobranca_Igual_Principal { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual Boolean Origem_Mobile { get; set; }
        public virtual Boolean Alterado { get; set; }
        public virtual StatusValidacaoCartao? StatusValidoCartao { get; set; }
        public virtual long? Venda { get; set; }
        #region Mobile
        public virtual String Token_Cartao { get; set; }
        #endregion
    }
    public enum StatusValidacaoCartao
    {
        Aguardando_Validacao = 0,
        Cartao_Valido = 1,
        Cartao_Rejeitado = 2
    }
}
