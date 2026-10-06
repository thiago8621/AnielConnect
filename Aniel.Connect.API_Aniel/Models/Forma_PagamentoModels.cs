using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.API_Aniel.Models
{

    public class FormaPamentoApiList
    {
        public Int64 Id { get; set; }
        public string Descricao { get; set; }
        public string Tipo_Forma_Pagamento { get; set; }
        public string Status { get; set; }
    }

    public class Forma_PagamentoListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }
        public virtual String Tipo_Forma_Pagamento { get; set; }
        public virtual Int64? Id_Banco { get; set; }
        public virtual String Descricao_Banco { get; set; }
        public virtual String Conta_Corrente { get; set; }
        public virtual String Agencia { get; set; }
        public virtual String Digito { get; set; }
        public virtual Int64 Id_Vencimento { get; set; }
        public virtual String Dia_Vencimento { get; set; }
        public virtual Cliente_FormaEnvio? Forma_Envio { get; set; }
    }


    public class Forma_PagamentoCadastroModel 
    {
        private List<Forma_PagamentoVencimentoCadastroModel> _forma_Pagamento_Vencimento;

        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Tipo_Forma_Pagamento Tipo_Forma_Pagamento { get; set; }

        public virtual List<Forma_PagamentoVencimentoCadastroModel> Forma_Pagamento_Vencimento
        {
            get
            {
                if (_forma_Pagamento_Vencimento == null)
                    _forma_Pagamento_Vencimento = new List<Forma_PagamentoVencimentoCadastroModel>();
                return _forma_Pagamento_Vencimento;
            }
            set
            {
                _forma_Pagamento_Vencimento = value;
            }
        }
    }

    public class Forma_PagamentoVencimentoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual Int64 IdVencimento { get; set; }

        public virtual Int16 DiaVencimento { get; set; }
    }

    public class Forma_Pagamento_DataTableRequest : DataTableRequest
    {

    }

    public class Forma_PagamentoComboModel : ObjComboResult<Int64?>
    {
        private List<Forma_PagamentoVencimentoCadastroModel> _forma_Pagamento_Vencimento;

        public virtual Tipo_Forma_Pagamento? Tipo_Forma_Pagamento { get; set; }
        public virtual List<Forma_PagamentoVencimentoCadastroModel> Forma_Pagamento_Vencimento
        {
            get
            {
                if (_forma_Pagamento_Vencimento == null)
                    _forma_Pagamento_Vencimento = new List<Forma_PagamentoVencimentoCadastroModel>();
                return _forma_Pagamento_Vencimento;
            }
            set
            {
                _forma_Pagamento_Vencimento = value;
            }
        }
    }
}