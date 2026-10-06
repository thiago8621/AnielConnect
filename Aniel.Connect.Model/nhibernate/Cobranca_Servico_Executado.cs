using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Cobranca_Servico_Executado : BaseModel
    {
        public virtual string Codct { get; set; }
        public virtual string Projeto { get; set; }
        public virtual string Num_Obra { get; set; }
        public virtual long Num_Doc { get; set; }
        public virtual Boolean GeradoFatura { get; set; }
        public virtual Boolean Processado { get; set; }
        public virtual long? Id_FaturamentoProtocolo { get; set; }
        public virtual long? Id_ClientePlanoProduto_Cobranca { get; set; }
        public virtual Faturamento_Cliente Faturamento_Cliente { get; set; }
        public virtual Plano Plano_Cobranca { get; set; }
        public virtual Forma_Pagamento Forma_Pagamento { get; set; }
        public virtual Cliente Cliente { get; set; }
    }
}
