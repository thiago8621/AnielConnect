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
    public class Area_ManutencaoMap : ClassMap<Area_Manutencao>
    {
        public Area_ManutencaoMap()
        {
            Table("TB_AREA_MANUTENCAO");
            Id(x => x.Id, "ID").GeneratedBy.Sequence("GN_AREA_MANUTENCAO");
            References(x => x.Elemento_de_Rede, "ID_ELEMENTO_REDE").ForeignKey("FK_AREA_MANUTENCAO_ELEMENTO");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.CodCt, "CODCT").Length(4);
            Map(x => x.Projeto, "PROJETO").Length(20);

            Map(x => x.Data_HoraFinal, "DATA_HORA_FIM");
            Map(x => x.Data_HoraInicial, "DATA_HORA_INICIAL");

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
