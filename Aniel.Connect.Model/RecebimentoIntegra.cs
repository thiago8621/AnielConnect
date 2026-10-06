using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model
{
    public class RecebimentoIntegra : BaseModel
    {
        public virtual string Dia { get; set; }
        public virtual string Hora { get; set; }
        public virtual string Pedido { get; set; }
        public virtual string CodEmpresa { get; set; }
        public virtual string CodFornecedor { get; set; }
        public virtual string CodFilial { get; set; }
        public virtual string NotaFiscal { get; set; }
        public virtual string Contrato { get; set; }
        public virtual string Projeto { get; set; }
        public virtual string Usuario { get; set; }
    }
    
    public class RecebimentoIntegraItem : BaseModel
    {
        public virtual string Dia { get; set; }
        public virtual string Hora { get; set; }
        public virtual string CodMaterial { get; set; }
        public virtual string CodContrato { get; set; }
        public virtual string Projeto { get; set; }
        public virtual string CodComplementar { get; set; }
        public virtual string NotaFiscal { get; set; }
        public virtual string Quantidade { get; set; }
        public virtual string ValorUnitario { get; set; }
        public virtual string ValorTotal { get; set; }
        public virtual DateTime DataRecebimento { get; set; }
        public virtual string Id_Receb { get; set; }
        public virtual string Usuario { get; set; }

    }
}
