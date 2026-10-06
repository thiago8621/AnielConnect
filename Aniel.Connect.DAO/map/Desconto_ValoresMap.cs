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
    public class Desconto_ValoresMap : ClassMapBase<Desconto_Valores>
    {
        public Desconto_ValoresMap() : base()
        {
            Table("TB_DESCONTO_VALORES");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_DESCONTO_VALORES");

            Map(x => x.MesInicial, "MESINICIAL");
            Map(x => x.MesFinal, "MESFINAL");
            Map(x => x.Valor, "VALOR");
            Map(x => x.Percentual, "PERCENTUAL");

            References(x => x.Desconto, "ID_DESCONTO").ForeignKey("FK_DESCONTOVALORES_DESCONTO");
            


        }
    }
}
