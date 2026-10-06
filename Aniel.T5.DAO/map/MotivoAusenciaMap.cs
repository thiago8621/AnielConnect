
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
namespace Aniel.T5.DAO.map
{
    public class MotivoAusenciaMap : ClassMapBase<Motivo_Ausencia>
    {
        public MotivoAusenciaMap()
        {
            Table("TB_MOTIVO_AUSENCIA");
            Id(p => p.Id, "ID").GeneratedBy.Sequence("GN_MOTIVO_AUSENCIA");
            Map(p => p.Motivo, "MOTIVO").Length(100);
            Map(p => p.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
