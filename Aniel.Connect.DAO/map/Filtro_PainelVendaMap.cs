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
    class Filtro_PainelVendaMap : ClassMapBase<Filtro_PainelVenda>
    {
        public Filtro_PainelVendaMap() : base()
        {
            Table("TB_RECURSO_PAINEL_VENDA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_RECURSO_PAINEL_VENDA");

            Map(x => x.Valor, "VALOR");
            References(x => x.Modelo_PainelVenda, "ID_MODELO").ForeignKey("FK_FILTRO_MOD_VENDA");
            Map(x => x.Filtro, "FILTRO").CustomType<Filtro_PainelVendaEnum>();
            Map(x => x.Data, "DATA").CustomType<ModeloPainel_Data?>();

        }
    }
}
