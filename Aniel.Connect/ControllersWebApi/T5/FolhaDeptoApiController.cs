using Aniel.Connect.Core;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System;
using NHibernate;
using System.Web.Http;
using System.Linq;
using OpenXmlPowerTools;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class FolhaDeptoApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/FolhaDeptoApi/GetCombo")]
        public async Task<ObjCombo<int>> GetCombo(RequestObjCombo<int?> request)
        {
            var retorno = new ObjCombo<int>
            {
                results = new List<IObjComboResult<int>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                //Filtros 
                var param = new Dictionary<string, object>();

                var sqlFrom = new StringBuilder();
                sqlFrom.AppendLine("FROM TB_FOLHA_DEPTO F");

                var condicoes = new List<string>();

                if (request.id.HasValue)
                {
                    condicoes.Add("F.COD_DEPTO = @COD_DEPTO");
                    param.Add("@COD_DEPTO", request.id.Value);
                }

                if (request != null && !String.IsNullOrEmpty(request.q))
                {
                    condicoes.Add("UPPER(F.DESCRICAO) LIKE @SEARCH");
                    param.Add("@SEARCH", $"%{StringService.Truncate(request.q.ToUpperInvariant(), 30)}%");
                }

                if (condicoes.Count() > 0)
                {
                    sqlFrom.AppendLine($"WHERE ({String.Join(" AND ", condicoes)})");
                }

                using (var com = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder();

                    sql.AppendLine("SELECT COUNT(F.COD_DEPTO) AS CO");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                            retorno.total_count = Convert.ToInt32(reader["CO"]);
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder();

                    sql.AppendLine("SELECT F.COD_DEPTO, F.DESCRICAO");
                    sql.AppendLine(sqlFrom.ToString());
                    sql.AppendLine("ORDER BY F.COD_DEPTO");

                    using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, param, session.Connection, null, request.page))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new ObjComboResult<int>
                            {
                                Id = Convert.ToInt32(reader["COD_DEPTO"]),
                                Text = Convert.ToString(reader["DESCRICAO"])
                            };

                            retorno.results.Add(obj);
                        }
                    }
                }
            }

            return retorno;
        }
    }
}
