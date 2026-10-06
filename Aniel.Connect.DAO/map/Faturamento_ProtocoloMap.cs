using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class Faturamento_ProtocoloMap : ClassMapBase<Faturamento_Protocolo>
    {
        public Faturamento_ProtocoloMap() : base()
        {
            Table("TB_FATURAMENTO_PROTOCOLO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_FATURAMENTO_PROTOCOLO");

            Map(x => x.Protocolo, "NUM_PROTOCOLO").Length(30);
            Map(x => x.Data_Vencimento, "DATA_VENCIMENTO").Index("IDX_FATU_PROT_VENCTO");
            Map(x => x.Data_Inicial, "DATA_INICIAL");
            Map(x => x.Data_Final, "DATA_FINAL");
            Map(x => x.Parcial, "PARCIAL").CustomType<Tipo_Faturamento>();
            Map(x => x.Tem_Valor_Avulso, "TEM_VALOR_AVULSO");
            Map(x => x.Valor_Avulso, "VALOR_AVULSO");
            Map(x => x.Recorrencia, "RECORRENCIA");
            Map(x => x.Quantidade_Recorrencia, "QTD_RECORRENCIA");
        }
    }
}
