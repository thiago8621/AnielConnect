using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.ModelLog.enumerados;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using FluentNHibernate.Testing.Values;
using NHibernate.Hql.Ast;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    public class IntegracaoConfigIntegratorApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/IntegracaoConfigIntegratorApi/GetAllLog")]
        public async Task<DataTableResponse<IntegracaoConfigIntegratorLogListaModel>> GetAllLog(IntegracaoConfigLogModel request)
        {
            DataTableResponse<IntegracaoConfigIntegratorLogListaModel> retorno = new DataTableResponse<IntegracaoConfigIntegratorLogListaModel>();
            List<IntegracaoConfigIntegratorLogListaModel> listaLogs = new List<IntegracaoConfigIntegratorLogListaModel>();
            Int32 recordsTotal = 0;
            DateTime data;
            try
            {
                if (DateTime.TryParse(request.Data, out data) == false)
                {
                    throw new Exception("Data precisa ser preenchida para realizar busca");
                }

                if (String.IsNullOrEmpty(CONSTANTES.CaminhoLogIntegracaoConfigIntegrator))
                {
                    throw new Exception("Caminho de log não configurado na aplicação");
                }

                DirectoryInfo d = new DirectoryInfo(@$"{CONSTANTES.CaminhoLogIntegracaoConfigIntegrator}"); //Loading your Folder
                FileInfo[] Files = d.GetFiles().Where(p => p.LastWriteTime.Date == data.Date).ToArray(); //Getting log files

                foreach (FileInfo file in Files)
                {
                    listaLogs.Add(new IntegracaoConfigIntegratorLogListaModel
                    {
                        Data = file.LastWriteTime.ToString("dd/MM/yyyy HH:mm"),
                        DataHoraDateTime = file.LastWriteTime,
                        Nome = file.Name
                    });
                }

                retorno.data = listaLogs.OrderByDescending(p => p.DataHoraDateTime).ToList().Skip(request.start).Take(request.length);
                retorno.recordsFiltered = listaLogs.Count();
                retorno.draw = request.draw;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.draw = request.draw;
                retorno.data = new List<IntegracaoConfigIntegratorLogListaModel>();
                retorno.error = ex.Message;
                return retorno;
            }
        }

        [HttpPost()]
        [Route("api/IntegracaoConfigIntegratorApi/GetLogMessages")]
        public async Task<DataTableResponse<IntegracaoLogMensagensModels>> GetLogMessages(IntegracaoConfigLogModel request)
        {
            List<IntegracaoLogMensagensModels> listLogsMessages = new List<IntegracaoLogMensagensModels>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            Int32 recordsTotal = 0;

            try
            {
                using (var sessionLog = HibernateUtil.GetSessionLog())
                {
                    StringBuilder sqlFrom = new StringBuilder();

                    if (string.IsNullOrEmpty(request.Data))
                        throw new Exception("Data não informada.");


                    parametros.Add("@DATA", request.Data);


                    sqlFrom.AppendLine("FROM TB_LOG_INTEGRACAO_OS LOG ");
                    sqlFrom.AppendLine("WHERE LOG.DATA = @DATA ");

                    if (!string.IsNullOrEmpty(request.NumeroOrdemServico))
                    {
                        sqlFrom.AppendLine(" AND LOG.NUMERO_OS = @NUM_ORD ");
                        parametros.Add("@NUM_ORD", request.NumeroOrdemServico);
                    }
                    if (!string.IsNullOrEmpty(request.IdMomentoLog))
                    {
                        sqlFrom.AppendLine(" AND LOG.MOMENTO = @MOMENT ");
                        parametros.Add("@MOMENT", request.IdMomentoLog);
                    }
                    if (!string.IsNullOrEmpty(request.Status))
                    {
                        sqlFrom.AppendLine(" AND LOG.STATUS = @STATUS ");
                        parametros.Add("@STATUS", request.Status);
                    }

                    using (var com = sessionLog.Connection.CreateCommand())
                    {
                        StringBuilder sqlCount = new StringBuilder("SELECT COUNT(LOG.ID) CO ");
                        sqlCount.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, sessionLog.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }

                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT LOG.ID, LOG.NUMERO_OS, LOG.MENSAGEM, LOG.DATA AS DATA, LOG.HORA AS HORA, LOG.MOMENTO, LOG.STATUS AS STATUS, NUM_DOC,PROJETO,CODCT,JSON_RETORNO,JSON_ENVIO");
                    sql.AppendLine(sqlFrom.ToString());
                    using (var comm = sessionLog.Connection.CreateCommand())
                    {
                        using (var reader = await CommandHelper.ExecuteReaderFirst(comm, sql, parametros, sessionLog.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                listLogsMessages.Add(new IntegracaoLogMensagensModels
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    NumOrdemServico = reader["NUMERO_OS"] != DBNull.Value ? Convert.ToString(reader["NUMERO_OS"]) : "",
                                    Mensagem = reader["MENSAGEM"] != DBNull.Value ? Convert.ToString(reader["MENSAGEM"]) : "",
                                    Momento = reader["MOMENTO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<ModelLog.enumerados.MomentoImportacao>(Convert.ToString(reader["MOMENTO"])) : "",
                                    Situacao = reader["STATUS"] != DBNull.Value ? Convert.ToBoolean(reader["STATUS"]) : false,
                                    Data = reader["DATA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA"]).ToString("dd/MM/yyyy") : "",
                                    //NUM_DOC = reader["NUM_DOC"] != DBNull.Value ? Convert.ToInt32("NUM_DOC"):,
                                    NUM_DOC = reader["NUM_DOC"] != DBNull.Value ? Convert.ToString(reader["NUM_DOC"]) : "",

                                    Projeto = reader["Projeto"] != DBNull.Value ? Convert.ToString(reader["Projeto"]) : "",

                                    CodCt    = reader["CodCt"] != DBNull.Value ? Convert.ToString(reader["CodCt"]) : "",
                                    JsonEnvio = reader["Json_Envio"] != DBNull.Value ? Convert.ToString(reader["Json_Envio"]) : "",
                                   JsonRetorno = reader["Json_Retorno"] != DBNull.Value ? Convert.ToString(reader["Json_Retorno"]) : "",

                                    //Hora = reader["HORA"] != DBNull.Value ? Convert.ToDateTime(reader["HORA"]).ToString("hh:mm:ss") : ""
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new DataTableResponse<IntegracaoLogMensagensModels>
                {
                    draw = request.draw,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = listLogsMessages,
                    error = ex.Message
                };
            }

            //Page e retornando DataTable
            return new DataTableResponse<IntegracaoLogMensagensModels>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listLogsMessages,
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/IntegracaoConfigIntegratorApi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (var session = HibernateUtil.GetSessionLog())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_LOG_INTEGRACAO_OS SET STATUS = {value.status} WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }
    }
}