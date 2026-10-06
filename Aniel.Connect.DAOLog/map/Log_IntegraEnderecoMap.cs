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
    public class Log_IntegraEnderecoMap : LogGenericoMap<Log_IntegraEndereco,Int64>
    {
        public Log_IntegraEnderecoMap() : base()
        {
            Table("TB_INTEGRA_ENDERECO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_INTEGRA_ENDERECO");

            Map(p => p.Chave, "OBJETO").Index("IDX_INTE_ENDERECO_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_INTE_ENDERECO_CAMPO");
        }
    }

}
