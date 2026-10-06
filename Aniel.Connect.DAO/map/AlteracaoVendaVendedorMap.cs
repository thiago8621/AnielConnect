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
    public class AlteracaoVendaVendedorMap : ClassMapBase<AlteracaoVendaVendedor>
    {
        public AlteracaoVendaVendedorMap() : base()
        {
            Table("TB_ALTERACAO_VENDA_VENDEDOR");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ALTERACAO_VENDA_VENDEDOR");

            Map(x => x.id_venda, "ID_VENDA"); 
            Map(x => x.id_supervisor, "ID_SUPERVISOR");
            Map(x => x.id_vendedor_de, "ID_VENDEDOR_DE");
            Map(x => x.id_vendedor_para, "ID_VENDEDOR_PARA");

            Map(x => x.status, "STATUS").CustomType<Status_Alteracao_Vendedor>();
        }

    }
}
