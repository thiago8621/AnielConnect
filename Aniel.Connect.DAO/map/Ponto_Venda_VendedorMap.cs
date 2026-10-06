using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Ponto_Venda_VendedorMap : ClassMapBase<Ponto_Venda_Vendedor>
    {
        public Ponto_Venda_VendedorMap() : base()
        {
            Table("TB_PONTO_VENDA_VENDEDOR");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PONTO_VENDA_VENDEDOR");

            Map(x => x.DataInicial, "DATA_INICIAL");
            Map(x => x.DataFinal, "DATA_FINAL");

            References(x => x.Ponto_Venda, "ID_PONTO_VENDA").ForeignKey("FK_PV_PV");
            References(x => x.Vendedor, "ID_VENDEDOR").ForeignKey("FK_PV_VENDEDOR");
        }
    }
}
