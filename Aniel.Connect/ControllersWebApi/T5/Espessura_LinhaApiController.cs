using Aniel.Connect.Core;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Core.Helpers;
using Sinapse.Model.enumerados;
using Aniel.T5.Model.enumerado;
namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Espessura_LinhaApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/Espessura_LinhaApi/GetAll")]
        public async Task<DataTableResponse<Espessura_LinhaListModel>> GetAll(Espessura_LinhaDataTableRequest request)
        {
            List<Espessura_LinhaListModel> retorno = new List<Espessura_LinhaListModel>();
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            using (var session = HibernateUtil.GetSessionT5())
            {
                using (var com = session.Connection.CreateCommand())
                {
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new Espessura_LinhaListModel
                            {
                            };
                            retorno.Add(obj);
                        }
                    }
                }
            }

            return new DataTableResponse<Espessura_LinhaListModel>
            {
                data = retorno,
            };
        }
        [HttpPost]
        [Route("api/Espessura_LinhaApi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjCombo<String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>();
            retorno.results = new List<IObjComboResult<string>>();
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_ESPESSURA_LINHA A ");
                    StringBuilder sqlWhere = new StringBuilder();

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        //condicoes.Add($"A.STATUS = 1");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID = '{request.id}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlWhere.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        if (condicoes.Count > 0)
                        {
                            sql.AppendLine(sqlWhere.ToString());
                        }

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;
        }
    }
}