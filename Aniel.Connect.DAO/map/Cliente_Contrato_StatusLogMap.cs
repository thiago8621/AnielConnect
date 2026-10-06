using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class Cliente_Contrato_StatusLogMap : ClassMapBase<Cliente_Contrato_StatusLog>
    {
        public Cliente_Contrato_StatusLogMap() : base()
        {
            Table("TB_CLIENTE_CONTRATO_STLOG");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CLIENTE_CONTRATO_STLOG");

            Map(x => x.DataHoraAcao, "DATAHORA_ACAO");

            Map(x => x.AntigoStatus, "ANTIGO_STATUS").CustomType<Status_Contrato>();
            Map(x => x.NovoStatus, "NOVO_STATUS").CustomType<Status_Contrato>();

            References(x => x.ClienteContrato, "ID_CLIENTE_CONTRATO").ForeignKey("FK_CLIENTE_CT_STATUS_CT");
        }

    }
}
