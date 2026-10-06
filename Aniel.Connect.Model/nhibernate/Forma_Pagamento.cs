using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Forma_Pagamento : BaseModel
    {
        private IList<Forma_Pagamento_Vencimento> _forma_Pagamento_Vencimento;

        public virtual String Descricao { get; set; }
        public virtual Tipo_Forma_Pagamento Tipo_Forma_Pagamento { get; set; }
        public virtual bool Alterado { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual string CodigoIntegracao { get; set; }
        public virtual Boolean Gerar_Boleto_Automatico { get; set; }
        public virtual Boolean UtilizaRecorrencia { get; set; }
        public virtual Banco Banco { get; set; }
        public virtual Conta_Bancaria Conta_Bancaria { get; set; }
        public virtual GateWayPagamento GateWayPagamento { get; set; }
        public virtual IList<Forma_Pagamento_Vencimento> Forma_Pagamento_Vencimento
        {
            get
            {
                if (_forma_Pagamento_Vencimento == null)
                    _forma_Pagamento_Vencimento = new List<Forma_Pagamento_Vencimento>();
                return _forma_Pagamento_Vencimento;
            }
            set
            {
                _forma_Pagamento_Vencimento = value;
            }
        }
    }
}
