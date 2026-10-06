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
    public class LogAtualizacaoLoginsMap : ClassMapBase<LogAtualizacaoLogins>
    {
        public LogAtualizacaoLoginsMap() : base()
        {
            Table("TB_LOG_ATUALIZACAO_LOGINS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOG_ATUALIZACAO_LOGINS");

            Map(x => x.Login, "LOGIN");
            Map(x => x.Data, "DATA");
            Map(x => x.Hora, "HORA");
            Map(x => x.Usuario, "USUARIO");
            Map(x => x.Detalhe, "DETALHE").Length(5000);
            Map(x => x.Sucesso, "SUCESSO");
        }
    }
}
