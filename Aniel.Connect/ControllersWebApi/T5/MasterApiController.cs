using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class MasterApiController : BaseApiController
    {
        public async Task<String> GetConfig_Doc(ISession session, String Sessao, String Parametro, String Default)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            SaldoEquipeModel retorno = new SaldoEquipeModel();

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(@"SELECT FIRST 1 A.config_doc
                                 FROM TB_MASTER A");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        String configDoc = Convert.ToString(reader["CONFIG_DOC"]);

                        var iniParser = new IniParser().LoadIniString(configDoc);

                        var tmp = iniParser.GetSetting(Sessao, Parametro);

                        return String.IsNullOrEmpty(tmp) ? Default : tmp;
                    }
                    return Default;
                }
            }
        }

        

    }
}