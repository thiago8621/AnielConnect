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
    public class LogVincularPerfilMap : LogGenericoMap<LogVincularPerfilAniel, Int64>
    {
        public LogVincularPerfilMap() : base()
        {
            Table("TB_LOG_VINCULAR_PERFIL");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOG_VINCULAR_PERFIL");
            Map(u => u.Tecnico, "TECNICO").Length(100);

            Map(p => p.Chave, "OBJETO").Index("IDX_VINCULARPERFIL_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_VINCULARPERFIL_CAMPO");

        }
    }

}
