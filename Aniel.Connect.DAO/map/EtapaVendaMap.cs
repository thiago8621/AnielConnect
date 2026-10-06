using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class EtapaVendaMap : ClassMapBase<EtapaVenda>
    {
        public EtapaVendaMap() : base()
        {
            Table("TB_ETAPA_VENDA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ETAPA_VENDA");
            
            References(u => u.Venda, "ID_VENDA").ForeignKey("FK_ETAPA_VENDA");
            References(u => u.Cliente, "ID_CLIENTE").ForeignKey("FK_ETAPA_CLIENTE");
            References(u => u.TrocaTitularidade, "ID_TROCA_TITULARIDADE").ForeignKey("FK_TROCA_TITULARIDADE");

            Map(u => u.Etapa, "ETAPA").CustomType<EnumEtapaVenda>();
            Map(u => u.Descricao, "DESCRICAO").Length(150);
            Map(u => u.Status, "STATUS").CustomType<StatusEtapaVenda>();
            Map(x => x.MotivoAprovManual, "MOTIVO_APROV_Manual");
            Map(x => x.MotivoAprovManualESC, "MOTIVO_APROV_ManualESC");

        }
    }
}
