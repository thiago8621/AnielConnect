using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.ReincidenciaDocProd;
using Aniel.Connect.Repository.Usuario;
using Aniel.T5.DAO.map;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentNHibernate.Testing.Values;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.Win32;
using MySqlX.XDevAPI;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Hql.Ast;
using NHibernate.Util;
using OpenXmlPowerTools;
using Remotion.Linq.Clauses;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.SerasaApi.Server.Responses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Telegram.Bot.Types;
using Xceed.Words.NET;
using static Aniel.Connect.ConnectBusiness.T5.DocumentoProducaoAnielBusiness;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Dashboard_ServicosApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/RelatorioRankingTecnicos")]
        public async Task<DataTableResponse<RankingTecnicos_Model>> RelatorioRankingTecnicos(RankingTecnicos_DataTableRequest request)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            List<RankingTecnicos_Model> data = new List<RankingTecnicos_Model>();

            DateTime dataInicio = new DateTime(Convert.ToInt32(request.Ano), Convert.ToInt32(request.Mes) + 1, 1);
            DateTime dataFim = new DateTime(Convert.ToInt32(request.Ano), Convert.ToInt32(request.Mes) + 1, DateTime.DaysInMonth(Convert.ToInt32(request.Ano), Convert.ToInt32(request.Mes) + 1));

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    parametros.Add("DATA_INI", dataInicio);
                    parametros.Add("DATA_FIM", dataFim);

                    StringBuilder sqlCount = new StringBuilder();

                    StringBuilder sql = new StringBuilder("SELECT A.EQUIPE,A.NOME,B.NOME AS SUPERVISOR, C.NOME AS MONITOR, E.EMPRESA,F.NOME AS FILIAL, ");
                    sql.AppendLine(" G.DESCRICAO AS CENTRO_CUSTO, COUNT(H.NUM_DOC) AS CONTADOR ");


                    sql.Append(sqlCount);

                    StringBuilder sqlFrom = new StringBuilder();
                    sqlFrom.AppendLine(" FROM TB_EQUIPE A ");
                    sqlFrom.AppendLine(" LEFT JOIN TB_FUNCIONARIO B ");
                    sqlFrom.AppendLine(" ON A.COD_SUPERV = B.NUM_FRE ");

                    sqlFrom.AppendLine(" LEFT JOIN TB_SENHA C ");
                    sqlFrom.AppendLine(" ON A.USUARIO_MONITOR = C.USUARIO ");

                    sqlFrom.AppendLine(" INNER JOIN TB_FUNCIONARIO D ");
                    sqlFrom.AppendLine(" ON A.NUM_FRE = D.NUM_FRE ");

                    sqlFrom.AppendLine(" INNER JOIN TB_EMPRESA E  ");
                    sqlFrom.AppendLine(" ON D.CODEMPRESA = E.CODEMPRESA  ");

                    sqlFrom.AppendLine(" INNER JOIN TB_FILIAL F ");
                    sqlFrom.AppendLine(" ON D.CODEMPRESA = F.CODEMPRESA AND D.CODFILIAL = F.CODFILIAL  ");

                    sqlFrom.AppendLine(" INNER JOIN TB_CENTRO_CUSTO G  ");
                    sqlFrom.AppendLine(" ON D.COD_CCUSTO = G.COD_CCUSTO ");

                    sqlFrom.AppendLine(" LEFT JOIN TB_DOCUMENTO_PRODUCAO H ");
                    sqlFrom.AppendLine(" ON H.EQUIPE = A.EQUIPE ");

                    sqlFrom.AppendLine(" LEFT JOIN TB_CODIGO_ENCERRAMENTO I ");
                    sqlFrom.AppendLine(" ON H.COD_ENCERRAMENTO = I.COD_ENCERRAMENTO ");

                    sqlFrom.AppendLine(" LEFT JOIN TB_TIPO_SERVICO_EQUIPE J ");
                    sqlFrom.AppendLine(" ON J.COD_TIPO_SERV = H.COD_TIPO_SERV AND H.CODCT = J.CODCT ");


                    sql.Append(sqlFrom);

                    #region Where

                    StringBuilder where = new StringBuilder();
                    int filtroCount = 0;

                    where.AppendLine(" H.DATA_PROD BETWEEN @DATA_INI AND @DATA_FIM");

                    if (!string.IsNullOrEmpty(request.Empresa))
                    {
                        where.AppendLine(" AND D.CODEMPRESA = @EMPRESA ");
                        parametros.Add("@EMPRESA", request.Empresa);
                        filtroCount++;
                    }

                    if (!string.IsNullOrEmpty(request.Filial))
                    {
                        where.AppendLine(" AND D.CODFILIAL = @FILIAL ");
                        parametros.Add("@FILIAL", request.Filial);
                    }

                    if (!string.IsNullOrEmpty(request.Supervisor))
                    {
                        where.AppendLine(" AND A.COD_SUPERV = @SUPERVISOR ");
                        parametros.Add("@SUPERVISOR", request.Supervisor);
                    }

                    if (!string.IsNullOrEmpty(request.Monitor))
                    {
                        where.AppendLine(" AND C.NOME = @MONITOR ");
                        parametros.Add("@MONITOR", request.Monitor);
                    }

                    if (!string.IsNullOrEmpty(request.CentroCusto))
                    {
                        where.AppendLine(" AND D.COD_CCUSTO = @CENTRO ");
                        parametros.Add("@CENTRO", request.CentroCusto);
                    }

                    if (!string.IsNullOrEmpty(request.Uf))
                    {
                        where.AppendLine(" AND H.ENDERECO_UF = @UF ");
                        parametros.Add("@UF", request.Uf);
                    }

                    if (!string.IsNullOrEmpty(request.Cidade) && request.Cidade != "Selecione")
                    {
                        where.AppendLine(" AND H.ENDERECO_CIDADE = @CIDADE ");
                        parametros.Add("@CIDADE", request.Cidade);
                    }
                    
                    if (!string.IsNullOrEmpty(request.Natureza))
                    {
                        where.AppendLine(" AND J.COD_NATUREZA = @COD_NATUREZA ");
                        parametros.Add("@COD_NATUREZA", request.Natureza);
                    }
                   
                    if (!string.IsNullOrEmpty(request.GrupoArea))
                    {
                        where.AppendLine(" AND H.cod_escritorio = @GRUPO_AREA ");
                        parametros.Add("@GRUPO_AREA", request.GrupoArea);
                    }

                    if (!string.IsNullOrEmpty(request.Area))
                    {
                        where.AppendLine(" AND H.cod_area = @AREA ");
                        parametros.Add("@AREA", request.Area);
                    }

                    if (!string.IsNullOrEmpty(request.Localidade))
                    {
                        where.AppendLine(" AND H.cod_localidade = @LOCALIDADE ");
                        parametros.Add("@LOCALIDADE", request.Localidade);
                    }                   

                    if (request.Produtiva)
                    {
                        where.AppendLine(" AND I.STATUS_EXECUCAO = 2");
                    }
                    else
                    {
                        where.AppendLine(" AND I.STATUS_EXECUCAO = 1");
                        if (request.VisitaTecnica)
                            where.AppendLine(" AND I.EXIGE_CIENTE_LOCAL = 'S'");
                    }

                    if (filtroCount > 0 && string.IsNullOrEmpty(request.Empresa))
                        where.Remove(0, 5);

                    if (where.Length > 0)
                        sql.Append(" WHERE ");
                    sql.Append(where);

                   sql.AppendLine(@"
                    GROUP BY 
                        A.EQUIPE, 
                        A.NOME, 
                        B.NOME, 
                        C.NOME, 
                        E.EMPRESA, 
                        F.NOME, 
                        G.DESCRICAO
                    ");

                    switch (request.Agrupamento)
                    {
                        case "supervisor":
                            sql.Append($"ORDER BY SUPERVISOR, CONTADOR DESC");
                            break;

                        case "monitor":
                            sql.Append($"ORDER BY MONITOR, CONTADOR DESC");
                            break;

                        case "empresa":
                            sql.Append($"ORDER BY EMPRESA, CONTADOR DESC");
                            break;

                        case "centroCusto":
                            sql.Append($"ORDER BY CENTRO_CUSTO, CONTADOR DESC");
                            break;
                    }

                    #endregion

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            data.Add(new RankingTecnicos_Model
                            {
                                Matricula = Convert.ToString(reader["EQUIPE"]),
                                Tecnico = Convert.ToString(reader["NOME"]),
                                Supervisor = Convert.ToString(reader["SUPERVISOR"]),
                                Monitor = Convert.ToString(reader["MONITOR"]),
                                Empresa = Convert.ToString(reader["EMPRESA"]),
                                Filial = Convert.ToString(reader["FILIAL"]),
                                CentroCusto = Convert.ToString(reader["CENTRO_CUSTO"]),
                                QuantidadeOS = Convert.ToString(reader["CONTADOR"]),
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return new DataTableResponse<RankingTecnicos_Model>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = data.ToList().Count,
                data = data.ToList(),
                error = ""
            };
        }

        [HttpPost]
        [Route("api/Dashboard_Servicosapi/RelatorioMediaServicosExecutados")]
        public async Task<DataTableResponse<MediaServicos_Model>> RelatorioMediaServicosExecutados(RelatorioMediaServicoExecutadoRequest request)
        {
            List<MediaServicos_Model> data = new List<MediaServicos_Model>();
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sqlCount = new StringBuilder();

                    StringBuilder sql = new StringBuilder("SELECT A.EQUIPE, A.NOME, B.NOME AS SUPERVISOR, C.NOME AS MONITOR,");
                    sql.AppendLine("E.EMPRESA AS EMPRESA, F.NOME AS FILIAL, G.DESCRICAO AS CENTRO_CUSTO,");

                    StringBuilder sqlWhere = new StringBuilder();

                    parametros.Add("@DATA_INI", DateTime.Parse(request.Inicial).ToString("yyyy-MM-dd"));
                    parametros.Add("@DATA_FIM", DateTime.Parse(request.Final).ToString("yyyy-MM-dd"));

                    var config = await new Conexao<Configuracoes_Gerais>(session).selectFirst();
                    Boolean Exibe_RDOs = config.ExibeRDOs;

                    if (!string.IsNullOrEmpty(request.GrupoArea)) parametros.Add("@GRUPO_AREA", request.GrupoArea);
                    if (!string.IsNullOrEmpty(request.Area)) parametros.Add("@AREA", request.Area);
                    if (!string.IsNullOrEmpty(request.Localidade)) parametros.Add("@LOCALIDADE", request.Localidade);
                    if (!string.IsNullOrEmpty(request.TipoServico)) parametros.Add("@TIPO_SERV", request.TipoServico);
                    if (!string.IsNullOrEmpty(request.PlanoProduto)) parametros.Add("@ID_PLANO_PRODUTO", request.PlanoProduto);
                    if (!string.IsNullOrEmpty(request.Uf)) parametros.Add("@UF", request.Uf);
                    if (!string.IsNullOrEmpty(request.Cidade)) parametros.Add("@CIDADE", "%" + request.Cidade.ToUpperInvariant() + "%"); // Ex: "Campinas"
                    if (!string.IsNullOrEmpty(request.Natureza)) parametros.Add("@NATUREZA", request.Natureza);

                    sql.AppendLine("(SELECT COUNT(DISTINCT H.DATA_PROD) ");
                    sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO H ");
                    if (!String.IsNullOrEmpty(request.Natureza))
                    {
                        sql.AppendLine("INNER JOIN TB_TIPO_SERVICO_EQUIPE TSE ");
                        sql.AppendLine("ON(H.COD_TIPO_SERV = TSE.COD_TIPO_SERV) ");
                    }

                    sql.AppendLine("WHERE H.EQUIPE = A.EQUIPE ");
                    if (!String.IsNullOrEmpty(request.GrupoArea)) sql.AppendLine("AND H.cod_escritorio = @GRUPO_AREA ");
                    if (!String.IsNullOrEmpty(request.Area)) sql.AppendLine("AND H.cod_area = @AREA ");
                    if (!String.IsNullOrEmpty(request.Localidade)) sql.AppendLine("AND H.cod_localidade = @LOCALIDADE ");
                    sql.AppendLine("AND H.DATA_PROD BETWEEN @DATA_INI AND @DATA_FIM ");
                    if (!String.IsNullOrEmpty(request.TipoServico)) sql.AppendLine("AND H.cod_tipo_serv = @TIPO_SERV ");
                    if (!String.IsNullOrEmpty(request.PlanoProduto)) sql.AppendLine("AND H.id_planoproduto = @ID_PLANO_PRODUTO ");
                    if (!string.IsNullOrEmpty(request.Cidade)) sql.AppendLine(" AND UPPER(H.ENDERECO_CIDADE) LIKE @CIDADE ");
                    if (!string.IsNullOrEmpty(request.Uf)) sql.AppendLine(" AND H.ENDERECO_UF = @UF ");
                    if (!string.IsNullOrEmpty(request.Natureza)) sql.AppendLine(" AND TSE.COD_NATUREZA = @NATUREZA ");
                    if (!Exibe_RDOs) sql.AppendLine(" AND COALESCE(H.SIGLA_MODULO, '') <> 'MCO' ");

                    sql.AppendLine("AND H.STATUS_EXECUCAO = 2 ");
                    sql.AppendLine(") AS DIAS, ");

                    sql.AppendLine("(SELECT COUNT(H.codct) ");
                    sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO H ");
                    if (!String.IsNullOrEmpty(request.Natureza))
                    {
                        sql.AppendLine("INNER JOIN TB_TIPO_SERVICO_EQUIPE TSE ");
                        sql.AppendLine("ON(H.COD_TIPO_SERV = TSE.COD_TIPO_SERV) ");
                    }
                    sql.AppendLine("WHERE H.EQUIPE = A.EQUIPE ");
                    if (!String.IsNullOrEmpty(request.GrupoArea)) sql.AppendLine("AND H.cod_escritorio = @GRUPO_AREA ");
                    if (!String.IsNullOrEmpty(request.Area)) sql.AppendLine("AND H.cod_area = @AREA ");
                    if (!String.IsNullOrEmpty(request.Localidade)) sql.AppendLine("AND H.cod_localidade = @LOCALIDADE ");
                    sql.AppendLine("AND H.DATA_PROD BETWEEN @DATA_INI AND @DATA_FIM ");
                    if (!String.IsNullOrEmpty(request.TipoServico)) sql.AppendLine("AND H.cod_tipo_serv = @TIPO_SERV ");
                    if (!String.IsNullOrEmpty(request.PlanoProduto)) sql.AppendLine("AND H.id_planoproduto = @ID_PLANO_PRODUTO ");
                    if (!string.IsNullOrEmpty(request.Cidade)) sql.AppendLine(" AND UPPER(H.ENDERECO_CIDADE) LIKE @CIDADE ");
                    if (!string.IsNullOrEmpty(request.Uf)) sql.AppendLine(" AND H.ENDERECO_UF = @UF ");
                    if (!string.IsNullOrEmpty(request.Natureza)) sql.AppendLine(" AND TSE.COD_NATUREZA = @NATUREZA ");
                    if (!Exibe_RDOs) sql.AppendLine(" AND COALESCE(H.SIGLA_MODULO, '') <> 'MCO' ");

                    sql.AppendLine("AND H.STATUS_EXECUCAO = 2 ");

                    sql.AppendLine(") AS PRODUTIVOS FROM TB_EQUIPE A ");

                    sql.AppendLine("LEFT JOIN TB_FUNCIONARIO B ");
                    sql.AppendLine("ON(A.COD_SUPERV = B.NUM_FRE) ");
                    sql.AppendLine("LEFT JOIN TB_SENHA C ");
                    sql.AppendLine("ON(A.USUARIO_MONITOR = C.USUARIO) ");
                    sql.AppendLine("INNER JOIN TB_FUNCIONARIO D ");
                    sql.AppendLine("ON(A.NUM_FRE = D.NUM_FRE) ");
                    sql.AppendLine("INNER JOIN TB_EMPRESA E ");
                    sql.AppendLine("ON(D.CODEMPRESA = E.CODEMPRESA) ");
                    sql.AppendLine("INNER JOIN TB_FILIAL F ");
                    sql.AppendLine("ON(D.CODEMPRESA = F.CODEMPRESA AND D.CODFILIAL = F.CODFILIAL) ");
                    sql.AppendLine("INNER JOIN TB_CENTRO_CUSTO G ");
                    sql.AppendLine("ON(D.COD_CCUSTO = G.COD_CCUSTO) ");

                    #region where

                    int filtroCount = 0;

                    if (!string.IsNullOrEmpty(request.Empresa))
                    {
                        sqlWhere.AppendLine(" D.CODEMPRESA = @EMPRESA ");
                        parametros.Add("@EMPRESA", request.Empresa);
                        filtroCount++;
                    }

                    if (!string.IsNullOrEmpty(request.Filial))
                    {
                        sqlWhere.AppendLine(" AND D.CODFILIAL = @FILIAL ");
                        parametros.Add("@FILIAL", request.Filial);
                        filtroCount++;
                    }

                    if (!string.IsNullOrEmpty(request.Tecnico))
                    {
                        sqlWhere.AppendLine(" AND A.EQUIPE = @EQUIPE  ");
                        parametros.Add("@EQUIPE", request.Tecnico);
                        filtroCount++;
                    }

                    if (!string.IsNullOrEmpty(request.Supervisor))
                    {
                        sqlWhere.AppendLine(" AND A.COD_SUPERV = @SUPERVISOR  ");
                        parametros.Add("@SUPERVISOR", request.Supervisor);
                        filtroCount++;
                    }

                    if (filtroCount > 0 && string.IsNullOrEmpty(request.Empresa))
                        sqlWhere.Remove(0, 5);

                    if (sqlWhere.Length > 0)
                        sql.Append(" WHERE ");
                    sql.Append(sqlWhere);

                    switch (request.Agrupamento)
                    {
                        case "nenhum":
                            sql.Append("ORDER BY NOME ASC");
                            break;
                        case "supervisor":
                            sql.Append("ORDER BY SUPERVISOR, NOME ASC");
                            break;
                        case "empresa":
                            sql.Append("ORDER BY EMPRESA, NOME ASC");
                            break;
                        case "filial":
                            sql.Append("ORDER BY FILIAL, NOME ASC");
                            break;
                    }

                    #endregion

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new MediaServicos_Model
                            {
                                Matricula = Convert.ToString(reader["EQUIPE"]),
                                Tecnico = Convert.ToString(reader["NOME"]),
                                DiasTrabalhados = Convert.ToString(reader["DIAS"]),
                                ServicosProdutivos = Convert.ToString(reader["PRODUTIVOS"]),
                                Supervisor = Convert.ToString(reader["SUPERVISOR"]),
                                Empresa = Convert.ToString(reader["EMPRESA"]),
                                Filial = Convert.ToString(reader["FILIAL"]),
                            };

                            var media = Convert.ToDouble(obj.ServicosProdutivos) > 0 ? (Convert.ToDouble(obj.ServicosProdutivos) / Convert.ToDouble(obj.DiasTrabalhados)) : 0;

                            obj.Media = Math.Round(media, 2).ToString("F");

                            data.Add(obj);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return new DataTableResponse<MediaServicos_Model>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = data.ToList().Count,
                data = data.ToList(),
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetServicosSemAgendamento")]
        public async Task<GetApiRetorno<Dashboard_Servicos_Models>> GetServicosSemAgendamento()
        {

            GetApiRetorno<Dashboard_Servicos_Models> retorno = new GetApiRetorno<Dashboard_Servicos_Models>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                Boolean Exibe_RDOs = (await repoConfig.selectFirst()).ExibeRDOs;
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    //sql.AppendLine(" SELECT COUNT(A.CODCT) CO FROM TB_DOCUMENTO_PRODUCAO A ");

                    //sql.AppendLine(" WHERE A.USUARIO = @USUARIO ");
                    //sql.AppendLine(" AND A.STATUS_EXECUCAO = '4' ");


                    sql.AppendLine(" SELECT COUNT(A.CODCT) CO ");

                    sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql.AppendLine(" LEFT JOIN TB_EQUIPE B ");
                    sql.AppendLine(" ON(A.EQUIPE = B.EQUIPE) ");
                    sql.AppendLine(" LEFT JOIN TB_ESCRITORIO C ");
                    sql.AppendLine(" ON(C.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = C.COD_ESCRITORIO) ");
                    sql.AppendLine(" LEFT JOIN TB_AREA D ");
                    sql.AppendLine(" ON(D.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA) ");
                    sql.AppendLine(" LEFT JOIN TB_LOCALIDADE E ");
                    sql.AppendLine(" ON(E.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND ");
                    sql.AppendLine(" A.COD_LOCALIDADE = E.COD_LOCALIDADE) ");
                    sql.AppendLine(" INNER JOIN TB_TIPO_SERVICO_EQUIPE F ");
                    sql.AppendLine(" ON(A.COD_TIPO_SERV = F.COD_TIPO_SERV) ");
                    sql.AppendLine(" LEFT JOIN TB_SUB_TIPO_SERVICO G ");
                    sql.AppendLine(" ON(A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV) ");
                    sql.AppendLine(" LEFT JOIN TB_CLIENTE H ");
                    sql.AppendLine(" ON(A.ID_CLIENTE = H.COD_CLIENTE) ");
                    sql.AppendLine(" LEFT JOIN TB_CODIGO_ENCERRAMENTO I ");
                    sql.AppendLine(" ON(I.CODCT = A.CODCT AND A.COD_ENCERRAMENTO = I.COD_ENCERRAMENTO) ");
                    sql.AppendLine(" LEFT JOIN TB_TIPO_IMOVEL J ");
                    sql.AppendLine(" ON(A.PARPRIM_PARA = J.COD_TIPO_IMOVEL) ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO K ");
                    sql.AppendLine(" ON(K.NUM_FRE = B.COD_SUPERV) ");
                    sql.AppendLine(" LEFT JOIN TB_NATUREZA_PRODUCAO L ");
                    sql.AppendLine(" ON(L.ID = A.NATUREZA) ");
                    sql.AppendLine(" WHERE COALESCE(A.SEQ, 1) = (SELECT FIRST 1 COALESCE(MAX(M.SEQ), 1) FROM TB_DOCUMENTO_PRODUCAO M ");
                    sql.AppendLine("  WHERE A.CODCT = M.CODCT AND A.PROJETO = M.PROJETO AND A.NUM_OBRA_ORIGINAL = M.NUM_OBRA_ORIGINAL ");
                    sql.AppendLine("   AND A.NUM_DOC = M.NUM_DOC ) ");
                    sql.AppendLine(" AND A.STATUS_EXECUCAO = '4' ");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    //parametros.Add("@USUARIO", User.Identity.GetUserId());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto = new Dashboard_Servicos_Models
                            {
                                Servicos_Sem_Agendamento = Convert.ToString(reader["CO"])
                            };
                        }
                    }
                    retorno.Ok = true;

                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetServicosNaoIniciados")]
        public async Task<GetApiRetorno<Dashboard_Servicos_Models>> GetServicosNaoIniciados()
        {

            GetApiRetorno<Dashboard_Servicos_Models> retorno = new GetApiRetorno<Dashboard_Servicos_Models>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                Boolean Exibe_RDOs = (await repoConfig.selectFirst()).ExibeRDOs;
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    //sql.AppendLine(" SELECT COUNT(CODCT) CO FROM TB_DOCUMENTO_PRODUCAO ");

                    //sql.AppendLine(" WHERE USUARIO = @USUARIO ");
                    //sql.AppendLine(" AND STATUS_EXECUCAO = '0' ");

                    sql.AppendLine(" SELECT COUNT(A.CODCT) CO ");

                    sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql.AppendLine(" LEFT JOIN TB_EQUIPE B ");
                    sql.AppendLine(" ON(A.EQUIPE = B.EQUIPE) ");
                    sql.AppendLine(" LEFT JOIN TB_ESCRITORIO C ");
                    sql.AppendLine(" ON(C.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = C.COD_ESCRITORIO) ");
                    sql.AppendLine(" LEFT JOIN TB_AREA D ");
                    sql.AppendLine(" ON(D.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA) ");
                    sql.AppendLine(" LEFT JOIN TB_LOCALIDADE E ");
                    sql.AppendLine(" ON(E.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND ");
                    sql.AppendLine(" A.COD_LOCALIDADE = E.COD_LOCALIDADE) ");
                    sql.AppendLine(" INNER JOIN TB_TIPO_SERVICO_EQUIPE F ");
                    sql.AppendLine(" ON(A.COD_TIPO_SERV = F.COD_TIPO_SERV) ");
                    sql.AppendLine(" LEFT JOIN TB_SUB_TIPO_SERVICO G ");
                    sql.AppendLine(" ON(A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV) ");
                    sql.AppendLine(" LEFT JOIN TB_CLIENTE H ");
                    sql.AppendLine(" ON(A.ID_CLIENTE = H.COD_CLIENTE) ");
                    sql.AppendLine(" LEFT JOIN TB_CODIGO_ENCERRAMENTO I ");
                    sql.AppendLine(" ON(I.CODCT = A.CODCT AND A.COD_ENCERRAMENTO = I.COD_ENCERRAMENTO) ");
                    sql.AppendLine(" LEFT JOIN TB_TIPO_IMOVEL J ");
                    sql.AppendLine(" ON(A.PARPRIM_PARA = J.COD_TIPO_IMOVEL) ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO K ");
                    sql.AppendLine(" ON(K.NUM_FRE = B.COD_SUPERV) ");
                    sql.AppendLine(" LEFT JOIN TB_NATUREZA_PRODUCAO L ");
                    sql.AppendLine(" ON(L.ID = A.NATUREZA) ");

                    sql.AppendLine(" WHERE COALESCE(A.SEQ, 1) = (SELECT FIRST 1 COALESCE(MAX(M.SEQ), 1) FROM TB_DOCUMENTO_PRODUCAO M ");
                    sql.AppendLine("  WHERE A.CODCT = M.CODCT AND A.PROJETO = M.PROJETO AND A.NUM_OBRA_ORIGINAL = M.NUM_OBRA_ORIGINAL ");
                    sql.AppendLine("   AND A.NUM_DOC = M.NUM_DOC ) ");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    //sql.AppendLine(" AND A.USUARIO = @USUARIO ");
                    sql.AppendLine(" AND A.STATUS_EXECUCAO = '0' ");

                    //parametros.Add("@USUARIO", User.Identity.GetUserId());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {

                            retorno.Objeto = new Dashboard_Servicos_Models
                            {
                                Servicos_Nao_Iniciados = Convert.ToString(reader["CO"])
                            };

                        }
                    }
                }
                retorno.Ok = true;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetServicosNaoRoteirizados")]
        public async Task<GetApiRetorno<Dashboard_Servicos_Models>> GetServicosNaoRoteirizados()
        {

            GetApiRetorno<Dashboard_Servicos_Models> retorno = new GetApiRetorno<Dashboard_Servicos_Models>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                Boolean Exibe_RDOs = (await repoConfig.selectFirst()).ExibeRDOs;
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT COUNT(CODCT) CO FROM TB_DOCUMENTO_PRODUCAO ");
                    sql.AppendLine("WHERE DATA_MAXIMA IS NOT NULL");
                    sql.AppendLine("AND HORA_MAXIMA IS NOT NULL");
                    sql.AppendLine("AND EQUIPE IS NULL");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(SIGLA_MODULO, '') <> 'MCO' ");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {

                            retorno.Objeto = new Dashboard_Servicos_Models
                            {
                                Servicos_Nao_Roteirizados = Convert.ToString(reader["CO"])
                            };

                        }
                    }
                }
                retorno.Ok = true;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetServicosParalisados")]
        public async Task<GetApiRetorno<Dashboard_Servicos_Models>> GetServicosParalisados()
        {

            GetApiRetorno<Dashboard_Servicos_Models> retorno = new GetApiRetorno<Dashboard_Servicos_Models>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    //sql.AppendLine(" SELECT COUNT(CODCT) CO FROM TB_DOCUMENTO_PRODUCAO ");

                    //sql.AppendLine(" WHERE USUARIO = @USUARIO ");
                    //sql.AppendLine(" AND STATUS_EXECUCAO = '10' ");


                    sql.AppendLine(" SELECT COUNT(A.CODCT) CO ");

                    sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                    //sql.AppendLine(" LEFT JOIN TB_EQUIPE B ");
                    //sql.AppendLine(" ON(A.EQUIPE = B.EQUIPE) ");
                    //sql.AppendLine(" LEFT JOIN TB_ESCRITORIO C ");
                    //sql.AppendLine(" ON(C.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = C.COD_ESCRITORIO) ");
                    //sql.AppendLine(" LEFT JOIN TB_AREA D ");
                    //sql.AppendLine(" ON(D.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA) ");
                    //sql.AppendLine(" LEFT JOIN TB_LOCALIDADE E ");
                    //sql.AppendLine(" ON(E.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND ");
                    //sql.AppendLine(" A.COD_LOCALIDADE = E.COD_LOCALIDADE) ");
                    //sql.AppendLine(" INNER JOIN TB_TIPO_SERVICO_EQUIPE F ");
                    //sql.AppendLine(" ON(A.COD_TIPO_SERV = F.COD_TIPO_SERV) ");
                    //sql.AppendLine(" LEFT JOIN TB_SUB_TIPO_SERVICO G ");
                    //sql.AppendLine(" ON(A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV) ");
                    //sql.AppendLine(" LEFT JOIN TB_CLIENTE H ");
                    //sql.AppendLine(" ON(A.ID_CLIENTE = H.COD_CLIENTE) ");
                    //sql.AppendLine(" LEFT JOIN TB_CODIGO_ENCERRAMENTO I ");
                    //sql.AppendLine(" ON(I.CODCT = A.CODCT AND A.COD_ENCERRAMENTO = I.COD_ENCERRAMENTO) ");
                    //sql.AppendLine(" LEFT JOIN TB_TIPO_IMOVEL J ");
                    //sql.AppendLine(" ON(A.PARPRIM_PARA = J.COD_TIPO_IMOVEL) ");
                    //sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO K ");
                    //sql.AppendLine(" ON(K.NUM_FRE = B.COD_SUPERV) ");
                    //sql.AppendLine(" LEFT JOIN TB_NATUREZA_PRODUCAO L ");
                    //sql.AppendLine(" ON(L.ID = A.NATUREZA) ");
                    //sql.AppendLine(" WHERE  COALESCE(A.SIGLA_MODULO, '') <> 'MCO' AND COALESCE(A.SEQ, 1) = (SELECT FIRST 1 COALESCE(MAX(M.SEQ), 1) FROM TB_DOCUMENTO_PRODUCAO M ");
                    //sql.AppendLine("  WHERE A.CODCT = M.CODCT AND A.PROJETO = M.PROJETO AND A.NUM_OBRA_ORIGINAL = M.NUM_OBRA_ORIGINAL ");
                    //sql.AppendLine("   AND A.NUM_DOC = M.NUM_DOC ) ");

                    //sql.AppendLine(" AND A.USUARIO = @USUARIO ");
                    sql.AppendLine(" WHERE A.STATUS_EXECUCAO = '10' ");

                    //parametros.Add("@USUARIO", User.Identity.GetUserId());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {

                            retorno.Objeto = new Dashboard_Servicos_Models
                            {
                                Servicos_Paralisados = Convert.ToString(reader["CO"])
                            };

                        }
                    }
                }
                retorno.Ok = true;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetServicosEmExecucao")]
        public async Task<GetApiRetorno<Dashboard_Servicos_Models>> GetServicosEmExecucao()
        {

            GetApiRetorno<Dashboard_Servicos_Models> retorno = new GetApiRetorno<Dashboard_Servicos_Models>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                Boolean Exibe_RDOs = (await repoConfig.selectFirst()).ExibeRDOs;

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    //sql.AppendLine(" SELECT COUNT(CODCT) CO FROM TB_DOCUMENTO_PRODUCAO ");

                    //sql.AppendLine(" WHERE USUARIO = @USUARIO ");
                    //sql.AppendLine(" AND STATUS_EXECUCAO IN ('3','11') ");


                    sql.AppendLine(" SELECT COUNT(A.CODCT) CO ");

                    sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql.AppendLine(" LEFT JOIN TB_EQUIPE B ");
                    sql.AppendLine(" ON(A.EQUIPE = B.EQUIPE) ");
                    sql.AppendLine(" LEFT JOIN TB_ESCRITORIO C ");
                    sql.AppendLine(" ON(C.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = C.COD_ESCRITORIO) ");
                    sql.AppendLine(" LEFT JOIN TB_AREA D ");
                    sql.AppendLine(" ON(D.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA) ");
                    sql.AppendLine(" LEFT JOIN TB_LOCALIDADE E ");
                    sql.AppendLine(" ON(E.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND ");
                    sql.AppendLine(" A.COD_LOCALIDADE = E.COD_LOCALIDADE) ");
                    sql.AppendLine(" INNER JOIN TB_TIPO_SERVICO_EQUIPE F ");
                    sql.AppendLine(" ON(A.COD_TIPO_SERV = F.COD_TIPO_SERV) ");
                    sql.AppendLine(" LEFT JOIN TB_SUB_TIPO_SERVICO G ");
                    sql.AppendLine(" ON(A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV) ");
                    sql.AppendLine(" LEFT JOIN TB_CLIENTE H ");
                    sql.AppendLine(" ON(A.ID_CLIENTE = H.COD_CLIENTE) ");
                    sql.AppendLine(" LEFT JOIN TB_CODIGO_ENCERRAMENTO I ");
                    sql.AppendLine(" ON(I.CODCT = A.CODCT AND A.COD_ENCERRAMENTO = I.COD_ENCERRAMENTO) ");
                    sql.AppendLine(" LEFT JOIN TB_TIPO_IMOVEL J ");
                    sql.AppendLine(" ON(A.PARPRIM_PARA = J.COD_TIPO_IMOVEL) ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO K ");
                    sql.AppendLine(" ON(K.NUM_FRE = B.COD_SUPERV) ");
                    sql.AppendLine(" LEFT JOIN TB_NATUREZA_PRODUCAO L ");
                    sql.AppendLine(" ON(L.ID = A.NATUREZA) ");
                    sql.AppendLine(" WHERE  COALESCE(A.SEQ, 1) = (SELECT FIRST 1 COALESCE(MAX(M.SEQ), 1) FROM TB_DOCUMENTO_PRODUCAO M ");
                    sql.AppendLine("  WHERE A.CODCT = M.CODCT AND A.PROJETO = M.PROJETO AND A.NUM_OBRA_ORIGINAL = M.NUM_OBRA_ORIGINAL ");
                    sql.AppendLine("   AND A.NUM_DOC = M.NUM_DOC ) ");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    //sql.AppendLine(" AND A.USUARIO = @USUARIO ");
                    sql.AppendLine(" AND A.STATUS_EXECUCAO IN(3,11) ");

                    parametros.Add("@USUARIO", User.Identity.GetUserId());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {

                            retorno.Objeto = new Dashboard_Servicos_Models
                            {
                                Servicos_Em_Execucao = Convert.ToString(reader["CO"])
                            };

                        }
                    }
                }
                retorno.Ok = true;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetServicosForaPrazo")]
        public async Task<GetApiRetorno<Dashboard_Servicos_Models>> GetServicosForaPrazo()
        {

            GetApiRetorno<Dashboard_Servicos_Models> retorno = new GetApiRetorno<Dashboard_Servicos_Models>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                Boolean Exibe_RDOs = (await repoConfig.selectFirst()).ExibeRDOs;
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {



                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.NUM_OBRA) AS CO FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql.AppendLine(" WHERE A.DATA_INDICADOR < CURRENT_TIMESTAMP ");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    //parametros.Add("@USUARIO", User.Identity.GetUserId());


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        var retorno_contagem = 0;

                        if (await reader.ReadAsync())
                        {
                            retorno_contagem = Convert.ToInt32(reader["CO"]);
                        }
                        retorno.Objeto = new Dashboard_Servicos_Models
                        {
                            Servicos_Fora_Prazo = Convert.ToString(retorno_contagem)
                        };

                    }

                }
                retorno.Ok = true;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetServicosEncerrados")]
        public async Task<GetApiRetorno<Dashboard_Servicos_Models>> GetServicosEncerrados()
        {

            GetApiRetorno<Dashboard_Servicos_Models> retorno = new GetApiRetorno<Dashboard_Servicos_Models>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                Boolean Exibe_RDOs = (await repoConfig.selectFirst()).ExibeRDOs;

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    //sql.AppendLine(" SELECT COUNT(CODCT) CO FROM TB_DOCUMENTO_PRODUCAO ");

                    //sql.AppendLine(" WHERE USUARIO = @USUARIO ");
                    //sql.AppendLine(" AND DATA_PROD IS NOT NULL ");

                    sql.AppendLine(" SELECT COUNT(A.CODCT) CO ");

                    sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql.AppendLine(" LEFT JOIN TB_EQUIPE B ");
                    sql.AppendLine(" ON(A.EQUIPE = B.EQUIPE) ");
                    sql.AppendLine(" LEFT JOIN TB_ESCRITORIO C ");
                    sql.AppendLine(" ON(C.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = C.COD_ESCRITORIO) ");
                    sql.AppendLine(" LEFT JOIN TB_AREA D ");
                    sql.AppendLine(" ON(D.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA) ");
                    sql.AppendLine(" LEFT JOIN TB_LOCALIDADE E ");
                    sql.AppendLine(" ON(E.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND ");
                    sql.AppendLine(" A.COD_LOCALIDADE = E.COD_LOCALIDADE) ");
                    sql.AppendLine(" INNER JOIN TB_TIPO_SERVICO_EQUIPE F ");
                    sql.AppendLine(" ON(A.COD_TIPO_SERV = F.COD_TIPO_SERV) ");
                    sql.AppendLine(" LEFT JOIN TB_SUB_TIPO_SERVICO G ");
                    sql.AppendLine(" ON(A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV) ");
                    sql.AppendLine(" LEFT JOIN TB_CLIENTE H ");
                    sql.AppendLine(" ON(A.ID_CLIENTE = H.COD_CLIENTE) ");
                    sql.AppendLine(" LEFT JOIN TB_CODIGO_ENCERRAMENTO I ");
                    sql.AppendLine(" ON(I.CODCT = A.CODCT AND A.COD_ENCERRAMENTO = I.COD_ENCERRAMENTO) ");
                    sql.AppendLine(" LEFT JOIN TB_TIPO_IMOVEL J ");
                    sql.AppendLine(" ON(A.PARPRIM_PARA = J.COD_TIPO_IMOVEL) ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO K ");
                    sql.AppendLine(" ON(K.NUM_FRE = B.COD_SUPERV) ");
                    sql.AppendLine(" LEFT JOIN TB_NATUREZA_PRODUCAO L ");
                    sql.AppendLine(" ON(L.ID = A.NATUREZA) ");
                    sql.AppendLine(" WHERE COALESCE(A.SEQ, 1) = (SELECT FIRST 1 COALESCE(MAX(M.SEQ), 1) FROM TB_DOCUMENTO_PRODUCAO M ");
                    sql.AppendLine("  WHERE A.CODCT = M.CODCT AND A.PROJETO = M.PROJETO AND A.NUM_OBRA_ORIGINAL = M.NUM_OBRA_ORIGINAL ");
                    sql.AppendLine("   AND A.NUM_DOC = M.NUM_DOC ) ");
                    sql.AppendLine(" AND A.DATA_PROD IS NOT NULL ");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {

                            retorno.Objeto = new Dashboard_Servicos_Models
                            {
                                Servicos_Encerrados = Convert.ToString(reader["CO"])
                            };

                        }
                    }
                }
                retorno.Ok = true;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetProdutividadeMes")]
        public async Task<GetApiRetorno<Dashboard_Servicos_Models>> GetProdutividadeMes()
        {

            GetApiRetorno<Dashboard_Servicos_Models> retorno = new GetApiRetorno<Dashboard_Servicos_Models>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {

                Boolean Exibe_RDOs = (await repoConfig.selectFirst()).ExibeRDOs;

                Int32 servicos_atual = 0;
                Int32 prod_atual = 0;
                Int32 servicos_anterior = 0;
                Int32 prod_anterior = 0;

                DateTime data_atual = DateTime.Now;
                DateTime _data = data_atual;

                for (int i = data_atual.Day; i > 1; i--)
                {
                    _data = _data.AddDays(-1);
                }

                DateTime data_inicio_mes = new DateTime(_data.Year, _data.Month, _data.Day, 0, 0, 0);

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    //sql.AppendLine(" SELECT COUNT(A.CODCT) CO FROM TB_DOCUMENTO_PRODUCAO A");
                    //sql.AppendLine(" WHERE A.USUARIO = @USUARIO ");
                    //sql.AppendLine(" AND A.DATA_PROD BETWEEN @DATA_INCIO_MES AND @DATA_ATUAL ");
                    //sql.AppendLine(" AND A.STATUS_EXECUCAO = '2' ");

                    sql.AppendLine(" SELECT COUNT(A.CODCT) CO ");

                    sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql.AppendLine(" LEFT JOIN TB_EQUIPE B ");
                    sql.AppendLine(" ON(A.EQUIPE = B.EQUIPE) ");
                    sql.AppendLine(" LEFT JOIN TB_ESCRITORIO C ");
                    sql.AppendLine(" ON(C.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = C.COD_ESCRITORIO) ");
                    sql.AppendLine(" LEFT JOIN TB_AREA D ");
                    sql.AppendLine(" ON(D.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA) ");
                    sql.AppendLine(" LEFT JOIN TB_LOCALIDADE E ");
                    sql.AppendLine(" ON(E.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND ");
                    sql.AppendLine(" A.COD_LOCALIDADE = E.COD_LOCALIDADE) ");
                    sql.AppendLine(" INNER JOIN TB_TIPO_SERVICO_EQUIPE F ");
                    sql.AppendLine(" ON(A.COD_TIPO_SERV = F.COD_TIPO_SERV) ");
                    sql.AppendLine(" LEFT JOIN TB_SUB_TIPO_SERVICO G ");
                    sql.AppendLine(" ON(A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV) ");
                    sql.AppendLine(" LEFT JOIN TB_CLIENTE H ");
                    sql.AppendLine(" ON(A.ID_CLIENTE = H.COD_CLIENTE) ");
                    sql.AppendLine(" LEFT JOIN TB_CODIGO_ENCERRAMENTO I ");
                    sql.AppendLine(" ON(I.CODCT = A.CODCT AND A.COD_ENCERRAMENTO = I.COD_ENCERRAMENTO) ");
                    sql.AppendLine(" LEFT JOIN TB_TIPO_IMOVEL J ");
                    sql.AppendLine(" ON(A.PARPRIM_PARA = J.COD_TIPO_IMOVEL) ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO K ");
                    sql.AppendLine(" ON(K.NUM_FRE = B.COD_SUPERV) ");
                    sql.AppendLine(" LEFT JOIN TB_NATUREZA_PRODUCAO L ");
                    sql.AppendLine(" ON(L.ID = A.NATUREZA) ");
                    sql.AppendLine(" WHERE  COALESCE(A.SEQ, 1) = (SELECT FIRST 1 COALESCE(MAX(M.SEQ), 1) FROM TB_DOCUMENTO_PRODUCAO M ");
                    sql.AppendLine("  WHERE A.CODCT = M.CODCT AND A.PROJETO = M.PROJETO AND A.NUM_OBRA_ORIGINAL = M.NUM_OBRA_ORIGINAL ");
                    sql.AppendLine("   AND A.NUM_DOC = M.NUM_DOC ) ");
                    sql.AppendLine(" AND A.DATA_PROD BETWEEN @DATA_INCIO_MES AND @DATA_ATUAL ");
                    sql.AppendLine(" AND A.STATUS_EXECUCAO = '2' ");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    parametros.Add("@DATA_ATUAL", data_atual);
                    parametros.Add("@DATA_INCIO_MES", data_inicio_mes);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            prod_atual = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                using (var com1 = sessionT5.Connection.CreateCommand())
                {
                    //Serviços Encerrados Totais

                    StringBuilder sql_servicos_atual = new StringBuilder();
                    //sql_servicos_atual.AppendLine(" SELECT COUNT(A.CODCT) CO FROM TB_DOCUMENTO_PRODUCAO A");
                    //sql_servicos_atual.AppendLine(" WHERE A.USUARIO = @USUARIO ");
                    //sql_servicos_atual.AppendLine(" AND A.DATA_PROD BETWEEN @DATA_INCIO_MES AND @DATA_ATUAL ");
                    //sql_servicos_atual.AppendLine(" AND A.STATUS_EXECUCAO IN ('1','2') ");

                    sql_servicos_atual.AppendLine(" SELECT COUNT(A.CODCT) CO ");

                    sql_servicos_atual.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql_servicos_atual.AppendLine(" LEFT JOIN TB_EQUIPE B ");
                    sql_servicos_atual.AppendLine(" ON(A.EQUIPE = B.EQUIPE) ");
                    sql_servicos_atual.AppendLine(" LEFT JOIN TB_ESCRITORIO C ");
                    sql_servicos_atual.AppendLine(" ON(C.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = C.COD_ESCRITORIO) ");
                    sql_servicos_atual.AppendLine(" LEFT JOIN TB_AREA D ");
                    sql_servicos_atual.AppendLine(" ON(D.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA) ");
                    sql_servicos_atual.AppendLine(" LEFT JOIN TB_LOCALIDADE E ");
                    sql_servicos_atual.AppendLine(" ON(E.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND ");
                    sql_servicos_atual.AppendLine(" A.COD_LOCALIDADE = E.COD_LOCALIDADE) ");
                    sql_servicos_atual.AppendLine(" INNER JOIN TB_TIPO_SERVICO_EQUIPE F ");
                    sql_servicos_atual.AppendLine(" ON(A.COD_TIPO_SERV = F.COD_TIPO_SERV) ");
                    sql_servicos_atual.AppendLine(" LEFT JOIN TB_SUB_TIPO_SERVICO G ");
                    sql_servicos_atual.AppendLine(" ON(A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV) ");
                    sql_servicos_atual.AppendLine(" LEFT JOIN TB_CLIENTE H ");
                    sql_servicos_atual.AppendLine(" ON(A.ID_CLIENTE = H.COD_CLIENTE) ");
                    sql_servicos_atual.AppendLine(" LEFT JOIN TB_CODIGO_ENCERRAMENTO I ");
                    sql_servicos_atual.AppendLine(" ON(I.CODCT = A.CODCT AND A.COD_ENCERRAMENTO = I.COD_ENCERRAMENTO) ");
                    sql_servicos_atual.AppendLine(" LEFT JOIN TB_TIPO_IMOVEL J ");
                    sql_servicos_atual.AppendLine(" ON(A.PARPRIM_PARA = J.COD_TIPO_IMOVEL) ");
                    sql_servicos_atual.AppendLine(" LEFT JOIN TB_FUNCIONARIO K ");
                    sql_servicos_atual.AppendLine(" ON(K.NUM_FRE = B.COD_SUPERV) ");
                    sql_servicos_atual.AppendLine(" LEFT JOIN TB_NATUREZA_PRODUCAO L ");
                    sql_servicos_atual.AppendLine(" ON(L.ID = A.NATUREZA) ");
                    sql_servicos_atual.AppendLine(" WHERE COALESCE(A.SEQ, 1) = (SELECT FIRST 1 COALESCE(MAX(M.SEQ), 1) FROM TB_DOCUMENTO_PRODUCAO M ");
                    sql_servicos_atual.AppendLine("  WHERE A.CODCT = M.CODCT AND A.PROJETO = M.PROJETO AND A.NUM_OBRA_ORIGINAL = M.NUM_OBRA_ORIGINAL ");
                    sql_servicos_atual.AppendLine("   AND A.NUM_DOC = M.NUM_DOC ) ");

                    sql_servicos_atual.AppendLine(" AND A.DATA_PROD BETWEEN @DATA_INCIO_MES AND @DATA_ATUAL ");
                    sql_servicos_atual.AppendLine(" AND A.STATUS_EXECUCAO IN(1,2) ");

                    if (!Exibe_RDOs)
                        sql_servicos_atual.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    using (var reader1 = await CommandHelper.ExecuteReader(com1, sql_servicos_atual, parametros, sessionT5.Connection, null))
                    {
                        if (await reader1.ReadAsync())
                        {
                            servicos_atual = Convert.ToInt32(reader1["CO"]);
                        }
                    }

                }

                using (var com2 = sessionT5.Connection.CreateCommand())
                {
                    //Serviços produtivos do Mês anterior

                    StringBuilder sql_prod_anterior = new StringBuilder();

                    //sql_prod_anterior.AppendLine(" SELECT COUNT(A.CODCT) CO FROM TB_DOCUMENTO_PRODUCAO A");
                    //sql_prod_anterior.AppendLine(" WHERE A.USUARIO = @USUARIO ");
                    //sql_prod_anterior.AppendLine(" AND A.DATA_PROD BETWEEN @DATA_INCIO_ANTERIOR AND @DATA_FIM_ANTERIOR ");
                    //sql_prod_anterior.AppendLine(" AND A.STATUS_EXECUCAO = '2' ");


                    sql_prod_anterior.AppendLine(" SELECT COUNT(A.CODCT) CO ");

                    sql_prod_anterior.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql_prod_anterior.AppendLine(" LEFT JOIN TB_EQUIPE B ");
                    sql_prod_anterior.AppendLine(" ON(A.EQUIPE = B.EQUIPE) ");
                    sql_prod_anterior.AppendLine(" LEFT JOIN TB_ESCRITORIO C ");
                    sql_prod_anterior.AppendLine(" ON(C.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = C.COD_ESCRITORIO) ");
                    sql_prod_anterior.AppendLine(" LEFT JOIN TB_AREA D ");
                    sql_prod_anterior.AppendLine(" ON(D.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA) ");
                    sql_prod_anterior.AppendLine(" LEFT JOIN TB_LOCALIDADE E ");
                    sql_prod_anterior.AppendLine(" ON(E.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND ");
                    sql_prod_anterior.AppendLine(" A.COD_LOCALIDADE = E.COD_LOCALIDADE) ");
                    sql_prod_anterior.AppendLine(" INNER JOIN TB_TIPO_SERVICO_EQUIPE F ");
                    sql_prod_anterior.AppendLine(" ON(A.COD_TIPO_SERV = F.COD_TIPO_SERV) ");
                    sql_prod_anterior.AppendLine(" LEFT JOIN TB_SUB_TIPO_SERVICO G ");
                    sql_prod_anterior.AppendLine(" ON(A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV) ");
                    sql_prod_anterior.AppendLine(" LEFT JOIN TB_CLIENTE H ");
                    sql_prod_anterior.AppendLine(" ON(A.ID_CLIENTE = H.COD_CLIENTE) ");
                    sql_prod_anterior.AppendLine(" LEFT JOIN TB_CODIGO_ENCERRAMENTO I ");
                    sql_prod_anterior.AppendLine(" ON(I.CODCT = A.CODCT AND A.COD_ENCERRAMENTO = I.COD_ENCERRAMENTO) ");
                    sql_prod_anterior.AppendLine(" LEFT JOIN TB_TIPO_IMOVEL J ");
                    sql_prod_anterior.AppendLine(" ON(A.PARPRIM_PARA = J.COD_TIPO_IMOVEL) ");
                    sql_prod_anterior.AppendLine(" LEFT JOIN TB_FUNCIONARIO K ");
                    sql_prod_anterior.AppendLine(" ON(K.NUM_FRE = B.COD_SUPERV) ");
                    sql_prod_anterior.AppendLine(" LEFT JOIN TB_NATUREZA_PRODUCAO L ");
                    sql_prod_anterior.AppendLine(" ON(L.ID = A.NATUREZA) ");
                    sql_prod_anterior.AppendLine(" WHERE COALESCE(A.SEQ, 1) = (SELECT FIRST 1 COALESCE(MAX(M.SEQ), 1) FROM TB_DOCUMENTO_PRODUCAO M ");
                    sql_prod_anterior.AppendLine("  WHERE A.CODCT = M.CODCT AND A.PROJETO = M.PROJETO AND A.NUM_OBRA_ORIGINAL = M.NUM_OBRA_ORIGINAL ");
                    sql_prod_anterior.AppendLine("   AND A.NUM_DOC = M.NUM_DOC ) ");

                    sql_prod_anterior.AppendLine(" AND A.DATA_PROD BETWEEN @DATA_INCIO_ANTERIOR AND @DATA_FIM_ANTERIOR ");
                    sql_prod_anterior.AppendLine(" AND A.STATUS_EXECUCAO = '2' ");

                    if (!Exibe_RDOs)
                        sql_prod_anterior.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    parametros.Add("@DATA_FIM_ANTERIOR", data_inicio_mes.AddMinutes(-1));
                    parametros.Add("@DATA_INCIO_ANTERIOR", data_inicio_mes.AddMonths(-1));

                    using (var reader2 = await CommandHelper.ExecuteReader(com2, sql_prod_anterior, parametros, sessionT5.Connection, null))
                    {
                        while (await reader2.ReadAsync())
                        {
                            prod_anterior = Convert.ToInt32(reader2["CO"]);
                        }
                    }

                }

                using (var com3 = sessionT5.Connection.CreateCommand())
                {
                    //Serviços Encerrados Totais Mês Anterior

                    StringBuilder sql_servicos_anterior = new StringBuilder();


                    //sql_servicos_anterior.AppendLine(" SELECT COUNT(A.CODCT) CO FROM TB_DOCUMENTO_PRODUCAO A");
                    //sql_servicos_anterior.AppendLine(" WHERE A.USUARIO = @USUARIO ");
                    //sql_servicos_anterior.AppendLine(" AND A.DATA_PROD BETWEEN @DATA_INCIO_ANTERIOR AND @DATA_FIM_ANTERIOR ");
                    //sql_servicos_anterior.AppendLine(" AND A.STATUS_EXECUCAO IN ('1','2') ");

                    sql_servicos_anterior.AppendLine(" SELECT COUNT(A.CODCT) CO ");

                    sql_servicos_anterior.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql_servicos_anterior.AppendLine(" LEFT JOIN TB_EQUIPE B ");
                    sql_servicos_anterior.AppendLine(" ON(A.EQUIPE = B.EQUIPE) ");
                    sql_servicos_anterior.AppendLine(" LEFT JOIN TB_ESCRITORIO C ");
                    sql_servicos_anterior.AppendLine(" ON(C.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = C.COD_ESCRITORIO) ");
                    sql_servicos_anterior.AppendLine(" LEFT JOIN TB_AREA D ");
                    sql_servicos_anterior.AppendLine(" ON(D.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA) ");
                    sql_servicos_anterior.AppendLine(" LEFT JOIN TB_LOCALIDADE E ");
                    sql_servicos_anterior.AppendLine(" ON(E.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND ");
                    sql_servicos_anterior.AppendLine(" A.COD_LOCALIDADE = E.COD_LOCALIDADE) ");
                    sql_servicos_anterior.AppendLine(" INNER JOIN TB_TIPO_SERVICO_EQUIPE F ");
                    sql_servicos_anterior.AppendLine(" ON(A.COD_TIPO_SERV = F.COD_TIPO_SERV) ");
                    sql_servicos_anterior.AppendLine(" LEFT JOIN TB_SUB_TIPO_SERVICO G ");
                    sql_servicos_anterior.AppendLine(" ON(A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV) ");
                    sql_servicos_anterior.AppendLine(" LEFT JOIN TB_CLIENTE H ");
                    sql_servicos_anterior.AppendLine(" ON(A.ID_CLIENTE = H.COD_CLIENTE) ");
                    sql_servicos_anterior.AppendLine(" LEFT JOIN TB_CODIGO_ENCERRAMENTO I ");
                    sql_servicos_anterior.AppendLine(" ON(I.CODCT = A.CODCT AND A.COD_ENCERRAMENTO = I.COD_ENCERRAMENTO) ");
                    sql_servicos_anterior.AppendLine(" LEFT JOIN TB_TIPO_IMOVEL J ");
                    sql_servicos_anterior.AppendLine(" ON(A.PARPRIM_PARA = J.COD_TIPO_IMOVEL) ");
                    sql_servicos_anterior.AppendLine(" LEFT JOIN TB_FUNCIONARIO K ");
                    sql_servicos_anterior.AppendLine(" ON(K.NUM_FRE = B.COD_SUPERV) ");
                    sql_servicos_anterior.AppendLine(" LEFT JOIN TB_NATUREZA_PRODUCAO L ");
                    sql_servicos_anterior.AppendLine(" ON(L.ID = A.NATUREZA) ");
                    sql_servicos_anterior.AppendLine(" WHERE COALESCE(A.SEQ, 1) = (SELECT FIRST 1 COALESCE(MAX(M.SEQ), 1) FROM TB_DOCUMENTO_PRODUCAO M ");
                    sql_servicos_anterior.AppendLine("  WHERE A.CODCT = M.CODCT AND A.PROJETO = M.PROJETO AND A.NUM_OBRA_ORIGINAL = M.NUM_OBRA_ORIGINAL ");
                    sql_servicos_anterior.AppendLine("   AND A.NUM_DOC = M.NUM_DOC ) ");
                    sql_servicos_anterior.AppendLine(" AND A.DATA_PROD BETWEEN @DATA_INCIO_ANTERIOR AND @DATA_FIM_ANTERIOR ");
                    sql_servicos_anterior.AppendLine(" AND A.STATUS_EXECUCAO IN(1,2) ");

                    if (!Exibe_RDOs)
                        sql_servicos_anterior.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    using (var reader3 = await CommandHelper.ExecuteReader(com3, sql_servicos_anterior, parametros, sessionT5.Connection, null))
                    {
                        if (await reader3.ReadAsync())
                        {
                            servicos_anterior = Convert.ToInt32(reader3["CO"]);
                        }
                    }

                }
                var _obj = new Dashboard_Servicos_Models();

                _obj.Produtividade_Mes = (servicos_atual == 0)
                                        ?
                                            "0"
                                        :
                                            String.Format("{0}%", Convert.ToString(Math.Round((Convert.ToDouble(prod_atual) / Convert.ToDouble(servicos_atual)) * 100, 2)));



                if ((servicos_atual != 0) && (servicos_anterior != 0)) //removi o && 
                {
                    _obj.Seta_Prod_Mes = ((Convert.ToDouble(prod_atual) / Convert.ToDouble(servicos_atual)) < (Convert.ToDouble(prod_anterior) / Convert.ToDouble(servicos_anterior)))
                                    ?
                                        "#FF0000" // Vermelho
                                    :
                                        "#22B14C"; // Verde
                }

                else if (servicos_anterior == 0 && servicos_atual != 0)
                {
                    _obj.Seta_Prod_Mes = "#22B14C"; // Verde
                }


                //_obj.Seta_Prod_Mes = "#FF0000";

                retorno.Objeto = _obj;

            }
            retorno.Ok = true;
            return retorno;
        }

        public async Task<GetApiRetorno<Double>> GetCustoOS(String CodCt, String Projeto, String Num_Obra, Int32? Num_Doc)
        {
            GetApiRetorno<Double> retorno = new GetApiRetorno<Double>();


            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var material_api = new Material_TarefaApiController())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                Boolean Exibe_RDOs = (await repoConfig.selectFirst()).ExibeRDOs;
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                String SIGLA_MODULO = await material_api.GetSiglaModulo(sessionT5, Num_Obra, Num_Doc.Value, Projeto, CodCt);
                String CHAVE = material_api.GetCampoChave(SIGLA_MODULO);

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT SUM(TOTAL) AS TOTAL ")
                    .AppendLine("FROM( ")

                    .AppendLine("SELECT COALESCE(SUM(A.QUANT_PROD * B.QUANTIDADE * C.RVALOR_FAT), 0) AS TOTAL ")
                    .AppendLine("FROM TB_CUSTOS_TAREFA A ")
                    .AppendLine("LEFT JOIN TB_CADERNO_ENCARGOS B ")
                    .AppendLine("ON(A.UNIDADE_CUSTO = B.UNIDADE_CUSTO AND B.CODIGO = A.CODIGO) ")
                    .AppendLine("LEFT JOIN SP_BUSCAR_VALOR_SERVICO(A.CODCT, A.PROJETO, Null, A.UNIDADE_CUSTO, A.COD_PRECO) C ")
                    .AppendLine("ON(C.RCLASSE_MO = B.CLASSE_MO) ")
                    .AppendLine("INNER JOIN TB_DOCUMENTO_PRODUCAO D ");

                    if (SIGLA_MODULO == "MCO")
                    {
                        sql.AppendLine($"ON(A.CODCT = D.CODCT AND A.PROJETO = D.PROJETO AND A.{CHAVE} = D.NUM_OBRA AND A.NUM_DOC = D.NUM_DOC) ");
                        sql.AppendLine($"WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.{CHAVE} = @CHAVE AND A.NUM_DOC = @NUM_DOC ");
                    }
                    else
                    {
                        sql.AppendLine($"ON(A.CODCT = D.CODCT AND A.PROJETO = D.PROJETO AND A.{CHAVE} = D.NUM_OBRA AND A.NUM_DOC = D.NUM_DOC) ");
                        sql.AppendLine($"WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.{CHAVE} LIKE @CHAVE AND A.NUM_DOC = @NUM_DOC ");
                    }

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(D.SIGLA_MODULO, '') <> 'MCO' ");

                    sql.AppendLine("UNION ALL ")

                    .AppendLine("SELECT COALESCE((A.QUANTIDADE * B.VALOR), 0) AS TOTAL ")
                    .AppendLine("FROM TB_MATERIAL_TAREFA A ")
                    .AppendLine("INNER JOIN TB_MATERIAL B ")
                    .AppendLine("ON(A.CODMAT = B.CODMAT) ")
                    .AppendLine("INNER JOIN TB_DOCUMENTO_PRODUCAO C ");
                    if (SIGLA_MODULO == "MCO")
                    {
                        sql.AppendLine($"ON(A.CODCT = C.CODCT AND A.PROJETO = C.PROJETO AND A.{CHAVE} = C.NUM_OBRA AND A.NUM_DOC = C.NUM_DOC) ");
                        sql.AppendLine($"WHERE  A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.{CHAVE} = @CHAVE AND A.NUM_DOC = @NUM_DOC ");
                    }
                    else
                    {
                        sql.AppendLine($"ON(A.CODCT = C.CODCT AND A.PROJETO = C.PROJETO AND A.{CHAVE} = C.NUM_OBRA AND A.NUM_DOC = C.NUM_DOC) ");
                        sql.AppendLine($"WHERE  A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.{CHAVE} LIKE @CHAVE AND A.NUM_DOC = @NUM_DOC ");
                    }

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(C.SIGLA_MODULO, '') <> 'MCO' ");

                    sql.AppendLine(")");

                    parametros.Add("@CODCT", CodCt);
                    parametros.Add("@PROJETO", Projeto);
                    parametros.Add("@NUM_DOC", Num_Doc);

                    if (SIGLA_MODULO == "MCO")
                        parametros.Add("@CHAVE", String.Format("{0}", Num_Obra.Split('/')[0]));
                    else
                        parametros.Add("@CHAVE", String.Format("{0}%", Num_Obra.Split('/')[0]));

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno.Objeto = reader["TOTAL"] != DBNull.Value ? Convert.ToDouble(reader["TOTAL"]) : 0;
                        }
                    }
                }

                retorno.Ok = true;
            }

            return retorno;
        }


        // P/ fazer: Custo Médio Serviço 
        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/CustoMedio")]
        public async Task<GetApiRetorno<Dashboard_Servicos_Models>> GetCustoMedio()
        {

            GetApiRetorno<Dashboard_Servicos_Models> retorno = new GetApiRetorno<Dashboard_Servicos_Models>();
            retorno.Objeto = new Dashboard_Servicos_Models();
            Double custo_medio_atual = 0;
            Double custo_medio_anterior = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                Boolean Exibe_RDOs = (await repoConfig.selectFirst()).ExibeRDOs;

                List<Rankprod> _rank_prod = new List<Rankprod>();


                DateTime data_atual = DateTime.Now;
                DateTime _data = data_atual;

                for (int i = data_atual.Day; i > 1; i--)
                {
                    _data = _data.AddDays(-1);
                }

                DateTime data_inicio_mes = new DateTime(_data.Year, _data.Month, _data.Day, 0, 0, 0);

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT SUM(TOTAL) AS TOTAL, SUM(QUANTIDADE_OS) AS QUANTIDADE_OS");
                    sql.AppendLine("FROM(");

                    sql.AppendLine("SELECT COALESCE(SUM(A.QUANT_PROD * B.QUANTIDADE * C.RVALOR_FAT), 0) AS TOTAL, 0 QUANTIDADE_OS");
                    sql.AppendLine("FROM TB_CUSTOS_TAREFA A");
                    sql.AppendLine("LEFT JOIN TB_CADERNO_ENCARGOS B");
                    sql.AppendLine("ON(A.UNIDADE_CUSTO = B.UNIDADE_CUSTO AND B.CODIGO = A.CODIGO)");
                    sql.AppendLine("LEFT JOIN SP_BUSCAR_VALOR_SERVICO(A.CODCT, A.PROJETO, Null, A.UNIDADE_CUSTO, A.COD_PRECO) C");
                    sql.AppendLine("ON(C.RCLASSE_MO = B.CLASSE_MO)");
                    sql.AppendLine("INNER JOIN TB_DOCUMENTO_PRODUCAO D");
                    sql.AppendLine("ON(A.CODCT = D.CODCT AND A.PROJETO = D.PROJETO AND A.NUM_OBRA = D.NUM_OBRA AND A.NUM_DOC = D.NUM_DOC) ");
                    sql.AppendLine($"WHERE  A.DATA_PROD BETWEEN @DATAINI AND @DATAFIM");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(D.SIGLA_MODULO, '') <> 'MCO' ");

                    sql.AppendLine("UNION ALL");

                    sql.AppendLine("SELECT COALESCE((A.QUANTIDADE * B.VALOR), 0) AS TOTAL, 0 QUANTIDADE_OS");
                    sql.AppendLine("FROM TB_MATERIAL_TAREFA A");
                    sql.AppendLine("INNER JOIN TB_MATERIAL B");
                    sql.AppendLine("ON(A.CODMAT = B.CODMAT)");
                    sql.AppendLine("INNER JOIN TB_DOCUMENTO_PRODUCAO C");
                    sql.AppendLine("ON(A.CODCT = C.CODCT AND A.PROJETO = C.PROJETO AND A.NUM_OBRA = C.NUM_OBRA AND A.NUM_DOC = C.NUM_DOC) ");
                    sql.AppendLine($"WHERE  C.DATA_PROD BETWEEN @DATAINI AND @DATAFIM");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(C.SIGLA_MODULO, '') <> 'MCO' ");

                    sql.AppendLine("UNION ALL");

                    sql.AppendLine("SELECT 0 AS TOTAL, COUNT(A.NUM_OBRA) QUANTIDADE_OS");
                    sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A");
                    sql.AppendLine($"WHERE A.DATA_PROD BETWEEN @DATAINI AND @DATAFIM");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    sql.AppendLine(")");


                    parametros.Add("@DATAINI", new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                    parametros.Add("@DATAFIM", new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)));

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var total = reader["TOTAL"] != DBNull.Value ? Convert.ToDouble(reader["TOTAL"]) : 0;
                            var qtde_os = reader["QUANTIDADE_OS"] != DBNull.Value ? Convert.ToInt32(reader["QUANTIDADE_OS"]) : 0;

                            custo_medio_atual = qtde_os != 0 ? total / qtde_os : 0;
                        }
                    }


                }

                parametros.Clear();

                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql1 = new StringBuilder();

                    sql1.AppendLine("SELECT SUM(TOTAL) AS TOTAL, SUM(QUANTIDADE_OS) AS QUANTIDADE_OS");
                    sql1.AppendLine("FROM(");

                    sql1.AppendLine("SELECT COALESCE(SUM(A.QUANT_PROD * B.QUANTIDADE * C.RVALOR_FAT), 0) AS TOTAL, 0 QUANTIDADE_OS");
                    sql1.AppendLine("FROM TB_CUSTOS_TAREFA A");
                    sql1.AppendLine("LEFT JOIN TB_CADERNO_ENCARGOS B");
                    sql1.AppendLine("ON(A.UNIDADE_CUSTO = B.UNIDADE_CUSTO AND B.CODIGO = A.CODIGO)");
                    sql1.AppendLine("LEFT JOIN SP_BUSCAR_VALOR_SERVICO(A.CODCT, A.PROJETO, Null, A.UNIDADE_CUSTO, A.COD_PRECO) C");
                    sql1.AppendLine("ON(C.RCLASSE_MO = B.CLASSE_MO)");
                    sql1.AppendLine("INNER JOIN TB_DOCUMENTO_PRODUCAO D");
                    sql1.AppendLine("ON(A.CODCT = D.CODCT AND A.PROJETO = D.PROJETO AND A.NUM_OBRA = D.NUM_OBRA AND A.NUM_DOC = D.NUM_DOC ) ");
                    sql1.AppendLine($"WHERE  A.DATA_PROD BETWEEN @DATAINI AND @DATAFIM"); // 

                    if (!Exibe_RDOs)
                        sql1.AppendLine(" AND COALESCE(D.SIGLA_MODULO, '') <> 'MCO' ");

                    sql1.AppendLine("UNION ALL");

                    sql1.AppendLine("SELECT COALESCE((A.QUANTIDADE * B.VALOR), 0) AS TOTAL, 0 QUANTIDADE_OS");
                    sql1.AppendLine("FROM TB_MATERIAL_TAREFA A");
                    sql1.AppendLine("INNER JOIN TB_MATERIAL B");
                    sql1.AppendLine("ON(A.CODMAT = B.CODMAT)");
                    sql1.AppendLine("INNER JOIN TB_DOCUMENTO_PRODUCAO C");
                    sql1.AppendLine("ON(A.CODCT = C.CODCT AND A.PROJETO = C.PROJETO AND A.NUM_OBRA = C.NUM_OBRA AND A.NUM_DOC = C.NUM_DOC ) ");
                    sql1.AppendLine($"WHERE  C.DATA_PROD BETWEEN @DATAINI AND @DATAFIM");

                    if (!Exibe_RDOs)
                        sql1.AppendLine(" AND COALESCE(C.SIGLA_MODULO, '') <> 'MCO' ");

                    sql1.AppendLine("UNION ALL");

                    sql1.AppendLine("SELECT 0 AS TOTAL, COUNT(A.NUM_OBRA) QUANTIDADE_OS");
                    sql1.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A");
                    sql1.AppendLine($"WHERE  A.DATA_PROD BETWEEN @DATAINI AND @DATAFIM ");

                    if (!Exibe_RDOs)
                        sql1.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    sql1.AppendLine(")");

                    DateTime mes = DateTime.Now.AddMonths(-1);
                    parametros.Add("@DATAINI", new DateTime(mes.Year, mes.Month, 1));
                    parametros.Add("@DATAFIM", new DateTime(mes.Year, mes.Month, DateTime.DaysInMonth(mes.Year, mes.Month)));


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql1, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var total = Convert.ToDouble(reader["TOTAL"]);
                            var qtde_os = Convert.ToInt32(reader["QUANTIDADE_OS"]);

                            custo_medio_anterior = qtde_os != 0 ? total / qtde_os : 0;
                        }
                    }

                    retorno.Objeto = new Dashboard_Servicos_Models
                    {
                        Custo_Medio_Servico = Math.Round(custo_medio_atual, 2).ToString("N2"),
                        Seta_Custo_Medio_Servico = (custo_medio_atual > custo_medio_anterior) ? "#FF0000" : "#22B14C"
                    };
                }
                retorno.Ok = true;
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetRankingProdutividade")]
        public async Task<GetApiRetorno<Dashboard_Servicos_Models>> GetRankingProdutividade(Dashboard_Servicos_Request request)
        {

            GetApiRetorno<Dashboard_Servicos_Models> retorno = new GetApiRetorno<Dashboard_Servicos_Models>();
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession sessionT5NH = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repository = new Conexao<PerfilAgendamento>(sessionT5NH))
            using (var repositoryArea = new Conexao<PerfilAgendamentoArea>(sessionT5NH))
            {
                IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                var usuarioBusiness = new UsuarioBusiness(repoUsuario);
                IdentityUser user = usuarioBusiness.GetById(User.Identity.GetUserId());
                Boolean Exibe_RDOs = (await repoConfig.selectFirst()).ExibeRDOs;
                List<Rankprod> _rank_prod = new List<Rankprod>();
                List<Rankprod> _rank_prod_superv = new List<Rankprod>();


                DateTime data_atual = String.IsNullOrEmpty(request?.DataReferencia) ? DateTime.Now : Convert.ToDateTime(request?.DataReferencia);
                DateTime _data = data_atual;

                for (int i = data_atual.Day; i > 1; i--)
                {
                    _data = _data.AddDays(-1);
                }

                DateTime data_inicio_mes = new DateTime(_data.Year, _data.Month, _data.Day, 0, 0, 0);

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT B.NOME,SUPERV.NUM_FRE AS NUM_FRE_SUPERVISOR, SUPERV.NOME AS NOME_SUPERVISOR, B.ID_PERFIL_AGENDAMENTO FROM TB_DOCUMENTO_PRODUCAO A");

                    sql.AppendLine("LEFT JOIN TB_EQUIPE B");
                    sql.AppendLine("ON(A.EQUIPE = B.EQUIPE)");
                    sql.AppendLine("LEFT JOIN TB_FUNCIONARIO SUPERV");
                    sql.AppendLine("ON(SUPERV.NUM_FRE = B.COD_SUPERV)");

                    //parametros.Add("@USUARIO", User.Identity.GetUserId());
                    List<string> condicoes = new List<string>();
                    condicoes.Add("(A.DATA_PROD BETWEEN @DATA_INCIO_MES AND @DATA_ATUAL)");
                    condicoes.Add("(A.STATUS_EXECUCAO = '2')");
                    condicoes.Add("(A.EQUIPE IS NOT NULL)");

                    var periodo = GetPeriodoFiltro(request.TipoFiltroData.Value, data_atual);
                    parametros.Add("@DATA_INCIO_MES", periodo.dataInicio);
                    parametros.Add("@DATA_ATUAL", periodo.dataFim);

                    if (!Exibe_RDOs)
                        condicoes.Add("(COALESCE(A.SIGLA_MODULO, '') <> 'MCO')");

                    if (String.IsNullOrEmpty(request.Contrato) == false)
                    {
                        condicoes.Add("(A.CODCT = @CONTRATO)");
                        parametros.Add("@CONTRATO", request.Contrato);
                    }

                    if (String.IsNullOrEmpty(request.Projeto) == false)
                    {
                        condicoes.Add("(A.PROJETO = @PROJETO)");
                        parametros.Add("@PROJETO", request.Projeto);
                    }

                    if (String.IsNullOrEmpty(request.Supervisor) == false)
                    {
                        condicoes.Add("(EQUIPE.COD_SUPERV = @SUPERV)");
                        parametros.Add("@SUPERV", request.Supervisor);
                    }

                    if (String.IsNullOrEmpty(request.Monitor) == false)
                    {
                        condicoes.Add("(EQUIPE.USUARIO_MONITOR = @USUARIO_MONITOR)");
                        parametros.Add("@USUARIO_MONITOR", request.Monitor);
                    }

                    if (String.IsNullOrEmpty(request.NaturezaServico) == false)
                    {
                        condicoes.Add("(B.COD_NATUREZA = @NATUREZA)");
                        parametros.Add("@NATUREZA", request.NaturezaServico);
                    }

                    if (String.IsNullOrEmpty(request.GrupoArea) == false)
                    {
                        condicoes.Add("(A.COD_ESCRITORIO = @GRUPO_AREA)");
                        parametros.Add("@GRUPO_AREA", request.GrupoArea);
                    }

                    if (String.IsNullOrEmpty(request.Area) == false)
                    {
                        condicoes.Add("(A.COD_AREA = @AREA)");
                        parametros.Add("@AREA", request.Area);
                    }

                    if (String.IsNullOrEmpty(request.Localidade) == false)
                    {
                        condicoes.Add("(A.COD_LOCALIDADE = @LOCALIDADE)");
                        parametros.Add("@LOCALIDADE", request.Localidade);
                    }

                    #region Filtro Perfil Visualização de Serviço

                    if (user.PerfilVisualizacaoServico != null)
                    {
                        // Verificação de contratos e projetos
                        if (user.PerfilVisualizacaoServico.ParamsContratoProjeto.Count() > 0)
                        {
                            List<string> contratosProjeto = new List<string>();
                            foreach (var paramContratoProj in user.PerfilVisualizacaoServico.ParamsContratoProjeto)
                            {
                                if (!String.IsNullOrEmpty(paramContratoProj.CodCt) && !String.IsNullOrEmpty(paramContratoProj.Projeto))
                                {
                                    contratosProjeto.Add($"(A.CODCT = '{paramContratoProj.CodCt}' AND A.PROJETO = '{paramContratoProj.Projeto}')");
                                }
                                else
                                {
                                    contratosProjeto.Add($"(A.CODCT = '{paramContratoProj.CodCt}')");
                                }
                            }

                            if (contratosProjeto.Count > 0)
                                condicoes.Add($"({String.Join(" OR ", contratosProjeto)})");
                        }

                        // Verificação de áreas
                        if (user.PerfilVisualizacaoServico.ParamsArea.Count() > 0)
                        {
                            List<string> areas = new List<string>();
                            foreach (var paramArea in user.PerfilVisualizacaoServico.ParamsArea)
                            {
                                if (!String.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                   !String.IsNullOrEmpty(paramArea.CodArea) &&
                                   paramArea.CodLocalidade.HasValue)
                                {
                                    areas.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}' AND A.COD_LOCALIDADE = {paramArea.CodLocalidade.Value})");
                                }
                                else if (!String.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                         !String.IsNullOrEmpty(paramArea.CodArea))
                                {
                                    areas.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}')");
                                }
                                else
                                {
                                    areas.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}')");
                                }
                            }

                            if (areas.Count > 0)
                                condicoes.Add($"({String.Join(" OR ", areas)})");
                        }

                        // Verificação de setores e departamentos
                        if (user.PerfilVisualizacaoServico.ParamsSetorDepto.Count() > 0)
                        {
                            List<string> departamentos = new List<string>();
                            foreach (var paramSetor in user.PerfilVisualizacaoServico.ParamsSetorDepto)
                            {
                                departamentos.Add($"(A.COD_DEPTO = {paramSetor.CodDepto})");
                            }

                            if (departamentos.Count > 0)
                                condicoes.Add($"({String.Join(" OR ", departamentos)})");
                        }

                        // Verificação de supervisores
                        if (user.PerfilVisualizacaoServico.ParamsSupervisor.Count() > 0)
                        {
                            List<string> supervisores = new List<string>();
                            foreach (var paramSupervisor in user.PerfilVisualizacaoServico.ParamsSupervisor)
                            {
                                supervisores.Add($"(A.COD_SUPERV = '{paramSupervisor.CodSuperv}')");
                            }

                            if (supervisores.Count > 0)
                                condicoes.Add($"({String.Join(" OR ", supervisores)})");
                        }

                        // Verificação de monitores
                        if (user.PerfilVisualizacaoServico.ParamsMonitor.Count() > 0)
                        {
                            List<string> monitores = new List<string>();
                            foreach (var paramMonitor in user.PerfilVisualizacaoServico.ParamsMonitor)
                            {
                                monitores.Add($"(A.USUARIO_MONITOR = '{paramMonitor.UsuarioMonitor}')");
                            }

                            if (monitores.Count > 0)
                                condicoes.Add($"({String.Join(" OR ", monitores)})");
                        }
                    }

                    #endregion

                    // Adicionando condições ao SQL
                    if (condicoes.Count > 0)
                    {
                        sql.AppendLine(" WHERE ");
                        sql.AppendLine(String.Join(" AND ", condicoes));
                    }

                    var perfis = (await repository.select()).ToList();
                    var perfisAreas = (await repositoryArea.select()).ToList();

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var codSuperv = Convert.ToString(reader["NOME"]);
                            var nomeSuperv = Convert.ToString(reader["NOME_SUPERVISOR"]);
                            long? idPerfil = reader["ID_PERFIL_AGENDAMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_PERFIL_AGENDAMENTO"]) : (Int64?)null;
                            var perfil = idPerfil.HasValue ? perfis.Find(p => p.Id == idPerfil) : null;
                            List<LocalidadeRoteirizacao> localidades = new List<LocalidadeRoteirizacao>();

                            if (perfil != null)
                            {
                                localidades = perfisAreas.FindAll(p => p.PerfilAgendamento?.Id == perfil.Id)
                                                         .Select(p =>
                                {
                                    return new LocalidadeRoteirizacao
                                    {
                                        Cod_Escritorio = p.Escritorio,
                                        Cod_Area = p.Area,
                                        LocalidadeStr = p.Localidade,
                                        Todas_Localidades = p.Todas_Localidades,
                                        Nivel = p.Nivel,
                                        Todas_Areas = p.Todas_Areas
                                    };

                                }).ToList();
                            }


                            if (idPerfil.HasValue)
                            {
                                if (user.PerfilVisualizacaoServico != null && user.PerfilVisualizacaoServico.ParamsArea.Count() > 0)
                                {
                                    var validacaoAreas = user.PerfilVisualizacaoServico.ParamsArea.Select(paramArea =>
                                    {
                                        if (paramArea.CodLocalidade.HasValue && !String.IsNullOrEmpty(paramArea.CodArea) && !String.IsNullOrEmpty(paramArea.CodEscritorio))
                                        {
                                            return localidades.FindAll(p => (p.Cod_Escritorio == paramArea.CodEscritorio &&
                                                                     p.Cod_Area == paramArea.CodArea &&
                                                                     p.Cod_Localidade == paramArea.CodLocalidade) ||
                                                                     (p.Cod_Escritorio == paramArea.CodEscritorio &&
                                                                     p.Cod_Area == paramArea.CodArea &&
                                                                     p.Todas_Localidades) ||
                                                                     (p.Cod_Escritorio == paramArea.CodEscritorio &&
                                                                     p.Todas_Areas &&
                                                                     p.Todas_Localidades)).Count > 0;
                                        }
                                        else if (!String.IsNullOrEmpty(paramArea.CodArea) && !String.IsNullOrEmpty(paramArea.CodEscritorio))
                                        {
                                            return localidades.FindAll(p => (p.Cod_Escritorio == paramArea.CodEscritorio &&
                                                                     p.Cod_Area == paramArea.CodArea) ||
                                                                     (p.Cod_Escritorio == paramArea.CodEscritorio &&
                                                                     p.Todas_Areas &&
                                                                     p.Todas_Localidades)).Count > 0;
                                        }
                                        else
                                        {
                                            return localidades.FindAll(p => p.Cod_Escritorio == paramArea.CodEscritorio).Count > 0;
                                        }

                                    });

                                    if (validacaoAreas.All(p => p == false))
                                        continue;
                                }


                            }

                            var nome = Convert.ToString(reader["NOME"]);
                            var equipe_rank = _rank_prod.Where(p => p.Nome == nome).FirstOrDefault();
                            if (equipe_rank == null)
                            {
                                _rank_prod.Add(new Rankprod
                                {
                                    Nome = nome,
                                    Qtde_OS = 1
                                });

                            }
                            else
                            {
                                var temp = equipe_rank;
                                temp.Qtde_OS++;
                                _rank_prod.Remove(equipe_rank);
                                _rank_prod.Add(temp);
                            }

                            if (String.IsNullOrEmpty(nomeSuperv))
                                continue;


                            var supervisor_rank = _rank_prod_superv.Where(p => p.Nome == nomeSuperv).FirstOrDefault();
                            if (supervisor_rank == null)
                            {
                                _rank_prod_superv.Add(new Rankprod
                                {
                                    Nome = nomeSuperv,
                                    Qtde_OS = 1
                                });

                            }
                            else
                            {
                                var temp = supervisor_rank;
                                temp.Qtde_OS++;
                                _rank_prod_superv.Remove(supervisor_rank);
                                _rank_prod_superv.Add(temp);
                            }

                        }
                    }

                    retorno.Objeto = new Dashboard_Servicos_Models
                    {
                        Ranking_Produtividade_Mes = _rank_prod.OrderByDescending(p => p.Qtde_OS).Take(10).ToList(),
                        Ranking_Produtividade_Mes_Supervisor = _rank_prod_superv.OrderByDescending(p => p.Qtde_OS).Take(10).ToList(),
                    };
                }
                retorno.Ok = true;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetPctServicosForaPrazo")]
        public async Task<GetApiRetorno<Dashboard_Servicos_Models>> GetPctServicosForaPrazo(Dashboard_Servicos_Request request)
        {

            GetApiRetorno<Dashboard_Servicos_Models> retorno = new GetApiRetorno<Dashboard_Servicos_Models>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                Boolean Exibe_RDOs = (await repoConfig.selectFirst()).ExibeRDOs;

                Int32 servicos_atual = 0;
                Int32 fora_prazo_atual = 0;
                Int32 servicos_anterior = 0;
                Int32 fora_prazo_anterior = 0;

                DateTime data_atual = String.IsNullOrEmpty(request?.DataReferencia) ? DateTime.Now : Convert.ToDateTime(request?.DataReferencia);
                DateTime _data = data_atual;

                for (int i = data_atual.Day; i > 1; i--)
                {
                    _data = _data.AddDays(-1);
                }

                DateTime data_inicio_mes = new DateTime(_data.Year, _data.Month, _data.Day, 0, 0, 0);

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder("SELECT COALESCE(A.DATA_INDICADOR, A.DIA +1) + COALESCE(A.HORA_INDICADOR, A.HORA) DATAHORA_INDICADOR,A.DATA_PROD + A.HORA_PROD  DATAHORA_PROD FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql.AppendLine("WHERE (A.DATA_INDICADOR BETWEEN @DATA_INCIO_MES AND @DATA_ATUAL) ");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    parametros.Add("@DATA_ATUAL", data_atual);
                    parametros.Add("@DATA_INCIO_MES", data_inicio_mes);


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        Int32 cont = 0;
                        while (await reader.ReadAsync())
                        {
                            DateTime DataHora_Prazo = reader["DATAHORA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INDICADOR"]) : data_atual;
                            DateTime DataHora_Baixa = reader["DATAHORA_PROD"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_PROD"]) : new DateTime(3000, 5, 1);

                            if (data_atual > DataHora_Prazo && DataHora_Baixa > DataHora_Prazo)
                                cont++;
                        }

                        fora_prazo_atual = cont;
                    }
                }

                using (var com1 = sessionT5.Connection.CreateCommand())
                {
                    //Serviços Encerrados Totais

                    StringBuilder sql_servicos_atual = new StringBuilder();

                    sql_servicos_atual.AppendLine("SELECT COUNT(CODCT) CO ");
                    sql_servicos_atual.AppendLine("FROM TB_DOCUMENTO_PRODUCAO");

                    sql_servicos_atual.AppendLine("WHERE DATA_INDICADOR BETWEEN @DATA_INCIO_MES AND @DATA_ATUAL");

                    if (!Exibe_RDOs)
                        sql_servicos_atual.AppendLine(" AND COALESCE(SIGLA_MODULO, '') <> 'MCO' ");

                    using (var reader1 = await CommandHelper.ExecuteReader(com1, sql_servicos_atual, parametros, sessionT5.Connection, null))
                    {
                        while (await reader1.ReadAsync())
                        {
                            servicos_atual = Convert.ToInt32(reader1["CO"]);
                        }

                    }

                }

                using (var com2 = sessionT5.Connection.CreateCommand())
                {
                    //Serviços Fora do Prazo do Mês anterior

                    StringBuilder sql_fora_prazo_anterior = new StringBuilder("SELECT COALESCE(A.DATA_INDICADOR, A.DIA +1) + COALESCE(A.HORA_INDICADOR, A.HORA) DATAHORA_INDICADOR,A.DATA_PROD + A.HORA_PROD  DATAHORA_PROD FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql_fora_prazo_anterior.AppendLine("WHERE (A.DATA_INDICADOR BETWEEN @DATA_INCIO_ANTERIOR AND @DATA_FIM_ANTERIOR) ");

                    if (!Exibe_RDOs)
                        sql_fora_prazo_anterior.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    parametros.Add("@DATA_FIM_ANTERIOR", data_inicio_mes.AddMinutes(-1));
                    parametros.Add("@DATA_INCIO_ANTERIOR", data_inicio_mes.AddMonths(-1));

                    using (var reader2 = await CommandHelper.ExecuteReader(com2, sql_fora_prazo_anterior, parametros, sessionT5.Connection, null))
                    {
                        Int32 cont = 0;
                        while (await reader2.ReadAsync())
                        {
                            DateTime DataHora_Prazo = reader2["DATAHORA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader2["DATAHORA_INDICADOR"]) : data_atual;
                            DateTime DataHora_Baixa = reader2["DATAHORA_PROD"] != DBNull.Value ? Convert.ToDateTime(reader2["DATAHORA_PROD"]) : new DateTime(3000, 5, 1);

                            if ((String.IsNullOrEmpty(request?.DataReferencia) ? data_atual : Convert.ToDateTime(request?.DataReferencia)) > DataHora_Prazo && DataHora_Baixa > DataHora_Prazo)
                                cont++;
                        }

                        fora_prazo_anterior = cont;
                    }

                }

                using (var com3 = sessionT5.Connection.CreateCommand())
                {
                    //Serviços Em Execução Totais Mês Anterior

                    StringBuilder sql_servicos_anterior = new StringBuilder();


                    sql_servicos_anterior.AppendLine("SELECT COUNT(CODCT) CO ");
                    sql_servicos_anterior.AppendLine("FROM TB_DOCUMENTO_PRODUCAO");

                    sql_servicos_anterior.AppendLine("WHERE DATA_INDICADOR BETWEEN  @DATA_INCIO_ANTERIOR AND @DATA_FIM_ANTERIOR");

                    if (!Exibe_RDOs)
                        sql_servicos_anterior.AppendLine(" AND COALESCE(SIGLA_MODULO, '') <> 'MCO' ");

                    using (var reader3 = await CommandHelper.ExecuteReader(com3, sql_servicos_anterior, parametros, sessionT5.Connection, null))
                    {
                        while (await reader3.ReadAsync())
                        {
                            servicos_anterior = Convert.ToInt32(reader3["CO"]);
                        }
                    }

                }
                var _obj = new Dashboard_Servicos_Models();

                _obj.Percentual_Servico_Fora_Prazo = (servicos_atual == 0)
                                        ?
                                            "0"
                                        :
                                            String.Format("{0}%", Math.Round((Convert.ToDouble(fora_prazo_atual) / Convert.ToDouble(servicos_atual)) * 100), 2);



                if ((servicos_atual != 0) && (servicos_anterior != 0))
                {
                    _obj.Seta_Pct_Servico_Fora_Prazo = (Convert.ToDouble(fora_prazo_atual) / Convert.ToDouble(servicos_atual)) > (Convert.ToDouble(fora_prazo_anterior) / Convert.ToDouble(servicos_anterior))
                                    ?
                                        "#FF0000" // Vermelho
                                    :
                                        "#22B14C"; // Verde
                }
                if ((servicos_atual != 0) && (servicos_anterior == 0))
                {
                    _obj.Seta_Pct_Servico_Fora_Prazo = "#FF0000"; // Verde
                }

                //_obj.Seta_Prod_Mes = "#FF0000";

                retorno.Objeto = _obj;

            }
            retorno.Ok = true;
            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetMediaAtraso")]
        public async Task<GetApiRetorno<Dashboard_Servicos_Models>> GetMediaAtraso(Dashboard_Servicos_Request request)
        {

            GetApiRetorno<Dashboard_Servicos_Models> retorno = new GetApiRetorno<Dashboard_Servicos_Models>();

            TimeSpan tempo_medio;
            TimeSpan tempo_medio_anterior;

            List<TimeSpan> media_atraso = new List<TimeSpan>();
            List<TimeSpan> media_atraso_anterior = new List<TimeSpan>();
            DateTime data_atual = String.IsNullOrEmpty(request?.DataReferencia) ? DateTime.Now : Convert.ToDateTime(request?.DataReferencia);
            String cor_seta;

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                Boolean Exibe_RDOs = (await repoConfig.selectFirst()).ExibeRDOs;

                TimeSpan atraso;
                DateTime data_ini_atend;

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                var dataReferencia = (String.IsNullOrEmpty(request?.DataReferencia) ? data_atual : Convert.ToDateTime(request?.DataReferencia));
                Int32 ano = dataReferencia.Year;
                Int32 mes = dataReferencia.Month;

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder("SELECT DATA_MAXIMA + HORA_MAXIMA AS DATAHORA_MAXIMA, DATAHORA_INI_ATEND FROM TB_DOCUMENTO_PRODUCAO ")
                    .AppendLine("WHERE DATAHORA_INI_ATEND IS NOT NULL ")
                    .AppendLine("AND DATA_MAXIMA IS NOT NULL ")
                    .AppendLine("AND HORA_MAXIMA IS NOT NULL ")
                    .AppendLine("AND DATAHORA_INI_ATEND BETWEEN @DATA_INICIO AND @DATA_FIM");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(SIGLA_MODULO, '') <> 'MCO' ");

                    //parametros.Add("@USUARIO", User.Identity.GetUserId());
                    parametros.Add("@DATA_INICIO", new DateTime(ano, mes, 1, 0, 0, 0));
                    parametros.Add("@DATA_FIM", (String.IsNullOrEmpty(request?.DataReferencia) ? dataReferencia : Convert.ToDateTime(request?.DataReferencia)));

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            data_ini_atend = Convert.ToDateTime(reader["DATAHORA_INI_ATEND"]);

                            var datahora_agendamento = reader["DATAHORA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_MAXIMA"]) : (DateTime?)null;

                            if (datahora_agendamento.HasValue)
                            {
                                atraso = data_ini_atend - datahora_agendamento.Value;
                                if (atraso.TotalMinutes > 0)
                                    media_atraso.Add(atraso);
                            }
                        }
                    }

                }

                tempo_medio = Tempo_Medio(media_atraso);
            }

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                Boolean Exibe_RDOs = (await repoConfig.selectFirst()).ExibeRDOs;

                TimeSpan atraso;
                DateTime data_ini_atend;

                Int32 ano = (String.IsNullOrEmpty(request?.DataReferencia) ? data_atual : Convert.ToDateTime(request?.DataReferencia)).Year;
                Int32 mes_anterior = (String.IsNullOrEmpty(request?.DataReferencia) ? data_atual : Convert.ToDateTime(request?.DataReferencia)).AddMonths(-1).Month;

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();


                    sql.AppendLine("SELECT DATA_MAXIMA + HORA_MAXIMA AS DATAHORA_MAXIMA, DATAHORA_INI_ATEND");
                    sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO");

                    sql.AppendLine("WHERE DATAHORA_INI_ATEND IS NOT NULL"); // COALESCE(SIGLA_MODULO, '') <> 'MCO'
                    sql.AppendLine("AND DATA_MAXIMA IS NOT NULL");
                    sql.AppendLine("AND HORA_MAXIMA IS NOT NULL");
                    sql.AppendLine("AND DATAHORA_INI_ATEND BETWEEN @DATA_INICIO_MES AND @DATA_FIM_MES");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(SIGLA_MODULO, '') <> 'MCO' ");

                    //parametros.Add("@USUARIO", User.Identity.GetUserId());
                    parametros.Add("@DATA_INICIO_MES", new DateTime(ano, mes_anterior, 1, 0, 0, 0));
                    parametros.Add("@DATA_FIM_MES", new DateTime(ano, mes_anterior, DateTime.DaysInMonth(ano, mes_anterior), 23, 59, 59));

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            data_ini_atend = Convert.ToDateTime(reader["DATAHORA_INI_ATEND"]);

                            var datahora_agendamento = reader["DATAHORA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_MAXIMA"]) : (DateTime?)null;

                            if (datahora_agendamento.HasValue)
                            {
                                atraso = data_ini_atend - datahora_agendamento.Value;
                                if (atraso.TotalMinutes > 0)
                                    media_atraso_anterior.Add(atraso);
                            }
                        }
                    }

                }

                tempo_medio_anterior = Tempo_Medio(media_atraso_anterior);
            }

            if (tempo_medio_anterior > tempo_medio)
                cor_seta = "#22B14C";
            else
                cor_seta = "#FF0000";

            retorno.Objeto = new Dashboard_Servicos_Models
            {
                Media_Atraso = media_atraso.Count != 0
                                   ?
                                       tempo_medio.TotalDays >= 1
                                       ?
                                           tempo_medio.TotalDays >= 1 && tempo_medio.TotalDays <= 2
                                            ?
                                                tempo_medio.ToString("dd") + " Dia " + tempo_medio.ToString(@"hh\:mm") + "h"
                                            :
                                                tempo_medio.ToString("dd") + " Dias " + tempo_medio.ToString(@"hh\:mm") + "h"

                                       :
                                            tempo_medio.ToString(@"hh\:mm") + "h"
                                   :
                                   "00:00",
                Seta_Media_Atraso = cor_seta

            };
            retorno.Ok = true;

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetTecnicosHabilidade")]
        public async Task<GetApiRetorno<TecnicosHabilidadesModel>> GetTecnicosHabilidade(Dashboard_Servicos_Request request)
        {
            GetApiRetorno<TecnicosHabilidadesModel> retorno = new GetApiRetorno<TecnicosHabilidadesModel>();
            retorno.Objeto = new TecnicosHabilidadesModel();
            List<TipoHabilidade?> listaHabilidades = new List<TipoHabilidade?>();
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionT5NH = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<PerfilAgendamento>(sessionT5NH))
                using (var repositoryNatureza = new Conexao<PerfilAgendamentoNatureza>(sessionT5NH))
                using (var repositoryArea = new Conexao<PerfilAgendamentoArea>(sessionT5NH))
                {
                    IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                    var usuarioBusiness = new UsuarioBusiness(repoUsuario);
                    IdentityUser user = usuarioBusiness.GetById(User.Identity.GetUserId());
                    List<string> condicoes = new List<string>();
                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {

                        var sql = new StringBuilder(@"SELECT B.EQUIPE, B.NOME,B.DEVICE_ID,SUPERV.NUM_FRE AS NUM_FRE_SUPERVISOR, SUPERV.NOME AS NOME_SUPERVISOR,B.USUARIO_MONITOR,FUNCIONARIO_MONITOR.NOME NOME_MONITOR,
                                                        B.ID_PERFIL_AGENDAMENTO");

                        sql.AppendLine(@" FROM TB_EQUIPE B
                                INNER JOIN TB_FUNCIONARIO F
                                ON(B.NUM_FRE = F.NUM_FRE)
                                
                                LEFT JOIN TB_FUNCIONARIO SUPERV
                                ON(SUPERV.NUM_FRE = B.COD_SUPERV) 
                                LEFT JOIN TB_SENHA USUARIO_MONITOR
                                ON(USUARIO_MONITOR.USUARIO = B.USUARIO_MONITOR) 
                                LEFT JOIN TB_FUNCIONARIO FUNCIONARIO_MONITOR
                                ON(FUNCIONARIO_MONITOR.NUM_FRE = USUARIO_MONITOR.NUM_FRE) ");


                        condicoes.Add("(F.SITUACAO = 0 OR F.SITUACAO = 4)");
                        condicoes.Add("B.EQUIPE_CONNECT = 'S'");

                        #region Filtro Perfil Visualização de Serviço

                        if (user.PerfilVisualizacaoServico != null)
                        {
                            if (user.PerfilVisualizacaoServico.ParamsSetorDepto.Count() > 0)
                            {
                                foreach (var paramSetor in user.PerfilVisualizacaoServico.ParamsSetorDepto)
                                {
                                    condicoes.Add($"(F.COD_DEPTO = {paramSetor.CodDepto})");
                                }
                            }

                            if (user.PerfilVisualizacaoServico.ParamsSupervisor.Count() > 0)
                            {
                                foreach (var paramSupervisor in user.PerfilVisualizacaoServico.ParamsSupervisor)
                                {
                                    condicoes.Add($"(B.COD_SUPERV = '{paramSupervisor.CodSuperv}')");
                                }
                            }

                            if (user.PerfilVisualizacaoServico.ParamsMonitor.Count() > 0)
                            {
                                foreach (var paramMonitor in user.PerfilVisualizacaoServico.ParamsMonitor)
                                {
                                    condicoes.Add($"(B.USUARIO_MONITOR = '{paramMonitor.UsuarioMonitor}')");
                                }
                            }
                        }

                        #endregion

                        if (condicoes.Count > 0)
                        {
                            sql.AppendLine(" WHERE ");
                            sql.AppendLine(String.Join(" AND ", condicoes));
                        }

                        var perfis = (await repository.select()).ToList();
                        var perfisAreas = (await repositoryArea.select()).ToList();
                        var perfisNaturezas = (await repositoryNatureza.select()).ToList();

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {

                                long? idPerfil = reader["ID_PERFIL_AGENDAMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_PERFIL_AGENDAMENTO"]) : (Int64?)null;
                                var perfil = idPerfil.HasValue ? perfis.Find(p => p.Id == idPerfil) : null;
                                List<LocalidadeRoteirizacao> localidades = new List<LocalidadeRoteirizacao>();
                                List<Natureza_ProducaoAniel> naturezas = new List<Natureza_ProducaoAniel>();
                                if (perfil != null)
                                {
                                    localidades = perfisAreas.FindAll(p => p.PerfilAgendamento?.Id == perfil.Id)
                                                             .Select(p =>
                                    {
                                        return new LocalidadeRoteirizacao
                                        {
                                            Cod_Escritorio = p.Escritorio,
                                            Cod_Area = p.Area,
                                            LocalidadeStr = p.Localidade,
                                            Todas_Localidades = p.Todas_Localidades,
                                            Nivel = p.Nivel,
                                            Todas_Areas = p.Todas_Areas
                                        };

                                    }).ToList();

                                    naturezas = perfisNaturezas.FindAll(p => p.PerfilAgendamento.Id == perfil.Id).
                                                                Select(p => p.Natureza)
                                                                .ToList();
                                }

                                if (idPerfil.HasValue)
                                {
                                    if (user.PerfilVisualizacaoServico != null && user.PerfilVisualizacaoServico.ParamsArea.Count() > 0)
                                    {


                                        var validacaoAreas = user.PerfilVisualizacaoServico.ParamsArea.Select(paramArea =>
                                        {
                                            if (paramArea.CodLocalidade.HasValue && !String.IsNullOrEmpty(paramArea.CodArea) && !String.IsNullOrEmpty(paramArea.CodEscritorio))
                                            {
                                                return localidades.FindAll(p => (p.Cod_Escritorio == paramArea.CodEscritorio &&
                                                                         p.Cod_Area == paramArea.CodArea &&
                                                                         p.Cod_Localidade == paramArea.CodLocalidade) ||
                                                                         (p.Cod_Escritorio == paramArea.CodEscritorio &&
                                                                         p.Cod_Area == paramArea.CodArea &&
                                                                         p.Todas_Localidades) ||
                                                                         (p.Cod_Escritorio == paramArea.CodEscritorio &&
                                                                         p.Todas_Areas &&
                                                                         p.Todas_Localidades)).Count > 0;
                                            }
                                            else if (!String.IsNullOrEmpty(paramArea.CodArea) && !String.IsNullOrEmpty(paramArea.CodEscritorio))
                                            {
                                                return localidades.FindAll(p => (p.Cod_Escritorio == paramArea.CodEscritorio &&
                                                                         p.Cod_Area == paramArea.CodArea) ||
                                                                         (p.Cod_Escritorio == paramArea.CodEscritorio &&
                                                                         p.Todas_Areas &&
                                                                         p.Todas_Localidades)).Count > 0;
                                            }
                                            else
                                            {
                                                return localidades.FindAll(p => p.Cod_Escritorio == paramArea.CodEscritorio).Count > 0;
                                            }

                                        });

                                        if (validacaoAreas.All(p => p == false))
                                            continue;
                                    }



                                    TipoHabilidade? tipoHabilidade = GetTipoHabilidade(naturezas);
                                    listaHabilidades.Add(tipoHabilidade);
                                }
                            }

                        }


                    }


                    retorno.Objeto.Reparo = listaHabilidades.Count(p => p == TipoHabilidade.Reparo);
                    retorno.Objeto.Instalacao = listaHabilidades.Count(p => p == TipoHabilidade.Instalacao);
                    retorno.Objeto.Multiskill = listaHabilidades.Count(p => p == TipoHabilidade.Multi);

                    retorno.Ok = true;

                    return retorno;
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetBancoInstalacao")]
        public async Task<GetApiRetorno<BancoInstalacaoModel>> GetBancoInstalacao(Dashboard_Servicos_Request request)
        {
            GetApiRetorno<BancoInstalacaoModel> retorno = new GetApiRetorno<BancoInstalacaoModel>();
            retorno.Objeto = new BancoInstalacaoModel();
            double TotalMinutos = 0;
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                using (var repoConfig = new Conexao<ConfiguracoesDashboardServicos>(session))
                using (var repoHorario = new Conexao<Horario_Connect>(sessionT5Nh))
                using (var repoConfigGerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    List<Horario_Connect> horarios_servico = (repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result).ToList();
                    var horario_padrao = horarios_servico.Find(p => p.Padrao == true);
                    var config = await repoConfig.selectFirst();
                    var configGerais = await repoConfigGerais.selectFirst();
                    TimeSpan? Tempo_Execuxao_Padrao = configGerais.Tempo_Execucao_Padrao?.TimeOfDay;
                    DateTime data_atual = String.IsNullOrEmpty(request?.DataReferencia) ? DateTime.Now : Convert.ToDateTime(request?.DataReferencia);
                    IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                    var usuarioBusiness = new UsuarioBusiness(repoUsuario);
                    IdentityUser user = usuarioBusiness.GetById(User.Identity.GetUserId());

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        var sql = new StringBuilder();
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        sql.AppendLine("SELECT A.NUM_OBRA,A.DIA,A.HORA,");
                        sql.AppendLine("(SELECT FIRST 1 M.HORAS FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M WHERE((M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO ");
                        sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A");
                        sql.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE B ");
                        sql.AppendLine("ON(A.COD_TIPO_SERV = B.COD_TIPO_SERV) ");
                        sql.AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO C ");
                        sql.AppendLine("ON(B.COD_NATUREZA = C.ID) ");
                        sql.AppendLine("LEFT JOIN TB_EQUIPE EQUIPE ");
                        sql.AppendLine("ON(A.EQUIPE = EQUIPE.EQUIPE) ");

                        List<string> condicoes = new List<string>();
                        condicoes.Add("(C.TIPO_HABILIDADE = 1)");
                        condicoes.Add("(A.STATUS_EXECUCAO NOT IN (8,9,1,2))");
                        condicoes.Add("(A.DIA BETWEEN @DATAINICIOBUSCA AND @DATAFIMBUSCA)");
                        var periodo = GetPeriodoFiltro(request.TipoFiltroData.Value, data_atual);
                        parametros.Add("@DATAINICIOBUSCA", periodo.dataInicio);
                        parametros.Add("@DATAFIMBUSCA", periodo.dataFim);

                        if (String.IsNullOrEmpty(request.Contrato) == false)
                        {
                            condicoes.Add("(A.CODCT = @CONTRATO)");
                            parametros.Add("@CONTRATO", request.Contrato);
                        }

                        if (String.IsNullOrEmpty(request.Projeto) == false)
                        {
                            condicoes.Add("(A.PROJETO = @PROJETO)");
                            parametros.Add("@PROJETO", request.Projeto);
                        }

                        if (String.IsNullOrEmpty(request.Supervisor) == false)
                        {
                            condicoes.Add("(EQUIPE.COD_SUPERV = @SUPERV)");
                            parametros.Add("@SUPERV", request.Supervisor);
                        }

                        if (String.IsNullOrEmpty(request.Monitor) == false)
                        {
                            condicoes.Add("(EQUIPE.USUARIO_MONITOR = @USUARIO_MONITOR)");
                            parametros.Add("@USUARIO_MONITOR", request.Monitor);
                        }

                        if (String.IsNullOrEmpty(request.NaturezaServico) == false)
                        {
                            condicoes.Add("(B.COD_NATUREZA = @NATUREZA)");
                            parametros.Add("@NATUREZA", request.NaturezaServico);
                        }

                        if (String.IsNullOrEmpty(request.GrupoArea) == false)
                        {
                            condicoes.Add("(A.COD_ESCRITORIO = @GRUPO_AREA)");
                            parametros.Add("@GRUPO_AREA", request.GrupoArea);
                        }

                        if (String.IsNullOrEmpty(request.Area) == false)
                        {
                            condicoes.Add("(A.COD_AREA = @AREA)");
                            parametros.Add("@AREA", request.Area);
                        }

                        if (String.IsNullOrEmpty(request.Localidade) == false)
                        {
                            condicoes.Add("(A.COD_LOCALIDADE = @LOCALIDADE)");
                            parametros.Add("@LOCALIDADE", request.Localidade);
                        }

                        #region Filtro Perfil Visualização de Serviço

                        if (user.PerfilVisualizacaoServico != null)
                        {
                            // ParamsContratoProjeto
                            if (user.PerfilVisualizacaoServico.ParamsContratoProjeto.Any())
                            {
                                var contratosProjeto = user.PerfilVisualizacaoServico.ParamsContratoProjeto
                                    .Select(param => !string.IsNullOrEmpty(param.CodCt) && !string.IsNullOrEmpty(param.Projeto)
                                        ? $"(A.CODCT = '{param.CodCt}' AND A.PROJETO = '{param.Projeto}')"
                                        : $"(A.CODCT = '{param.CodCt}')")
                                    .Where(condition => !string.IsNullOrEmpty(condition))
                                    .ToList();

                                if (contratosProjeto.Any())
                                {
                                    condicoes.Add($"({string.Join(" OR ", contratosProjeto)})");
                                }
                            }

                            // ParamsArea
                            if (user.PerfilVisualizacaoServico.ParamsArea.Any())
                            {
                                var areas = user.PerfilVisualizacaoServico.ParamsArea
                                    .Select(param => !string.IsNullOrEmpty(param.CodEscritorio) && !string.IsNullOrEmpty(param.CodArea) && param.CodLocalidade.HasValue
                                        ? $"(A.COD_ESCRITORIO = '{param.CodEscritorio}' AND A.COD_AREA = '{param.CodArea}' AND A.COD_LOCALIDADE = {param.CodLocalidade.Value})"
                                        : !string.IsNullOrEmpty(param.CodEscritorio) && !string.IsNullOrEmpty(param.CodArea)
                                            ? $"(A.COD_ESCRITORIO = '{param.CodEscritorio}' AND A.COD_AREA = '{param.CodArea}')"
                                            : !string.IsNullOrEmpty(param.CodEscritorio)
                                                ? $"(A.COD_ESCRITORIO = '{param.CodEscritorio}')"
                                                : null)
                                    .Where(condition => !string.IsNullOrEmpty(condition))
                                    .ToList();

                                if (areas.Any())
                                {
                                    condicoes.Add($"({string.Join(" OR ", areas)})");
                                }
                            }

                            // ParamsSetorDepto
                            if (user.PerfilVisualizacaoServico.ParamsSetorDepto.Any())
                            {
                                var departamentos = user.PerfilVisualizacaoServico.ParamsSetorDepto
                                    .Where(param => param.CodDepto != default(short))
                                    .Select(param => $"(A.COD_DEPTO = {param.CodDepto})")
                                    .ToList();

                                if (departamentos.Any())
                                {
                                    condicoes.Add($"({string.Join(" OR ", departamentos)})");
                                }
                            }

                            // ParamsSupervisor
                            if (user.PerfilVisualizacaoServico.ParamsSupervisor.Any())
                            {
                                var supervisores = user.PerfilVisualizacaoServico.ParamsSupervisor
                                    .Where(param => !string.IsNullOrEmpty(param.CodSuperv))
                                    .Select(param => $"(A.COD_SUPERV = '{param.CodSuperv}')")
                                    .ToList();

                                if (supervisores.Any())
                                {
                                    condicoes.Add($"({string.Join(" OR ", supervisores)})");
                                }
                            }

                            // ParamsMonitor
                            if (user.PerfilVisualizacaoServico.ParamsMonitor.Any())
                            {
                                var monitores = user.PerfilVisualizacaoServico.ParamsMonitor
                                    .Where(param => !string.IsNullOrEmpty(param.UsuarioMonitor))
                                    .Select(param => $"(A.USUARIO_MONITOR = '{param.UsuarioMonitor}')")
                                    .ToList();

                                if (monitores.Any())
                                {
                                    condicoes.Add($"({string.Join(" OR ", monitores)})");
                                }
                            }
                        }

                        #endregion

                        // Combina todas as condições com AND
                        if (condicoes.Any())
                        {
                            sql.AppendLine(" WHERE ");
                            sql.AppendLine(string.Join(" AND ", condicoes));
                        }


                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                TotalMinutos += reader["TEMPO_EXECUCAO"] != DBNull.Value ? ((TimeSpan)(reader["TEMPO_EXECUCAO"])).TotalMinutes : Tempo_Execuxao_Padrao.Value.TotalMinutes;
                                DateTime? dataAbertura = reader["DIA"] != DBNull.Value ? Convert.ToDateTime(reader["DIA"]) : (DateTime?)null;

                                if (dataAbertura.HasValue == false)
                                    continue;

                                retorno.Objeto.Total++;
                                var dias = ((String.IsNullOrEmpty(request?.DataReferencia) ? data_atual : Convert.ToDateTime(request?.DataReferencia)).Date - dataAbertura.Value.Date).TotalDays;
                                if (dias <= 7)
                                    retorno.Objeto.SeteDias++;
                                else if (dias <= 10)
                                    retorno.Objeto.DezDias++;
                                else if (dias <= 15)
                                    retorno.Objeto.QuinzeDias++;
                                else if (dias <= 30)
                                    retorno.Objeto.TrintaDias++;
                                else if (dias > 30)
                                    retorno.Objeto.MaisTrintaDias++;
                            }
                        }

                    }

                    var tempoMedio = TotalMinutos / retorno.Objeto.Total;
                    var capacidade = Capacidade(sessionT5, sessionT5Nh, configGerais, horarios_servico, tempoMedio, TipoHabilidade.Instalacao);
                    var capacidadeFator = capacidade * (config?.MetaBancoInstalacao / 100);
                    var percentualFator = retorno.Objeto.Total / capacidadeFator;

                    if (capacidadeFator < retorno.Objeto.Total)
                    {
                        retorno.Objeto.MetaCumprida = 0; // vermelho
                    }
                    else if (percentualFator < 0.9)
                    {
                        retorno.Objeto.MetaCumprida = 1; // verde
                    }
                    else
                    {
                        retorno.Objeto.MetaCumprida = 2; //amarelo
                    }
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException?.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetBacklogReparo")]
        public async Task<GetApiRetorno<BacklogReparoModel>> GetBacklogReparo(Dashboard_Servicos_Request request)
        {
            GetApiRetorno<BacklogReparoModel> retorno = new GetApiRetorno<BacklogReparoModel>();
            retorno.Objeto = new BacklogReparoModel();
            double TotalMinutos = 0;
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                using (var repoConfig = new Conexao<ConfiguracoesDashboardServicos>(session))
                using (var repoHorario = new Conexao<Horario_Connect>(sessionT5Nh))
                using (var repoConfigGerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                    var usuarioBusiness = new UsuarioBusiness(repoUsuario);
                    IdentityUser user = usuarioBusiness.GetById(User.Identity.GetUserId());
                    List<Horario_Connect> horarios_servico = (repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result).ToList();
                    var horario_padrao = horarios_servico.Find(p => p.Padrao == true);
                    var config = await repoConfig.selectFirst();
                    var configGerais = await repoConfigGerais.selectFirst();
                    TimeSpan? Tempo_Execuxao_Padrao = configGerais.Tempo_Execucao_Padrao?.TimeOfDay;
                    DateTime data_atual = String.IsNullOrEmpty(request?.DataReferencia) ? DateTime.Now : Convert.ToDateTime(request?.DataReferencia);

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        var sql = new StringBuilder();
                        sql.AppendLine("SELECT A.NUM_OBRA,A.DIA,A.HORA,");
                        sql.AppendLine("(SELECT FIRST 1 M.HORAS FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M WHERE((M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO ");
                        sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A");

                        sql.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE B ");
                        sql.AppendLine("ON(A.COD_TIPO_SERV = B.COD_TIPO_SERV) ");
                        sql.AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO C ");
                        sql.AppendLine("ON(B.COD_NATUREZA = C.ID) ");
                        sql.AppendLine("LEFT JOIN TB_EQUIPE EQUIPE ");
                        sql.AppendLine("ON(A.EQUIPE = EQUIPE.EQUIPE) ");

                        List<string> condicoes = new List<string>();
                        condicoes.Add("(C.TIPO_HABILIDADE = 0)");
                        condicoes.Add("A.STATUS_EXECUCAO NOT IN (8,9,1,2)");
                        condicoes.Add("(A.DIA BETWEEN @DATAINICIOBUSCA AND @DATAFIMBUSCA)");
                        var periodo = GetPeriodoFiltro(request.TipoFiltroData.Value, data_atual);
                        parametros.Add("@DATAINICIOBUSCA", periodo.dataInicio);
                        parametros.Add("@DATAFIMBUSCA", periodo.dataFim);


                        if (String.IsNullOrEmpty(request.Contrato) == false)
                        {
                            condicoes.Add("(A.CODCT = @CONTRATO)");
                            parametros.Add("@CONTRATO", request.Contrato);
                        }

                        if (String.IsNullOrEmpty(request.Projeto) == false)
                        {
                            condicoes.Add("(A.PROJETO = @PROJETO)");
                            parametros.Add("@PROJETO", request.Projeto);
                        }

                        if (String.IsNullOrEmpty(request.Supervisor) == false)
                        {
                            condicoes.Add("(EQUIPE.COD_SUPERV = @SUPERV)");
                            parametros.Add("@SUPERV", request.Supervisor);
                        }

                        if (String.IsNullOrEmpty(request.Monitor) == false)
                        {
                            condicoes.Add("(EQUIPE.USUARIO_MONITOR = @USUARIO_MONITOR)");
                            parametros.Add("@USUARIO_MONITOR", request.Monitor);
                        }

                        if (String.IsNullOrEmpty(request.NaturezaServico) == false)
                        {
                            condicoes.Add("(B.COD_NATUREZA = @NATUREZA)");
                            parametros.Add("@NATUREZA", request.NaturezaServico);
                        }

                        if (String.IsNullOrEmpty(request.GrupoArea) == false)
                        {
                            condicoes.Add("(A.COD_ESCRITORIO = @GRUPO_AREA)");
                            parametros.Add("@GRUPO_AREA", request.GrupoArea);
                        }

                        if (String.IsNullOrEmpty(request.Area) == false)
                        {
                            condicoes.Add("(A.COD_AREA = @AREA)");
                            parametros.Add("@AREA", request.Area);
                        }

                        if (String.IsNullOrEmpty(request.Localidade) == false)
                        {
                            condicoes.Add("(A.COD_LOCALIDADE = @LOCALIDADE)");
                            parametros.Add("@LOCALIDADE", request.Localidade);
                        }

                        #region Filtro Perfil Visualização de Serviço

                        if (user.PerfilVisualizacaoServico != null)
                        {
                            // Grupo para ParamsContratoProjeto
                            if (user.PerfilVisualizacaoServico.ParamsContratoProjeto.Any())
                            {
                                var contratosProjeto = user.PerfilVisualizacaoServico.ParamsContratoProjeto
                                    .Select(paramContratoProj =>
                                        !string.IsNullOrEmpty(paramContratoProj.CodCt) && !string.IsNullOrEmpty(paramContratoProj.Projeto)
                                        ? $"(A.CODCT = '{paramContratoProj.CodCt}' AND A.PROJETO = '{paramContratoProj.Projeto}')"
                                        : $"(A.CODCT = '{paramContratoProj.CodCt}')");
                                condicoes.Add($"({string.Join(" OR ", contratosProjeto)})");
                            }

                            // Grupo para ParamsArea
                            if (user.PerfilVisualizacaoServico.ParamsArea.Any())
                            {
                                var areas = user.PerfilVisualizacaoServico.ParamsArea
                                    .Select(paramArea =>
                                        !string.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                        !string.IsNullOrEmpty(paramArea.CodArea) &&
                                        paramArea.CodLocalidade.HasValue
                                        ? $"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}' AND A.COD_LOCALIDADE = {paramArea.CodLocalidade.Value})"
                                        : (!string.IsNullOrEmpty(paramArea.CodEscritorio) && !string.IsNullOrEmpty(paramArea.CodArea)
                                            ? $"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}')"
                                            : $"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}')"));
                                condicoes.Add($"({string.Join(" OR ", areas)})");
                            }

                            // Grupo para ParamsSetorDepto
                            if (user.PerfilVisualizacaoServico.ParamsSetorDepto.Any())
                            {
                                var departamentos = user.PerfilVisualizacaoServico.ParamsSetorDepto
                                    .Select(paramSetor => $"(A.COD_DEPTO = {paramSetor.CodDepto})");
                                condicoes.Add($"({string.Join(" OR ", departamentos)})");
                            }

                            // Grupo para ParamsSupervisor
                            if (user.PerfilVisualizacaoServico.ParamsSupervisor.Any())
                            {
                                var supervisores = user.PerfilVisualizacaoServico.ParamsSupervisor
                                    .Select(paramSupervisor => $"(A.COD_SUPERV = '{paramSupervisor.CodSuperv}')");
                                condicoes.Add($"({string.Join(" OR ", supervisores)})");
                            }

                            // Grupo para ParamsMonitor
                            if (user.PerfilVisualizacaoServico.ParamsMonitor.Any())
                            {
                                var monitores = user.PerfilVisualizacaoServico.ParamsMonitor
                                    .Select(paramMonitor => $"(A.USUARIO_MONITOR = '{paramMonitor.UsuarioMonitor}')");
                                condicoes.Add($"({string.Join(" OR ", monitores)})");
                            }
                        }
                        #endregion
                        // Adiciona as condições no SQL
                        if (condicoes.Any())
                        {
                            sql.AppendLine(" WHERE ");
                            sql.AppendLine(string.Join(" AND ", condicoes));
                        }



                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {

                                TotalMinutos += reader["TEMPO_EXECUCAO"] != DBNull.Value ? ((TimeSpan)(reader["TEMPO_EXECUCAO"])).TotalMinutes : Tempo_Execuxao_Padrao.Value.TotalMinutes;

                                DateTime? dataAbertura = reader["DIA"] != DBNull.Value ? Convert.ToDateTime(reader["DIA"]) : (DateTime?)null;
                                if (dataAbertura.HasValue == false)
                                    continue;

                                retorno.Objeto.Total++;
                                var horas = ((String.IsNullOrEmpty(request?.DataReferencia) ? data_atual : Convert.ToDateTime(request?.DataReferencia)).Date - dataAbertura.Value.Date).TotalHours;
                                if (horas <= 24)
                                    retorno.Objeto.VinteQuatroHoras++;
                                else if (horas <= 36)
                                    retorno.Objeto.TrintaSeisHoras++;
                                else if (horas <= 48)
                                    retorno.Objeto.QuarentaOitoHoras++;
                                else if (horas <= 72)
                                    retorno.Objeto.SetentaDuasHoras++;
                                else if (horas > 120)
                                    retorno.Objeto.CentoVinteHoras++;
                                else if (horas > 120)
                                    retorno.Objeto.MaisCentoVinteHoras++;


                            }
                        }

                    }

                    var tempoMedio = TotalMinutos / retorno.Objeto.Total;
                    var capacidade = Capacidade(sessionT5, sessionT5Nh, configGerais, horarios_servico, tempoMedio, TipoHabilidade.Reparo);
                    var capacidadeFator = capacidade * (config?.MetaBacklogReparo / 100);
                    var percentualFator = retorno.Objeto.Total / capacidadeFator;

                    if (capacidadeFator < retorno.Objeto.Total)
                    {
                        retorno.Objeto.MetaCumprida = 0; // vermelho
                    }
                    else if (percentualFator < 0.9)
                    {
                        retorno.Objeto.MetaCumprida = 1; // verde
                    }
                    else
                    {
                        retorno.Objeto.MetaCumprida = 2; //amarelo
                    }


                }


                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException?.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetEficienciaInstalacaoMes")]
        public async Task<GetApiRetorno<EficienciaMesModel>> GetEficienciaInstalacaoMes(Dashboard_Servicos_Request request)
        {
            GetApiRetorno<EficienciaMesModel> retorno = new GetApiRetorno<EficienciaMesModel>();
            retorno.Objeto = new EficienciaMesModel();
            double TotalMinutos = 0;
            double Total7Dias = 0;
            double Total7DiasProdutivas = 0;

            double Total15Dias = 0;
            double Total15DiasProdutivas = 0;

            Dictionary<DateTime, List<(string numObra, String equipe)>> Produtivos = new Dictionary<DateTime, List<(string numObra, string equipe)>>();
            var dataReferencia = String.IsNullOrEmpty(request?.DataReferencia) ? DateTime.Now : Convert.ToDateTime(request?.DataReferencia);
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                using (var repoConfig = new Conexao<ConfiguracoesDashboardServicos>(session))
                using (var repoHorario = new Conexao<Horario_Connect>(sessionT5Nh))
                using (var repoConfigGerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                    var usuarioBusiness = new UsuarioBusiness(repoUsuario);
                    IdentityUser user = usuarioBusiness.GetById(User.Identity.GetUserId());
                    List<Horario_Connect> horarios_servico = (repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result).ToList();
                    var horario_padrao = horarios_servico.Find(p => p.Padrao == true);
                    var config = await repoConfig.selectFirst();
                    var configGerais = await repoConfigGerais.selectFirst();
                    TimeSpan? Tempo_Execuxao_Padrao = configGerais.Tempo_Execucao_Padrao?.TimeOfDay;

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        var sql = new StringBuilder();
                        sql.AppendLine("SELECT A.NUM_OBRA,A.DIA,A.HORA,A.DATA_PROD,A.STATUS_EXECUCAO,A.EQUIPE,");
                        sql.AppendLine("(SELECT FIRST 1 M.HORAS FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M WHERE((M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO ");
                        sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A");

                        sql.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE B ");
                        sql.AppendLine("ON(A.COD_TIPO_SERV = B.COD_TIPO_SERV) ");
                        sql.AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO C ");
                        sql.AppendLine("ON(B.COD_NATUREZA = C.ID) ");
                        sql.AppendLine("LEFT JOIN TB_EQUIPE EQUIPE ");
                        sql.AppendLine("ON(A.EQUIPE = EQUIPE.EQUIPE) ");

                        List<string> condicoes = new List<string>();
                        condicoes.Add("(C.TIPO_HABILIDADE = 1)");
                        condicoes.Add("(A.STATUS_EXECUCAO = 1 OR A.STATUS_EXECUCAO = 2)");
                        condicoes.Add("(A.DATA_MAXIMA BETWEEN @DATAINICIOBUSCA AND @DATAFIMBUSCA)");

                        var periodo = GetPeriodoFiltro(request.TipoFiltroData.Value, dataReferencia);
                        parametros.Add("@DATAINICIOBUSCA", periodo.dataInicio);
                        parametros.Add("@DATAFIMBUSCA", periodo.dataFim);


                        if (String.IsNullOrEmpty(request.Contrato) == false)
                        {
                            condicoes.Add("(A.CODCT = @CONTRATO)");
                            parametros.Add("@CONTRATO", request.Contrato);
                        }

                        if (String.IsNullOrEmpty(request.Projeto) == false)
                        {
                            condicoes.Add("(A.PROJETO = @PROJETO)");
                            parametros.Add("@PROJETO", request.Projeto);
                        }

                        if (String.IsNullOrEmpty(request.Supervisor) == false)
                        {
                            condicoes.Add("(EQUIPE.COD_SUPERV = @SUPERV)");
                            parametros.Add("@SUPERV", request.Supervisor);
                        }

                        if (String.IsNullOrEmpty(request.Monitor) == false)
                        {
                            condicoes.Add("(EQUIPE.USUARIO_MONITOR = @USUARIO_MONITOR)");
                            parametros.Add("@USUARIO_MONITOR", request.Monitor);
                        }

                        if (String.IsNullOrEmpty(request.NaturezaServico) == false)
                        {
                            condicoes.Add("(B.COD_NATUREZA = @NATUREZA)");
                            parametros.Add("@NATUREZA", request.NaturezaServico);
                        }

                        if (String.IsNullOrEmpty(request.GrupoArea) == false)
                        {
                            condicoes.Add("(A.COD_ESCRITORIO = @GRUPO_AREA)");
                            parametros.Add("@GRUPO_AREA", request.GrupoArea);
                        }

                        if (String.IsNullOrEmpty(request.Area) == false)
                        {
                            condicoes.Add("(A.COD_AREA = @AREA)");
                            parametros.Add("@AREA", request.Area);
                        }

                        if (String.IsNullOrEmpty(request.Localidade) == false)
                        {
                            condicoes.Add("(A.COD_LOCALIDADE = @LOCALIDADE)");
                            parametros.Add("@LOCALIDADE", request.Localidade);
                        }

                        #region Filtro Perfil Visualização de Serviço

                        if (user.PerfilVisualizacaoServico != null)
                        {
                            // Filtro por Contrato e Projeto
                            if (user.PerfilVisualizacaoServico.ParamsContratoProjeto.Count() > 0)
                            {
                                List<string> contratosProjeto = new List<string>();
                                foreach (var paramContratoProj in user.PerfilVisualizacaoServico.ParamsContratoProjeto)
                                {
                                    if (!String.IsNullOrEmpty(paramContratoProj.CodCt) && !String.IsNullOrEmpty(paramContratoProj.Projeto))
                                    {
                                        contratosProjeto.Add($"(A.CODCT = '{paramContratoProj.CodCt}' AND A.PROJETO = '{paramContratoProj.Projeto}')");
                                    }
                                    else if (!String.IsNullOrEmpty(paramContratoProj.CodCt))
                                    {
                                        contratosProjeto.Add($"(A.CODCT = '{paramContratoProj.CodCt}')");
                                    }
                                }

                                if (contratosProjeto.Count > 0)
                                    condicoes.Add($"({String.Join(" OR ", contratosProjeto)})");
                            }

                            // Filtro por Área
                            if (user.PerfilVisualizacaoServico.ParamsArea.Count() > 0)
                            {
                                List<string> areas = new List<string>();
                                foreach (var paramArea in user.PerfilVisualizacaoServico.ParamsArea)
                                {
                                    if (!String.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                        !String.IsNullOrEmpty(paramArea.CodArea) &&
                                        paramArea.CodLocalidade.HasValue)
                                    {
                                        areas.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}' AND A.COD_LOCALIDADE = {paramArea.CodLocalidade.Value})");
                                    }
                                    else if (!String.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                             !String.IsNullOrEmpty(paramArea.CodArea))
                                    {
                                        areas.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}')");
                                    }
                                    else if (!String.IsNullOrEmpty(paramArea.CodEscritorio))
                                    {
                                        areas.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}')");
                                    }
                                }

                                if (areas.Count > 0)
                                    condicoes.Add($"({String.Join(" OR ", areas)})");
                            }

                            // Filtro por Setor/Departamento
                            if (user.PerfilVisualizacaoServico.ParamsSetorDepto.Count() > 0)
                            {
                                List<string> departamentos = new List<string>();
                                foreach (var paramSetor in user.PerfilVisualizacaoServico.ParamsSetorDepto)
                                {
                                    if (paramSetor.CodDepto != default(short)) // Verifica se o valor não é o padrão para short (0)
                                    {
                                        departamentos.Add($"(A.COD_DEPTO = {paramSetor.CodDepto})");
                                    }
                                }


                                if (departamentos.Count > 0)
                                    condicoes.Add($"({String.Join(" OR ", departamentos)})");
                            }

                            // Filtro por Supervisor
                            if (user.PerfilVisualizacaoServico.ParamsSupervisor.Count() > 0)
                            {
                                List<string> supervisores = new List<string>();
                                foreach (var paramSupervisor in user.PerfilVisualizacaoServico.ParamsSupervisor)
                                {
                                    if (!String.IsNullOrEmpty(paramSupervisor.CodSuperv))
                                        supervisores.Add($"(A.COD_SUPERV = '{paramSupervisor.CodSuperv}')");
                                }

                                if (supervisores.Count > 0)
                                    condicoes.Add($"({String.Join(" OR ", supervisores)})");
                            }

                            // Filtro por Monitor
                            if (user.PerfilVisualizacaoServico.ParamsMonitor.Count() > 0)
                            {
                                List<string> monitores = new List<string>();
                                foreach (var paramMonitor in user.PerfilVisualizacaoServico.ParamsMonitor)
                                {
                                    if (!String.IsNullOrEmpty(paramMonitor.UsuarioMonitor))
                                        monitores.Add($"(A.USUARIO_MONITOR = '{paramMonitor.UsuarioMonitor}')");
                                }

                                if (monitores.Count > 0)
                                    condicoes.Add($"({String.Join(" OR ", monitores)})");
                            }
                        }

                        #endregion

                        // Adicionando condições ao SQL
                        if (condicoes.Count > 0)
                        {
                            sql.AppendLine(" WHERE ");
                            sql.AppendLine(String.Join(" AND ", condicoes));
                        }



                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {

                                TotalMinutos += reader["TEMPO_EXECUCAO"] != DBNull.Value ? ((TimeSpan)(reader["TEMPO_EXECUCAO"])).TotalMinutes : Tempo_Execuxao_Padrao.Value.TotalMinutes;


                                DateTime? dataAbertura = reader["DIA"] != DBNull.Value ? Convert.ToDateTime(reader["DIA"]) : (DateTime?)null;
                                if (dataAbertura.HasValue == false)
                                    continue;

                                retorno.Objeto.Total++;
                                DateTime? dataProd = reader["DATA_PROD"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_PROD"]) : (DateTime?)null;


                                if (Convert.ToInt16(reader["STATUS_EXECUCAO"]) == 2)
                                {
                                    if (dataProd.HasValue)
                                    {
                                        if (Produtivos.ContainsKey(dataProd.Value) == false)
                                        {
                                            Produtivos.Add(dataProd.Value, new List<(string numObra, string equipe)>());

                                        }

                                        Produtivos[dataProd.Value].Add((numObra: Convert.ToString(reader["NUM_OBRA"]), equipe: Convert.ToString(reader["EQUIPE"])));

                                    }
                                }


                                var dias = (dataReferencia.Date - dataAbertura.Value.Date).TotalDays;
                                if (dias <= 7)
                                {
                                    Total7Dias++;
                                    if (Convert.ToInt16(reader["STATUS_EXECUCAO"]) == 2)
                                    {
                                        Total7DiasProdutivas++;
                                    }

                                }

                                if (dias <= 15)
                                {
                                    Total15Dias++;

                                    if (Convert.ToInt16(reader["STATUS_EXECUCAO"]) == 2)
                                    {
                                        Total15DiasProdutivas++;
                                    }

                                }
                            }
                        }



                        var resultadoSeteDias = Total7Dias != 0 ? (Total7DiasProdutivas / Total7Dias) * 100 : 0;
                        var resultadoQuinzeDias = Total15Dias != 0 ? (Total15DiasProdutivas / Total15Dias) * 100 : 0;
                        retorno.Objeto.QuinzeDias = Math.Round(resultadoQuinzeDias, 0);
                        retorno.Objeto.SeteDias = Math.Round(resultadoSeteDias, 0);

                        if (resultadoSeteDias >= config?.MetaEficienciaMesInstalacao)
                        {
                            retorno.Objeto.MetaSeteDias = 1;
                        }
                        else if (Math.Round(resultadoSeteDias, 0) >= config?.MetaEficienciaMesInstalacao)
                        {
                            retorno.Objeto.MetaSeteDias = 2;
                        }
                        else
                        {
                            retorno.Objeto.MetaSeteDias = 0;
                        }


                        if (resultadoQuinzeDias >= config?.MetaEficienciaMesInstalacao)
                        {
                            retorno.Objeto.MetaQuinzeDias = 1;
                        }
                        else if (Math.Round(resultadoQuinzeDias, 0) >= config?.MetaEficienciaMesInstalacao)
                        {
                            retorno.Objeto.MetaQuinzeDias = 2;
                        }
                        else
                        {
                            retorno.Objeto.MetaQuinzeDias = 0;
                        }

                        int numOSPeriodo = 0;
                        int qtdeTecnicoPeriodo = 0;
                        foreach (var item in Produtivos)
                        {
                            int numOSDia = item.Value.Count;
                            int qtdeTecnicosDia = item.Value.Select(p => p.equipe).Distinct().Count();

                            numOSPeriodo += numOSDia;
                            qtdeTecnicoPeriodo += qtdeTecnicosDia;
                        }

                        double MediaProdutiva = (double)numOSPeriodo / (double)qtdeTecnicoPeriodo;
                        retorno.Objeto.MediaProdutiva = Math.Round(MediaProdutiva, 1);

                    }




                }


                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException?.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetEficienciaReparoMes")]
        public async Task<GetApiRetorno<EficienciaMesModel>> GetEficienciaReparoMes(Dashboard_Servicos_Request request)
        {
            GetApiRetorno<EficienciaMesModel> retorno = new GetApiRetorno<EficienciaMesModel>();
            retorno.Objeto = new EficienciaMesModel();
            double TotalMinutos = 0;
            double Total24Horas = 0;
            double Total48Horas = 0;
            double Total24HorasProdutivas = 0;
            double Total48HorasProdutivas = 0;
            Dictionary<DateTime, List<(string numObra, String equipe)>> Produtivos = new Dictionary<DateTime, List<(string numObra, string equipe)>>();
            var dataReferencia = String.IsNullOrEmpty(request?.DataReferencia) ? DateTime.Now : Convert.ToDateTime(request?.DataReferencia);
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                using (var repoConfig = new Conexao<ConfiguracoesDashboardServicos>(session))
                using (var repoHorario = new Conexao<Horario_Connect>(sessionT5Nh))
                using (var repoConfigGerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                    var usuarioBusiness = new UsuarioBusiness(repoUsuario);
                    IdentityUser user = usuarioBusiness.GetById(User.Identity.GetUserId());
                    List<Horario_Connect> horarios_servico = (repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result).ToList();
                    var horario_padrao = horarios_servico.Find(p => p.Padrao == true);
                    var config = await repoConfig.selectFirst();
                    var configGerais = await repoConfigGerais.selectFirst();
                    TimeSpan? Tempo_Execuxao_Padrao = configGerais.Tempo_Execucao_Padrao?.TimeOfDay;

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        var sql = new StringBuilder();
                        sql.AppendLine("SELECT A.NUM_OBRA,A.DIA,A.HORA,A.DATA_PROD,A.STATUS_EXECUCAO,A.EQUIPE,");
                        sql.AppendLine("(SELECT FIRST 1 M.HORAS FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M WHERE((M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO ");
                        sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A");

                        sql.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE B ");
                        sql.AppendLine("ON(A.COD_TIPO_SERV = B.COD_TIPO_SERV) ");
                        sql.AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO C ");
                        sql.AppendLine("ON(B.COD_NATUREZA = C.ID) ");
                        sql.AppendLine("LEFT JOIN TB_EQUIPE EQUIPE ");
                        sql.AppendLine("ON(A.EQUIPE = EQUIPE.EQUIPE) ");

                        List<string> condicoes = new List<string>();
                        condicoes.Add("(C.TIPO_HABILIDADE = 0)");
                        condicoes.Add("(A.STATUS_EXECUCAO = 1 OR A.STATUS_EXECUCAO = 2)");
                        condicoes.Add("(A.DATA_MAXIMA BETWEEN @DATAINICIOBUSCA AND @DATAFIMBUSCA)");

                        var periodo = GetPeriodoFiltro(request.TipoFiltroData.Value, dataReferencia);
                        parametros.Add("@DATAINICIOBUSCA", periodo.dataInicio);
                        parametros.Add("@DATAFIMBUSCA", periodo.dataFim);


                        if (String.IsNullOrEmpty(request.Contrato) == false)
                        {
                            condicoes.Add("(A.CODCT = @CONTRATO)");
                            parametros.Add("@CONTRATO", request.Contrato);
                        }

                        if (String.IsNullOrEmpty(request.Projeto) == false)
                        {
                            condicoes.Add("(A.PROJETO = @PROJETO)");
                            parametros.Add("@PROJETO", request.Projeto);
                        }

                        if (String.IsNullOrEmpty(request.Supervisor) == false)
                        {
                            condicoes.Add("(EQUIPE.COD_SUPERV = @SUPERV)");
                            parametros.Add("@SUPERV", request.Supervisor);
                        }

                        if (String.IsNullOrEmpty(request.Monitor) == false)
                        {
                            condicoes.Add("(EQUIPE.USUARIO_MONITOR = @USUARIO_MONITOR)");
                            parametros.Add("@USUARIO_MONITOR", request.Monitor);
                        }

                        if (String.IsNullOrEmpty(request.NaturezaServico) == false)
                        {
                            condicoes.Add("(B.COD_NATUREZA = @NATUREZA)");
                            parametros.Add("@NATUREZA", request.NaturezaServico);
                        }

                        if (String.IsNullOrEmpty(request.GrupoArea) == false)
                        {
                            condicoes.Add("(A.COD_ESCRITORIO = @GRUPO_AREA)");
                            parametros.Add("@GRUPO_AREA", request.GrupoArea);
                        }

                        if (String.IsNullOrEmpty(request.Area) == false)
                        {
                            condicoes.Add("(A.COD_AREA = @AREA)");
                            parametros.Add("@AREA", request.Area);
                        }

                        if (String.IsNullOrEmpty(request.Localidade) == false)
                        {
                            condicoes.Add("(A.COD_LOCALIDADE = @LOCALIDADE)");
                            parametros.Add("@LOCALIDADE", request.Localidade);
                        }

                        #region Filtro Perfil Visualização de Serviço

                        if (user.PerfilVisualizacaoServico != null)
                        {
                            // ParamsContratoProjeto
                            if (user.PerfilVisualizacaoServico.ParamsContratoProjeto.Count() > 0)
                            {
                                List<string> contratosProjeto = new List<string>();
                                foreach (var paramContratoProj in user.PerfilVisualizacaoServico.ParamsContratoProjeto)
                                {
                                    if (!String.IsNullOrEmpty(paramContratoProj.CodCt) && !String.IsNullOrEmpty(paramContratoProj.Projeto))
                                    {
                                        contratosProjeto.Add($"(A.CODCT = '{paramContratoProj.CodCt}' AND A.PROJETO = '{paramContratoProj.Projeto}')");
                                    }
                                    else if (!String.IsNullOrEmpty(paramContratoProj.CodCt))
                                    {
                                        contratosProjeto.Add($"(A.CODCT = '{paramContratoProj.CodCt}')");
                                    }
                                }

                                if (contratosProjeto.Count > 0)
                                {
                                    condicoes.Add($"({String.Join(" OR ", contratosProjeto)})");
                                }
                            }

                            // ParamsArea
                            if (user.PerfilVisualizacaoServico.ParamsArea.Count() > 0)
                            {
                                List<string> areas = new List<string>();
                                foreach (var paramArea in user.PerfilVisualizacaoServico.ParamsArea)
                                {
                                    if (!String.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                        !String.IsNullOrEmpty(paramArea.CodArea) &&
                                        paramArea.CodLocalidade.HasValue)
                                    {
                                        areas.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}' AND A.COD_LOCALIDADE = {paramArea.CodLocalidade.Value})");
                                    }
                                    else if (!String.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                             !String.IsNullOrEmpty(paramArea.CodArea))
                                    {
                                        areas.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}')");
                                    }
                                    else if (!String.IsNullOrEmpty(paramArea.CodEscritorio))
                                    {
                                        areas.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}')");
                                    }
                                }

                                if (areas.Count > 0)
                                {
                                    condicoes.Add($"({String.Join(" OR ", areas)})");
                                }
                            }

                            // ParamsSetorDepto
                            if (user.PerfilVisualizacaoServico.ParamsSetorDepto.Count() > 0)
                            {
                                List<string> departamentos = new List<string>();
                                foreach (var paramSetor in user.PerfilVisualizacaoServico.ParamsSetorDepto)
                                {
                                    departamentos.Add($"(A.COD_DEPTO = {paramSetor.CodDepto})");
                                }

                                if (departamentos.Count > 0)
                                {
                                    condicoes.Add($"({String.Join(" OR ", departamentos)})");
                                }
                            }

                            // ParamsSupervisor
                            if (user.PerfilVisualizacaoServico.ParamsSupervisor.Count() > 0)
                            {
                                List<string> supervisores = new List<string>();
                                foreach (var paramSupervisor in user.PerfilVisualizacaoServico.ParamsSupervisor)
                                {
                                    supervisores.Add($"(A.COD_SUPERV = '{paramSupervisor.CodSuperv}')");
                                }

                                if (supervisores.Count > 0)
                                {
                                    condicoes.Add($"({String.Join(" OR ", supervisores)})");
                                }
                            }

                            // ParamsMonitor
                            if (user.PerfilVisualizacaoServico.ParamsMonitor.Count() > 0)
                            {
                                List<string> monitores = new List<string>();
                                foreach (var paramMonitor in user.PerfilVisualizacaoServico.ParamsMonitor)
                                {
                                    monitores.Add($"(A.USUARIO_MONITOR = '{paramMonitor.UsuarioMonitor}')");
                                }

                                if (monitores.Count > 0)
                                {
                                    condicoes.Add($"({String.Join(" OR ", monitores)})");
                                }
                            }
                        }

                        #endregion
                        // Combina todas as condições com AND
                        if (condicoes.Count > 0)
                        {
                            sql.AppendLine(" WHERE ");
                            sql.AppendLine(String.Join(" AND ", condicoes));
                        }




                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {

                                TotalMinutos += reader["TEMPO_EXECUCAO"] != DBNull.Value ? ((TimeSpan)(reader["TEMPO_EXECUCAO"])).TotalMinutes : Tempo_Execuxao_Padrao.Value.TotalMinutes;


                                DateTime? dataAbertura = reader["DIA"] != DBNull.Value ? Convert.ToDateTime(reader["DIA"]) : (DateTime?)null;
                                if (dataAbertura.HasValue == false)
                                    continue;

                                retorno.Objeto.Total++;
                                DateTime? dataProd = reader["DATA_PROD"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_PROD"]) : (DateTime?)null;

                                if (Convert.ToInt16(reader["STATUS_EXECUCAO"]) == 2)
                                {
                                    if (dataProd.HasValue)
                                    {
                                        if (Produtivos.ContainsKey(dataProd.Value) == false)
                                        {
                                            Produtivos.Add(dataProd.Value, new List<(string numObra, string equipe)>());

                                        }

                                        Produtivos[dataProd.Value].Add((numObra: Convert.ToString(reader["NUM_OBRA"]), equipe: Convert.ToString(reader["EQUIPE"])));

                                    }
                                }



                                var horas = (dataReferencia.Date - dataAbertura.Value.Date).TotalHours;
                                if (horas <= 24)
                                {
                                    Total24Horas++;

                                    if (Convert.ToInt16(reader["STATUS_EXECUCAO"]) == 2)
                                    {
                                        Total24HorasProdutivas++;
                                    }
                                }

                                if (horas <= 48)
                                {
                                    Total48Horas++;

                                    if (Convert.ToInt16(reader["STATUS_EXECUCAO"]) == 2)
                                    {
                                        Total48HorasProdutivas++;
                                    }
                                }
                            }
                        }


                        var resultado24Horas = Total24Horas != 0 ? (Total24HorasProdutivas / Total24Horas) * 100 : 0;
                        var resultado48Horas = Total48Horas != 0 ? (Total48HorasProdutivas / Total48Horas) * 100 : 0;
                        retorno.Objeto.QuinzeDias = Math.Round(resultado48Horas, 0);
                        retorno.Objeto.SeteDias = Math.Round(resultado24Horas, 0);

                        if (resultado24Horas >= config?.MetaEficienciaMesReparo)
                        {
                            retorno.Objeto.MetaSeteDias = 1;
                        }
                        else if (Math.Round(resultado24Horas, 0) >= config?.MetaEficienciaMesReparo)
                        {
                            retorno.Objeto.MetaSeteDias = 2;
                        }
                        else
                        {
                            retorno.Objeto.MetaSeteDias = 0;
                        }


                        if (resultado48Horas >= config?.MetaEficienciaMesReparo)
                        {
                            retorno.Objeto.MetaQuinzeDias = 1;
                        }
                        else if (Math.Round(resultado48Horas, 0) >= config?.MetaEficienciaMesReparo)
                        {
                            retorno.Objeto.MetaQuinzeDias = 2;
                        }
                        else
                        {
                            retorno.Objeto.MetaQuinzeDias = 0;
                        }

                        int numOSPeriodo = 0;
                        int qtdeTecnicoPeriodo = 0;
                        foreach (var item in Produtivos)
                        {
                            int numOSDia = item.Value.Count;
                            int qtdeTecnicosDia = item.Value.Select(p => p.equipe).Distinct().Count();

                            numOSPeriodo += numOSDia;
                            qtdeTecnicoPeriodo += qtdeTecnicosDia;
                        }

                        double MediaProdutiva = (double)numOSPeriodo / (double)qtdeTecnicoPeriodo;

                        retorno.Objeto.MediaProdutiva = Math.Round(MediaProdutiva, 1);

                    }




                }


                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException?.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetEficienciaDia")]
        public async Task<GetApiRetorno<EficienciaDiaModel>> GetEficienciaDia(Dashboard_Servicos_Request request)
        {
            GetApiRetorno<EficienciaDiaModel> retorno = new GetApiRetorno<EficienciaDiaModel>();
            retorno.Objeto = new EficienciaDiaModel();
            double TotalMinutos = 0;
            double Total = 0;
            double TotalReparo = 0;
            double TotalInstalacao = 0;
            double TotalOutros = 0;
            double TotalReparoProdutivas = 0;
            double TotalInstalacaoProdutivas = 0;
            double TotalOutrosProdutivas = 0;
            double TotalProdutivos = 0;
            var dataReferencia = String.IsNullOrEmpty(request?.DataReferencia) ? DateTime.Now : Convert.ToDateTime(request?.DataReferencia);
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                using (var repoConfig = new Conexao<ConfiguracoesDashboardServicos>(session))
                using (var repoHorario = new Conexao<Horario_Connect>(sessionT5Nh))
                using (var repoConfigGerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                    var usuarioBusiness = new UsuarioBusiness(repoUsuario);
                    IdentityUser user = usuarioBusiness.GetById(User.Identity.GetUserId());
                    List<Horario_Connect> horarios_servico = (repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result).ToList();
                    var horario_padrao = horarios_servico.Find(p => p.Padrao == true);
                    var config = await repoConfig.selectFirst();
                    var configGerais = await repoConfigGerais.selectFirst();
                    TimeSpan? Tempo_Execuxao_Padrao = configGerais.Tempo_Execucao_Padrao?.TimeOfDay;

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        var sql = new StringBuilder();
                        sql.AppendLine("SELECT A.NUM_OBRA,A.DIA,A.HORA,A.DATA_PROD,A.STATUS_EXECUCAO,C.TIPO_HABILIDADE,");
                        sql.AppendLine("(SELECT FIRST 1 M.HORAS FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M WHERE((M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO ");
                        sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A");

                        sql.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE B ");
                        sql.AppendLine("ON(A.COD_TIPO_SERV = B.COD_TIPO_SERV) ");
                        sql.AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO C ");
                        sql.AppendLine("ON(B.COD_NATUREZA = C.ID) ");
                        sql.AppendLine("LEFT JOIN TB_EQUIPE EQUIPE ");
                        sql.AppendLine("ON(A.EQUIPE = EQUIPE.EQUIPE) ");

                        List<string> condicoes = new List<string>();
                        condicoes.Add("(A.STATUS_EXECUCAO = 1 OR A.STATUS_EXECUCAO = 2)");
                        condicoes.Add("(A.DATA_MAXIMA BETWEEN @DATAINICIOBUSCA AND @DATAFIMBUSCA)");

                        var periodo = GetPeriodoFiltro(request.TipoFiltroData.Value, dataReferencia);
                        parametros.Add("@DATAINICIOBUSCA", periodo.dataInicio);
                        parametros.Add("@DATAFIMBUSCA", periodo.dataFim);


                        if (String.IsNullOrEmpty(request.Contrato) == false)
                        {
                            condicoes.Add("(A.CODCT = @CONTRATO)");
                            parametros.Add("@CONTRATO", request.Contrato);
                        }

                        if (String.IsNullOrEmpty(request.Projeto) == false)
                        {
                            condicoes.Add("(A.PROJETO = @PROJETO)");
                            parametros.Add("@PROJETO", request.Projeto);
                        }

                        if (String.IsNullOrEmpty(request.Supervisor) == false)
                        {
                            condicoes.Add("(EQUIPE.COD_SUPERV = @SUPERV)");
                            parametros.Add("@SUPERV", request.Supervisor);
                        }

                        if (String.IsNullOrEmpty(request.Monitor) == false)
                        {
                            condicoes.Add("(EQUIPE.USUARIO_MONITOR = @USUARIO_MONITOR)");
                            parametros.Add("@USUARIO_MONITOR", request.Monitor);
                        }

                        if (String.IsNullOrEmpty(request.NaturezaServico) == false)
                        {
                            condicoes.Add("(B.COD_NATUREZA = @NATUREZA)");
                            parametros.Add("@NATUREZA", request.NaturezaServico);
                        }

                        if (String.IsNullOrEmpty(request.GrupoArea) == false)
                        {
                            condicoes.Add("(A.COD_ESCRITORIO = @GRUPO_AREA)");
                            parametros.Add("@GRUPO_AREA", request.GrupoArea);
                        }

                        if (String.IsNullOrEmpty(request.Area) == false)
                        {
                            condicoes.Add("(A.COD_AREA = @AREA)");
                            parametros.Add("@AREA", request.Area);
                        }

                        if (String.IsNullOrEmpty(request.Localidade) == false)
                        {
                            condicoes.Add("(A.COD_LOCALIDADE = @LOCALIDADE)");
                            parametros.Add("@LOCALIDADE", request.Localidade);
                        }

                        #region Filtro Perfil Visualização de Serviço

                        if (user.PerfilVisualizacaoServico != null)
                        {
                            // ParamsContratoProjeto
                            if (user.PerfilVisualizacaoServico.ParamsContratoProjeto.Count() > 0)
                            {
                                List<string> contratosProjeto = new List<string>();
                                foreach (var paramContratoProj in user.PerfilVisualizacaoServico.ParamsContratoProjeto)
                                {
                                    if (!String.IsNullOrEmpty(paramContratoProj.CodCt) && !String.IsNullOrEmpty(paramContratoProj.Projeto))
                                    {
                                        contratosProjeto.Add($"(A.CODCT = '{paramContratoProj.CodCt}' AND A.PROJETO = '{paramContratoProj.Projeto}')");
                                    }
                                    else if (!String.IsNullOrEmpty(paramContratoProj.CodCt))
                                    {
                                        contratosProjeto.Add($"(A.CODCT = '{paramContratoProj.CodCt}')");
                                    }
                                }

                                if (contratosProjeto.Count > 0)
                                {
                                    condicoes.Add($"({String.Join(" OR ", contratosProjeto)})");
                                }
                            }

                            // ParamsArea
                            if (user.PerfilVisualizacaoServico.ParamsArea.Count() > 0)
                            {
                                List<string> areas = new List<string>();
                                foreach (var paramArea in user.PerfilVisualizacaoServico.ParamsArea)
                                {
                                    if (!String.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                        !String.IsNullOrEmpty(paramArea.CodArea) &&
                                        paramArea.CodLocalidade.HasValue)
                                    {
                                        areas.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}' AND A.COD_LOCALIDADE = {paramArea.CodLocalidade.Value})");
                                    }
                                    else if (!String.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                             !String.IsNullOrEmpty(paramArea.CodArea))
                                    {
                                        areas.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}')");
                                    }
                                    else if (!String.IsNullOrEmpty(paramArea.CodEscritorio))
                                    {
                                        areas.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}')");
                                    }
                                }

                                if (areas.Count > 0)
                                {
                                    condicoes.Add($"({String.Join(" OR ", areas)})");
                                }
                            }

                            // ParamsSetorDepto
                            if (user.PerfilVisualizacaoServico.ParamsSetorDepto.Count() > 0)
                            {
                                List<string> departamentos = new List<string>();
                                foreach (var paramSetor in user.PerfilVisualizacaoServico.ParamsSetorDepto)
                                {
                                    departamentos.Add($"(A.COD_DEPTO = {paramSetor.CodDepto})");
                                }

                                if (departamentos.Count > 0)
                                {
                                    condicoes.Add($"({String.Join(" OR ", departamentos)})");
                                }
                            }

                            // ParamsSupervisor
                            if (user.PerfilVisualizacaoServico.ParamsSupervisor.Count() > 0)
                            {
                                List<string> supervisores = new List<string>();
                                foreach (var paramSupervisor in user.PerfilVisualizacaoServico.ParamsSupervisor)
                                {
                                    supervisores.Add($"(A.COD_SUPERV = '{paramSupervisor.CodSuperv}')");
                                }

                                if (supervisores.Count > 0)
                                {
                                    condicoes.Add($"({String.Join(" OR ", supervisores)})");
                                }
                            }

                            // ParamsMonitor
                            if (user.PerfilVisualizacaoServico.ParamsMonitor.Count() > 0)
                            {
                                List<string> monitores = new List<string>();
                                foreach (var paramMonitor in user.PerfilVisualizacaoServico.ParamsMonitor)
                                {
                                    monitores.Add($"(A.USUARIO_MONITOR = '{paramMonitor.UsuarioMonitor}')");
                                }

                                if (monitores.Count > 0)
                                {
                                    condicoes.Add($"({String.Join(" OR ", monitores)})");
                                }
                            }
                        }

                        #endregion

                        // Combina todas as condições com AND
                        if (condicoes.Count > 0)
                        {
                            sql.AppendLine(" WHERE ");
                            sql.AppendLine(String.Join(" AND ", condicoes));
                        }



                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {

                                TotalMinutos += reader["TEMPO_EXECUCAO"] != DBNull.Value ? ((TimeSpan)(reader["TEMPO_EXECUCAO"])).TotalMinutes : Tempo_Execuxao_Padrao.Value.TotalMinutes;


                                DateTime? dataAbertura = reader["DIA"] != DBNull.Value ? Convert.ToDateTime(reader["DIA"]) : (DateTime?)null;
                                if (dataAbertura.HasValue == false)
                                    continue;

                                Total++;

                                var tipoHabilidade = reader["TIPO_HABILIDADE"] != DBNull.Value ? Convert.ToInt16(reader["TIPO_HABILIDADE"]) : (short?)null;
                                if (tipoHabilidade == 0)
                                    TotalReparo++;
                                else if (tipoHabilidade == 1)
                                    TotalInstalacao++;
                                else
                                    TotalOutros++;


                                if (Convert.ToInt16(reader["STATUS_EXECUCAO"]) == 2)
                                {
                                    TotalProdutivos++;

                                    if (tipoHabilidade == 0)
                                        TotalReparoProdutivas++;
                                    else if (tipoHabilidade == 1)
                                        TotalInstalacaoProdutivas++;
                                    else
                                        TotalOutrosProdutivas++;
                                }


                            }
                        }

                        var resultadoInstalacao = TotalInstalacao != 0 ? (TotalInstalacaoProdutivas / TotalInstalacao) * 100 : 0;
                        var resultadoReparo = TotalReparo != 0 ? (TotalReparoProdutivas / TotalReparo) * 100 : 0;
                        var resultadoOutros = TotalOutros != 0 ? (TotalOutrosProdutivas / TotalOutros) * 100 : 0;
                        var resultadoTotal = Total != 0 ? (TotalProdutivos / Total) * 100 : 0;


                        retorno.Objeto.Instalacao = Total != 0 ? Math.Round(resultadoInstalacao, 0) : 0;
                        retorno.Objeto.Reparo = Total != 0 ? Math.Round(resultadoReparo, 0) : 0;
                        retorno.Objeto.Outros = Total != 0 ? Math.Round(resultadoOutros, 0) : 0;
                        retorno.Objeto.Total = Total != 0 ? Math.Round(resultadoTotal, 0) : 0;

                        //instalacao
                        if (resultadoInstalacao >= config?.MetaEficienciaDiaInstalacao)
                        {
                            retorno.Objeto.MetaInstalacao = 1;
                        }
                        else if (Math.Round(resultadoInstalacao, 0) >= config?.MetaEficienciaDiaInstalacao)
                        {
                            retorno.Objeto.MetaInstalacao = 2;
                        }
                        else
                        {
                            retorno.Objeto.MetaInstalacao = 0;
                        }

                        //reparo
                        if (resultadoReparo >= config?.MetaEficienciaDiaReparo)
                        {
                            retorno.Objeto.MetaReparo = 1;
                        }
                        else if (Math.Round(resultadoReparo, 0) >= config?.MetaEficienciaDiaReparo)
                        {
                            retorno.Objeto.MetaReparo = 2;
                        }
                        else
                        {
                            retorno.Objeto.MetaReparo = 0;
                        }

                        //outros
                        if (resultadoOutros >= config?.MetaEficienciaDiaOutros)
                        {
                            retorno.Objeto.MetaOutros = 1;
                        }
                        else if (Math.Round(resultadoOutros, 0) >= config?.MetaEficienciaDiaOutros)
                        {
                            retorno.Objeto.MetaOutros = 2;
                        }
                        else
                        {
                            retorno.Objeto.MetaOutros = 0;
                        }

                        //total
                        if (resultadoTotal >= config?.MetaEficienciaDiaTotal)
                        {
                            retorno.Objeto.MetaTotal = 1;
                        }
                        else if (Math.Round(resultadoTotal, 0) >= config?.MetaEficienciaDiaTotal)
                        {
                            retorno.Objeto.MetaTotal = 2;
                        }
                        else
                        {
                            retorno.Objeto.MetaTotal = 0;
                        }

                    }
                }


                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException?.Message;
            }

            return retorno;
        }

        #region DASHBOARD MODELO2
        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetPainelEncerramentos")]
        public async Task<DataTableResponse<TopEncerramentosModel>> GetPainelEncerramentos(TopEncerramentos_Request request)
        {
            List<TopEncerramentosModel> data = new List<TopEncerramentosModel>();
            List<TopEncerramentosModel> retorno = new List<TopEncerramentosModel>();
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                var dataReferencia = String.IsNullOrEmpty(request?.DataReferencia) ? DateTime.Now : Convert.ToDateTime(request?.DataReferencia);
                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT FIRST 20 C.DESCRICAO, COUNT(D.COD_ENCERRAMENTO) AS TOTAL");
                    sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO D");
                    sql.AppendLine("LEFT JOIN TB_CODIGO_ENCERRAMENTO C");
                    sql.AppendLine("    ON D.COD_ENCERRAMENTO = C.COD_ENCERRAMENTO");
                    sql.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE TSE");
                    sql.AppendLine("    ON D.COD_TIPO_SERV = TSE.COD_TIPO_SERV");
                    sql.AppendLine("LEFT JOIN TB_LOCALIDADE LOC");
                    sql.AppendLine("    ON D.COD_ESCRITORIO = LOC.COD_ESCRITORIO");
                    sql.AppendLine("    AND D.COD_AREA = LOC.COD_AREA");
                    sql.AppendLine("    AND D.COD_LOCALIDADE = LOC.COD_LOCALIDADE");
                    sql.AppendLine("LEFT JOIN TB_ESCRITORIO ESC");
                    sql.AppendLine("    ON D.COD_ESCRITORIO = ESC.COD_ESCRITORIO");
                    sql.AppendLine("LEFT JOIN TB_AREA F");
                    sql.AppendLine("    ON D.COD_ESCRITORIO = F.COD_ESCRITORIO");
                    sql.AppendLine("    AND D.COD_AREA = F.COD_AREA");
                    sql.AppendLine("LEFT JOIN TB_EQUIPE EQ");
                    sql.AppendLine("    ON EQ.NUM_FRE = D.EQUIPE");

                    StringBuilder whereClause = new StringBuilder();
                    whereClause.AppendLine("WHERE (D.DATA_PROD BETWEEN @DATAINICIOBUSCA AND @DATAFIMBUSCA)");
                    parametros.Clear();

                    //Ativo
                    if (int.TryParse(request.TipoStatusExecucao, out int tipoStatus))
                    {
                        if (tipoStatus == 1 || tipoStatus == 2)
                        {
                            whereClause.AppendLine(" AND (D.STATUS_EXECUCAO IN(@STATUS_EXECUCAO))");
                            parametros.Add("@STATUS_EXECUCAO", tipoStatus);
                        }
                        else
                        {
                            whereClause.AppendLine(" AND (D.STATUS_EXECUCAO IN(1,2))");
                        }
                    }

                    // Adiciona filtros adicionais conforme necessário
                    if (!string.IsNullOrEmpty(request.NaturezaServico))
                    {
                        whereClause.AppendLine("AND TSE.COD_NATUREZA = @COD_NATUREZA");
                        parametros.Add("@COD_NATUREZA", request.NaturezaServico);
                    }

                    if (!string.IsNullOrEmpty(request.Area))
                    {
                        whereClause.AppendLine("AND D.COD_AREA = @AREA");
                        parametros.Add("@AREA", request.Area);
                    }

                    if (!string.IsNullOrEmpty(request.GrupoArea))
                    {
                        whereClause.AppendLine("AND D.COD_ESCRITORIO = @ESCRITORIO");
                        parametros.Add("@ESCRITORIO", request.GrupoArea);
                    }

                    if (!string.IsNullOrEmpty(request.Localidade))
                    {
                        whereClause.AppendLine("AND LOC.COD_LOCALIDADE = @LOCALIDADE");
                        parametros.Add("@LOCALIDADE", request.Localidade);
                    }

                    if (!string.IsNullOrEmpty(request.Supervisor))
                    {
                        whereClause.AppendLine("AND EQ.COD_SUPERV = @SUPERVISOR");
                        parametros.Add("@SUPERVISOR", request.Supervisor);
                    }

                    if (!string.IsNullOrEmpty(request.Tecnico))
                    {
                        whereClause.AppendLine("AND D.EQUIPE = @EQUIPE");
                        parametros.Add("@EQUIPE", request.Tecnico);
                    }

                    sql.Append(whereClause.ToString());

                    sql.AppendLine("GROUP BY C.DESCRICAO");
                    sql.AppendLine("ORDER BY TOTAL DESC");


                    var periodo = GetPeriodoFiltro(request.TipoFiltroData.Value, dataReferencia);
                    parametros.Add("@DATAINICIOBUSCA", periodo.dataInicio);
                    parametros.Add("@DATAFIMBUSCA", periodo.dataFim);


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        int valortotal = 0;
                        while (await reader.ReadAsync())
                        {
                            valortotal = valortotal + Convert.ToInt16(reader["TOTAL"]);
                            data.Add(new TopEncerramentosModel
                            {
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Total = Convert.ToInt16(reader["TOTAL"])
                            });

                        }

                        foreach (var dados in data)
                        {
                            dados.Percentual = dados.Percentual = ((double)dados.Total / valortotal * 100.00).ToString("F2") + "%";
                            retorno.Add(dados);
                        }
                    }
                }
            }

            return new DataTableResponse<TopEncerramentosModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.ToList().Count,
                data = retorno.ToList(),
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetNaturezasStatusExecucao")]
        public async Task<List<ServicosStatusExecucaoModel>> GetNaturezasStatusExecucao(ServicosProxVencimentoPerdaPrazoModel_Request request)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            var dataReferencia = String.IsNullOrEmpty(request?.DataReferencia) ? DateTime.Now : Convert.ToDateTime(request?.DataReferencia);

            List<ServicosStatusExecucaoModel> retorno = new List<ServicosStatusExecucaoModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoHorario = new Conexao<Turno_ServicoAniel>(sessionT5))
            using (var repoDoc = new Conexao<Documento_ProducaoAniel>(sessionT5))
            using (var repoNatu = new Conexao<Natureza_ProducaoAniel>(sessionT5))
            {
                var periodos = await repoHorario.select(p => p.Status == Status_Default.Ativo);
                var descricaoPeriodos = periodos.Select(p => p.Descricao).ToArray();
                Array.Resize(ref descricaoPeriodos, descricaoPeriodos.Length + 1);
                descricaoPeriodos[descricaoPeriodos.Length - 1] = "Sem Turno";

                // Selecionar colunas fixas
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT");
                    sql.AppendLine("    TOS.NATUREZA_PRODUCAO,");
                    sql.AppendLine("    TOS.ID_NATUREZA,");
                    sql.AppendLine("    TOS.TOTAL_OS,");
                    sql.AppendLine("    COALESCE(TDB.TOTAL_OS_ROTEIRIZADA, 0) AS TOTAL_OS_ROTEIRIZADA,");
                    sql.AppendLine("    COALESCE(TDE.TOTAL_OS_ENCAIXE, 0) AS TOTAL_OS_ENCAIXE");
                    sql.AppendLine("FROM ( ");
                    sql.AppendLine("    SELECT ");
                    sql.AppendLine("        NATU.DESCRICAO AS NATUREZA_PRODUCAO,");
                    sql.AppendLine("        NATU.ID AS ID_NATUREZA,");
                    sql.AppendLine("        COUNT(DISTINCT DOCP.NUM_OBRA) AS TOTAL_OS");
                    sql.AppendLine("    FROM TB_DOCUMENTO_PRODUCAO DOCP");
                    sql.AppendLine("    LEFT JOIN TB_TIPO_SERVICO_EQUIPE TSE");
                    sql.AppendLine("        ON DOCP.CODCT = TSE.CODCT AND DOCP.COD_TIPO_SERV = TSE.COD_TIPO_SERV ");
                    sql.AppendLine("    LEFT JOIN TB_NATUREZA_PRODUCAO NATU ");
                    sql.AppendLine("        ON NATU.ID = TSE.COD_NATUREZA ");
                    sql.AppendLine("    LEFT JOIN TB_EQUIPE EQ ");
                    sql.AppendLine("        ON EQ.EQUIPE = DOCP.EQUIPE ");
                    sql.AppendLine("    WHERE DOCP.DATA_MAXIMA = @DATAHOJE");
                    sql.AppendLine("      AND NATU.ID IS NOT NULL");

                    StringBuilder whereClause = new StringBuilder();

                    if (!string.IsNullOrEmpty(request.Area))
                    {
                        whereClause.AppendLine("    AND DOCP.COD_AREA = @AREA");
                        parametros.Add("@AREA", request.Area);
                    }

                    if (!string.IsNullOrEmpty(request.GrupoArea))
                    {
                        whereClause.AppendLine("    AND DOCP.COD_ESCRITORIO = @ESCRITORIO");
                        parametros.Add("@ESCRITORIO", request.GrupoArea);
                    }

                    if (!string.IsNullOrEmpty(request.Localidade))
                    {
                        whereClause.AppendLine("    AND DOCP.COD_LOCALIDADE = @LOCALIDADE");
                        parametros.Add("@LOCALIDADE", request.Localidade);
                    }

                    if (!string.IsNullOrEmpty(request.Supervisor))
                    {
                        whereClause.AppendLine("    AND EQ.COD_SUPERV = @SUPERVISOR");
                        parametros.Add("@SUPERVISOR", request.Supervisor);
                    }

                    if (!string.IsNullOrEmpty(request.Tecnico))
                    {
                        whereClause.AppendLine("    AND DOCP.EQUIPE = @EQUIPE");
                        parametros.Add("@EQUIPE", request.Tecnico);
                    }

                    if (!string.IsNullOrEmpty(request.NaturezaServico))
                    {
                        whereClause.AppendLine("    AND NATU.ID IN (@NATUREZA)");
                        parametros.Add("@NATUREZA", request.NaturezaServico);
                    }

                    sql.Append(whereClause.ToString());
                    sql.AppendLine("    GROUP BY NATU.DESCRICAO, NATU.ID ");
                    sql.AppendLine(") AS TOS ");
                    sql.AppendLine("LEFT JOIN ( ");
                    sql.AppendLine("    SELECT ");
                    sql.AppendLine("        NATU.DESCRICAO AS NATUREZA_PRODUCAO,");
                    sql.AppendLine("        NATU.ID AS ID_NATUREZA,");
                    sql.AppendLine("        COUNT(DISTINCT DOCP.NUM_OBRA) AS TOTAL_OS_ROTEIRIZADA");
                    sql.AppendLine("    FROM TB_DOCUMENTO_PRODUCAO DOCP");
                    sql.AppendLine("    LEFT JOIN TB_TIPO_SERVICO_EQUIPE TSE ");
                    sql.AppendLine("        ON DOCP.CODCT = TSE.CODCT AND DOCP.COD_TIPO_SERV = TSE.COD_TIPO_SERV");
                    sql.AppendLine("    LEFT JOIN TB_NATUREZA_PRODUCAO NATU ");
                    sql.AppendLine("        ON NATU.ID = TSE.COD_NATUREZA ");
                    sql.AppendLine("    LEFT JOIN TB_EQUIPE EQ ");
                    sql.AppendLine("        ON EQ.EQUIPE = DOCP.EQUIPE");
                    sql.AppendLine("    WHERE DOCP.DATA_DASHBOARD = @DATAHOJE");
                    sql.AppendLine("      AND NATU.ID IS NOT NULL ");

                    sql.Append(whereClause.ToString());

                    sql.AppendLine("    GROUP BY NATU.DESCRICAO, NATU.ID ");
                    sql.AppendLine(") AS TDB ");
                    sql.AppendLine("    ON TOS.ID_NATUREZA = TDB.ID_NATUREZA");
                    sql.AppendLine("LEFT JOIN ( ");
                    sql.AppendLine("    SELECT ");
                    sql.AppendLine("        NATU.DESCRICAO AS NATUREZA_PRODUCAO,");
                    sql.AppendLine("        NATU.ID AS ID_NATUREZA,");
                    sql.AppendLine("        COUNT(DISTINCT DOCP.NUM_OBRA) AS TOTAL_OS_ENCAIXE");
                    sql.AppendLine("    FROM TB_DOCUMENTO_PRODUCAO DOCP");
                    sql.AppendLine("    LEFT JOIN TB_TIPO_SERVICO_EQUIPE TSE ");
                    sql.AppendLine("        ON DOCP.CODCT = TSE.CODCT AND DOCP.COD_TIPO_SERV = TSE.COD_TIPO_SERV");
                    sql.AppendLine("    LEFT JOIN TB_NATUREZA_PRODUCAO NATU ");
                    sql.AppendLine("        ON NATU.ID = TSE.COD_NATUREZA ");
                    sql.AppendLine("    LEFT JOIN TB_EQUIPE EQ ");
                    sql.AppendLine("        ON EQ.EQUIPE = DOCP.EQUIPE");
                    sql.AppendLine("    WHERE DOCP.DATA_MAXIMA = @DATAHOJE");
                    sql.AppendLine("      AND DOCP.data_dashboard IS NULL");
                    sql.AppendLine("      AND NATU.ID IS NOT NULL ");

                    sql.Append(whereClause.ToString());

                    sql.AppendLine("    GROUP BY NATU.DESCRICAO, NATU.ID ");
                    sql.AppendLine(") AS TDE");
                    sql.AppendLine("    ON TOS.ID_NATUREZA = TDE.ID_NATUREZA;");


                    parametros.Add("@DATAHOJE", dataReferencia.Date);


                    // Busca com paginação
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        var lista = await GetServicosStatusExecucao(request);

                        while (await reader.ReadAsync())
                        {
                            var idNatureza = Convert.ToInt16(reader["ID_NATUREZA"]);
                            var total_os = Convert.ToString(reader["TOTAL_OS_ROTEIRIZADA"]);
                            var descNatureza = Convert.ToString(reader["NATUREZA_PRODUCAO"]);
                            var totalOsEncaixe = Convert.ToString(reader["TOTAL_OS_ENCAIXE"]);

                            retorno.Add(new ServicosStatusExecucaoModel
                            {
                                id = idNatureza,
                                descricao = descNatureza,
                                total_os = total_os,
                                total_os_encaixe = totalOsEncaixe,
                                periodos = descricaoPeriodos,
                                lista = lista
                            });
                        }
                    }

                    foreach (var reinc in retorno)
                    {
                        parametros.Clear();

                        StringBuilder sqlReinc = new StringBuilder();

                        sqlReinc.AppendLine("WITH Reincidencias AS (");
                        sqlReinc.AppendLine("    SELECT ");
                        sqlReinc.AppendLine("        NP.DESCRICAO,");
                        sqlReinc.AppendLine("        NP.ID AS ID_NATUREZA,");
                        sqlReinc.AppendLine("        COUNT(REINC.NUM_DOC) AS QTD_REINCIDENCIAS");
                        sqlReinc.AppendLine("    FROM ");
                        sqlReinc.AppendLine("        TB_DOCUMENTO_PRODUCAO D");
                        sqlReinc.AppendLine("    INNER JOIN ");
                        sqlReinc.AppendLine("        TB_TIPO_SERVICO_EQUIPE TS ON TS.COD_TIPO_SERV = D.COD_TIPO_SERV");
                        sqlReinc.AppendLine("    INNER JOIN ");
                        sqlReinc.AppendLine("        TB_NATUREZA_PRODUCAO NP ON NP.ID = TS.COD_NATUREZA");
                        sqlReinc.AppendLine("    INNER JOIN ");
                        sqlReinc.AppendLine("        TB_REINCIDENCIA_DOCPROD REINC");
                        sqlReinc.AppendLine("        ON REINC.CODCT_ANTERIOR = D.CODCT");
                        sqlReinc.AppendLine("        AND REINC.PROJETO_ANTERIOR = D.PROJETO");
                        sqlReinc.AppendLine("        AND REINC.NUM_OBRA_ANTERIOR = D.NUM_OBRA");
                        sqlReinc.AppendLine("        AND REINC.NUM_DOC_ANTERIOR = D.NUM_DOC");
                        sqlReinc.AppendLine("    WHERE");
                        sqlReinc.AppendLine("        D.DATA_MAXIMA = @DATAHOJEINICIO");
                        sqlReinc.AppendLine("        AND NP.ID = @ID_NATUREZA");
                        sqlReinc.AppendLine("    GROUP BY ");
                        sqlReinc.AppendLine("        NP.DESCRICAO, NP.ID, D.CODCT, D.PROJETO, D.NUM_OBRA, D.NUM_DOC");
                        sqlReinc.AppendLine(")");
                        sqlReinc.AppendLine("SELECT ");
                        sqlReinc.AppendLine("    DESCRICAO,");
                        sqlReinc.AppendLine("    ID_NATUREZA,");

                        sqlReinc.AppendLine("    -- QTD_REV3");
                        sqlReinc.AppendLine("    COUNT(");
                        sqlReinc.AppendLine("        CASE");
                        sqlReinc.AppendLine("            WHEN (");
                        sqlReinc.AppendLine("                SELECT COUNT(REINC2.NUM_DOC || NP2.ID)");
                        sqlReinc.AppendLine("                FROM TB_REINCIDENCIA_DOCPROD REINC2");
                        sqlReinc.AppendLine("                LEFT JOIN TB_DOCUMENTO_PRODUCAO D2");
                        sqlReinc.AppendLine("                    ON REINC2.CODCT_ANTERIOR = D2.CODCT ");
                        sqlReinc.AppendLine("                    AND REINC2.PROJETO_ANTERIOR = D2.PROJETO ");
                        sqlReinc.AppendLine("                    AND REINC2.NUM_DOC_ANTERIOR = D2.NUM_DOC ");
                        sqlReinc.AppendLine("                    AND REINC2.NUM_OBRA_ANTERIOR = D2.NUM_OBRA");
                        sqlReinc.AppendLine("                LEFT JOIN TB_TIPO_SERVICO_EQUIPE TS2");
                        sqlReinc.AppendLine("                    ON TS2.CODCT = D2.CODCT ");
                        sqlReinc.AppendLine("                    AND TS2.COD_TIPO_SERV = D2.COD_TIPO_SERV");
                        sqlReinc.AppendLine("                LEFT JOIN TB_NATUREZA_PRODUCAO NP2");
                        sqlReinc.AppendLine("                    ON NP2.ID = TS2.COD_NATUREZA");
                        sqlReinc.AppendLine("                WHERE REINC2.NUM_DOC = D2.num_doc");
                        sqlReinc.AppendLine("                  AND REINC2.DATA BETWEEN (D2.DIA - 30) AND D2.DIA");
                        sqlReinc.AppendLine("                  AND NP2.TIPO_HABILIDADE <> '1'");
                        sqlReinc.AppendLine("                HAVING COUNT(REINC2.NUM_DOC || NP2.ID) >= 3");
                        sqlReinc.AppendLine("            ) >= 1");
                        sqlReinc.AppendLine("            THEN 1");
                        sqlReinc.AppendLine("            ELSE NULL");
                        sqlReinc.AppendLine("        END");
                        sqlReinc.AppendLine("    ) AS QTD_REV3,");

                        sqlReinc.AppendLine("  -- Soma de QTD_REV1 e QTD_RTI");
                        sqlReinc.AppendLine("COUNT(");
                        sqlReinc.AppendLine("    CASE");
                        sqlReinc.AppendLine("        WHEN (");
                        sqlReinc.AppendLine("            SELECT COUNT(REINC3.NUM_OBRA)");
                        sqlReinc.AppendLine("            FROM TB_REINCIDENCIA_DOCPROD REINC3");
                        sqlReinc.AppendLine("            LEFT JOIN TB_DOCUMENTO_PRODUCAO D3");
                        sqlReinc.AppendLine("                ON REINC3.CODCT_ANTERIOR = D3.CODCT ");
                        sqlReinc.AppendLine("                AND REINC3.PROJETO_ANTERIOR = D3.PROJETO ");
                        sqlReinc.AppendLine("                AND REINC3.NUM_DOC_ANTERIOR = D3.NUM_DOC ");
                        sqlReinc.AppendLine("                AND REINC3.NUM_OBRA_ANTERIOR = D3.NUM_OBRA");
                        sqlReinc.AppendLine("            LEFT JOIN TB_TIPO_SERVICO_EQUIPE TS3");
                        sqlReinc.AppendLine("                ON TS3.CODCT = D3.CODCT ");
                        sqlReinc.AppendLine("                AND TS3.COD_TIPO_SERV = D3.COD_TIPO_SERV");
                        sqlReinc.AppendLine("            LEFT JOIN TB_NATUREZA_PRODUCAO NP3");
                        sqlReinc.AppendLine("                ON NP3.ID = TS3.COD_NATUREZA");
                        sqlReinc.AppendLine("            WHERE REINC3.CODCT = D3.CODCT");
                        sqlReinc.AppendLine("              AND REINC3.PROJETO = D3.PROJETO");
                        sqlReinc.AppendLine("              AND REINC3.NUM_DOC = D3.NUM_DOC");
                        sqlReinc.AppendLine("              AND REINC3.NUM_OBRA = D3.NUM_OBRA");
                        sqlReinc.AppendLine("              AND NP3.TIPO_HABILIDADE <> 1");
                        sqlReinc.AppendLine("        ) >= 1");
                        sqlReinc.AppendLine("        THEN 1");
                        sqlReinc.AppendLine("        ELSE NULL");
                        sqlReinc.AppendLine("    END");
                        sqlReinc.AppendLine(") +");
                        sqlReinc.AppendLine("COUNT(");
                        sqlReinc.AppendLine("    CASE");
                        sqlReinc.AppendLine("        WHEN (");
                        sqlReinc.AppendLine("            SELECT COUNT(REINC4.NUM_OBRA)");
                        sqlReinc.AppendLine("            FROM TB_REINCIDENCIA_DOCPROD REINC4");
                        sqlReinc.AppendLine("            LEFT JOIN TB_DOCUMENTO_PRODUCAO D4");
                        sqlReinc.AppendLine("                ON REINC4.CODCT_ANTERIOR = D4.CODCT ");
                        sqlReinc.AppendLine("                AND REINC4.PROJETO_ANTERIOR = D4.PROJETO ");
                        sqlReinc.AppendLine("                AND REINC4.NUM_DOC_ANTERIOR = D4.NUM_DOC ");
                        sqlReinc.AppendLine("                AND REINC4.NUM_OBRA_ANTERIOR = D4.NUM_OBRA");
                        sqlReinc.AppendLine("            LEFT JOIN TB_TIPO_SERVICO_EQUIPE TS4");
                        sqlReinc.AppendLine("                ON TS4.CODCT = D4.CODCT ");
                        sqlReinc.AppendLine("                AND TS4.COD_TIPO_SERV = D4.COD_TIPO_SERV");
                        sqlReinc.AppendLine("            LEFT JOIN TB_NATUREZA_PRODUCAO NP4");
                        sqlReinc.AppendLine("                ON NP4.ID = TS4.COD_NATUREZA");
                        sqlReinc.AppendLine("            WHERE REINC4.CODCT = D4.CODCT");
                        sqlReinc.AppendLine("              AND REINC4.PROJETO = D4.PROJETO");
                        sqlReinc.AppendLine("              AND REINC4.NUM_DOC = D4.NUM_DOC");
                        sqlReinc.AppendLine("              AND REINC4.NUM_OBRA = D4.NUM_OBRA");
                        sqlReinc.AppendLine("              AND NP4.TIPO_HABILIDADE = 1");
                        sqlReinc.AppendLine("        ) >= 1");
                        sqlReinc.AppendLine("        THEN 1");
                        sqlReinc.AppendLine("        ELSE NULL");
                        sqlReinc.AppendLine("    END");
                        sqlReinc.AppendLine(") AS REV1_RTI");

                        sqlReinc.AppendLine("FROM ");
                        sqlReinc.AppendLine("    Reincidencias");
                        sqlReinc.AppendLine("GROUP BY ");
                        sqlReinc.AppendLine("    DESCRICAO, ID_NATUREZA;");



                        parametros.Add("@DATAHOJEINICIO", dataReferencia.Date);
                        parametros.Add("@ID_NATUREZA", reinc.id);


                        using (var comReinc = sessionT5.Connection.CreateCommand())
                        using (var readerReinc = await CommandHelper.ExecuteReader(comReinc, sqlReinc, parametros, sessionT5.Connection, null))
                        {
                            while (await readerReinc.ReadAsync())
                            {
                                var rev1 = Convert.ToString(readerReinc["REV1_RTI"]) ?? "0";
                                var rev3 = Convert.ToString(readerReinc["QTD_REV3"]) ?? "0";

                                var idNaturezaReinc = Convert.ToInt64(readerReinc["ID_NATUREZA"]);
                                retorno.FirstOrDefault(r => r.id == idNaturezaReinc).rev1 = rev1;
                                retorno.FirstOrDefault(r => r.id == idNaturezaReinc).rev3 = rev3;
                            }
                        }
                    }
                }
            }
            return retorno;
        }


        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetNaturezaServicosFechadosProdImprod")]
        public async Task<List<ServicosStatusExecucaoModel>> GetNaturezaServicosFechadosProdImprod(ServicosFechadosProdImprodModel_Request request)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            var dataReferencia = String.IsNullOrEmpty(request?.DataReferencia) ? DateTime.Now : Convert.ToDateTime(request?.DataReferencia);

            List<ServicosStatusExecucaoModel> retorno = new List<ServicosStatusExecucaoModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoHorario = new Conexao<Turno_ServicoAniel>(sessionT5))
            using (var repoDoc = new Conexao<Documento_ProducaoAniel>(sessionT5))
            using (var repoNatu = new Conexao<Natureza_ProducaoAniel>(sessionT5))
            {
                var periodos = await repoHorario.select(p => p.Status == Status_Default.Ativo);
                var descricaoPeriodos = periodos.Select(p => p.Descricao).ToArray();
                Array.Resize(ref descricaoPeriodos, descricaoPeriodos.Length + 1);
                descricaoPeriodos[descricaoPeriodos.Length - 1] = "Sem Turno";

                //Select colunas fixas
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT");
                    sql.AppendLine("    TOS.NATUREZA_PRODUCAO,");
                    sql.AppendLine("    TOS.ID_NATUREZA,");
                    sql.AppendLine("    TOS.TOTAL_OS,");
                    sql.AppendLine("    COALESCE(TDB.TOTAL_OS_ROTEIRIZADA, 0) AS TOTAL_OS_ROTEIRIZADA,");
                    sql.AppendLine("    COALESCE(TDE.TOTAL_OS_ENCAIXE, 0) AS TOTAL_OS_ENCAIXE");
                    sql.AppendLine("FROM ( ");
                    sql.AppendLine("    SELECT ");
                    sql.AppendLine("        NATU.DESCRICAO AS NATUREZA_PRODUCAO,");
                    sql.AppendLine("        NATU.ID AS ID_NATUREZA,");
                    sql.AppendLine("        COUNT(DISTINCT DOCP.NUM_OBRA) AS TOTAL_OS");
                    sql.AppendLine("    FROM TB_DOCUMENTO_PRODUCAO DOCP");
                    sql.AppendLine("    LEFT JOIN TB_TIPO_SERVICO_EQUIPE TSE");
                    sql.AppendLine("        ON DOCP.CODCT = TSE.CODCT AND DOCP.COD_TIPO_SERV = TSE.COD_TIPO_SERV ");
                    sql.AppendLine("    LEFT JOIN TB_NATUREZA_PRODUCAO NATU ");
                    sql.AppendLine("        ON NATU.ID = TSE.COD_NATUREZA ");
                    sql.AppendLine("    LEFT JOIN TB_EQUIPE EQ ");
                    sql.AppendLine("        ON EQ.EQUIPE = DOCP.EQUIPE ");
                    sql.AppendLine("    WHERE DOCP.DATA_MAXIMA = @DATAHOJE");
                    sql.AppendLine("      AND NATU.ID IS NOT NULL");

                    StringBuilder whereClause = new StringBuilder();

                    if (!string.IsNullOrEmpty(request.Area))
                    {
                        whereClause.AppendLine("    AND DOCP.COD_AREA = @AREA");
                        parametros.Add("@AREA", request.Area);
                    }

                    if (!string.IsNullOrEmpty(request.GrupoArea))
                    {
                        whereClause.AppendLine("    AND DOCP.COD_ESCRITORIO = @ESCRITORIO");
                        parametros.Add("@ESCRITORIO", request.GrupoArea);
                    }

                    if (!string.IsNullOrEmpty(request.Localidade))
                    {
                        whereClause.AppendLine("    AND DOCP.COD_LOCALIDADE = @LOCALIDADE");
                        parametros.Add("@LOCALIDADE", request.Localidade);
                    }

                    if (!string.IsNullOrEmpty(request.Supervisor))
                    {
                        whereClause.AppendLine("    AND EQ.COD_SUPERV = @SUPERVISOR");
                        parametros.Add("@SUPERVISOR", request.Supervisor);
                    }

                    if (!string.IsNullOrEmpty(request.Tecnico))
                    {
                        whereClause.AppendLine("    AND DOCP.EQUIPE = @EQUIPE");
                        parametros.Add("@EQUIPE", request.Tecnico);
                    }

                    if (!string.IsNullOrEmpty(request.NaturezaServico))
                    {
                        whereClause.AppendLine("    AND NATU.ID IN (@NATUREZA)");
                        parametros.Add("@NATUREZA", request.NaturezaServico);
                    }

                    sql.Append(whereClause.ToString());
                    sql.AppendLine("    GROUP BY NATU.DESCRICAO, NATU.ID ");
                    sql.AppendLine(") AS TOS ");
                    sql.AppendLine("LEFT JOIN ( ");
                    sql.AppendLine("    SELECT ");
                    sql.AppendLine("        NATU.DESCRICAO AS NATUREZA_PRODUCAO,");
                    sql.AppendLine("        NATU.ID AS ID_NATUREZA,");
                    sql.AppendLine("        COUNT(DISTINCT DOCP.NUM_OBRA) AS TOTAL_OS_ROTEIRIZADA");
                    sql.AppendLine("    FROM TB_DOCUMENTO_PRODUCAO DOCP");
                    sql.AppendLine("    LEFT JOIN TB_TIPO_SERVICO_EQUIPE TSE ");
                    sql.AppendLine("        ON DOCP.CODCT = TSE.CODCT AND DOCP.COD_TIPO_SERV = TSE.COD_TIPO_SERV");
                    sql.AppendLine("    LEFT JOIN TB_NATUREZA_PRODUCAO NATU ");
                    sql.AppendLine("        ON NATU.ID = TSE.COD_NATUREZA ");
                    sql.AppendLine("    LEFT JOIN TB_EQUIPE EQ ");
                    sql.AppendLine("        ON EQ.EQUIPE = DOCP.EQUIPE");
                    sql.AppendLine("    WHERE DOCP.DATA_DASHBOARD = @DATAHOJE");
                    sql.AppendLine("      AND NATU.ID IS NOT NULL ");

                    sql.Append(whereClause.ToString());

                    sql.AppendLine("    GROUP BY NATU.DESCRICAO, NATU.ID ");
                    sql.AppendLine(") AS TDB ");
                    sql.AppendLine("    ON TOS.ID_NATUREZA = TDB.ID_NATUREZA");
                    sql.AppendLine("LEFT JOIN ( ");
                    sql.AppendLine("    SELECT ");
                    sql.AppendLine("        NATU.DESCRICAO AS NATUREZA_PRODUCAO,");
                    sql.AppendLine("        NATU.ID AS ID_NATUREZA,");
                    sql.AppendLine("        COUNT(DISTINCT DOCP.NUM_OBRA) AS TOTAL_OS_ENCAIXE");
                    sql.AppendLine("    FROM TB_DOCUMENTO_PRODUCAO DOCP");
                    sql.AppendLine("    LEFT JOIN TB_TIPO_SERVICO_EQUIPE TSE ");
                    sql.AppendLine("        ON DOCP.CODCT = TSE.CODCT AND DOCP.COD_TIPO_SERV = TSE.COD_TIPO_SERV");
                    sql.AppendLine("    LEFT JOIN TB_NATUREZA_PRODUCAO NATU ");
                    sql.AppendLine("        ON NATU.ID = TSE.COD_NATUREZA ");
                    sql.AppendLine("    LEFT JOIN TB_EQUIPE EQ ");
                    sql.AppendLine("        ON EQ.EQUIPE = DOCP.EQUIPE");
                    sql.AppendLine("    WHERE DOCP.DATA_MAXIMA = @DATAHOJE");
                    sql.AppendLine("      AND DOCP.data_dashboard IS NULL");
                    sql.AppendLine("      AND NATU.ID IS NOT NULL ");

                    sql.Append(whereClause.ToString());

                    sql.AppendLine("    GROUP BY NATU.DESCRICAO, NATU.ID ");
                    sql.AppendLine(") AS TDE");
                    sql.AppendLine("    ON TOS.ID_NATUREZA = TDE.ID_NATUREZA;");


                    parametros.Add("@DATAHOJE", dataReferencia.Date);


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        var lista = await GetServicosFechadosProdImprod(request);

                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ServicosStatusExecucaoModel
                            {
                                id = Convert.ToInt16(reader["ID_NATUREZA"]),
                                descricao = Convert.ToString(reader["NATUREZA_PRODUCAO"]),
                                total_os = string.IsNullOrEmpty(reader["TOTAL_OS_ROTEIRIZADA"].ToString()) ? "0" : Convert.ToString(reader["TOTAL_OS_ROTEIRIZADA"]),
                                total_os_encaixe = string.IsNullOrEmpty(reader["TOTAL_OS_ENCAIXE"].ToString()) ? "0" : Convert.ToString(reader["TOTAL_OS_ENCAIXE"]),
                                periodos = descricaoPeriodos,
                                lista = lista
                            });

                        }
                    }
                }

                return retorno;
            }
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetServicosFechadosProdImprod")]
        public async Task<List<ResultadoFinal>> GetServicosFechadosProdImprod(ServicosFechadosProdImprodModel_Request request)
        {
            List<ResultadoFinal> resultadoFinal = new List<ResultadoFinal>();
            var dataReferencia = string.IsNullOrEmpty(request?.DataReferencia) ? DateTime.Now : Convert.ToDateTime(request?.DataReferencia);

            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            List<ServicosStatusExecucaoModel> retorno = new List<ServicosStatusExecucaoModel>();

            using (var session = HibernateUtil.GetSession())
            using (var sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoHorario = new Conexao<Turno_ServicoAniel>(sessionT5))
            {

                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT");
                    sql.AppendLine("    NP.DESCRICAO AS NATUREZA_PRODUCAO,");
                    sql.AppendLine("    NP.ID AS ID_NATUREZA,");
                    sql.AppendLine("    COALESCE (TSV.DESCRICAO, NULL, 'Sem Turno') AS DESCRICAO_TURNO,");
                    sql.AppendLine("    COUNT(CASE WHEN D.DATA_DASHBOARD = @DATAHOJE THEN D.NUM_OBRA END) AS ROTEIRIZADO,");
                    sql.AppendLine("    COUNT(CASE WHEN D.STATUS_EXECUCAO IN (1, 2) THEN 1 END) AS EXECUTADO,");
                    sql.AppendLine();
                    sql.AppendLine("    -- SLA");
                    sql.AppendLine("    COUNT(");
                    sql.AppendLine("        CASE");
                    sql.AppendLine("            WHEN D.STATUS_EXECUCAO IN (1, 2)");
                    sql.AppendLine("                AND D.datahora_ini_atend IS NULL");
                    sql.AppendLine("                AND D.datahora_fim_atend <= CAST(D.DATA_INDICADOR || ' ' || D.hora_indicador AS TIMESTAMP)");
                    sql.AppendLine("            THEN 1");
                    sql.AppendLine("            WHEN D.STATUS_EXECUCAO IN (1, 2)");
                    sql.AppendLine("                AND D.datahora_ini_atend IS NOT NULL");
                    sql.AppendLine("                AND D.datahora_ini_atend <= CAST(D.DATA_INDICADOR || ' ' || D.hora_indicador AS TIMESTAMP)");
                    sql.AppendLine("            THEN 1");
                    sql.AppendLine("            ELSE NULL");
                    sql.AppendLine("        END) AS SLA,");
                    sql.AppendLine();
                    sql.AppendLine("    -- TEC1");
                    sql.AppendLine("    COUNT(");
                    sql.AppendLine("        CASE");
                    sql.AppendLine("            WHEN D.STATUS_EXECUCAO = '2'");
                    sql.AppendLine("                AND D.seq = '1'");
                    sql.AppendLine("            THEN 1");
                    sql.AppendLine("            ELSE NULL");
                    sql.AppendLine("        END) AS TEC1,");
                    sql.AppendLine();
                    sql.AppendLine("    -- AGE1");
                    sql.AppendLine("    COUNT(");
                    sql.AppendLine("        CASE");
                    sql.AppendLine("            WHEN D.STATUS_EXECUCAO IN (1, 2)");
                    sql.AppendLine("                AND D.datahora_ini_atend IS NULL");
                    sql.AppendLine("                AND D.datahora_fim_atend <= AG.datahora_fim");
                    sql.AppendLine("            THEN 1");
                    sql.AppendLine("            WHEN D.STATUS_EXECUCAO IN (1, 2)");
                    sql.AppendLine("                AND D.datahora_ini_atend IS NOT NULL");
                    sql.AppendLine("                AND D.datahora_ini_atend <= AG.datahora_fim");
                    sql.AppendLine("            THEN 1");
                    sql.AppendLine("            ELSE NULL");
                    sql.AppendLine("        END) AS AGE1");
                    sql.AppendLine();
                    sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO D");
                    sql.AppendLine("    LEFT JOIN (SELECT DESCRICAO,HORA_INI,HORA_FIM FROM TB_TURNO_SERVICO WHERE STATUS = '1') TSV ON TSV.HORA_INI <= D.HORA_MAXIMA AND TSV.HORA_FIM >= D.HORA_MAXIMA");
                    sql.AppendLine("    LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL AG ON (AG.CODCT = D.CODCT AND AG.PROJETO = D.PROJETO AND AG.NUM_DOC = D.NUM_DOC AND AG.NUM_OBRA = D.NUM_OBRA)");
                    sql.AppendLine("    LEFT JOIN TB_TIPO_SERVICO_EQUIPE TS ON TS.COD_TIPO_SERV = D.COD_TIPO_SERV AND D.CODCT = TS.CODCT");
                    sql.AppendLine("    LEFT JOIN TB_NATUREZA_PRODUCAO NP ON NP.ID = TS.COD_NATUREZA");
                    sql.AppendLine("    LEFT JOIN TB_LOCALIDADE LOC ON D.COD_ESCRITORIO = LOC.COD_ESCRITORIO AND D.COD_AREA = LOC.COD_AREA AND D.COD_LOCALIDADE = LOC.COD_LOCALIDADE");
                    sql.AppendLine("    LEFT JOIN TB_ESCRITORIO ESC ON D.COD_ESCRITORIO = ESC.COD_ESCRITORIO");
                    sql.AppendLine("    LEFT JOIN TB_AREA F ON D.COD_ESCRITORIO = F.COD_ESCRITORIO AND D.COD_AREA = F.COD_AREA");
                    sql.AppendLine("    LEFT JOIN TB_EQUIPE EQ ON EQ.NUM_FRE = D.EQUIPE");
                    sql.AppendLine("WHERE D.DATA_DASHBOARD = @DATAHOJE");

                    StringBuilder whereClause = new StringBuilder();

                    // Adiciona filtros adicionais conforme necessário
                    if (!string.IsNullOrEmpty(request.Area))
                    {
                        whereClause.AppendLine("AND D.COD_AREA = @AREA");
                        parametros.Add("@AREA", request.Area);
                    }

                    if (!string.IsNullOrEmpty(request.GrupoArea))
                    {
                        whereClause.AppendLine("AND D.COD_ESCRITORIO = @ESCRITORIO");
                        parametros.Add("@ESCRITORIO", request.GrupoArea);
                    }

                    if (!string.IsNullOrEmpty(request.Localidade))
                    {
                        whereClause.AppendLine("AND LOC.COD_LOCALIDADE = @LOCALIDADE");
                        parametros.Add("@LOCALIDADE", request.Localidade);
                    }

                    if (!string.IsNullOrEmpty(request.Supervisor))
                    {
                        whereClause.AppendLine("AND EQ.COD_SUPERV = @SUPERVISOR");
                        parametros.Add("@SUPERVISOR", request.Supervisor);
                    }

                    if (!string.IsNullOrEmpty(request.Tecnico))
                    {
                        whereClause.AppendLine("AND D.EQUIPE = @EQUIPE");
                        parametros.Add("@EQUIPE", request.Tecnico);
                    }

                    if (!string.IsNullOrEmpty(request.NaturezaServico))
                    {
                        whereClause.AppendLine("    AND NP.ID IN (@NATUREZA)");
                        parametros.Add("@NATUREZA", request.NaturezaServico);
                    }


                    sql.Append(whereClause.ToString());

                    sql.AppendLine("GROUP BY NP.DESCRICAO, NP.ID, TSV.DESCRICAO;");

                    parametros.Add("@DATAHOJE", dataReferencia.Date);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {

                        while (await reader.ReadAsync())
                        {
                            var natureza = reader["ID_NATUREZA"].ToString();
                            var turnoDescricao = reader["DESCRICAO_TURNO"].ToString();

                            resultadoFinal.Add(new ResultadoFinal
                            {
                                Id = $"Roteirizado_{natureza}_{turnoDescricao}",
                                Valor = string.IsNullOrEmpty(reader["ROTEIRIZADO"].ToString()) ? 0 : Convert.ToInt16(reader["ROTEIRIZADO"]),
                            });
                            resultadoFinal.Add(new ResultadoFinal
                            {
                                Id = $"Executado_{natureza}_{turnoDescricao}",
                                Valor = string.IsNullOrEmpty(reader["EXECUTADO"].ToString()) ? 0 : Convert.ToInt16(reader["EXECUTADO"]),
                            });
                            resultadoFinal.Add(new ResultadoFinal
                            {
                                Id = $"Sla_{natureza}_{turnoDescricao}",
                                Valor = string.IsNullOrEmpty(reader["SLA"].ToString()) ? 0 : Convert.ToInt16(reader["SLA"]),
                            });
                            resultadoFinal.Add(new ResultadoFinal
                            {
                                Id = $"Tec1_{natureza}_{turnoDescricao}",
                                Valor = string.IsNullOrEmpty(reader["TEC1"].ToString()) ? 0 : Convert.ToInt16(reader["TEC1"]),
                            });
                            resultadoFinal.Add(new ResultadoFinal
                            {
                                Id = $"Age1_{natureza}_{turnoDescricao}",
                                Valor = string.IsNullOrEmpty(reader["AGE1"].ToString()) ? 0 : Convert.ToInt16(reader["AGE1"]),
                            });
                        }
                    }
                }
            }

            return resultadoFinal;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetProxVencimentoPerdaPrazo")]
        public async Task<DataTableResponse<ServicosProxVencimentoPerdaPrazoModel>> GetProxVencimentoPerdaPrazo(ServicosProxVencimentoPerdaPrazoModel_Request request)
        {

            List<ServicosProxVencimentoPerdaPrazoModel> retorno = new List<ServicosProxVencimentoPerdaPrazoModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                var dataReferencia = String.IsNullOrEmpty(request?.DataReferencia) ? DateTime.Now : Convert.ToDateTime(request?.DataReferencia);
                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT");
                    sql.AppendLine("    G.DESCRICAO AS NATUREZA_PRODUCAO,");
                    sql.AppendLine("    COUNT(B.NUM_OBRA) AS SLA_HOJE,");
                    sql.AppendLine("    SUM(CASE WHEN B.DATA_MAXIMA <= @DATAHOJE AND B.EQUIPE IS NOT NULL THEN 1 ELSE 0 END) AS ROTEIRIZADO,");
                    sql.AppendLine("    SUM(CASE WHEN B.DATA_MAXIMA <= @DATAHOJE AND B.EQUIPE IS NULL THEN 1 ELSE 0 END) AS SEM_ROTEIRIZAR,");
                    sql.AppendLine("    SUM(CASE WHEN B.DATA_MAXIMA BETWEEN DATEADD(DAY, 1, @DATAHOJE) AND DATEADD(DAY, 2, @DATAHOJE) THEN 1 ELSE 0 END) AS BACKLOG,");
                    sql.AppendLine("    SUM(CASE WHEN B.DATA_MAXIMA > DATEADD(DAY, 2, @DATAHOJE) THEN 1 ELSE 0 END) AS BACKLOG4,");
                    sql.AppendLine("    SUM(CASE WHEN B.DATA_MAXIMA IS NULL THEN 1 ELSE 0 END) AS SEM_AGENDAMENTO");
                    sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO B");
                    sql.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE C ON B.COD_TIPO_SERV = C.COD_TIPO_SERV");
                    sql.AppendLine("LEFT JOIN TB_LOCALIDADE D ON B.COD_ESCRITORIO = D.COD_ESCRITORIO AND B.COD_AREA = D.COD_AREA AND B.COD_LOCALIDADE = D.COD_LOCALIDADE");
                    sql.AppendLine("LEFT JOIN TB_ESCRITORIO E ON B.COD_ESCRITORIO = E.COD_ESCRITORIO");
                    sql.AppendLine("LEFT JOIN TB_AREA F ON B.COD_ESCRITORIO = F.COD_ESCRITORIO AND B.COD_AREA = F.COD_AREA");
                    sql.AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO G ON G.ID = C.COD_NATUREZA");
                    sql.AppendLine("LEFT JOIN TB_EQUIPE EQ ON EQ.NUM_FRE = B.EQUIPE");

                    StringBuilder whereClause = new StringBuilder();
                    whereClause.AppendLine("WHERE B.DATA_INDICADOR = @DATAHOJE");

                    // Adiciona filtros adicionais conforme necessário
                    if (!string.IsNullOrEmpty(request.Area))
                    {
                        whereClause.AppendLine("AND B.COD_AREA = @AREA");
                        parametros.Add("@AREA", request.Area);
                    }

                    if (!string.IsNullOrEmpty(request.GrupoArea))
                    {
                        whereClause.AppendLine("AND B.COD_ESCRITORIO = @ESCRITORIO");
                        parametros.Add("@ESCRITORIO", request.GrupoArea);
                    }

                    if (!string.IsNullOrEmpty(request.Localidade))
                    {
                        whereClause.AppendLine("AND D.COD_LOCALIDADE = @LOCALIDADE");
                        parametros.Add("@LOCALIDADE", request.Localidade);
                    }

                    if (!string.IsNullOrEmpty(request.Supervisor))
                    {
                        whereClause.AppendLine("AND EQ.COD_SUPERV = @SUPERVISOR");
                        parametros.Add("@SUPERVISOR", request.Supervisor);
                    }

                    if (!string.IsNullOrEmpty(request.Tecnico))
                    {
                        whereClause.AppendLine("AND B.EQUIPE = @EQUIPE");
                        parametros.Add("@EQUIPE", request.Tecnico);
                    }

                    sql.Append(whereClause.ToString());

                    sql.AppendLine("GROUP BY G.DESCRICAO;");
                    parametros.Add("@DATAHOJE", dataReferencia.Date);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ServicosProxVencimentoPerdaPrazoModel
                            {
                                Descricao = string.IsNullOrEmpty(Convert.ToString(reader["NATUREZA_PRODUCAO"])) ? "0" : Convert.ToString(reader["NATUREZA_PRODUCAO"]),
                                Sla_hoje = string.IsNullOrEmpty(Convert.ToString(reader["SLA_HOJE"])) ? "0" : Convert.ToString(reader["SLA_HOJE"]),
                                Roteirizado = string.IsNullOrEmpty(Convert.ToString(reader["ROTEIRIZADO"])) ? "0" : Convert.ToString(reader["ROTEIRIZADO"]),
                                SemRoteirizar = string.IsNullOrEmpty(Convert.ToString(reader["SEM_ROTEIRIZAR"])) ? "0" : Convert.ToString(reader["SEM_ROTEIRIZAR"]),
                                Backlog = string.IsNullOrEmpty(Convert.ToString(reader["BACKLOG"])) ? "0" : Convert.ToString(reader["BACKLOG"]),
                                Backlog4 = string.IsNullOrEmpty(Convert.ToString(reader["BACKLOG4"])) ? "0" : Convert.ToString(reader["BACKLOG4"]),
                                SemAgendamento = string.IsNullOrEmpty(Convert.ToString(reader["SEM_AGENDAMENTO"])) ? "0" : Convert.ToString(reader["SEM_AGENDAMENTO"]),
                            });

                        }
                    }
                }
            }

            return new DataTableResponse<ServicosProxVencimentoPerdaPrazoModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.ToList().Count,
                data = retorno.ToList(),
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetServicosStatusExecucao")]
        public async Task<List<ResultadoFinal>> GetServicosStatusExecucao(ServicosProxVencimentoPerdaPrazoModel_Request request)
        {
            List<ResultadoFinal> resultadoFinal = new List<ResultadoFinal>();
            var dataReferencia = string.IsNullOrEmpty(request?.DataReferencia) ? DateTime.Now : Convert.ToDateTime(request?.DataReferencia);

            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            List<ServicosStatusExecucaoModel> retorno = new List<ServicosStatusExecucaoModel>();

            using (var session = HibernateUtil.GetSession())
            using (var sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoHorario = new Conexao<Turno_ServicoAniel>(sessionT5))
            {

                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT");
                    sql.AppendLine("    NP.ID,");
                    sql.AppendLine("    NP.DESCRICAO,");
                    sql.AppendLine("    COALESCE (TSV.DESCRICAO, null, 'Sem Turno') AS DESCRICAO_TURNO,");
                    sql.AppendLine("     COUNT(D.NUM_OBRA) AS ROTEIRIZADO, ");
                    sql.AppendLine("    COUNT(CASE WHEN D.STATUS_EXECUCAO IN (2) THEN 1 END) AS PRODUTIVO,");
                    sql.AppendLine("    COUNT(CASE WHEN D.STATUS_EXECUCAO IN (1) AND CE.ENCERRA_OS_APOS_BAIXA = 'S' THEN 1 END) AS IMPRODUTIVO,");
                    sql.AppendLine("    COUNT(CASE WHEN D.STATUS_EXECUCAO IN (3, 6, 7, 8, 10, 11, 12) THEN 1 END) AS EMANDAMENTO,");
                    sql.AppendLine("                COUNT(CASE WHEN D.STATUS_EXECUCAO in (1, 4, 13) AND (D.COD_ENCERRAMENTO IS null or CE.ENCERRA_OS_APOS_BAIXA = 'N')  THEN 1 END) AS REAGENDAMENTO,");
                    sql.AppendLine("    COUNT(CASE WHEN D.STATUS_EXECUCAO = 0 THEN 1 END) AS AEXECUTAR");
                    sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO D");
                    sql.AppendLine("LEFT JOIN (SELECT DESCRICAO, HORA_INI, HORA_FIM FROM tb_turno_servico WHERE STATUS = '1') TSV");
                    sql.AppendLine("    ON TSV.HORA_INI <= D.HORA_MAXIMA AND TSV.HORA_FIM >= D.HORA_MAXIMA");
                    sql.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE TS ON TS.COD_TIPO_SERV = D.COD_TIPO_SERV AND D.CODCT = TS.CODCT");
                    sql.AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO NP ON NP.ID = TS.COD_NATUREZA");
                    sql.AppendLine("LEFT JOIN TB_LOCALIDADE LOC ON D.COD_ESCRITORIO = LOC.COD_ESCRITORIO AND D.COD_AREA = LOC.COD_AREA AND D.COD_LOCALIDADE = LOC.COD_LOCALIDADE");
                    sql.AppendLine("LEFT JOIN TB_ESCRITORIO ESC ON D.COD_ESCRITORIO = ESC.COD_ESCRITORIO");
                    sql.AppendLine("LEFT JOIN TB_AREA F ON D.COD_ESCRITORIO = F.COD_ESCRITORIO AND D.COD_AREA = F.COD_AREA");
                    sql.AppendLine("LEFT JOIN TB_EQUIPE EQ ON EQ.NUM_FRE = D.EQUIPE");
                    sql.AppendLine("LEFT JOIN TB_CODIGO_ENCERRAMENTO CE ON CE.CODCT = D.CODCT AND CE.COD_ENCERRAMENTO = D.COD_ENCERRAMENTO");
                    sql.AppendLine("WHERE D.DATA_DASHBOARD = @DATAHOJE OR D.DATA_MAXIMA = @DATAHOJE");

                    if (!string.IsNullOrEmpty(request.Area))
                    {
                        sql.AppendLine("AND D.COD_AREA = @AREA");
                        parametros.Add("@AREA", request.Area);
                    }

                    if (!string.IsNullOrEmpty(request.GrupoArea))
                    {
                        sql.AppendLine("AND D.COD_ESCRITORIO = @ESCRITORIO");
                        parametros.Add("@ESCRITORIO", request.GrupoArea);
                    }

                    if (!string.IsNullOrEmpty(request.Localidade))
                    {
                        sql.AppendLine("AND LOC.COD_LOCALIDADE = @LOCALIDADE");
                        parametros.Add("@LOCALIDADE", request.Localidade);
                    }

                    if (!string.IsNullOrEmpty(request.Supervisor))
                    {
                        sql.AppendLine("AND EQ.COD_SUPERV = @SUPERVISOR");
                        parametros.Add("@SUPERVISOR", request.Supervisor);
                    }

                    if (!string.IsNullOrEmpty(request.Tecnico))
                    {
                        sql.AppendLine("AND D.EQUIPE = @EQUIPE");
                        parametros.Add("@EQUIPE", request.Tecnico);
                    }

                    sql.AppendLine("GROUP BY NP.DESCRICAO, NP.ID, TSV.DESCRICAO");

                    parametros.Add("@DATAHOJE", dataReferencia.Date);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {

                        while (await reader.ReadAsync())
                        {
                            var natureza = reader["ID"].ToString();
                            var turnoDescricao = reader["DESCRICAO_TURNO"].ToString();

                            resultadoFinal.Add(new ResultadoFinal
                            {
                                Id = $"Roteirizado_{natureza}_{turnoDescricao}",
                                Valor = string.IsNullOrEmpty(reader["ROTEIRIZADO"].ToString()) ? 0 : Convert.ToInt16(reader["ROTEIRIZADO"])
                            });
                            resultadoFinal.Add(new ResultadoFinal
                            {
                                Id = $"Produtivo_{natureza}_{turnoDescricao}",
                                Valor = string.IsNullOrEmpty(reader["PRODUTIVO"].ToString()) ? 0 : Convert.ToInt16(reader["PRODUTIVO"])
                            });
                            resultadoFinal.Add(new ResultadoFinal
                            {
                                Id = $"Improdutivo_{natureza}_{turnoDescricao}",
                                Valor = string.IsNullOrEmpty(reader["IMPRODUTIVO"].ToString()) ? 0 : Convert.ToInt16(reader["IMPRODUTIVO"])
                            });
                            resultadoFinal.Add(new ResultadoFinal
                            {
                                Id = $"Em_andamento_{natureza}_{turnoDescricao}",
                                Valor = string.IsNullOrEmpty(reader["EMANDAMENTO"].ToString()) ? 0 : Convert.ToInt16(reader["EMANDAMENTO"])
                            });
                            resultadoFinal.Add(new ResultadoFinal
                            {
                                Id = $"Reagendamento_{natureza}_{turnoDescricao}",
                                Valor = string.IsNullOrEmpty(reader["REAGENDAMENTO"].ToString()) ? 0 : Convert.ToInt16(reader["REAGENDAMENTO"])
                            });
                            resultadoFinal.Add(new ResultadoFinal
                            {
                                Id = $"A_Executar_{natureza}_{turnoDescricao}",
                                Valor = string.IsNullOrEmpty(reader["AEXECUTAR"].ToString()) ? 0 : Convert.ToInt16(reader["AEXECUTAR"])
                            });
                        }
                    }
                }
            }

            return resultadoFinal;
        }

        [HttpPost()]
        [Route("api/Dashboard_Servicosapi/GetPainelIndicadores")]
        public async Task<DataTableResponse<DocumentosPainelIndicadores>> GetPainelIndicadores(PainelIndicadoresModel_Request request)
        {
            List<DocumentosPainelIndicadores> retorno = new List<DocumentosPainelIndicadores>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoArea = new Conexao<AreaAniel>(sessionT5Nh))
            using (var repoGrupoArea = new Conexao<EscritorioAniel>(sessionT5Nh))
            using (var repoDoc = new Conexao<Documento_ProducaoAniel>(sessionT5Nh))
            using (var repoReinc = new Conexao<ReincidenciaDocProdAniel>(sessionT5Nh))
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                var dataReferencia = String.IsNullOrEmpty(request?.DataReferencia) ? DateTime.Now : Convert.ToDateTime(request?.DataReferencia);
                var periodo = GetPeriodoFiltro(request.TipoFiltroData.Value, dataReferencia);

                //Select
                using (var com = sessionT5Nh.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    if (request.tipoFiltroRegional == "1")
                    {

                        sql.AppendLine("SELECT * FROM SP_DASHBOARD_SERVICOS_AREA(@DATAINICIO, @DATAFIM) WHERE COD_AREA IS NOT NULL");
                    }
                    else
                    {
                        sql.AppendLine("SELECT * FROM SP_DASHBOARD_SERVICOS_GR_AREA(@DATAINICIO, @DATAFIM)");
                    }


                    parametros.Add("@DATAINICIO", periodo.dataInicio);
                    parametros.Add("@DATAFIM", periodo.dataFim);

                    //if (!string.IsNullOrEmpty(request.Area))
                    //{
                    //    parametros.Add("@AREA", request.Area);
                    //}

                    //if (!string.IsNullOrEmpty(request.GrupoArea))
                    //{
                    //    parametros.Add("@ESCRITORIO", request.GrupoArea);
                    //}

                    //if (!string.IsNullOrEmpty(request.Localidade))
                    //{
                    //    parametros.Add("@LOCALIDADE", request.Localidade);
                    //}

                    //if (!string.IsNullOrEmpty(request.Supervisor))
                    //{
                    //    parametros.Add("@SUPERVISOR", request.Supervisor);
                    //}

                    //if (!string.IsNullOrEmpty(request.Tecnico))
                    //{
                    //    parametros.Add("@EQUIPE", request.Tecnico);
                    //}
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new DocumentosPainelIndicadores
                            {
                                Descricao = request.tipoFiltroRegional == "1" ? reader["DESCRICAO"].ToString() : "",
                                Escritorio = reader["ESCRITORIO"].ToString(),
                                PorcProdu = string.IsNullOrEmpty(reader["PRODUTIVO"].ToString()) ? "0" : reader["PRODUTIVO"].ToString(),
                                PorcRTI = string.IsNullOrEmpty(reader["QTD_RTI"].ToString()) ? "0" : reader["QTD_RTI"].ToString(),
                                PorcRev1 = string.IsNullOrEmpty(reader["QTD_REV1"].ToString()) ? "0" : reader["QTD_REV1"].ToString(),
                                PorcRev3 = string.IsNullOrEmpty(reader["QTD_REV3"].ToString()) ? "0" : reader["QTD_REV3"].ToString(),
                                PorcSLA = string.IsNullOrEmpty(reader["SLA"].ToString()) ? "0" : reader["SLA"].ToString(),
                                PorcTEC1 = string.IsNullOrEmpty(reader["TEC1"].ToString()) ? "0" : reader["TEC1"].ToString(),
                                PorcAGE1 = string.IsNullOrEmpty(reader["AGE1"].ToString()) ? "0" : reader["AGE1"].ToString()
                            });
                        }
                    }
                }

            }

            return new DataTableResponse<DocumentosPainelIndicadores>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.ToList().Count,
                data = retorno.ToList(),
                error = ""
            };
        }

        #endregion

        public (DateTime? dataInicio, DateTime? dataFim) GetPeriodoFiltro(TipoFiltroData tipoFiltroData, DateTime dataReferencia)
        {
            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (tipoFiltroData == TipoFiltroData.DataAtual)
            {
                dataInicio = dataReferencia;
                dataFim = dataReferencia;
            }

            else if (tipoFiltroData == TipoFiltroData.UltimosSeteDias)
            {
                dataInicio = dataReferencia.AddDays(-7);
                dataFim = dataReferencia;
            }

            else if (tipoFiltroData == TipoFiltroData.UltimosTrintaDias)
            {
                dataInicio = dataReferencia.AddDays(-30);
                dataFim = dataReferencia;
            }

            else if (tipoFiltroData == TipoFiltroData.EstaSemana)
            {
                dataInicio = StartOfWeek(dataReferencia, DayOfWeek.Sunday);
                dataFim = dataReferencia;
            }

            else if (tipoFiltroData == TipoFiltroData.EsteMes)
            {
                dataInicio = new DateTime(dataReferencia.Year, dataReferencia.Month, 1);
                dataFim = dataReferencia;
            }


            return (dataInicio, dataFim);
        }

        public DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        private double Capacidade(ISession sessionT5, ISession sessionT5NH, Configuracoes_Gerais configGerais, List<Horario_Connect> horarios_servico, double TempoMedio, TipoHabilidade habilidade)
        {
            var dao = new RoteirizacaoDAO(sessionT5: sessionT5, sessionT5NH: sessionT5NH);
            var tecnicos = dao.GetTecnicoPlanejamentoRoteirizacaoSql(configGerais, new RoteirizacaoDiaConf
            {
                Data = DateTime.Today,
                Alterar_DataHora_SemAgendamentoAniel = true,
                horarios_servico = horarios_servico,
            }, new GestaoCapacidadeRoteirizacaoRequest()).Result;

            tecnicos = tecnicos.FindAll(p => p.TipoHabilidade == habilidade);

            double capacidade = tecnicos.Select(p => p.HorasTrabalhadasHorario).Sum() / TempoMedio;
            return capacidade;
        }

        private TipoHabilidade? GetTipoHabilidade(List<Natureza_ProducaoAniel> naturezasProducao)
        {
            TipoHabilidade? tipoHabilidade = null;
            if (naturezasProducao.Where(p => p.TipoHabilidade == TipoHabilidade.Instalacao).Any() && naturezasProducao.Where(p => p.TipoHabilidade == TipoHabilidade.Reparo).Any())
            {
                tipoHabilidade = TipoHabilidade.Multi;
            }
            else if (naturezasProducao.Where(p => p.TipoHabilidade == TipoHabilidade.Instalacao).Any())
            {
                tipoHabilidade = TipoHabilidade.Instalacao;
            }
            else if (naturezasProducao.Where(p => p.TipoHabilidade == TipoHabilidade.Reparo).Any())
            {
                tipoHabilidade = TipoHabilidade.Reparo;
            }

            return tipoHabilidade;
        }
        private TimeSpan Tempo_Medio(IEnumerable<TimeSpan> timeSpans)
        {
            TimeSpan tempo_atraso_total = new TimeSpan();

            foreach (var tempo in timeSpans)
            {
                tempo_atraso_total += tempo;
            }


            if (timeSpans.Count() != 0)
            {
                var media = tempo_atraso_total.TotalMinutes / timeSpans.Count();
                return TimeSpan.FromMinutes(media);
            }

            else
            {
                return TimeSpan.FromMinutes(0);
            }


        }
    }

}
