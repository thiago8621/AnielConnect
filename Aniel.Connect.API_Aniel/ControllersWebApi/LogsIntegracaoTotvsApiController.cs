using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class LogsIntegracaoTotvsApiController : BaseApiController
    {
        /// <summary>
        /// Retorna todos os logs dos Centro de Custos vindo do Totvs registradas  no banco do Aniel 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/LogsIntegracaoTotvs/GetLogCentroCusto")]
        public async Task<ApiResponse<LogsTotvsResponse>> GetLogCentroCusto(LogsTotvsRequest request)
        {
            try
            {
                List<LogsTotvsResponse> retorno = new List<LogsTotvsResponse>();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.COD_CENTRO_CUSTO, A.DESCRICAO, A.RETORNO, A.DATA_HORA, A.USUARIO ");
                    sql.AppendLine(" FROM TB_LOG_CENTRO_CUSTO_TOTVS A ");
                    sql.AppendLine(" WHERE A.ID_LOG IS NOT NULL ");
                    if (request != null)
                    {
                        if (request.DataInicio > DateTime.MinValue && request.DataFim > DateTime.MinValue && request.DataFim >= request.DataInicio)
                        {
                            sql.AppendLine(" AND CAST(A.DATA_HORA AS DATE) BETWEEN @DATAINICIO AND @DATAFIM ");
                            parametros.Add("@DATAINICIO", request.DataInicio);
                            parametros.Add("@DATAFIM", request.DataFim);
                        }

                        if (!string.IsNullOrEmpty(request.CCusto))
                        {
                            sql.AppendLine(" AND A.COD_CENTRO_CUSTO = @CCUSTO ");
                            parametros.Add("@CCUSTO", request.CCusto);
                        }
                    }

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new LogsTotvsResponse
                            {
                                CCusto = reader["COD_CENTRO_CUSTO"] != DBNull.Value ? Convert.ToString(reader["COD_CENTRO_CUSTO"]) : "",
                                Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "",
                                Retorno = reader["RETORNO"] != DBNull.Value ? Convert.ToString(reader["RETORNO"]) : "",
                                Usuario = reader["USUARIO"] != DBNull.Value ? Convert.ToString(reader["USUARIO"]) : "",
                                Data_Hora = reader["DATA_HORA"] != DBNull.Value ? Convert.ToString(reader["DATA_HORA"]) :""


                            });
                        }
                    }
                }

                return new ApiResponse<LogsTotvsResponse>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<LogsTotvsResponse>()
                {
                    error = e.Message,
                };
            }

        }

        /// <summary>
        /// Retorna todos os logs dos Materiais vindo do Totvs registradas no banco do Aniel 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/LogsIntegracaoTotvs/GetLogMaterial")]
        public async Task<ApiResponse<LogsTotvsMatResponse>> GetLogMaterial(LogsTotvsRequest request)
        {
            try
            {
                List<LogsTotvsMatResponse> retorno = new List<LogsTotvsMatResponse>();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.CODMAT, A.DESCRICAO,A.UNID,A.RETORNO, A.DATA_HORA, A.USUARIO ");
                    sql.AppendLine(" FROM TB_LOG_MATERIAL A ");
                    sql.AppendLine(" WHERE A.ID_LOG IS NOT NULL ");
                    if (request != null)
                    {
                        if (request.DataInicio > DateTime.MinValue && request.DataFim > DateTime.MinValue && request.DataFim >= request.DataInicio)
                        {
                            sql.AppendLine(" AND CAST(A.DATA_HORA AS DATE) BETWEEN @DATAINICIO AND @DATAFIM ");
                            parametros.Add("@DATAINICIO", request.DataInicio);
                            parametros.Add("@DATAFIM", request.DataFim);
                        }

                        if (!string.IsNullOrEmpty(request.CCusto))
                        {
                            sql.AppendLine(" AND A.COD_CENTRO_CUSTO = @CCUSTO ");
                            parametros.Add("@CCUSTO", request.CCusto);
                        }
                    }

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new LogsTotvsMatResponse
                            {
                                CodMat = reader["CODMAT"] != DBNull.Value ? Convert.ToString(reader["CODMAT"]) : "",
                                Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "",
                                Unidade = reader["UNID"] != DBNull.Value ? Convert.ToString(reader["UNID"]) : "",
                                Retorno = reader["RETORNO"] != DBNull.Value ? Convert.ToString(reader["RETORNO"]) : "",
                                Usuario = reader["USUARIO"] != DBNull.Value ? Convert.ToString(reader["USUARIO"]) : "",
                                Data_Hora = reader["DATA_HORA"] != DBNull.Value ? Convert.ToString(reader["DATA_HORA"]) : ""


                            });
                        }
                    }
                }

                return new ApiResponse<LogsTotvsMatResponse>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<LogsTotvsMatResponse>()
                {
                    error = e.Message,
                };
            }

        }

        /// <summary>
        /// Retorna todos os logs dos Recebimento vindo do Totvs registradas no banco do Aniel 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/LogsIntegracaoTotvs/GetLogRecebimento")]
        public async Task<ApiResponse<LogsTotvsRecResponse>> GetLogRecebimento(LogsTotvsRequest request)
        {
            try
            {
                List<LogsTotvsRecResponse> retorno = new List<LogsTotvsRecResponse>();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.PEDIDO, A.NOTA_FISCAL,A.CODCT,A.PROJETO,A.CODMAT,A.RETORNO,A.DATA_HORA, A.USUARIO ");
                    sql.AppendLine(" FROM TB_LOG_RECEBIMENTO A ");
                    sql.AppendLine(" WHERE A.ID_LOG IS NOT NULL ");
                    if (request != null)
                    {
                        if (request.DataInicio > DateTime.MinValue && request.DataFim > DateTime.MinValue && request.DataFim >= request.DataInicio)
                        {
                            sql.AppendLine(" AND CAST(A.DATA_HORA AS DATE) BETWEEN @DATAINICIO AND @DATAFIM ");
                            parametros.Add("@DATAINICIO", request.DataInicio);
                            parametros.Add("@DATAFIM", request.DataFim);
                        }

                        if (!string.IsNullOrEmpty(request.CCusto))
                        {
                            sql.AppendLine(" AND A.COD_CENTRO_CUSTO = @CCUSTO ");
                            parametros.Add("@CCUSTO", request.CCusto);
                        }
                    }

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new LogsTotvsRecResponse
                            {
                                Pedido = reader["PEDIDO"] != DBNull.Value ? Convert.ToString(reader["PEDIDO"]) : "",
                                Nota_Fiscal = reader["NOTA_FISCAL"] != DBNull.Value ? Convert.ToString(reader["NOTA_FISCAL"]) : "",
                                Codmat = reader["CODMAT"] != DBNull.Value ? Convert.ToString(reader["CODMAT"]) : "",
                                CodCt = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : "",
                                Projeto = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : "",
                                Retorno = reader["RETORNO"] != DBNull.Value ? Convert.ToString(reader["RETORNO"]) : "",
                                Usuario = reader["USUARIO"] != DBNull.Value ? Convert.ToString(reader["USUARIO"]) : "",
                                Data_Hora = reader["DATA_HORA"] != DBNull.Value ? Convert.ToString(reader["DATA_HORA"]) : ""


                            });
                        }
                    }
                }

                return new ApiResponse<LogsTotvsRecResponse>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<LogsTotvsRecResponse>()
                {
                    error = e.Message,
                };
            }

        }

        /// <summary>
        /// Retorna todos os logs dos Equipe vindo do Totvs registradas no banco do Aniel 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/LogsIntegracaoTotvs/GetLogEquipe")]
        public async Task<ApiResponse<LogsTotvsEqResponse>> GetLogEquipe(LogsTotvsRequest request)
        {
            try
            {
                List<LogsTotvsEqResponse> retorno = new List<LogsTotvsEqResponse>();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.EQUIPE, A.NOME,A.ENCERRAMENTO,A.RETORNO,A.USUARIO,A.DATA_HORA ");
                    sql.AppendLine(" FROM TB_LOG_EQUIPE_TOTVS A ");
                    sql.AppendLine(" WHERE A.ID_LOG IS NOT NULL ");
                    if (request != null)
                    {
                        if (request.DataInicio > DateTime.MinValue && request.DataFim > DateTime.MinValue && request.DataFim >= request.DataInicio)
                        {
                            sql.AppendLine(" AND CAST(A.DATA_HORA AS DATE) BETWEEN @DATAINICIO AND @DATAFIM ");
                            parametros.Add("@DATAINICIO", request.DataInicio);
                            parametros.Add("@DATAFIM", request.DataFim);
                        }

                        if (!string.IsNullOrEmpty(request.CCusto))
                        {
                            sql.AppendLine(" AND A.COD_CENTRO_CUSTO = @CCUSTO ");
                            parametros.Add("@CCUSTO", request.CCusto);
                        }
                    }

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new LogsTotvsEqResponse
                            {
                                Equipe = reader["EQUIPE"] != DBNull.Value ? Convert.ToString(reader["EQUIPE"]) : "",
                                Nome = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"]) : "",
                                Desligamento = reader["ENCERRAMENTO"] != DBNull.Value ? Convert.ToString(reader["ENCERRAMENTO"]) : "",                              
                                Retorno = reader["RETORNO"] != DBNull.Value ? Convert.ToString(reader["RETORNO"]) : "",
                                Usuario = reader["USUARIO"] != DBNull.Value ? Convert.ToString(reader["USUARIO"]) : "",
                                Data_Hora = reader["DATA_HORA"] != DBNull.Value ? Convert.ToString(reader["DATA_HORA"]) : ""

                            });
                        }
                    }
                }

                return new ApiResponse<LogsTotvsEqResponse>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<LogsTotvsEqResponse>()
                {
                    error = e.Message,
                };
            }

        }

        /// <summary>
        /// Retorna todos os logs dos Funcao vindo do Totvs registradas no banco do Aniel 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/LogsIntegracaoTotvs/GetLogFuncao")]
        public async Task<ApiResponse<LogsTotvsFcResponse>> GetLogFuncao(LogsTotvsRequest request)
        {
            try
            {
                List<LogsTotvsFcResponse> retorno = new List<LogsTotvsFcResponse>();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.CODFUNCAO, A.DESCRICAO,A.RETORNO,A.USUARIO,A.DATA_HORA ");
                    sql.AppendLine(" FROM TB_LOG_FUNCAO_TOTVS A ");
                    sql.AppendLine(" WHERE A.ID_LOG IS NOT NULL ");
                    if (request != null)
                    {
                        if (request.DataInicio > DateTime.MinValue && request.DataFim > DateTime.MinValue && request.DataFim >= request.DataInicio)
                        {
                            sql.AppendLine(" AND CAST(A.DATA_HORA AS DATE) BETWEEN @DATAINICIO AND @DATAFIM ");
                            parametros.Add("@DATAINICIO", request.DataInicio);
                            parametros.Add("@DATAFIM", request.DataFim);
                        }

                        if (!string.IsNullOrEmpty(request.CCusto))
                        {
                            sql.AppendLine(" AND A.COD_CENTRO_CUSTO = @CCUSTO ");
                            parametros.Add("@CCUSTO", request.CCusto);
                        }
                    }

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new LogsTotvsFcResponse
                            {
                                Cod_Func = reader["CODFUNCAO"] != DBNull.Value ? Convert.ToString(reader["CODFUNCAO"]) : "",
                                Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "",
                                Retorno = reader["RETORNO"] != DBNull.Value ? Convert.ToString(reader["RETORNO"]) : "",
                                Usuario = reader["USUARIO"] != DBNull.Value ? Convert.ToString(reader["USUARIO"]) : "",
                                Data_Hora = reader["DATA_HORA"] != DBNull.Value ? Convert.ToString(reader["DATA_HORA"]) : ""

                            });
                        }
                    }
                }

                return new ApiResponse<LogsTotvsFcResponse>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<LogsTotvsFcResponse>()
                {
                    error = e.Message,
                };
            }

        }

        /// <summary>
        /// Retorna todos os logs dos Funcao vindo do Totvs registradas no banco do Aniel 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/LogsIntegracaoTotvs/GetLogFuc")]
        public async Task<ApiResponse<LogsTotvsFuNCResponse>> GetLogFuc(LogsTotvsRequest request)
        {
            try
            {
                List<LogsTotvsFuNCResponse> retorno = new List<LogsTotvsFuNCResponse>();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.NUM_FRE, A.NOME,A.RETORNO,A.USUARIO,A.DATA_HORA ");
                    sql.AppendLine(" FROM TB_LOG_FUNCIONARIO A ");
                    sql.AppendLine(" WHERE A.ID_LOG IS NOT NULL ");
                    if (request != null)
                    {
                        if (request.DataInicio > DateTime.MinValue && request.DataFim > DateTime.MinValue && request.DataFim >= request.DataInicio)
                        {
                            sql.AppendLine(" AND CAST(A.DATA_HORA AS DATE) BETWEEN @DATAINICIO AND @DATAFIM ");
                            parametros.Add("@DATAINICIO", request.DataInicio);
                            parametros.Add("@DATAFIM", request.DataFim);
                        }

                        if (!string.IsNullOrEmpty(request.CCusto))
                        {
                            sql.AppendLine(" AND A.COD_CENTRO_CUSTO = @CCUSTO ");
                            parametros.Add("@CCUSTO", request.CCusto);
                        }
                    }

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new LogsTotvsFuNCResponse
                            {
                                Cod_Func = reader["NUM_FRE"] != DBNull.Value ? Convert.ToString(reader["NUM_FRE"]) : "",
                                Nome = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"]) : "",
                                Retorno = reader["RETORNO"] != DBNull.Value ? Convert.ToString(reader["RETORNO"]) : "",
                                Usuario = reader["USUARIO"] != DBNull.Value ? Convert.ToString(reader["USUARIO"]) : "",
                                Data_Hora = reader["DATA_HORA"] != DBNull.Value ? Convert.ToString(reader["DATA_HORA"]) : ""

                            });
                        }
                    }
                }

                return new ApiResponse<LogsTotvsFuNCResponse>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<LogsTotvsFuNCResponse>()
                {
                    error = e.Message,
                };
            }

        }


    }
}