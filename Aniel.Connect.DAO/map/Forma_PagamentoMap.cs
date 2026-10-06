using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Forma_PagamentoMap : ClassMapBase<Forma_Pagamento>
    {

        public Forma_PagamentoMap() : base()
        {
            Table("TB_FORMA_PAGAMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_FORMA_PAGAMENTO");

            Map(x => x.Descricao, "DESCRICAO");
            Map(x => x.Tipo_Forma_Pagamento, "TIPO_FORMA_PAGAMENTO").CustomType<Tipo_Forma_Pagamento>();
            Map(x => x.Alterado, "ALTERADO");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.CodigoIntegracao, "CODIGO_INTEGRACAO");
            Map(x => x.Gerar_Boleto_Automatico, "GERAR_BOLETO_AUTOMATICO");
            Map(x => x.UtilizaRecorrencia, "UTILIZA_RECORRENCIA");
            References(x => x.Banco, "ID_BANCO").ForeignKey("FK_FORMA_PAGTO_BANCO");
            References(x => x.Conta_Bancaria, "ID_CONTA_BANCARIA").ForeignKey("FK_FORMA_PAGTO_CBANCARIA");
            References(x => x.GateWayPagamento, "ID_GATEWAY_PAGAMENTO").ForeignKey("FK_FORMA_GATEWAY");
            HasMany(x => x.Forma_Pagamento_Vencimento).KeyColumn("ID_FORMA_PAGAMENTO").Cascade.All().Inverse();
        }

    }
}
