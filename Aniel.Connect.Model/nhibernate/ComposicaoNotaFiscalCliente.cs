using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class ComposicaoNotaFiscalCliente : BaseModel
    {
        public virtual Double? ValorDiscriminado { get; set; }
        public virtual Double? PercentualDiscriminado { get; set; }
        public virtual Double? ValorDiscriminadoNf { get; set; }
        public virtual String ModeloNotaFiscal { get; set; }
        public virtual Boolean Instalacao { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual ClienteContrato ClienteContrato { get; set; }
        public virtual Cliente_PlanoProduto ClientePlanoProduto { get; set; }
        public virtual Configuracao_Tributaria Configuracao_Tributaria { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Filial Filial { get; set; }
        public virtual ProdutosComp ProdutosComp { get; set; }
    }
}
