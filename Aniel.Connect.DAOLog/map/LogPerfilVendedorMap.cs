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
    public class LogPerfilVendedorMap : LogGenericoMap<LogHistoricoVenda,Int64>
    {
        public LogPerfilVendedorMap() : base()
        {
            Table("TB_HISTORICO_VENDAlOG");

            Id(u => u.Id, "COD_PERFIL_VENDEDOR").GeneratedBy.Sequence("GN_PERFIL_VENDEDOR");

            Map(p => p.Chave, "OBJETO").Index("IDX_AREA_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_AREA_CAMPO");
            Map(p => p.DescricaoResource, "OBSERVACAO").Index("IDX_OBS_AREA");
        }
    }

}
