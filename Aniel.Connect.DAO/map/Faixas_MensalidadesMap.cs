using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Faixas_MensalidadesMap : ClassMapBase<Faixas_Mensalidades>
    {
        public Faixas_MensalidadesMap() : base()
        {
            Table("TB_FAIXAS_MENSALIDADE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_FAIXAS_MENSALIDADE");

            Map(x => x.Parcela_Ini, "PARCELA_INICIAL");
            Map(x => x.Parcela_Fin, "PARCELA_FINAL");
            Map(x => x.Percentual_Desconto, "PERCENTUAL_DESCONTO");
            Map(x => x.Valor_Desconto, "VALOR_DESCONTO");

            References(x => x.Plano, "ID_PLANO").ForeignKey("FK_FAIXAS_MENSALIDADE_PLANO");
            References(x => x.Produto, "ID_PRODUTO").ForeignKey("FK_FAIXAS_MENSALIDADE_PRODUTO");
        }
    }
}
