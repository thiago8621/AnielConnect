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
    public class LogPerfilAgendamentoMap : LogGenericoMap<LogPerfilAgendamentoAniel, Int64>
    {
        public LogPerfilAgendamentoMap() : base()
        {
            Table("TB_LOG_PERFIL_AGENDAMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOG_PERFILAGENDAMENTO");
            Map(p => p.Descricao, "DESCRICAO");
            Map(p => p.Natureza, "NATUREZA");
            Map(p => p.GrupoArea, "GRUPOAREA");           
            Map(p => p.Area, "AREA");
            Map(p => p.Localidade, "LOCALIDADE");
        }
    }

}
