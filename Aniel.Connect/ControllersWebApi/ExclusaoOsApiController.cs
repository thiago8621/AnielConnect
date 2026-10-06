using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aniel.Connect.Models;
using System.Web.Http;
using System.Text;
using NHibernate;
using Sinapse.DAO.repository;
using System.Threading.Tasks;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using Sinapse.Util;
using System.ComponentModel.DataAnnotations;
using Sinapse.Util.Excel;
using System.Reflection;
using Aniel.Connect.Models.T5;
using Microsoft.AspNet.Identity;
using System.Net;



namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class ExclusaoOsApiController : BaseApiController
    {
        List<string> erros = new List<string>();
        public async Task<List<String>> ExcluiOrdemServico(List<ExclusaoServicoPlanilha> listDocProd)
        {

            foreach (ExclusaoServicoPlanilha _docprod in listDocProd)
            {
                if(!String.IsNullOrEmpty(_docprod.DataCriacao))
                {
                    DateTime parsedDate;
                    bool isValidDate = DateTime.TryParse(_docprod.DataCriacao, out parsedDate);

                    if (!isValidDate)
                    {
                        erros.Add($"O campo DATA CRIAÇÃO da O.S {_docprod.OrdemServico}, na planilha, não está em um formato de data/hora válido.");
                        continue;
                    }
                }
                await ExcluiOrdemServico(_docprod);
            }
            return erros;
        }

        private async Task ExcluiOrdemServico(ExclusaoServicoPlanilha docProd)
        {
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                string _obs = null;
                string _temOs = "N";
                string numObra = null;
                string dataMaxima = null;
                string usuarioCriacao = null;
                string diaCriacao = null;
                string horaCriacao = null;

                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                sql.AppendLine(" SELECT A.NUM_OBRA,A.DATA_MAXIMA,A.USUARIO,A.DIA,A.HORA,COALESCE(A.STATUS_EXECUCAO,0)AS STATUS_EXECUCAO,");
                sql.AppendLine("(SELECT FIRST 1 B.CODMAT FROM TB_MATERIAL_TAREFA B");
                sql.AppendLine("WHERE B.CODCT = @CODCT AND B.PROJETO = @PROJETO");
                sql.AppendLine("AND B.NUM_OBRA = A.NUM_OBRA AND B.NUM_DOC = @NUM_DOC) AS MATERIAL");
                sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A");
                sql.AppendLine(" WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA_ORIGINAL = @NUM_OBRA_ORIGINAL ");
                sql.AppendLine(" AND A.NUM_DOC = @NUM_DOC AND A.DIA = @DIA ");

                parametros.Add("@CODCT", docProd.Contrato);
                parametros.Add("@PROJETO", docProd.Projeto);
                parametros.Add("@NUM_OBRA_ORIGINAL", docProd.OrdemServico);
                parametros.Add("@NUM_DOC", docProd.IdentificacaoCliente);
                parametros.Add("@DIA", docProd.DataCriacao);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    _temOs = "N";
                    while (await reader.ReadAsync())
                    {
                        _temOs = "S";
                        numObra = Convert.ToString(reader["NUM_OBRA"]);
                        dataMaxima = Convert.ToString(reader["DATA_MAXIMA"]);
                        usuarioCriacao = Convert.ToString(reader["USUARIO"]);
                        diaCriacao = Convert.ToString(reader["DIA"]);
                        horaCriacao = Convert.ToString(reader["HORA"]);

                        var temMaterial = String.IsNullOrEmpty(Convert.ToString(reader["MATERIAL"])) == false;
                        var osFechada = Convert.ToInt64(reader["STATUS_EXECUCAO"]) == 1 || Convert.ToInt64(reader["STATUS_EXECUCAO"]) == 2 || Convert.ToInt64(reader["STATUS_EXECUCAO"]) == 8;
                        var osCancelada = Convert.ToInt64(reader["STATUS_EXECUCAO"]) == 9;

                        if (osFechada == false)
                        {
                            if (osCancelada == false)
                            {
                                if (temMaterial == false)
                                {
                                    using (var comExclusao = session.Connection.CreateCommand())
                                    {
                                        sql.Clear();
                                        sql.AppendLine(" DELETE FROM TB_CUSTOS_TAREFA ");
                                        sql.AppendLine(" WHERE CODCT = @CODCT AND PROJETO = @PROJETO ");
                                        sql.AppendLine(" AND NUM_OBRA =  @NUM_OBRA AND NUM_DOC = @NUM_DOC ");
                                        parametros.Add("@NUM_OBRA", numObra);
                                        await CommandHelper.ExecuteNonQuery(comExclusao, sql, parametros, session.Connection, null);

                                        //sql.Clear();
                                        //sql.AppendLine(" DELETE FROM TB_MATERIAL_TAREFA ");
                                        //sql.AppendLine(" WHERE CODCT = @CODCT AND PROJETO = @PROJETO ");
                                        //sql.AppendLine(" AND NUM_OBRA =  @NUM_OBRA AND NUM_DOC = @NUM_DOC ");
                                        //await CommandHelper.ExecuteNonQuery(comExclusao, sql, parametros, session.Connection, null);

                                        sql.Clear();
                                        sql.AppendLine(" DELETE FROM TB_DOCUMENTO_PRODUCAO ");
                                        sql.AppendLine(" WHERE CODCT = @CODCT AND PROJETO = @PROJETO ");
                                        sql.AppendLine(" AND NUM_OBRA =  @NUM_OBRA AND NUM_DOC = @NUM_DOC ");
                                        await CommandHelper.ExecuteNonQuery(comExclusao, sql, parametros, session.Connection, null);
                                    }
                                    _obs = $"A Ordem de Servico {numObra} foi excluída com Sucesso.";
                                    //erros.Add($"A Ordem de Servico {numObra} foi excluída com Sucesso.");
                                }
                                else
                                {
                                    _obs = $"A Ordem de Servico {numObra} possui lançamento de materiais.";
                                    erros.Add($"Ordem de Servico {numObra} possui lançamento de materiais.");
                                }
                            }
                            else
                            {
                                _obs = $"A Ordem de Servico {numObra} encontra-se cancelada.";
                                erros.Add($"Ordem de Servico {numObra} encontra-se cancelada.");
                            }
                        }
                        else
                        {
                            _obs = $"A Ordem de Servico {numObra} encontra-se fechada.";
                            erros.Add($"Ordem de Servico {numObra} encontra-se fechada.");
                        }


                        DocumentoProducaoLog pObjLogProd = new DocumentoProducaoLog();

                        pObjLogProd.codCt = docProd.Contrato;
                        pObjLogProd.projeto = docProd.Projeto;
                        pObjLogProd.numObra = numObra;
                        pObjLogProd.numObraOriginal = docProd.OrdemServico;
                        pObjLogProd.numDoc = docProd.IdentificacaoCliente;
                        pObjLogProd.dataMaxima = String.IsNullOrEmpty(dataMaxima) ? null : dataMaxima;
                        pObjLogProd.usuarioCriacao = usuarioCriacao;
                        pObjLogProd.diaCriacao = String.IsNullOrEmpty(diaCriacao) ? null : diaCriacao;
                        pObjLogProd.horaCriacao = String.IsNullOrEmpty(horaCriacao) ? null : horaCriacao;
                        pObjLogProd.usuarioExclusao = User.Identity.GetUserId();
                        pObjLogProd.nomeMaquina = Dns.GetHostName();
                        IPAddress[] ip = Dns.GetHostAddresses(pObjLogProd.nomeMaquina);
                        if (ip != null && ip.Length >= 10)
                        {
                            pObjLogProd.ipMaquina = ip[9].ToString();
                        }
                        pObjLogProd.obs = _obs;
                        await InsereLog(session, pObjLogProd);
                    }
                    if (_temOs == "N")
                    {
                        numObra = docProd.OrdemServico;
                        _obs = $"A Ordem de Servico {docProd.OrdemServico} não encontrada no sistema.";
                        erros.Add($"A Ordem de Servico {docProd.OrdemServico} não encontrada no sistema.");

                        DocumentoProducaoLog pObjLogProd = new DocumentoProducaoLog();

                        pObjLogProd.codCt = docProd.Contrato;
                        pObjLogProd.projeto = docProd.Projeto;
                        pObjLogProd.numObra = numObra;
                        pObjLogProd.numObraOriginal = docProd.OrdemServico;
                        pObjLogProd.numDoc = docProd.IdentificacaoCliente;
                        pObjLogProd.dataMaxima = String.IsNullOrEmpty(dataMaxima) ? null : dataMaxima;
                        pObjLogProd.usuarioCriacao = usuarioCriacao;
                        pObjLogProd.diaCriacao = String.IsNullOrEmpty(diaCriacao) ? null : diaCriacao;
                        pObjLogProd.horaCriacao = String.IsNullOrEmpty(horaCriacao) ? null : horaCriacao;
                        pObjLogProd.usuarioExclusao = User.Identity.GetUserId();
                        pObjLogProd.nomeMaquina = Dns.GetHostName();
                        IPAddress[] ip = Dns.GetHostAddresses(pObjLogProd.nomeMaquina);
                        if (ip != null && ip.Length >= 10)
                        {
                            pObjLogProd.ipMaquina = ip[9].ToString();
                        }
                        pObjLogProd.obs = _obs;
                        await InsereLog(session, pObjLogProd);
                    }
                }
            }
        }


        private async Task InsereLog(ISession session, DocumentoProducaoLog docProdLog)
        {

            using (var com = session.Connection.CreateCommand())
            {
                string _msn = null;
                try
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    sql.Clear();
                    sql.AppendLine(" INSERT INTO TB_LOG_EXCLUSAO_OS (CODCT,PROJETO,NUM_OBRA,NUM_OBRA_ORIGINAL,NUM_DOC,DATA_MAXIMA,USUARIO, ");
                    sql.AppendLine("                                 DIA,HORA,USUARIO_EXCLUSAO,DIA_EXCLUSAO,HORA_EXCLUSAO, ");
                    sql.AppendLine("                                 HOSTNAME,IP_MAQUINA_ORIGEM,OBS) ");
                    sql.AppendLine("             VALUES (@pCODCT,@pPROJETO,@pNUM_OBRA,@pNUM_OBRA_ORIGINAL,@pNUM_DOC,@pDATA_MAXIMA,@pUSUARIO, ");
                    sql.AppendLine("                     @pDIA,@pHORA,@pUSUARIO_EXCLUSAO,CURRENT_DATE,CURRENT_TIME, ");
                    sql.AppendLine("                     @pHOSTNAME,@pIP_MAQUINA_ORIGEM,@pOBS) ");
                    parametros.Clear();
                    parametros.Add("pCODCT", docProdLog.codCt);
                    parametros.Add("pPROJETO", docProdLog.projeto);
                    parametros.Add("pNUM_OBRA", docProdLog.numObra);
                    parametros.Add("pNUM_OBRA_ORIGINAL", docProdLog.numObraOriginal);
                    parametros.Add("pNUM_DOC", docProdLog.numDoc);
                    parametros.Add("pDATA_MAXIMA", Convert.ToDateTime(docProdLog.dataMaxima));
                    parametros.Add("pUSUARIO", docProdLog.usuarioCriacao);
                    parametros.Add("pDIA", Convert.ToDateTime(docProdLog.diaCriacao));
                    parametros.Add("pHORA", docProdLog.horaCriacao);
                    parametros.Add("pUSUARIO_EXCLUSAO", docProdLog.usuarioExclusao);
                    parametros.Add("pIP_MAQUINA_ORIGEM", docProdLog.ipMaquina);
                    parametros.Add("pHOSTNAME", docProdLog.nomeMaquina);
                    parametros.Add("pOBS", docProdLog.obs);
                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                }
                catch (Exception ex)
                {
                    _msn = ex.Message;
                }
            }
        }

        private async Task<Boolean?> OsFechada(ExclusaoServicoPlanilha docProd, string pNumObra)
        {
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                //return false;

                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT (COALESCE(STATUS_EXECUCAO,0))AS STATUS_EXECUCAO FROM TB_DOCUMENTO_PRODUCAO ");
                sql.AppendLine(" WHERE CODCT = @CODCT AND PROJETO = @PROJETO AND NUM_OBRA = @NUM_OBRA AND NUM_DOC = @NUM_DOC ");
                parametros.Add("@CODCT", docProd.Contrato);
                parametros.Add("@PROJETO", docProd.Projeto);
                parametros.Add("@NUM_OBRA", pNumObra);
                parametros.Add("@NUM_DOC", docProd.IdentificacaoCliente);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                        return Convert.ToInt64(reader["STATUS_EXECUCAO"]) == 1 || Convert.ToInt64(reader["STATUS_EXECUCAO"]) == 2;
                    else
                        return null;


                }
            }
        }
        [HttpPost]
        [Route("api/ExclusaoOsApi/Log")]
        public async Task<DataTableResponse<LogExclusaoOS>> Log(LogRequest request)
        {
            List<LogExclusaoOS> retorno = new List<LogExclusaoOS>();

            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {

                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT DIA_EXCLUSAO,HORA_EXCLUSAO,USUARIO_EXCLUSAO,CODCT,PROJETO,NUM_DOC,NUM_OBRA,NUM_OBRA_ORIGINAL, ");
                sql.AppendLine(" DATA_MAXIMA,USUARIO,DIA,HORA,HOSTNAME,IP_MAQUINA_ORIGEM,OBS ");
                sql.AppendLine(" FROM TB_LOG_EXCLUSAO_OS ");

                if (!String.IsNullOrEmpty(request.Data))
                {
                    parametros.Add("@pDIA", Convert.ToDateTime(request.Data));

                    sql.AppendLine(" WHERE DIA_EXCLUSAO = @pDIA ");
                }

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new LogExclusaoOS
                        {
                            data_exclusao = reader["DIA_EXCLUSAO"].ToString(),
                            hora_exclusao = reader["HORA_EXCLUSAO"].ToString(),
                            usuario_exclusao = reader["USUARIO_EXCLUSAO"].ToString(),
                            codct = reader["CODCT"].ToString(),
                            projeto = reader["PROJETO"].ToString(),
                            num_doc = reader["NUM_DOC"].ToString(),
                            num_obra = reader["NUM_OBRA"].ToString(),
                            num_obra_original = reader["NUM_OBRA_ORIGINAL"].ToString(),
                            data_maxima = reader["DATA_MAXIMA"].ToString(),
                            usuario_criacao = reader["USUARIO"].ToString(),
                            data_criacao = reader["DIA"].ToString(),
                            hora_criacao = reader["HORA"].ToString(),
                            nome_maquina = reader["HOSTNAME"].ToString(),
                            ip_maquina = reader["IP_MAQUINA_ORIGEM"].ToString(),
                            obs = reader["OBS"].ToString()
                        });
                    }
                }
            }

            return new DataTableResponse<LogExclusaoOS>
            {
                data = retorno
            };

        }

    }
}