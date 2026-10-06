using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class Cobranca_Servico_ExecutadoMap : ClassMapBase<Cobranca_Servico_Executado>
    {

        public Cobranca_Servico_ExecutadoMap() : base()
        {
            Table("TB_COB_SERVICO_EXEC");

            Id(x => x.Id, "ID").GeneratedBy.Sequence("GN_COBSERV_EXEC");
            Map(x => x.Codct, "CODCT");
            Map(x => x.Projeto, "PROJETO");
            Map(x => x.Num_Obra, "NUM_OBRA");
            Map(x => x.Num_Doc, "NUM_DOC");
            Map(x => x.GeradoFatura, "GERADO_FATURA");
            Map(x => x.Processado, "FATURA_PROCESSADA");
            Map(x => x.Id_FaturamentoProtocolo, "ID_FATURAMENTO_PROT");
            Map(x => x.Id_ClientePlanoProduto_Cobranca, "ID_CLI_PLAN_PROD_COBRANCA");

            References(x => x.Faturamento_Cliente, "ID_FATURAMENTO_CLI");
            References(x => x.Plano_Cobranca, "ID_CLI_PLAN_COBRANCA");
            References(x => x.Forma_Pagamento, "ID_FORMA_PAG");
            References(x => x.Cliente, "ID_CLIENTE");
        }
    }
}
