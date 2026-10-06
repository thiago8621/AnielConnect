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
    public class Log_BairroMap : LogGenericoMap<Log_Bairro,Int64>
    {
        public Log_BairroMap() : base()
        {
            Table("TB_BAIRRO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_BAIRRO");

            Map(p => p.Chave, "OBJETO").Index("IDX_BAIRRO_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_BAIRRO_CAMPO");
        }
    }

}
