using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Faturamento_Cliente_Avulso_DetMap : ClassMapBase<Faturamento_Cliente_Avulso_Det>
    {
        public Faturamento_Cliente_Avulso_DetMap() : base()
        {
            Table("TB_FATURAMENTO_CLI_AVULSODT");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_TB_FATURAMENTO_CLI_AVULSODT");

            Map(x => x.DataHora_Abertura, "DATAHORA_ABERTURA");
            Map(x => x.DataHora_Fechamento, "DATAHORA_FECHAMENTO");
            Map(x => x.Valor_Avulso, "VALOR_AVULSO");
            Map(x => x.UtilizaRecorrencia, "UTILIZA_RECORRENCIA");
            Map(x => x.NumeroSeqRecorrencia, "NUM_SEQ_RECORRENCIA");
            References(x => x.Faturamento_Protocolo, "ID_FAT_PRT").ForeignKey("FK_FAT_FATPROT");
            References(x => x.Faturamento_Cliente, "ID_FAT_CLI").ForeignKey("FK_FAT_FATCLI");
        }
    }
}
