using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAOLog.map
{
    public class Log_PerfilAgendamentoAnielMap : ClassMapBaseLog<LogPerfilAgendamentoAniel>
    {
        public Log_PerfilAgendamentoAnielMap() : base()
        {
            Table("TB_LOG_PERFILAGENDAMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOG_PERFILAGENDAMENTO");
            Map(p => p.Perfil, "PERFIL");
            Map(p => p.GrupoArea, "GRUPOAREA");
            Map(p => p.Area, "AREA");
            Map(p => p.Localidade, "LOCALIDADE");
            Map(p => p.Natureza, "NATUREZA");
            Map(p => p.Tipo_Alteracao, "TIPOALTERACAO");
            Map(p => p.DataHora_Alteracao, "DATAHORAALTERACAO");
            Map(p => p.Usuario_Alteracao, "USUARIOALTERACAO");

        }
    }

}
