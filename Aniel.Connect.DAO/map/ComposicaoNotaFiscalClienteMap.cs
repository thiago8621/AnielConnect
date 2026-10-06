using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Model.enumerado;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class ComposicaoNotaFiscalClienteMap : ClassMapBase<ComposicaoNotaFiscalCliente>
    {
        public ComposicaoNotaFiscalClienteMap() : base()
        {
            Table("TB_COMP_NF_CLIENTE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_COMP_NOTA");

            Map(x => x.ValorDiscriminado, "VALOR_DISCRIMINADO");
            Map(x => x.PercentualDiscriminado, "PERCENTUAL_DISCRIMINADO");

            Map(x => x.ValorDiscriminadoNf, "VALOR_NF_DISCRIMINADO");

            Map(x => x.ModeloNotaFiscal, "MODELO_NOTA");
            Map(x => x.Cliente, "ID_CLIENTE");

            Map(x => x.ClienteContrato, "ID_CLIENTE_CONTRATO");
            Map(x => x.ClientePlanoProduto, "ID_CLIENTE_PLANO");

            Map(x => x.Configuracao_Tributaria, "ID_CONFIG_TRIBU");
            Map(x => x.Empresa, "ID_EMPRESA");

            Map(x => x.Filial, "ID_FILIAL");
            Map(x => x.ProdutosComp, "ID_PRODUTO_COMP");

            Map(x => x.Instalacao, "INSTALACAO");
        }
    }
}
