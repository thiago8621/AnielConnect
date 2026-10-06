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
    public class LogLocalidadeMap : LogGenericoMap<LogLocalidadeAniel,Int64>
    {
        public LogLocalidadeMap() : base()
        {
            Table("TB_LOCALIDADE");

            Id(u => u.Id, "COD_LOCALIDADE").GeneratedBy.Sequence("GN_LOCALIDADE");

            Map(p => p.Chave, "OBJETO").Index("IDX_LOCALIDADE_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_LOCALIDADE_CAMPO");
        }
    }

}
