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
    class CobrancaAdicionalMap : ClassMapBase<CobrancaAdicional>
    {
        public CobrancaAdicionalMap() : base()
        {
            Table("TB_COBRANCA_ADICIONAL");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_COBRANCA_ADICIONAL");

            References(x => x.Faturamento_Cliente, "ID_FATURAMENTO_CLIENTE").ForeignKey("FK_COB_ADC_FAT_CLIENTE");
            References(x => x.Cliente, "ID_CLIENTE").ForeignKey("FK_COB_ADC_CLIENTE");
            References(x => x.ClienteContrato, "ID_CLIENTE_CONTRATO").ForeignKey("FK_COB_ADC_CONTRATO");

            Map(x => x.Valor, "VALOR");
            Map(x => x.DataCobranca, "DATA_COBRANCA");
            Map(x => x.Motivo, "MOTIVO").CustomType<MotivoCobrancaAdicional>();

        }

    }
}
