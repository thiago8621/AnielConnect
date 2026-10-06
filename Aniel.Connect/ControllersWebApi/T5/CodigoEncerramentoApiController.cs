using Aniel.Connect.Core;
using Aniel.Connect.Models;
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

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class CodigoEncerramentoApiController : BaseApiController
    {
        [HttpGet()]
        [Route("api/Codigoencerramentoapi/GetList")]
        public async Task<GetApiRetorno<List<ObjBusca<String>>>> GetList()
        {
            GetApiRetorno<List<ObjBusca<String>>> retorno = new GetApiRetorno<List<ObjBusca<String>>>();

            retorno.Objeto = new List<ObjBusca<String>>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.COD_ENCERRAMENTO, A.DESCRICAO ");
                        sql.AppendLine("FROM TB_CODIGO_ENCERRAMENTO A ");
                        sql.AppendLine("ORDER BY UPPER(A.COD_ENCERRAMENTO)");


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjBusca<String>
                                {
                                    Value = Convert.ToString(reader["COD_ENCERRAMENTO"]),
                                    Label = $"{Convert.ToString(reader["COD_ENCERRAMENTO"])} - {Convert.ToString(reader["DESCRICAO"])}"
                                };
                                retorno.Objeto.Add(obj);
                            }
                        }

                        retorno.Ok = true;
                    }
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    throw ex;
                }
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/CodigoEncerramentoApi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboPai<String, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_CODIGO_ENCERRAMENTO A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.ATIVO = 'S' ");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.COD_ENCERRAMENTO) = '{request.id.ToUpperInvariant()}'");
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"UPPER(A.CODCT) = '{request.idpai.ToUpperInvariant()}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.COD_ENCERRAMENTO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 4)}%' OR " +
                                      $" UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.COD_ENCERRAMENTO) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.COD_ENCERRAMENTO,A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.COD_ENCERRAMENTO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["COD_ENCERRAMENTO"]),
                                    Text = $"{Convert.ToString(reader["COD_ENCERRAMENTO"])} - {Convert.ToString(reader["DESCRICAO"])}"
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