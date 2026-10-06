using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class GateWayPagamentoMap : ClassMapBase<GateWayPagamento>
    {
        public GateWayPagamentoMap() : base()
        {
            Table("TB_GATEWAY_PAGAMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_GATEWAY_PAGAMENTO");

            Map(x => x.ClientId, "CLIENT_ID");
            Map(x => x.ClientSecret, "CLIENT_SECRET");
            Map(x => x.SellerId, "SELLER_ID");
            Map(x => x.Digito_Conta, "DIGITO_CONTA");
            Map(x => x.Digito_Agencia, "DIGITO_AGENCIA");
            Map(x => x.Agencia, "AGENCIA");
            Map(x => x.Conta_Corrente, "CONTA_CORRENTE");
            Map(x => x.Utilizar_Transferencia, "UTILIZAR_TRANSFERENCIA");
            Map(x => x.ResourceToken, "RESOURCE_TOKEN");
            Map(x => x.UtilizarAmbienteSandBox, "UTILIZAR_AMBIENTE_SANDBOX");
            Map(x => x.AuthorizationToken, "AUTHORIZATION_TOKEN").Length(5000);
            Map(x => x.DataHora_AuthTokenExpira, "DATAHORA_AUTHTOKENEXPIRA");
            Map(x => x.Cooperativa, "COOPERATIVA");
            Map(x => x.Posto, "POSTO");
            Map(x => x.CodigoBeficiario, "CODIGO_BENEFICARIO");
            Map(x => x.Todas_Filiais, "TODAS_FILIAIS");

            References(x => x.Conta_Bancaria, "ID_CONTA_BANCARIA").ForeignKey("FK_GATEWAY_CONTA_BANCARIA");
            References(x => x.Empresa, "ID_EMPRESA").ForeignKey("FK_GATEWAY_EMPRESA");
            References(x => x.Filial, "ID_FILIAL").ForeignKey("FK_GATEWAY_FILIAL");
            References(x => x.Banco, "ID_BANCO").ForeignKey("FK_GATEWAY_BANCO");
        }
    }
}
