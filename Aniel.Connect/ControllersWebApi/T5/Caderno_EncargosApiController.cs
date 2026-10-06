using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Caderno_EncargosApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Caderno_EncargosApi/GetCombo")]
        public async Task<ObjCombo<String>> GetComboDescricao(RequestObjComboPai<String, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_CADERNO_ENCARGOS A ");

                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.CODIGO = {request.id}");
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"A.UNIDADE_CUSTO = '{request.idpai}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 40)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.CODIGO) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.UNIDADE_CUSTO, A.CODIGO, A.DESCRICAO, A.UNIDADE, A.QUANTIDADE, A.CLASSE_MO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new Caderno_EncargosListaModel
                                {
                                    Unidade_Custo = Convert.ToString(reader["UNIDADE_CUSTO"]),
                                    Id = Convert.ToString(reader["CODIGO"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    Classe_MO = Convert.ToString(reader["CLASSE_MO"]),
                                    Quantidade = reader["QUANTIDADE"] != DBNull.Value ? Convert.ToDouble(reader["QUANTIDADE"]): 0,
                                    Unidade = Convert.ToString(reader["UNIDADE"])
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

        //[HttpPost()]
        //[Route("api/FilialApi/GetCombo")]
        //public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<String> request)
        //{
        //    ObjCombo<Int64> retorno = new ObjCombo<Int64>
        //    {
        //        results = new List<IObjComboResult<Int64>>()
        //    };

        //    using (ISession session = HibernateUtil.GetSession())
        //    {
        //        try
        //        {
        //            //Filtros 
        //            Dictionary<String, Object> parametros = new Dictionary<string, object>();

        //            StringBuilder sqlFrom = new StringBuilder("FROM TB_FILIAL A ");

        //            List<String> condicoes = new List<String>();                  

        //            if (!String.IsNullOrEmpty(request.id))
        //            {
        //                condicoes.Add($"ID = {request.id}");
        //            }

        //            if (request != null && !String.IsNullOrEmpty(request.q))
        //            {
        //                condicoes.Add($"( UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
        //            }

        //            if (condicoes.Count() > 0)
        //            {
        //                sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes) } ) ");
        //            }

        //            using (var com = session.Connection.CreateCommand())
        //            {
        //                StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
        //                sql.AppendLine(sqlFrom.ToString());

        //                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
        //                {
        //                    if (await reader.ReadAsync())
        //                        retorno.total_count = Convert.ToInt32(reader["CO"]);
        //                }
        //            }

        //            //Select
        //            using (var com = session.Connection.CreateCommand())
        //            {
        //                StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOME ");
        //                sql.AppendLine(sqlFrom.ToString());
        //                sql.AppendLine("ORDER BY UPPER(A.NOME)");

        //                using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var obj = new ObjComboResult<Int64>
        //                        {
        //                            Id = Convert.ToInt64(reader["ID"]),
        //                            Text = Convert.ToString(reader["NOME"])
        //                        };
        //                        retorno.results.Add(obj);
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }
        //    return retorno;

        //}

        //[HttpPost()]
        //[Route("api/FilialApi/GetComboFilialAniel")]
        //public async Task<ObjCombo<Int64>> GetComboFilialAniel(RequestObjComboPai<String,String> request)
        //{
        //    ObjCombo<Int64> retorno = new ObjCombo<Int64>
        //    {
        //        results = new List<IObjComboResult<Int64>>()
        //    };

        //    using (ISession session = HibernateUtil.GetSessionT5())
        //    {
        //        try
        //        {
        //            //Filtros 
        //            Dictionary<String, Object> parametros = new Dictionary<string, object>();

        //            StringBuilder sqlFrom = new StringBuilder("FROM TB_FILIAL A ");

        //            List<String> condicoes = new List<String>();

        //            if (!String.IsNullOrEmpty(request.id))
        //            {
        //                condicoes.Add($"CODFILIAL = {request.id}");
        //            }
        //            if (!String.IsNullOrEmpty(request.idpai))
        //            {
        //                condicoes.Add($"CODEMPRESA = {request.idpai}");
        //            }

        //            if (request != null && !String.IsNullOrEmpty(request.q))
        //            {
        //                condicoes.Add($"( UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
        //            }

        //            if (condicoes.Count() > 0)
        //            {
        //                sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes) } ) ");
        //            }

        //            using (var com = session.Connection.CreateCommand())
        //            {
        //                StringBuilder sql = new StringBuilder("SELECT COUNT(A.CODFILIAL) AS CO ");
        //                sql.AppendLine(sqlFrom.ToString());

        //                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
        //                {
        //                    if (await reader.ReadAsync())
        //                        retorno.total_count = Convert.ToInt32(reader["CO"]);
        //                }
        //            }

        //            //Select
        //            using (var com = session.Connection.CreateCommand())
        //            {
        //                StringBuilder sql = new StringBuilder("SELECT A.CODFILIAL, A.NOME ");
        //                sql.AppendLine(sqlFrom.ToString());
        //                sql.AppendLine("ORDER BY UPPER(A.NOME)");

        //                using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var obj = new ObjComboResult<Int64>
        //                        {
        //                            Id = Convert.ToInt64(reader["CODFILIAL"]),
        //                            Text = Convert.ToString(reader["NOME"])
        //                        };
        //                        retorno.results.Add(obj);
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }
        //    return retorno;

        //}


    }
}
