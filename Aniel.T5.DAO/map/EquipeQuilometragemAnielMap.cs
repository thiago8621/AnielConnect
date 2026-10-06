using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class EquipeQuilometragemAnielMap : ClassMap<EquipeQuilometragemAniel>
    {
        public EquipeQuilometragemAnielMap()
        {
            Table("TB_EQUIPE_QUILOMETRAGEM");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_TIPO_ELEMENTO_MATERIAL_ID");
            References(x => x.Equipe, "EQUIPE").ForeignKey("FK_EQUIPE_QUILOMETRAGEM").Index("IDX_DATA_EQUIPE_KM");
            Map(x => x.Data, "DATA").Index("IDX_DATA_EQUIPE_KM");
            Map(p => p.DataHoraCriacao);
            Map(x => x.DistanciaPlanejada, "DISTANCIA_PLANEJADA");
            Map(x => x.DistanciaRealizada, "DISTANCIA_REALIZADA");
            Map(x => x.DistanciaRealRastreamento, "DISTANCIA_REAL_RASTREAMENTO");
            Map(x => x.TempoParalisadoMinutos, "MINUTOS_PARALISADO");
            Map(x => x.Quantidade_OS, "QUANTIDADE_OS");
        }
    }
}
