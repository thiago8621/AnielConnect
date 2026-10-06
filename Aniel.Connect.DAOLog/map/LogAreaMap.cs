using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAO.map;
using Sinapse.DAOLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAOLog.map
{
    public class LogAreaMap : LogGenericoMap<LogAreaAniel,Int64>
    {
        public LogAreaMap() : base()
        {
            Table("TB_AREA");

            Id(u => u.Id, "COD_AREA").GeneratedBy.Sequence("GN_AREA");

            Map(p => p.Chave, "OBJETO").Index("IDX_AREA_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_AREA_CAMPO");
            Map(p => p.DescricaoResource, "OBSERVACAO").Index("IDX_OBS_AREA");
        }
    }

}
