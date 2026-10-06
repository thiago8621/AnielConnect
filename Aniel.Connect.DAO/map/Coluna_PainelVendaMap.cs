using Aniel.Connect.Model;
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
    class Coluna_PainelVendaMap : ClassMapBase<Coluna_PainelVenda>
    {
        public Coluna_PainelVendaMap() : base()
        {
            Table("TB_COLUNA_PAINEL_VENDA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_COLUNA_PAINEL_VENDA");

            References(x => x.Modelo_PainelVenda, "ID_MODELO").ForeignKey("FK_COLUNA_MOD_VENDA");
            Map(x => x.Coluna, "COLUNA").CustomType<Coluna_PainelVendaEnum>();

        }
    }
}
