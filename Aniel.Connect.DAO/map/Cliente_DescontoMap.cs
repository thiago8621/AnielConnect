using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Cliente_DescontoMap : ClassMapBase<Cliente_Desconto>
    {
        public Cliente_DescontoMap() : base()
        {
            Table("TB_CLIENTE_DESCONTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CLIENTE_DESCONTO");

            References(x => x.Cliente,      "ID_CLIENTE").ForeignKey("FK_CLIENTE_DESC_CLIENTE");
            References(x => x.Desconto,     "ID_DESCONTO").ForeignKey("FK_CLIENTE_DESC_DESC");
            Map(x => x.Valor_Desconto,      "VALOR_DESCONTO");
            Map(x => x.Percentual_Desconto, "PERCENTUAL_DESCONTO");
            Map(x => x.Numero_ContratoVinculado, "NUMERO_CONTRATO");
            Map(x => x.DataAtribuicaoDesconto, "DATA_ATT_DESC");
            Map(x => x.DataVencimentoDesconto, "DATA_VENC_DESC");
            Map(x => x.DescontoIndeterminado, "DESC_INDETERMINADO");
        }
    }
}
