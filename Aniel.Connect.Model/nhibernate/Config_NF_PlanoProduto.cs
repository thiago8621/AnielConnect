using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Config_NF_PlanoProduto : BaseModel
    {
        public virtual Double? ValorDiscriminado { get; set; }
        public virtual Double? PercentualDiscriminado { get; set; }
        public virtual Double? ValorDiscriminadoNf { get; set; }
        public virtual Configuracao_Tributaria Configuracao_Tributaria { get; set; }

        public virtual Modelo_NotaFiscal? ModeloNotaFiscal { get; set; }

        public virtual Plano Plano { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Filial Filial { get; set; }
        public virtual ProdutosComp ProdutosComp { get; set; }

        public virtual Boolean Instalacao { get; set; }
    }
}
