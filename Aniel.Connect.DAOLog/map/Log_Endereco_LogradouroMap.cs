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
    public class Log_Endereco_LogradouroMap : LogGenericoMap<Log_Endereco_Logradouro,Int64>
    {
        public Log_Endereco_LogradouroMap() : base()
        {
            Table("TB_ENDERECO_LOGRADOURO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ATENDIMENTO");

            Map(p => p.Chave, "OBJETO").Index("IDX_ATENDIMENTO_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_ATENDIMENTO_CAMPO");
        }
    }

}
