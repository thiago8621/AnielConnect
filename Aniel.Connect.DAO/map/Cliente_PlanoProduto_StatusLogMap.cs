using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class Cliente_PlanoProduto_StatusLogMap : ClassMapBase<Cliente_PlanoProduto_StatusLog>
    {
        public Cliente_PlanoProduto_StatusLogMap() : base()
        {
            Table("TB_CLIENTE_PLANO_PRODUTO_STLOG");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CLIENTE_PLANO_PRODUTO_STLOG");

            Map(x => x.DataHoraAcao, "DATAHORA_ACAO");

            Map(x => x.AntigoStatus, "ANTIGO_STATUS").CustomType<Status_PlanoProduto>();
            Map(x => x.NovoStatus, "NOVO_STATUS").CustomType<Status_PlanoProduto>();

            References(x => x.PlanoProduto, "ID_PLANO_PRODUTO").ForeignKey("FK_CLIENTE_PP_STATUS_PP");
            References(x => x.ClienteContrato, "ID_CLIENTE_CONTRATO").ForeignKey("FK_CLIENTE_CT_STATUS_CT");
        }

    }
}
