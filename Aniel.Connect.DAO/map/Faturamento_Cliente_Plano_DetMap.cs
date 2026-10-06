using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Faturamento_Cliente_Plano_DetMap : ClassMapBase<Faturamento_Cliente_Plano_Det>
    {
        public Faturamento_Cliente_Plano_DetMap() : base()
        {
            Table("TB_FATURAMENTO_CLIENTE_PLANODT");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_FATURAMENTO_CLIENTE_PLANODT");

            References(x => x.Faturamento_Cliente_Plano, "ID_FATURAMENTO_CLIENTE_PLANO").ForeignKey("FK_FAT_CPDET_FATCLIPLANO");

            Map(x => x.DataHora_Abertura, "DATAHORA_ABERTURA");
            Map(x => x.DataHora_Fechamento, "DATAHORA_FECHAMENTO");

            Map(x => x.Valor_Periodo, "VALOR");
        }
    }
}
