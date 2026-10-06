using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAOLog.map
{
    public class LogPerfilAgendamentoAreaMap : ClassMapBaseLog<LogEquipe_AreaAniel>
    {
        public LogPerfilAgendamentoAreaMap() : base()
        {
            Table("TB_LOG_PERFILAGENDAMENTO_AREA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOG_AGENDAMENTO_AREA");
            Map(p => p.Escritorio, "ESCRITORIO");
            Map(p => p.Area, "AREA");
            Map(p => p.Localidade, "LOCALIDADE");
            Map(p => p.Nivel, "NIVEL");
            Map(p => p.Todas_Localidades, "TODAS_LOCALIDADES");
            Map(p => p.Todas_Areas, "TODAS_AREAS");


        }
    }

}
