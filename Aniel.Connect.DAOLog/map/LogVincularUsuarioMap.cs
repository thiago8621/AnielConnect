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
    public class LogVincularUsuarioMap : LogGenericoMap<LogVincularPerfilBase, Int64>
    {
        public LogVincularUsuarioMap() : base()
        {
            Table("TB_VINCULAR_USUARIOlOG");

            Id(u => u.Id, "COD_VINCULAR_USUARIO").GeneratedBy.Sequence("GN_VINCULAR_USUARIO");

            Map(p => p.Chave, "OBJETO").Index("IDX_AREA_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_AREA_CAMPO");
            Map(p => p.DescricaoResource, "OBSERVACAO").Index("IDX_OBS_AREA");
        }
    }

}
