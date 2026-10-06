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
    public class Log_CidadeMap : LogGenericoMap<Log_Cidade,Int64>
    {
        public Log_CidadeMap() : base()
        {
            Table("TB_CIDADE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ATENDIMENTO");

            Map(p => p.Chave, "OBJETO").Index("IDX_ATENDIMENTO_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_ATENDIMENTO_CAMPO");
        }
    }

}
