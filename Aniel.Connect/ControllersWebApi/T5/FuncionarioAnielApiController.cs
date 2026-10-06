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
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class FuncionarioAnielApiController : BaseApiController
    {


        [HttpGet()]
        [Route("api/FuncionarioAnielapi/buscaSupervisor")]
        public async Task<List<ObjBusca<String>>> buscaSupervisor(String term)
        {
            List<ObjBusca<String>> retorno = new List<ObjBusca<String>>();

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

                        sql.AppendLine("SELECT A.NUM_FRE, A.NOME");
                        sql.AppendLine("FROM TB_FUNCIONARIO A");
                        sql.AppendLine("WHERE (UPPER(A.NOME) LIKE @VALOR) AND SUPERVISOR = 'S' ");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<String>
                                {
                                    Value = Convert.ToString(reader["NUM_FRE"]),
                                    Label = Convert.ToString(reader["NOME"])
                                });
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

        [HttpPost()]
        [Route("api/FuncionarioAnielapi/GetComboSupervisor")]
        public async Task<ObjCombo<String>> GetComboSupervisor(RequestObjCombo<String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FUNCIONARIO A ");
                    sqlFrom.AppendLine("LEFT JOIN TB_EQUIPE B ON(A.NUM_FRE = B.NUM_FRE)");
                    List<String> condicoes = new List<String>();

                    condicoes.Add($"WHERE A.SUPERVISOR = 'S'");

                    if (request.soativos)
                    {
                        condicoes.Add($"A.SITUACAO IN (0,4)");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.NUM_FRE) = '{request.id.ToUpperInvariant()}'");
                    }


                        if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        //condicoes.Add($"( UPPER(A.NUM_FRE) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 6)}%' OR " +
                        //              $" UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                        condicoes.Add($"(UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" {String.Join(" AND ", condicoes) } ");
                    }


                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.NUM_FRE) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.NUM_FRE, A.NOME, B.EQUIPE ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResultSupervisorT5
                                {
                                    Id = Convert.ToString(reader["NUM_FRE"]),
                                    Text = Convert.ToString(reader["NOME"]),
                                    CodEquipe = reader["EQUIPE"] != DBNull.Value ? Convert.ToString(reader["EQUIPE"]) : null
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

        public async Task<byte[]> GetFotoFuncionario(String equipe)
        {
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


                        sql.AppendLine("SELECT A.FOTO ");
                        sql.AppendLine("FROM TB_FUNCIONARIO A ");
                        sql.AppendLine("INNER JOIN TB_EQUIPE B ");
                        sql.AppendLine("ON(A.NUM_FRE = B.NUM_FRE) ");
                        sql.AppendLine("WHERE B.EQUIPE = @EQUIPE");

                        parametros.Add("@EQUIPE", equipe);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                var fotoByte = reader["FOTO"] != DBNull.Value ? (Byte[])reader["FOTO"] : null;
                                return fotoByte;
                            }
                            else
                                return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex);
                    throw ex;
                }
            }
        }

    }
}