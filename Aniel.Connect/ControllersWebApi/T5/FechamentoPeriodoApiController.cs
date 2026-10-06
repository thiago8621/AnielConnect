using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class FechamentoPeriodoApiController : BaseApiController
    {
        //[HttpPost()]
        public async Task<(DateTime?, DateTime?)> PeriodoFechado(ISession session, String codct, String projeto, DateTime data)
        {
            (DateTime?, DateTime?) retorno = (null, null);
            try
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(@"SELECT DATA_INICIAL,DATA_FINAL FROM TB_FECHAMENTO_PERIODO
                                        WHERE CODCT=@CODCT AND PROJETO=@PROJETO AND (@DATA BETWEEN DATA_INICIAL AND DATA_FINAL)");

                    parametros.Add("@CODCT", codct);
                    parametros.Add("@PROJETO", projeto);
                    parametros.Add("@DATA", data);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            var dataIni = reader["DATA_INICIAL"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DATA_INICIAL"]);
                            var dataFim = reader["DATA_INICIAL"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DATA_INICIAL"]);

                            retorno = (dataIni, dataFim);
                        }
                    }
                }
                return retorno;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}


