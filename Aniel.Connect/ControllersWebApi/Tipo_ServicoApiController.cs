using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
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
    public class Tipo_ServicoApiController : BaseApiController
    {

        [HttpGet()]
        [Route("api/Tipo_Servicoapi/GetList")]
        public async Task<GetApiRetorno<List<Tipo_ServicoListaModel>>> GetList(String CodCt)
        {
            GetApiRetorno<List<Tipo_ServicoListaModel>> retorno = new GetApiRetorno<List<Tipo_ServicoListaModel>>
            {
                Objeto = new List<Tipo_ServicoListaModel>()
            };

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


                        sql.AppendLine("SELECT A.COD_TIPO_SERV, A.DESCRICAO");
                        sql.AppendLine("  FROM TB_TIPO_SERVICO_EQUIPE A");

                        if (!String.IsNullOrEmpty(CodCt))
                        {
                            sql.AppendLine("WHERE A.CODCT = @CODCT");
                            parametros.Add("@CODCT", CodCt);
                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new Tipo_ServicoListaModel
                                {
                                    Id = Convert.ToInt32(reader["COD_TIPO_SERV"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"])
                                });
                            }
                        }
                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }

        [HttpGet()]
        [Route("api/Tipo_Servicoapi/GetListAgrupado")]
        public async Task<GetApiRetorno<List<Tipo_ServicoListaModel>>> GetListAgrupado(String CodCt)
        {
            GetApiRetorno<List<Tipo_ServicoListaModel>> retorno = new GetApiRetorno<List<Tipo_ServicoListaModel>>
            {
                Objeto = new List<Tipo_ServicoListaModel>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.DESCRICAO")
                        .AppendLine("  FROM TB_TIPO_SERVICO_EQUIPE A ");

                        if (!String.IsNullOrEmpty(CodCt))
                        {
                            sql.AppendLine("WHERE A.CODCT = @CODCT ");
                            parametros.Add("@CODCT", CodCt);
                        }

                        sql.AppendLine("GROUP BY A.DESCRICAO");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new Tipo_ServicoListaModel
                                {
                                    Descricao = Convert.ToString(reader["DESCRICAO"])
                                });
                            }
                        }
                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }


        [HttpGet()]
        [Route("api/Tipo_Servicoapi/GetDescricao")]
        public async Task<String> GetDescricao(Int16 cod_tipo_serv)
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


                        sql.AppendLine("SELECT A.DESCRICAO");
                        sql.AppendLine("FROM   TB_TIPO_SERVICO_EQUIPE A");
                        sql.AppendLine("WHERE A.COD_TIPO_SERV = @COD_TIPO_SERV");

                        parametros.Add("@COD_TIPO_SERV", cod_tipo_serv);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                return Convert.ToString(reader["DESCRICAO"]);
                            }
                            else
                                return String.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        [HttpGet()]
        [Route("api/Tipo_Servicoapi/busca")]
        public async Task<List<ObjBusca<Int64>>> busca(String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

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

                        sql.AppendLine("SELECT A.COD_TIPO_SERV, A.DESCRICAO");
                        sql.AppendLine("FROM TB_TIPO_SERVICO_EQUIPE A");
                        sql.AppendLine("WHERE (UPPER(A.DESCRICAO) LIKE @VALOR) ");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["COD_TIPO_SERV"]),
                                    Label = Convert.ToString(reader["DESCRICAO"])
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
        [Route("api/Tipo_Servicoapi/GetCombo")]
        public async Task<ObjCombo<Int32>> GetCombo(RequestObjComboPai<Int32?, String> request)
        {
            ObjCombo<Int32> retorno = new ObjCombo<Int32>
            {
                results = new List<IObjComboResult<Int32>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_TIPO_SERVICO_EQUIPE A ")
                                                .AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO B")
                                                .AppendLine("ON A.COD_NATUREZA = B.ID");

                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.cod_area))
                    {
                        condicoes.Add(" A.COD_NATUREZA = @COD_NATUREZA");
                        parametros.Add("@COD_NATUREZA", request.cod_area);
                    }

                    if (request.soativos)
                    {
                        condicoes.Add($"A.SITUACAO = 'S'");
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"UPPER(A.CODCT) = '{request.idpai.ToUpperInvariant()}'");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.COD_TIPO_SERV = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(DISTINCT A.COD_TIPO_SERV) AS CO ");
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
                        StringBuilder sql = new StringBuilder(" SELECT DISTINCT A.COD_TIPO_SERV, A.DESCRICAO, A.CODCT, B.DESCRICAO AS NATUREZA,A.COD_NATUREZA, B.TIPO_PRIORIDADE, B.ID_DEPARTAMENTO_PADRAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" GROUP BY A.COD_TIPO_SERV, A.DESCRICAO, A.CODCT, B.DESCRICAO, A.COD_NATUREZA, B.TIPO_PRIORIDADE, B.ID_DEPARTAMENTO_PADRAO ");
                        sql.AppendLine(" ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                long tipoPrioridade = 0;
                                if (reader["TIPO_PRIORIDADE"] != DBNull.Value) 
                                {
                                    Int64.TryParse(Convert.ToString(reader["TIPO_PRIORIDADE"]), out tipoPrioridade);
                                }

                                var obj = new Tipo_ServicoComboModel
                                {
                                    Id = Convert.ToInt32(reader["COD_TIPO_SERV"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    Natureza = Convert.ToString(reader["NATUREZA"]),
                                    CodNatureza = reader["COD_NATUREZA"] != DBNull.Value ? Convert.ToInt64(reader["COD_NATUREZA"]) : (long?)null,
                                    CodCt = Convert.ToString(reader["CODCT"]),
                                    PrioridadeNatureza = reader["TIPO_PRIORIDADE"] != DBNull.Value ? tipoPrioridade : (long?)null,
                                    IdDepartamentoPadrao = reader["ID_DEPARTAMENTO_PADRAO"] != DBNull.Value ? Convert.ToInt16(reader["ID_DEPARTAMENTO_PADRAO"]) : (Int16?)null,
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

        [HttpPost()]
        [Route("api/Tipo_Servicoapi/GetComboDescricao")]
        public async Task<ObjCombo<String>> GetComboDescricao(RequestObjComboPai<String, String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_TIPO_SERVICO_EQUIPE A ")
                                                .AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO B")
                                                .AppendLine("ON A.COD_NATUREZA = B.ID");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"SITUACAO = 'S'");
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"UPPER(A.CODCT) = '{request.idpai.ToUpperInvariant()}'");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.COD_TIPO_SERV = '{request.id}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.COD_TIPO_SERV) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT DISTINCT COD_TIPO_SERV,A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["COD_TIPO_SERV"]),
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

        [HttpPost()]
        [Route("api/Tipo_Servicoapi/GetComboTipoServicoNatureza")]
        public async Task<ObjCombo<String>> GetComboTipoServicoNatureza(RequestObjComboPai<String, String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_TIPO_SERVICO_EQUIPE A ")
                                                .AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO B")
                                                .AppendLine("ON A.COD_NATUREZA = B.ID");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"SITUACAO = 'S'");
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"UPPER(A.COD_NATUREZA) = '{request.idpai.ToUpperInvariant()}'");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.DESCRICAO = '{request.id}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.COD_TIPO_SERV) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.DESCRICAO,A.COD_TIPO_SERV ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["COD_TIPO_SERV"]),
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
