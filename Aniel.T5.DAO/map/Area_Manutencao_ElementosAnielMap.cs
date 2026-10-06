using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class Area_Manutencao_ElementosMap : ClassMap<Area_Manutencao_Elementos>
    {
        public Area_Manutencao_ElementosMap()
        {
            Table("TB_AREA_MANUTENCAO_ELEMENTOS");
            Id(x => x.Id, "ID").GeneratedBy.Sequence("GN_AREA_MANUTENCAO_ELEMENTOS");
            References(x => x.AreaManutencao, "ID_AREA_MANUTENCAO");
            References(x => x.Elemento_de_Rede, "ID_ELEMENTOS_AFETADOS");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Id_Node, "ID_TREEVIEW").Length(255);
            Map(x => x.Data_HoraFinal, "DATA_HORA_FIM");
            Map(x => x.Data_HoraInicial, "DATA_HORA_INICIAL");          
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
