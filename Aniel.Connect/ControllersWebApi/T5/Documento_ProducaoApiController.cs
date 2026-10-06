using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Attributes;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Integracoes.FaturamentoEVA.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.ClientePlanoProduto;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.CodigoEncerramento;
using Aniel.Connect.Repository.T5.Contrato;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.Natureza;
using Aniel.Connect.Repository.Usuario;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.mongo;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using NHibernate;
using Resources;
using RestSharp;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model;
using Sinapse.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using static Aniel.Connect.ConnectBusiness.T5.DocumentoProducaoAnielBusiness;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Documento_ProducaoApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/Documento_ProducaoApi/GetOSsAcompamentoEquipe")]
        public async Task<DataTableResponse<Acompanhamento_EquipeListaItemModel>> GetOSsAcompamentoEquipe(Acompanhamento_EquipesOss_DataTableRequest request)
        {
            List<Acompanhamento_EquipeListaItemModel> retorno = new List<Acompanhamento_EquipeListaItemModel>();
            IOrderedEnumerable<Acompanhamento_EquipeListaItemModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<Atendimento> repoAtendimento = new Conexao<Atendimento>(session))
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                var usuarioBusiness = new UsuarioBusiness(repoUsuario);

                IdentityUser user = usuarioBusiness.GetById(User.Identity.GetUserId());
                Boolean Exibe_RDOs = (await new Conexao<Configuracoes_Gerais>(session).selectFirst()).ExibeRDOs;

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder(@" SELECT A.NUM_OBRA_ORIGINAL,A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, B.DESCRICAO AS TIPO_SERVICO, A.STATUS_EXECUCAO,A.ID_ATENDIMENTO_CONNECT,
                                                                    A.DATA_INDICADOR + A.HORA_INDICADOR AS DATAHORA_INDICADOR, A.TITULAR, A.ENDERECO, A.ENDERECO_TIPO, A.ENDERECO_BAIRRO, A.ENDERECO_CPL AS CEP, A.DIA + A.HORA AS DATAHORA_CRIACAO,
                                                                    A.ENDERECO_TIPO AS CIDADE, A.ENDERECO_COMPL, A.ENDERECO_NUMERO AS NUMERO, A.DATAHORA_DESLOCAMENTO, A.DATAHORA_INI_ATEND, A.DATAHORA_FIM_ATEND, A.DATAHORA_PAUSOU, A.OBSERVACAO_PAUSA,
                                                                    A.DATAHORA_REINICIO, A.DATA_PROD,A.HORA_PROD, A.DATA_MAXIMA + COALESCE(A.HORA_MAXIMA, CAST('00:00' AS TIME)) AS DATAHORA_AGENDAMENTO, A.SIGLA_MODULO, A.RESPONSABILIDADE, A.ROTERIZAR,A.STATUS_COBRANCA,G.NOME AS NOME_EQUIPE,
                                                                    SUM(D.QUANT_PROD * E.QUANTIDADE) QTDE_PROD, SUM(D.QUANT_CATAL * E.QUANTIDADE) QTDE_PREV,F.DESCRICAO AS DESC_TURNO
                                                            FROM TB_DOCUMENTO_PRODUCAO A
                                                            INNER JOIN TB_TIPO_SERVICO_EQUIPE B
                                                            ON (A.COD_TIPO_SERV = B.COD_TIPO_SERV)
                                                            LEFT JOIN TB_CODIGO_ENCERRAMENTO C
                                                            ON (A.COD_ENCERRAMENTO = C.COD_ENCERRAMENTO AND A.CODCT = C.CODCT)
                                                            LEFT JOIN TB_CUSTOS_TAREFA D
                                                            ON (A.CODCT = D.CODCT AND A.PROJETO = D.PROJETO AND A.NUM_OBRA = D.NUM_OBRA AND A.NUM_DOC = D.NUM_DOC)
                                                            LEFT JOIN TB_CADERNO_ENCARGOS E
                                                            ON (D.CODIGO = E.CODIGO AND D.UNIDADE_CUSTO = E.UNIDADE_CUSTO)
                                                            LEFT JOIN TB_TURNO_SERVICO F
                                                            ON (A.COD_TURNO_SERV = F.ID)
                                                            INNER JOIN TB_EQUIPE G
                                                            ON (G.EQUIPE = A.EQUIPE)
                                                            
                                                        WHERE A.EQUIPE = @EQUIPE ");

                    sql.AppendLine(" AND (");
                    // Aberto Aguardando Atendimento(0), Enviado ao Técnico(), OS em Deslocamento, Atendimento Iniciado/Reiniciado (3) e Aberta Aguardando Fechamento ().
                    sql.AppendLine("(A.DATA_MAXIMA BETWEEN @DataAbertoIni AND @DataAbertoFim AND A.STATUS_EXECUCAO IN (0,6,3))");
                    parametros.Add("@DataAbertoIni", request.DataAbertoIni);
                    parametros.Add("@DataAbertoFim", request.DataAbertoFim);

                    // Atendimento Paralisado
                    sql.AppendLine("OR (A.DATA_MAXIMA BETWEEN @DataParalisadosIni AND @DataParalisadosFim AND A.STATUS_EXECUCAO = 10)");
                    parametros.Add("@DataParalisadosIni", request.DataParalisadosIni);
                    parametros.Add("@DataParalisadosFim", request.DataParalisadosFim);

                    // Atendimento Finalizado, Fechada Improdutiva, Fechada Produtiva e Cancelado
                    sql.AppendLine("OR (A.DATA_MAXIMA BETWEEN @DataEncerradosIni AND @DataEncerradosFim AND A.STATUS_EXECUCAO IN (8,1,2,9))");
                    parametros.Add("@DataEncerradosIni", request.DataEncerradosIni);
                    parametros.Add("@DataEncerradosFim", request.DataEncerradosFim);

                    sql.AppendLine(")");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    if (request.Situacao.HasValue)
                    {
                        switch (request.Situacao)
                        {
                            case 1: //Em Andamento
                                sql.AppendLine(" AND (A.STATUS_EXECUCAO NOT IN(1,2,9,8)) "); //1 = IMPRODUTIVA, 2 = PRODUTIVA, 9 = CANCELADA,8 = ATENDIMENTO FINALIZADO
                                break;
                            case 2: //Concluídas Produtivas
                                sql.AppendLine(" AND (C.STATUS_EXECUCAO = 2) "); //2 = PRODUTIVA
                                break;
                            case 3: //Concluídas Improdutivas
                                sql.AppendLine(" AND (C.STATUS_EXECUCAO = 1) "); //1 = IMPRODUTIVA
                                break;
                        }

                    }

                    #region Filtro Perfil Visualização de Serviço

                    if (user.PerfilVisualizacaoServico != null)
                    {
                        if (user.PerfilVisualizacaoServico.ParamsContratoProjeto.Count() > 0)
                        {
                            var filtrosContratoProj = new List<string>();

                            foreach (var paramContratoProj in user.PerfilVisualizacaoServico.ParamsContratoProjeto)
                            {
                                if (!String.IsNullOrEmpty(paramContratoProj.CodCt) && !String.IsNullOrEmpty(paramContratoProj.Projeto))
                                {
                                    filtrosContratoProj.Add($"(A.CODCT = '{paramContratoProj.CodCt}' AND A.PROJETO = '{paramContratoProj.Projeto}')");
                                }
                                else
                                {
                                    filtrosContratoProj.Add($"(A.CODCT = '{paramContratoProj.CodCt}')");
                                }
                            }

                            sql.Append($" AND ({String.Join(" OR ", filtrosContratoProj)})");
                        }

                        if (user.PerfilVisualizacaoServico.ParamsArea.Count() > 0)
                        {
                            var filtrosArea = new List<string>();

                            foreach (var paramArea in user.PerfilVisualizacaoServico.ParamsArea)
                            {
                                if (!String.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                   !String.IsNullOrEmpty(paramArea.CodArea) &&
                                   paramArea.CodLocalidade.HasValue)
                                {
                                    filtrosArea.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}' AND A.COD_LOCALIDADE = {paramArea.CodLocalidade.Value})");
                                }
                                else if (!String.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                         !String.IsNullOrEmpty(paramArea.CodArea))
                                {
                                    filtrosArea.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}')");
                                }
                                else
                                {
                                    filtrosArea.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}')");
                                }
                            }

                            sql.Append($" AND ({String.Join(" OR ", filtrosArea)})");
                        }

                        if (user.PerfilVisualizacaoServico.ParamsSetorDepto.Count() > 0)
                        {
                            var filtrosSetor = new List<string>();

                            foreach (var paramSetor in user.PerfilVisualizacaoServico.ParamsSetorDepto)
                            {
                                filtrosSetor.Add($"(A.COD_DEPTO = {paramSetor.CodDepto})");
                            }

                            sql.Append($" AND ({String.Join(" OR ", filtrosSetor)})");
                        }

                        if (user.PerfilVisualizacaoServico.ParamsSupervisor.Count() > 0)
                        {
                            var filtrosSupervisor = new List<string>();

                            foreach (var paramSupervisor in user.PerfilVisualizacaoServico.ParamsSupervisor)
                            {
                                filtrosSupervisor.Add($"(A.COD_SUPERV = '{paramSupervisor.CodSuperv}')");
                            }

                            sql.Append($" AND ({String.Join(" OR ", filtrosSupervisor)})");
                        }

                        if (user.PerfilVisualizacaoServico.ParamsMonitor.Count() > 0)
                        {
                            var filtrosMonitor = new List<string>();

                            foreach (var paramMonitor in user.PerfilVisualizacaoServico.ParamsMonitor)
                            {
                                filtrosMonitor.Add($"(A.USUARIO_MONITOR = '{paramMonitor.UsuarioMonitor}')");
                            }

                            sql.Append($" AND ({String.Join(" OR ", filtrosMonitor)})");
                        }
                    }

                    #endregion

                    sql.AppendLine(@" GROUP BY A.NUM_OBRA_ORIGINAL, A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, B.DESCRICAO, A.STATUS_EXECUCAO,
                                               A.DATA_INDICADOR + A.HORA_INDICADOR, A.TITULAR, A.ENDERECO, A.ENDERECO_TIPO, A.ENDERECO_BAIRRO,
                                               A.ENDERECO_CPL, A.DIA + A.HORA, A.ENDERECO_TIPO, A.ENDERECO_COMPL, A.ENDERECO_NUMERO,
                                               A.DATAHORA_DESLOCAMENTO, A.DATAHORA_INI_ATEND, A.DATAHORA_FIM_ATEND, A.DATAHORA_PAUSOU, A.OBSERVACAO_PAUSA,
                                               A.DATAHORA_REINICIO, A.DATA_PROD,A.HORA_PROD, A.DATA_MAXIMA + COALESCE(A.HORA_MAXIMA, CAST('00:00' AS TIME)),
                                               A.SIGLA_MODULO, A.RESPONSABILIDADE, A.ROTERIZAR,F.DESCRICAO,G.NOME,A.STATUS_COBRANCA,A.ID_ATENDIMENTO_CONNECT");


                    parametros.Add("@EQUIPE", request.Equipe);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new Acompanhamento_EquipeListaItemModel();

                            obj.CodCt = Convert.ToString(reader["CODCT"]);
                            obj.Projeto = Convert.ToString(reader["PROJETO"]);
                            obj.Numero_Os = Convert.ToString(reader["NUM_OBRA"]);
                            obj.Numero_Os_Original = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]);
                            obj.Numero_Doc = Convert.ToInt32(reader["NUM_DOC"]);
                            obj.Data_Agendamento = reader["DATAHORA_AGENDAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_AGENDAMENTO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty;
                            obj.Periodo = Convert.ToString(reader["DESC_TURNO"]);
                            obj.Tipo_Servico = Convert.ToString(reader["TIPO_SERVICO"]);
                            obj.Status_Execucao = reader["STATUS_EXECUCAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(reader["STATUS_EXECUCAO"])) : "";
                            obj.Status_Cobranca = reader["STATUS_COBRANCA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_CobrancaAniel>(Convert.ToString(reader["STATUS_COBRANCA"])) : "";
                            //obj.Status_Cobranca = HtmlEnumExtensions.GetEnumDescription<Status_CobrancaAniel>(Convert.ToString(reader["STATUS_COBRANCA"]));
                            obj.Assinante = Convert.ToString(reader["SIGLA_MODULO"]) == "MCO" ? Convert.ToString(reader["NUM_DOC"]) : Convert.ToString(reader["TITULAR"]);
                            obj.Recebido_Tecnico = Convert.ToString(reader["RESPONSABILIDADE"]);
                            obj.Roterizar = Convert.ToString(reader["ROTERIZAR"]);
                            obj.Tecnico = Convert.ToString(reader["NOME_EQUIPE"]);


                            if (obj.Recebido_Tecnico == "2" && obj.Roterizar == "S")
                            {
                                obj.Recebido_Tecnico = "Sim";
                            }
                            else
                            {
                                obj.Recebido_Tecnico = "Não";
                            }

                            obj.Endereco = Convert.ToString(reader["ENDERECO"]);
                            obj.Endereco_Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]);
                            obj.Endereco_Tipo = Convert.ToString(reader["ENDERECO_TIPO"]);

                            long? _IdAtendimento = reader["ID_ATENDIMENTO_CONNECT"] != DBNull.Value ? Convert.ToInt64(reader["ID_ATENDIMENTO_CONNECT"]) : (long?)null;
                            var atendimento = _IdAtendimento != null ? await repoAtendimento.selectFirst(x => x.Id == _IdAtendimento) : null;
                            obj.Numero_Atendimento = atendimento?.Protocolo;
                            obj.Numero = Convert.ToString(reader["NUMERO"]);
                            obj.Deslocamento = reader["DATAHORA_DESLOCAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_DESLOCAMENTO"]).ToString("dd/MM/yyyy HH:mm:ss") : String.Empty;
                            obj.Inicio_Atendimento = reader["DATAHORA_INI_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INI_ATEND"]).ToString("dd/MM/yyyy HH:mm:ss") : String.Empty;
                            obj.Fim_Atendimento = reader["DATAHORA_FIM_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM_ATEND"]).ToString("dd/MM/yyyy HH:mm:ss") : String.Empty;
                            obj.Motivo_Pausa = Convert.ToString(reader["OBSERVACAO_PAUSA"]);
                            obj.Tempo_Execucao = "-";
                            obj.Tempo_Paralizacoes = "-";
                            obj.Duracao_Total_Visita = "-";
                            obj.Pontos_Previstos = reader["QTDE_PREV"] != DBNull.Value ? Convert.ToDouble(reader["QTDE_PREV"]).ToString("N2") : "0,00";
                            obj.Pontos_Realizados = reader["QTDE_PROD"] != DBNull.Value ? Convert.ToDouble(reader["QTDE_PROD"]).ToString("N2") : "0,00";

                            if (reader["DATAHORA_INDICADOR"] != DBNull.Value)
                            {
                                DateTime? data_criacao = reader["DATAHORA_CRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_CRIACAO"]) : (DateTime?)null;
                                DateTime? data_indicador = reader["DATAHORA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INDICADOR"]) : (DateTime?)null;
                                DateTime? data_fechamento = reader["DATAHORA_FIM_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM_ATEND"]) : (DateTime?)null;

                                obj.SLA_Servicos = GetDescricaoSla(data_indicador, data_criacao, data_fechamento);
                                obj.DataHora_Indicador = data_indicador.Value.ToString("dd/MM/yyyy HH:mm");
                            }
                            else
                                obj.SLA_Servicos = "Sem Indicador de Sla";


                            DateTime? data_encerramento = reader["DATA_PROD"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_PROD"]) : (DateTime?)null;
                            TimeSpan? hora_encerramento = reader["HORA_PROD"] != DBNull.Value ? (TimeSpan?)reader["HORA_PROD"] : (TimeSpan?)null;
                            DateTime? datahora_encerramento = data_encerramento.HasValue && hora_encerramento.HasValue ? data_encerramento.Value.Add(hora_encerramento.Value) : (DateTime?)null;

                            var _statusTemp = GetTempoStatus(obj.CodCt, obj.Projeto, obj.Numero_Os, obj.Numero_Doc, datahora_encerramento);
                            var _paralisacaotemp = _statusTemp.tempoParalisacao;
                            var execucaotemp = _statusTemp.tempoAtendimento;
                            obj.Tempo_Execucao = execucaotemp.ToString(@"hh\:mm");
                            obj.Tempo_Paralizacoes = _paralisacaotemp.ToString(@"hh\:mm");
                            obj.Duracao_Total_Visita = (execucaotemp + _paralisacaotemp).ToString(@"hh\:mm");

                            retorno.Add(obj);

                        }
                    }

                }
            }


            listaOrdenada = retorno.OrderBy(p => p.Data_Agendamento);
            recordsTotal = listaOrdenada.Count();

            //Page e retornando DataTable
            return new DataTableResponse<Acompanhamento_EquipeListaItemModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/Documento_ProducaoApi/BuscaOSsAcompamentoEquipe")]
        public async Task<DataTableResponse<Acompanhamento_EquipeListaItemModel>> BuscaOSsAcompamentoEquipe(Acompanhamento_EquipesOss_DataTableRequest request)
        {
            List<Acompanhamento_EquipeListaItemModel> retorno = new List<Acompanhamento_EquipeListaItemModel>();
            IOrderedEnumerable<Acompanhamento_EquipeListaItemModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                Boolean Exibe_RDOs = (await new Conexao<Configuracoes_Gerais>(session).selectFirst()).ExibeRDOs;

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder(@" SELECT A.NUM_OBRA_ORIGINAL,A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, B.DESCRICAO AS TIPO_SERVICO, A.STATUS_EXECUCAO,
                                                                    A.DATA_INDICADOR + A.HORA_INDICADOR AS DATAHORA_INDICADOR, A.TITULAR, A.ENDERECO, A.ENDERECO_TIPO, A.ENDERECO_BAIRRO, A.ENDERECO_CPL AS CEP, A.DIA + A.HORA AS DATAHORA_CRIACAO,
                                                                    A.ENDERECO_TIPO AS CIDADE, A.ENDERECO_COMPL, A.ENDERECO_NUMERO AS NUMERO, A.DATAHORA_DESLOCAMENTO, A.DATAHORA_INI_ATEND, A.DATAHORA_FIM_ATEND, A.DATAHORA_PAUSOU, A.OBSERVACAO_PAUSA,
                                                                    A.DATAHORA_REINICIO, A.DATA_PROD,A.HORA_PROD, A.DATA_MAXIMA + COALESCE(A.HORA_MAXIMA, CAST('00:00' AS TIME)) AS DATAHORA_AGENDAMENTO, A.SIGLA_MODULO, A.RESPONSABILIDADE, A.ROTERIZAR,A.STATUS_COBRANCA,G.NOME AS NOME_EQUIPE,
                                                                    SUM(D.QUANT_PROD * E.QUANTIDADE) QTDE_PROD, SUM(D.QUANT_CATAL * E.QUANTIDADE) QTDE_PREV,F.DESCRICAO AS DESC_TURNO
                                                            FROM TB_DOCUMENTO_PRODUCAO A
                                                            INNER JOIN TB_TIPO_SERVICO_EQUIPE B
                                                            ON (A.COD_TIPO_SERV = B.COD_TIPO_SERV)
                                                            LEFT JOIN TB_CODIGO_ENCERRAMENTO C
                                                            ON (A.COD_ENCERRAMENTO = C.COD_ENCERRAMENTO AND A.CODCT = C.CODCT)
                                                            LEFT JOIN TB_CUSTOS_TAREFA D
                                                            ON (A.CODCT = D.CODCT AND A.PROJETO = D.PROJETO AND A.NUM_OBRA = D.NUM_OBRA AND A.NUM_DOC = D.NUM_DOC)
                                                            LEFT JOIN TB_CADERNO_ENCARGOS E
                                                            ON (D.CODIGO = E.CODIGO AND D.UNIDADE_CUSTO = E.UNIDADE_CUSTO)
                                                            LEFT JOIN TB_TURNO_SERVICO F
                                                            ON (A.COD_TURNO_SERV = F.ID)
                                                            INNER JOIN TB_EQUIPE G
                                                            ON (G.EQUIPE = A.EQUIPE)
                                                            
                                                        WHERE A.ID_CLIENTE = @VALOR ");


                    sql.AppendLine(" AND (");
                    // Aberto Aguardando Atendimento(0), Enviado ao Técnico(), OS em Deslocamento, Atendimento Iniciado/Reiniciado (3) e Aberta Aguardando Fechamento ().
                    sql.AppendLine("(A.DATA_MAXIMA BETWEEN @DataAbertoIni AND @DataAbertoFim AND A.STATUS_EXECUCAO IN (0,6,3))");
                    parametros.Add("@DataAbertoIni", request.DataAbertoIni);
                    parametros.Add("@DataAbertoFim", request.DataAbertoFim);

                    // Atendimento Paralisado
                    sql.AppendLine("OR (A.DATA_MAXIMA BETWEEN @DataParalisadosIni AND @DataParalisadosFim AND A.STATUS_EXECUCAO = 10)");
                    parametros.Add("@DataParalisadosIni", request.DataParalisadosIni);
                    parametros.Add("@DataParalisadosFim", request.DataParalisadosFim);

                    // Atendimento Finalizado, Fechada Improdutiva, Fechada Produtiva e Cancelado
                    sql.AppendLine("OR (A.DATA_MAXIMA BETWEEN @DataEncerradosIni AND @DataEncerradosFim AND A.STATUS_EXECUCAO IN (8,1,2,9))");
                    parametros.Add("@DataEncerradosIni", request.DataEncerradosIni);
                    parametros.Add("@DataEncerradosFim", request.DataEncerradosFim);

                    sql.AppendLine(")");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    if (request.Situacao.HasValue)
                    {
                        switch (request.Situacao)
                        {
                            case 1: //Em Andamento
                                sql.AppendLine(" AND (A.STATUS_EXECUCAO NOT IN(1,2,9,8)) "); //1 = IMPRODUTIVA, 2 = PRODUTIVA, 9 = CANCELADA,8 = ATENDIMENTO FINALIZADO
                                break;
                            case 2: //Concluídas Produtivas
                                sql.AppendLine(" AND (C.STATUS_EXECUCAO = 2) "); //2 = PRODUTIVA
                                break;
                            case 3: //Concluídas Improdutivas
                                sql.AppendLine(" AND (C.STATUS_EXECUCAO = 1) "); //1 = IMPRODUTIVA
                                break;
                        }

                    }

                    sql.AppendLine(@" GROUP BY A.NUM_OBRA_ORIGINAL, A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, B.DESCRICAO, A.STATUS_EXECUCAO,
                                               A.DATA_INDICADOR + A.HORA_INDICADOR, A.TITULAR, A.ENDERECO, A.ENDERECO_TIPO, A.ENDERECO_BAIRRO,
                                               A.ENDERECO_CPL, A.DIA + A.HORA, A.ENDERECO_TIPO, A.ENDERECO_COMPL, A.ENDERECO_NUMERO,
                                               A.DATAHORA_DESLOCAMENTO, A.DATAHORA_INI_ATEND, A.DATAHORA_FIM_ATEND, A.DATAHORA_PAUSOU, A.OBSERVACAO_PAUSA,
                                               A.DATAHORA_REINICIO, A.DATA_PROD,A.HORA_PROD, A.DATA_MAXIMA + COALESCE(A.HORA_MAXIMA, CAST('00:00' AS TIME)),
                                               A.SIGLA_MODULO, A.RESPONSABILIDADE, A.ROTERIZAR,F.DESCRICAO,G.NOME,A.STATUS_COBRANCA");


                    parametros.Add("@VALOR", request.Id_Cliente);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new Acompanhamento_EquipeListaItemModel();

                            obj.CodCt = Convert.ToString(reader["CODCT"]);
                            obj.Projeto = Convert.ToString(reader["PROJETO"]);
                            obj.Numero_Os = Convert.ToString(reader["NUM_OBRA"]);
                            obj.Numero_Os_Original = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]);
                            obj.Numero_Doc = Convert.ToInt32(reader["NUM_DOC"]);
                            obj.Data_Agendamento = reader["DATAHORA_AGENDAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_AGENDAMENTO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty;
                            obj.Periodo = Convert.ToString(reader["DESC_TURNO"]);
                            obj.Tipo_Servico = Convert.ToString(reader["TIPO_SERVICO"]);
                            obj.Status_Execucao = reader["STATUS_EXECUCAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(reader["STATUS_EXECUCAO"])) : "";
                            obj.Status_Cobranca = reader["STATUS_COBRANCA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_CobrancaAniel>(Convert.ToString(reader["STATUS_COBRANCA"])) : "";
                            //obj.Status_Cobranca = HtmlEnumExtensions.GetEnumDescription<Status_CobrancaAniel>(Convert.ToString(reader["STATUS_COBRANCA"]));
                            obj.Assinante = Convert.ToString(reader["SIGLA_MODULO"]) == "MCO" ? Convert.ToString(reader["NUM_DOC"]) : Convert.ToString(reader["TITULAR"]);
                            obj.Recebido_Tecnico = Convert.ToString(reader["RESPONSABILIDADE"]);
                            obj.Roterizar = Convert.ToString(reader["ROTERIZAR"]);
                            obj.Tecnico = Convert.ToString(reader["NOME_EQUIPE"]);


                            if (obj.Recebido_Tecnico == "2" && obj.Roterizar == "S")
                            {
                                obj.Recebido_Tecnico = "Sim";
                            }
                            else
                            {
                                obj.Recebido_Tecnico = "Não";
                            }

                            obj.Endereco = Convert.ToString(reader["ENDERECO"]);
                            obj.Endereco_Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]);
                            obj.Endereco_Tipo = Convert.ToString(reader["ENDERECO_TIPO"]);
                            obj.Numero = Convert.ToString(reader["NUMERO"]);
                            obj.Deslocamento = reader["DATAHORA_DESLOCAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_DESLOCAMENTO"]).ToString("dd/MM/yyyy HH:mm:ss") : String.Empty;
                            obj.Inicio_Atendimento = reader["DATAHORA_INI_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INI_ATEND"]).ToString("dd/MM/yyyy HH:mm:ss") : String.Empty;
                            obj.Fim_Atendimento = reader["DATAHORA_FIM_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM_ATEND"]).ToString("dd/MM/yyyy HH:mm:ss") : String.Empty;
                            obj.Motivo_Pausa = Convert.ToString(reader["OBSERVACAO_PAUSA"]);
                            obj.Tempo_Execucao = "-";
                            obj.Tempo_Paralizacoes = "-";
                            obj.Duracao_Total_Visita = "-";
                            obj.Pontos_Previstos = reader["QTDE_PREV"] != DBNull.Value ? Convert.ToDouble(reader["QTDE_PREV"]).ToString("N2") : "0,00";
                            obj.Pontos_Realizados = reader["QTDE_PROD"] != DBNull.Value ? Convert.ToDouble(reader["QTDE_PROD"]).ToString("N2") : "0,00";

                            if (reader["DATAHORA_INDICADOR"] != DBNull.Value)
                            {
                                DateTime? data_criacao = reader["DATAHORA_CRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_CRIACAO"]) : (DateTime?)null;
                                DateTime? data_indicador = reader["DATAHORA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INDICADOR"]) : (DateTime?)null;
                                DateTime? data_fechamento = reader["DATAHORA_FIM_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM_ATEND"]) : (DateTime?)null;

                                obj.SLA_Servicos = GetDescricaoSla(data_indicador, data_criacao, data_fechamento);
                                obj.DataHora_Indicador = data_indicador.Value.ToString("dd/MM/yyyy HH:mm");
                            }
                            else
                                obj.SLA_Servicos = "Sem Indicador de Sla";


                            DateTime? data_encerramento = reader["DATA_PROD"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_PROD"]) : (DateTime?)null;
                            TimeSpan? hora_encerramento = reader["HORA_PROD"] != DBNull.Value ? (TimeSpan?)reader["HORA_PROD"] : (TimeSpan?)null;
                            DateTime? datahora_encerramento = data_encerramento.HasValue && hora_encerramento.HasValue ? data_encerramento.Value.Add(hora_encerramento.Value) : (DateTime?)null;

                            var _statusTemp = GetTempoStatus(obj.CodCt, obj.Projeto, obj.Numero_Os, obj.Numero_Doc, datahora_encerramento);
                            var _paralisacaotemp = _statusTemp.tempoParalisacao;
                            var execucaotemp = _statusTemp.tempoAtendimento;
                            obj.Tempo_Execucao = execucaotemp.ToString(@"hh\:mm");
                            obj.Tempo_Paralizacoes = _paralisacaotemp.ToString(@"hh\:mm");
                            obj.Duracao_Total_Visita = (execucaotemp + _paralisacaotemp).ToString(@"hh\:mm");

                            retorno.Add(obj);

                        }
                    }

                }
            }


            listaOrdenada = retorno.OrderBy(p => p.Data_Agendamento);
            recordsTotal = listaOrdenada.Count();

            //Page e retornando DataTable
            return new DataTableResponse<Acompanhamento_EquipeListaItemModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [AcceptVerbs("GET")]
        [Route("api/EquipeApi/Afastado")]
        public async Task<Boolean> TaNoMobile(String codct, String projeto, Int32 num_doc, String num_obra, DateTime data)
        {
            Boolean retorno = false;
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(@"SELECT FIRST 1 A.ROTERIZAR 
                                           FROM TB_DOCUMENTO_PRODUCAO A 
                                          WHERE A.DATA_MAXIMA = @DATA_MAXIMA 
                                                AND A.CODCT = @CODCT
                                                AND A.PROJETO = @PROJETO
                                                AND A.NUM_DOC = @NUM_DOC
                                                AND A.ROTERIZAR = @ROTERIZAR ");

                        if (!String.IsNullOrEmpty(num_obra))
                        {
                            sql.AppendLine(" AND A.NUM_OBRA = @NUM_OBRA ");
                            parametros.Add("@NUM_OBRA", num_obra);
                        }

                        parametros.Add("@DATA_MAXIMA", data);
                        parametros.Add("@CODCT", codct);
                        parametros.Add("@PROJETO", projeto);
                        parametros.Add("@NUM_DOC", num_doc);
                        parametros.Add("@ROTERIZAR", "S");


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                retorno = reader["ROTERIZAR"] != DBNull.Value ? Convert.ToString(reader["ROTERIZAR"]) == "S" : false;
                            }
                        }
                    }
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Double CalculaSla(DateTime? DataHora_Indicador, DateTime? DataHora_Criacao, DateTime? DataHoraFechamento, DateTime? DataComparar = null)
        {
            var totais = (DataHora_Indicador - DataHora_Criacao).Value.TotalMinutes;
            DateTime data_comparar;

            var agora = new Double();
            if (DataHoraFechamento.HasValue)
            {
                data_comparar = DataHoraFechamento.Value;
                agora = (DataHora_Indicador - data_comparar).Value.TotalMinutes;
            }
            else
            {
                data_comparar = DataComparar ?? DateTime.Now;
                agora = (DataHora_Indicador - data_comparar).Value.TotalMinutes;
            }

            var porcentagem = ((100 - (agora / totais) * 100));
            var valor_calculado = Math.Round(porcentagem, 2);

            return valor_calculado;
        }
        public Double CalculaHoraSla(DateTime? DataHora_Indicador, DateTime? DataHora_Criacao, DateTime? DataHoraFechamento, DateTime? DataComparar = null)
        {
            var totais = (DataHora_Indicador - DataHora_Criacao).Value.TotalMinutes;
            DateTime data_comparar;

            var agora = new Double();
            if (DataHoraFechamento.HasValue)
            {
                data_comparar = DataHoraFechamento.Value;
                agora = (DataHora_Indicador - data_comparar).Value.TotalMinutes;
            }
            else
            {
                data_comparar = DataComparar ?? DateTime.Now;
                agora = (DataHora_Indicador - data_comparar).Value.TotalMinutes;
            }

            var porcentagem = ((100 - (agora / totais) * 100));
            var valor_calculado = Math.Round(porcentagem, 2);

            return agora;
        }


        public String GetDescricaoSla(DateTime? DataHora_Indicador, DateTime? DataHora_Criacao, DateTime? DataHoraFechamento, DateTime? DataComparar = null)
        {
            try
            {
                Double Porcentagem = CalculaSla(DataHora_Indicador, DataHora_Criacao, DataHoraFechamento, DataComparar);

                if (DataHoraFechamento.HasValue == false)
                {
                    if ((Porcentagem >= 0 && Porcentagem <= 25) || Porcentagem < 0)
                    {
                        return RsCadastros.DENTRO_PRAZO;
                    }
                    else if (Porcentagem > 25 && Porcentagem <= 50)
                    {
                        return RsCadastros.ATENCAO;
                    }
                    else if (Porcentagem > 50 && Porcentagem <= 75)
                    {
                        return RsCadastros.VENCIMENTO_PROXIMO;
                    }
                    else if (Porcentagem > 75 && Porcentagem <= 100)
                    {
                        return RsCadastros.RISCO_PERDA_PRAZO;
                    }
                    else if (Porcentagem > 100)
                    {
                        return RsCadastros.PRAZO_VENCIDO;
                    }
                    else
                    {
                        return RsCadastros.RISCO_PERDA_PRAZO;
                    }
                }
                else
                {
                    if (Porcentagem > 100)
                    {
                        return RsCadastros.PRAZO_VENCIDO;
                    }
                    else
                    {
                        return RsCadastros.PRAZO_CUMPRIDO;
                    }
                }
            }
            catch
            {
                return "Sem Indicador de Sla";
            }
        }
        public String GetDescricaoHorasSla(DateTime? DataHora_Indicador, DateTime? DataHora_Criacao, DateTime? DataHoraFechamento, DateTime? DataComparar = null)
        {
            try
            {
                DateTime strt_date = DateTime.Now;
                DateTime end_date = Convert.ToDateTime(DataHora_Indicador);
                if (DataHoraFechamento.HasValue == false)
                {
                    TimeSpan tempoVencimento = (end_date - strt_date);
                    if (tempoVencimento.TotalHours >= 4)
                    {
                        return RsCadastros.DENTRO_PRAZO_HORAS;
                    }
                    else if (tempoVencimento.TotalHours > 2 && tempoVencimento.TotalHours < 4)
                    {
                        return RsCadastros.ATENCAO_HORAS;
                    }
                    else if (tempoVencimento.TotalHours > 1 && tempoVencimento.TotalHours < 2)
                    {
                        return RsCadastros.VENCIMENTO_PROXIMO_HORAS;
                    }
                    else if (tempoVencimento.TotalHours >= 0 && tempoVencimento.Hours < 1)
                    {
                        return RsCadastros.RISCO_PERDA_PRAZO_HORAS;
                    }
                    else if (tempoVencimento.TotalHours < 0)
                    {
                        return RsCadastros.SLA_VENCIDO;
                    }
                    else
                    {
                        return RsCadastros.RISCO_PERDA_PRAZO_HORAS;
                    }
                }
                else
                {
                    strt_date = DataHoraFechamento.Value;
                    TimeSpan tempoVencimento = (end_date - strt_date);

                    if (tempoVencimento.TotalHours < 0)
                    {
                        return RsCadastros.FECHADO_FORA_PRAZO;
                    }
                    else
                    {
                        return RsCadastros.FECHADO_PRAZO_HORAS;
                    }
                }
            }
            catch
            {
                return "Sem Indicador de Sla";
            }
        }

        public (TimeSpan tempoDeslocamento, TimeSpan tempoParalisacao, TimeSpan tempoAtendimento) GetTempoStatus(String CodCt, String Projeto, String Num_Os, Int32 Num_Doc, DateTime? DataHoraProd)
        {
            TimeSpan tempoDeslocamento = new TimeSpan();
            TimeSpan tempoParalisacao = new TimeSpan();
            TimeSpan tempoAtendimento = new TimeSpan();
            using (ISession session = HibernateUtil.GetSessionT5())
            using (ConexaoMongo conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo))
            {
                var _collection = repository.GetCollection();
                var mongoDocumentos = _collection.Aggregate().Match(p => p.codCt == CodCt && p.numObra == Num_Os && p.contrato == Num_Doc && p.projeto == Projeto).ToList();
                var status = mongoDocumentos.FirstOrDefault()?.status.OrderBy(p => p.hora).ToList();
                if (status == null)
                    return (tempoDeslocamento, tempoParalisacao, tempoAtendimento);

                for (int i = 0; i < status.Count; i++)
                {
                    if (status[i].statusExecucao == Convert.ToInt32(StatusExecucaoAniel.OS_Em_deslocamento))
                    {
                        if (i + 1 < status.Count)
                        {
                            tempoDeslocamento = tempoDeslocamento.Add(status[i + 1].hora - status[i].hora);
                        }
                        else
                        {
                            tempoDeslocamento = tempoDeslocamento.Add((DataHoraProd ?? DateTime.Now) - status[i].hora);
                        }

                    }
                }


                for (int i = 0; i < status.Count; i++)
                {
                    if (status[i].statusExecucao == Convert.ToInt32(StatusExecucaoAniel.Atendimento_Paralizado))
                    {
                        if (i + 1 < status.Count)
                        {
                            tempoParalisacao = tempoParalisacao.Add(status[i + 1].hora - status[i].hora);
                        }
                        else
                        {
                            tempoParalisacao = tempoParalisacao.Add((DataHoraProd ?? DateTime.Now) - status[i].hora);
                        }

                    }
                }


                for (int i = 0; i < status.Count; i++)
                {
                    if (status[i].statusExecucao == Convert.ToInt32(StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado) || status[i].statusExecucao == Convert.ToInt32(StatusExecucaoAniel.Reiniciado_Atendimento))
                    {
                        if (i + 1 < status.Count)
                        {
                            tempoAtendimento = tempoAtendimento.Add(status[i + 1].hora - status[i].hora);
                        }
                        else
                        {
                            tempoAtendimento = tempoAtendimento.Add((DataHoraProd ?? DateTime.Now) - status[i].hora);
                        }

                    }
                }

            }

            return (tempoDeslocamento, tempoParalisacao, tempoAtendimento);
        }

        public TimeSpan? GetTempoParalizacao(String CodCt, String Projeto, String Num_Os, Int32 Num_Doc, DateTime? DataHoraProd)
        {
            using (ISession session = HibernateUtil.GetSessionT5())
            using (ConexaoMongo conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo))
            {
                TimeSpan retorno = new TimeSpan();
                var _collection = repository.GetCollection();
                var mongoDocumentos = _collection.Aggregate().Match(p => p.codCt == CodCt && p.numObra == Num_Os && p.contrato == Num_Doc && p.projeto == Projeto).ToList();
                var status = mongoDocumentos.FirstOrDefault()?.status.OrderBy(p => p.hora).ToList();

                if (status == null)
                    return retorno;

                for (int i = 0; i < status.Count; i++)
                {
                    if (status[i].statusExecucao == Convert.ToInt32(StatusExecucaoAniel.Atendimento_Paralizado))
                    {
                        if (i + 1 < status.Count)
                        {
                            retorno = retorno.Add(status[i + 1].hora - status[i].hora);
                        }
                        else
                        {
                            retorno = retorno.Add((DataHoraProd ?? DateTime.Now) - status[i].hora);
                        }

                    }
                }

                return retorno;
            }
        }

        public TimeSpan GetTempoAtendimento(String CodCt, String Projeto, String Num_Os, Int32 Num_Doc, DateTime? DataHoraProd)
        {
            using (ISession session = HibernateUtil.GetSessionT5())
            using (ConexaoMongo conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo))
            {
                TimeSpan retorno = new TimeSpan();
                var _collection = repository.GetCollection();
                var mongoDocumentos = _collection.Aggregate().Match(p => p.codCt == CodCt && p.numObra == Num_Os && p.contrato == Num_Doc && p.projeto == Projeto).ToList();
                var status = mongoDocumentos.FirstOrDefault()?.status.OrderBy(p => p.hora).ToList();
                if (status == null)
                    return retorno;

                for (int i = 0; i < status.Count; i++)
                {
                    if (status[i].statusExecucao == Convert.ToInt32(StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado) || status[i].statusExecucao == Convert.ToInt32(StatusExecucaoAniel.Reiniciado_Atendimento))
                    {
                        if (i + 1 < status.Count)
                        {
                            retorno = retorno.Add(status[i + 1].hora - status[i].hora);
                        }
                        else
                        {
                            retorno = retorno.Add((DataHoraProd ?? DateTime.Now) - status[i].hora);
                        }

                    }
                }

                return retorno;
            }
        }

        public async Task<TimeSpan?> GetTempoParalizacaIndividual(String CodCt, String Projeto, String Num_Os, Int32 Num_Doc)
        {
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT SUM(COALESCE((SELECT FIRST 1 D.HORA");
                    sql.AppendLine("FROM TB_LOG_LOCALIZACAO D");
                    sql.AppendLine("WHERE C.CODCT = D.CODCT AND C.PROJETO = D.PROJETO AND C.NUM_OBRA = D.NUM_OBRA AND C.CONTRATO = D.CONTRATO AND D.STATUS <> 10");
                    sql.AppendLine("AND D.ID > C.ID");
                    sql.AppendLine("ORDER BY D.HORA),CURRENT_TIME) -C.HORA) AS TEMPO");

                    sql.AppendLine("FROM TB_LOG_LOCALIZACAO C");
                    sql.AppendLine("WHERE C.CODCT = @CODCT AND C.PROJETO = @PROJETO AND C.NUM_OBRA = @NUM_OBRA AND C.CONTRATO = @CONTRATO AND C.STATUS = 10");

                    parametros.Add("@CODCT", CodCt);
                    parametros.Add("@PROJETO", Projeto);
                    parametros.Add("@NUM_OBRA", Num_Os);
                    parametros.Add("@CONTRATO", Num_Doc);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            if (reader["TEMPO"] == DBNull.Value)
                                return null;
                            else
                            {
                                Int32 ms = Convert.ToInt32(reader["TEMPO"]);
                                TimeSpan ts = TimeSpan.FromSeconds(ms);
                                return ts;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }

                }
            }
        }



        public TimeSpan GetTempoDeslocamento(String CodCt, String Projeto, String Num_Os, Int32 Num_Doc, DateTime? DataHoraProd)
        {
            using (ISession session = HibernateUtil.GetSessionT5())
            using (ConexaoMongo conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo))
            {
                TimeSpan retorno = new TimeSpan();
                var _collection = repository.GetCollection();
                var mongoDocumentos = _collection.Aggregate().Match(p => p.codCt == CodCt && p.numObra == Num_Os && p.contrato == Num_Doc && p.projeto == Projeto).ToList();
                var status = mongoDocumentos.FirstOrDefault()?.status.OrderBy(p => p.hora).ToList();
                if (status == null)
                    return retorno;

                for (int i = 0; i < status.Count; i++)
                {
                    if (status[i].statusExecucao == Convert.ToInt32(StatusExecucaoAniel.OS_Em_deslocamento))
                    {
                        if (i + 1 < status.Count)
                        {
                            retorno = retorno.Add(status[i + 1].hora - status[i].hora);
                        }
                        else
                        {
                            retorno = retorno.Add((DataHoraProd ?? DateTime.Now) - status[i].hora);
                        }

                    }
                }

                return retorno;
            }
        }

        [HttpPost()]
        [Route("api/Documento_ProducaoApi/GetListClientesOS")]
        public async Task<DataTableResponse<Documento_ProducaoListaModel>> GetListClientesOS(Documento_Producao_DataTableRequest request)
        {
            List<Documento_ProducaoListaModel> retorno = new List<Documento_ProducaoListaModel>();
            IOrderedEnumerable<Documento_ProducaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_CLIENTE A WHERE A.STATUS = 1");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();


                    sql.AppendLine(" SELECT A.ID, A.NOMEFANTASIA, A.ID_ENDERECO_PRINCIPAL, A.CPF, A.STATUS, ");
                    sql.AppendLine("        B.ENDERECO_LOGRADOURO, B.ENDERECO_BAIRRO, B.ENDERECO_CIDADE, B.ENDERECO_COMPLEMENTO, ");
                    sql.AppendLine("        B.ENDERECO_NUMERO, B.ENDERECO_UF, B.ENDERECO_PONTO_REFERENCIA, B.ENDERECO_CEP, ");
                    sql.AppendLine("        C.DESCRICAO AS TIPO_IMOVEL, A.TELEFONE_FIXO, A.TELEFONE_RECADO, A.TELEFONE_CELULAR, ");
                    sql.AppendLine("        A.EMAIL_PRINCIPAL, A.NOME_RESP_FINANCEIRO, A.DATA_VENDA, D.DESCRICAO AS CATEGORIA ");
                    sql.AppendLine("   FROM TB_CLIENTE A ");
                    sql.AppendLine("   LEFT JOIN TB_ENDERECO_CLIENTE B ");
                    sql.AppendLine("     ON (A.ID_ENDERECO_PRINCIPAL = B.ID) ");
                    sql.AppendLine("   LEFT JOIN TB_TIPO_IMOVEL C ");
                    sql.AppendLine("     ON (B.ID_TIPO_IMOVEL = C.COD_TIPO_IMOVEL) ");
                    sql.AppendLine("   LEFT JOIN TB_CATEGORIA_CLIENTE D ");
                    sql.AppendLine("     ON (A.ID_CATEGORIA = D.ID) ");
                    sql.AppendLine("   WHERE A.STATUS = 1");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" AND ((UPPER(A.CPF) LIKE UPPER(@SEARCH)) OR (UPPER(A.NOMEFANTASIA) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (request.Cpfcnpj_Cliente != null)
                    {
                        sql.AppendLine(" AND (A.CPF = @VALOR) ");
                        parametros.Add("@VALOR", request.Cpfcnpj_Cliente);
                    }

                    if (!String.IsNullOrEmpty(request.Plano_Produto))
                    {
                        {
                            sql.AppendLine("AND(0 < (SELECT COUNT(E.ID_CLIENTE) FROM TB_CLIENTE_PLANO_PRODUTO E WHERE E.ID_CLIENTE = A.ID AND (E.ID_PLANO = @ID_PLANO_PRODUTO OR E.id_produto = @ID_PLANO_PRODUTO )))");
                            parametros.Add("@ID_PLANO_PRODUTO", request.Plano_Produto);
                        }
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            String endereco = String.Empty;
                            if (reader["ENDERECO_LOGRADOURO"] != DBNull.Value)
                                endereco = $@"{reader["ENDERECO_LOGRADOURO"]} {reader["ENDERECO_NUMERO"]}, {reader["ENDERECO_BAIRRO"]} - {reader["ENDERECO_CIDADE"]}";


                            var obj = new Documento_ProducaoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Cpfcnpj_Cliente = Convert.ToString(reader["CPF"]),
                                Cliente = Convert.ToString(reader["NOMEFANTASIA"]),
                                Tempo_Cadastro = Convert.ToString(reader["DATA_VENDA"]),
                                Categoria_Cliente = Convert.ToString(reader["CATEGORIA"]),
                                Telefone_Recado = Convert.ToString(reader["TELEFONE_RECADO"]),
                                Telefone_Fixo = Convert.ToString(reader["TELEFONE_FIXO"]),
                                Telefone_Celular = Convert.ToString(reader["TELEFONE_CELULAR"]),
                                Email = Convert.ToString(reader["EMAIL_PRINCIPAL"]),
                                Tipo_Imovel = Convert.ToString(reader["TIPO_IMOVEL"]),
                                Cep = Convert.ToString(reader["ENDERECO_CEP"]),
                                Endereco_Cliente = endereco,
                                Uf = Convert.ToString(reader["ENDERECO_UF"]),
                                Complemento = Convert.ToString(reader["ENDERECO_COMPLEMENTO"]),
                                Ponto_Referencia_Cliente = Convert.ToString(reader["ENDERECO_PONTO_REFERENCIA"]),
                                Id_Endereco_Cliente = Convert.ToString(reader["ID_ENDERECO_PRINCIPAL"]),
                                PlanosProdutos = new PlanoProdutoApiController().getByCliente(Convert.ToInt64(reader["ID"])).Result,
                            };
                        }
                    }
                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Cliente);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Cliente);
                            break;
                        }
                }
            }

            recordsTotal = listaOrdenada.Count();

            //Page e retornando DataTable
            return new DataTableResponse<Documento_ProducaoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        //[HttpGet()]
        //[Route("api/Documento_ProducaoApi/CriarOS")]
        //public async Task<PostApiRetorno<Int64>> CriarOS(Int64? Id_Cliente, String Id_Usuario)
        //{
        //    PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

        //    try
        //    {
        //        Documento_ProducaoAniel modelo = new Documento_ProducaoAniel();

        //        using (ISession session = HibernateUtil.GetSessionT5())
        //        {
        //            using (var repository = new Conexao<Documento_ProducaoAniel>(session))
        //            {

        //                modelo.Num_Obra =(await repository.Max(p => p.Num_Obra)) + 1;
        //                modelo.Status = StatusExecucaoAniel.Aberta_Aguard_Atendimento;
        //                modelo.Cliente = Id_Cliente.HasValue ? new BuscaGenericaRepository<Cliente>(session, null).FindById(Id_Cliente.Value).Result : null;
        //                modelo.UsuarioCriacao = Id_Usuario;
        //                modelo.Responsavel = await new Conexao<IdentityUser>(session).selectFirst(p => p.Id == Id_Usuario);

        //                await repository.SaveOrUpdate(modelo);
        //            }
        //            retorno.IdObjeto = modelo.Id;
        //            retorno.Ok = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retorno.IdObjeto = 0;
        //        retorno.Mensagem = ex.Message;
        //        if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
        //    }

        //    return retorno;
        //}

        [HttpGet()]
        [Route("api/Documento_ProducaoApi/buscaCpfOs")]
        public async Task<List<ObjBusca<String>>> buscaCpf(String term)
        {
            List<ObjBusca<String>> retorno = new List<ObjBusca<String>>();

            retorno = new List<ObjBusca<String>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID,A.CPF ");
                        sql.AppendLine("FROM TB_CLIENTE A ");
                        sql.AppendLine("WHERE (UPPER(A.CPF) = @VALOR)");
                        parametros.Add("@VALOR", $"{term.ToUpperInvariant()}");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<String>
                                {
                                    Value = Convert.ToString(reader["ID"]),
                                    Label = Convert.ToString(reader["CPF"])
                                });
                            }
                        }


                    }
                }
                catch (Exception ex)
                {
                    String Mensagem = ex.Message;
                }
            }


            return retorno;
        }

        public async Task<ClienteAniel> GetClienteAniel(ISession sessionT5, Cliente cliente)
        {
            using (var repoClienteAniel = new Conexao<ClienteAniel>(sessionT5))
            {
                var clienteAniel = await repoClienteAniel.selectFirst(p => (p.CPF != null && p.CPF != String.Empty && p.CPF == cliente.CpfCnpj) || (p.Cod_Cliente == cliente.Cod_Cliente));

                if (clienteAniel == null)
                {
                    var tipo_cliente = cliente.Tipo_Pessoa != null ? cliente.Tipo_Pessoa : null;
                    clienteAniel = new ClienteAniel
                    {
                        Cod_Cliente = Convert.ToInt32(await repoClienteAniel.Max(p => p.Cod_Cliente) + 1),

                        CPF = cliente.CpfCnpj,
                        NomeFantasia = cliente.NomeFantasia,
                        Status = cliente.Status.Value,
                        Tipo_Pessoa = tipo_cliente,
                        Email_Principal = cliente.Email_Principal,
                        Telefone_Celular = cliente.Telefone_Celular,
                        Telefone_Fixo = cliente.Telefone_Fixo,
                        Data_Cadastro = cliente.DataHoraCriacao,
                        Categoria_Cliente = await GetCategoriaClienteAniel(sessionT5, cliente.Categoria_Cliente)
                    };

                    await repoClienteAniel.Save(clienteAniel);
                }

                return clienteAniel;
            }
        }

        private async Task<Categoria_ClienteAniel> GetCategoriaClienteAniel(ISession sessionT5, Categoria_Cliente categoria)
        {
            if (categoria == null)
                return null;

            var categoriaAniel = (await sessionT5.QueryOver<Categoria_ClienteAniel>().Where(p => p.Id == categoria.Id).ListAsync()).FirstOrDefault();

            if (categoriaAniel == null)
            {
                categoriaAniel = new Categoria_ClienteAniel
                {
                    Id = categoria.Id,
                    Descricao = categoria.Descricao,
                    Status = categoria.Status.Value
                };
                await sessionT5.SaveAsync(categoriaAniel);
            }

            return categoriaAniel;
        }

        public async Task<List<PlanoProdutoAniel>> GetPlanoProdutoAniel(ISession sessionT5, Cliente_PlanoProduto cliente_PlanoProduto)
        {
            List<PlanoProdutoAniel> retorno = new List<PlanoProdutoAniel>();
            using (var repoPlanoProdutoAniel = new Conexao<PlanoProdutoAniel>(sessionT5))
            //using (var repoCobrancaServico = new Conexao<Cobranca_Servico>(session))
            {
                if (cliente_PlanoProduto.Combo != null)
                {
                    foreach (var item in cliente_PlanoProduto.Combo.PlanosProdutos)
                    {
                        Int64? id_planoproduto = item.Plano != null ? item?.Plano?.Id : item?.Produto?.Id;
                        if (id_planoproduto.HasValue == false)
                            return null;

                        var planoprodutoAniel = await repoPlanoProdutoAniel.selectFirst(p => p.Id == id_planoproduto) ?? new PlanoProdutoAniel();

                        if (item.Plano != null)
                        {
                            planoprodutoAniel.Id = item.Plano.Id;
                            planoprodutoAniel.Codigo = item.Plano.Codigo;
                            planoprodutoAniel.Descricao = item.Plano.Descricao;
                            planoprodutoAniel.Tipo = Tipo_PlanoProduto.Plano;
                        }
                        else
                        {
                            planoprodutoAniel.Id = item.Produto.Id;
                            planoprodutoAniel.Codigo = item.Produto.Codigo;
                            planoprodutoAniel.Descricao = item.Produto.Descricao;
                            planoprodutoAniel.Tipo = Tipo_PlanoProduto.Produto;
                        }
                        ;

                        await repoPlanoProdutoAniel.SaveOrUpdate(planoprodutoAniel);
                        retorno.Add(planoprodutoAniel);
                    }
                }
                else
                {
                    Int64? id_planoproduto = cliente_PlanoProduto.Plano != null ? cliente_PlanoProduto?.Plano?.Id : cliente_PlanoProduto?.Produto?.Id;
                    if (id_planoproduto.HasValue == false)
                        return new List<PlanoProdutoAniel>();

                    var planoprodutoAniel = await repoPlanoProdutoAniel.selectFirst(p => p.Id == id_planoproduto) ?? new PlanoProdutoAniel();

                    if (cliente_PlanoProduto.Plano != null)
                    {
                        planoprodutoAniel.Id = cliente_PlanoProduto.Plano.Id;
                        planoprodutoAniel.Codigo = cliente_PlanoProduto.Plano.Codigo;
                        planoprodutoAniel.Descricao = cliente_PlanoProduto.Plano.Descricao;
                        planoprodutoAniel.Tipo = Tipo_PlanoProduto.Plano;
                    }

                    else
                    {
                        planoprodutoAniel.Id = cliente_PlanoProduto.Produto.Id;
                        planoprodutoAniel.Codigo = cliente_PlanoProduto.Produto.Codigo;
                        planoprodutoAniel.Descricao = cliente_PlanoProduto.Produto.Descricao;
                        planoprodutoAniel.Tipo = Tipo_PlanoProduto.Produto;
                    }
                    ;

                    await repoPlanoProdutoAniel.SaveOrUpdate(planoprodutoAniel);
                    retorno.Add(planoprodutoAniel);
                }
            }

            return retorno;
        }

        private async Task BindViewToObj(ISession sessionConnect, ISession sessionT5, Conexao<Cliente> repoCliente, Conexao<Documento_ProducaoAniel> repository, Documento_ProducaoAniel objBanco, Documento_ProducaoCadastroModel view, LocalidadeAniel localidade, Tipo_Servico_EquipeAniel tipoServico)
        {
            objBanco.Cod_Escritorio = localidade?.Cod_Escritorio;
            objBanco.Cod_Area = localidade?.Cod_Area;
            objBanco.Cod_Localidade = localidade?.Cod_Localidade;
            objBanco.Origem_Documento = Origem_Documento_Producao.Connect;
            IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
            var documentoBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);

            if (objBanco.Tipo == "C")
            {
                Cliente cliente = (await repoCliente.queryover().Where(p => p.Id == view.Id_Cliente).ListAsync()).FirstOrDefault();
                ClienteAniel clienteAniel = cliente != null ? await GetClienteAniel(sessionT5, cliente) : null;

                if (view.CadastrarNovoCliente
                    && cliente == null
                    && clienteAniel == null)
                {
                    using (var repositoryCliente = new Conexao<Cliente>(sessionConnect))
                    using (var repositoryClienteAniel = new Conexao<ClienteAniel>(sessionT5))
                    using (var repositoryCategoria = new Conexao<Categoria_Cliente>(sessionConnect))
                    using (var repositoryCategoriaAniel = new Conexao<Categoria_ClienteAniel>(sessionT5))
                    {
                        var categoria = repositoryCategoria.selectFirst(y => y.Id == view.Id_Categoria_Cliente).Result;
                        var clienteConsulta = await repoCliente.selectFirst(p => p.CpfCnpj == view.CPF_CNPJ_Cliente);
                        view.CPF_CNPJ_Cliente = view.CPF_CNPJ_Cliente.Replace(".", String.Empty).Replace("-", String.Empty);

                        if (clienteConsulta == null)
                        {
                            clienteConsulta = await repoCliente.selectFirst(p => p.CpfCnpj == view.CPF_CNPJ_Cliente);
                        }

                        if (clienteConsulta != null)
                        {
                            throw new Exception("Cpf/Cnpj já Cadastrado.");
                        }

                        Tipo_Pessoa tipoPessoa = new Tipo_Pessoa();
                        String tipo = null;
                        switch (view.TipoPessoa)
                        {
                            case Tipo_Pessoa.Fisica:
                                tipoPessoa = Tipo_Pessoa.Fisica;
                                tipo = "F";
                                break;

                            case Tipo_Pessoa.Juridica:
                                tipoPessoa = Tipo_Pessoa.Juridica;
                                tipo = "J";
                                break;
                        }

                        cliente = new Cliente
                        {
                            Email_Principal = view.Email_Principal,
                            CpfCnpj = view.CPF_CNPJ_Cliente,
                            Telefone_Fixo = view.Telefone_Fixo,
                            Telefone_Celular = view.Telefone_Celular,
                            Categoria_Cliente = categoria,
                            NomeFantasia = view.Nome_Cliente,
                            Status = Status_Cliente.Ativo,
                            Tipo_Pessoa = tipoPessoa,
                            Tipo_Cliente = Tipo_Cliente.Cliente,
                        };

                        await repoCliente.Save(cliente);
                        cliente.Numero_Contrato = Convert.ToString(cliente.Id);
                        await repoCliente.Update(cliente);

                        var idJaExiste = await repositoryClienteAniel.selectFirst(c => c.Cod_Cliente == cliente.Id);
                        clienteAniel = new ClienteAniel
                        {
                            NomeFantasia = view.Nome_Cliente,
                            CPF = view.CPF_CNPJ_Cliente,
                            Email_Principal = view.Email_Principal,
                            Telefone_Fixo = view.Telefone_Fixo,
                            Telefone_Celular = view.Telefone_Celular,
                            Status = Status_Cliente.Ativo,
                            Tipo_Pessoa = tipoPessoa,
                            Data_Cadastro = DateTime.Now,
                            Categoria_Cliente = categoria != null ? repositoryCategoriaAniel.selectFirst(p => p.Descricao == categoria.Descricao).Result : null,
                            Tipo = tipo

                        };

                        if (idJaExiste == null)
                        {
                            clienteAniel.Cod_Cliente = Convert.ToInt32(cliente.Id);
                        }
                        else
                        {
                            clienteAniel.Cod_Cliente = Convert.ToInt32(await repositoryClienteAniel.Max(c => c.Cod_Cliente) + 1);
                        }

                        await repositoryClienteAniel.Save(clienteAniel);
                    }
                }

                Cliente_PlanoProduto planoProduto = null;
                PlanoProdutoAniel clientePlanoProduto = null;

                if (view.Id_Cliente_Plano_Produto != null)
                {
                    planoProduto = cliente.PlanosProdutos.FirstOrDefault(p => p.Id == view.Id_Cliente_Plano_Produto);
                    clientePlanoProduto = planoProduto != null ? (await GetPlanoProdutoAniel(sessionT5, planoProduto)).FirstOrDefault() : null;
                }

                Int64 _seq = documentoBusiness.GerarSeq(localidade.CodCt,
                                                        localidade.Projeto,
                                                        objBanco.Numero_Obra_Original,
                                                        planoProduto != null ?
                                                        Convert.ToInt32(planoProduto.ClienteContrato.NumeroContrato) :
                                                        (!string.IsNullOrEmpty(cliente.Numero_Contrato) ? Convert.ToInt32(cliente.Numero_Contrato) :
                                                        Convert.ToInt32(cliente.Id)));


                // Int64 _seq = documentoBusiness.GerarSeq(view.Contrato ?? localidade?.CodCt, view.Projeto ?? localidade?.Projeto, objBanco.Numero_Obra_Original, Convert.ToInt32(cliente.Numero_Contrato));
                //Cliente_PlanoProduto planoProduto = cliente.PlanosProdutos.FirstOrDefault(p => p.Id == view.Id_Cliente_Plano_Produto);
                //PlanoProdutoAniel clientePlanoProduto = planoProduto != null ? (await GetPlanoProdutoAniel(sessionT5, planoProduto)).FirstOrDefault() : null;

                objBanco.CodCt = view.Contrato ?? localidade.CodCt;
                objBanco.Projeto = view.Projeto ?? localidade.Projeto;
                //objBanco.Num_Doc = planoProduto?.ClienteContrato?.NumeroContrato != null ? Convert.ToInt32(planoProduto.ClienteContrato.NumeroContrato) : Convert.ToInt32(cliente.Numero_Contrato);
                //objBanco.OXBDPEE = cliente.Numero_Contrato;
                objBanco.Num_Doc = planoProduto?.ClienteContrato?.NumeroContrato != null ?
                                   Convert.ToInt32(planoProduto.ClienteContrato.NumeroContrato) :
                                   (!string.IsNullOrEmpty(cliente.Numero_Contrato) ? Convert.ToInt32(cliente.Numero_Contrato) :
                                   Convert.ToInt32(cliente.Id));

                objBanco.OXBDPEE = !string.IsNullOrEmpty(cliente.Numero_Contrato) ? cliente.Numero_Contrato : Convert.ToString(cliente.Id);
                objBanco.Tarefa = String.IsNullOrEmpty(cliente.Telefone_Fixo) ? "1" : cliente.Telefone_Fixo;
                objBanco.Num_Obra = String.Format("{0}/{1}", objBanco.Numero_Obra_Original, _seq);
                objBanco.Seq = Convert.ToInt32(_seq);

                objBanco.TelefonePrincipal = cliente.Telefone_Fixo;
                objBanco.CelularCliente = cliente.Telefone_Celular;
                objBanco.CPF_CLIENTE = cliente.CpfCnpj;
                objBanco.Titular = cliente.NomeFantasia;
                objBanco.RG_Assinante = cliente.Rg;
                if (cliente.Email_Principal != null)
                {
                    string email = cliente.Email_Principal.Length > 50 ? cliente.Email_Principal?.Substring(0, 50) : cliente.Email_Principal;
                    var filaEmail = new Queue<string>(email.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                    objBanco.Email_Assinante = filaEmail.Count > 0 ? filaEmail.Peek() : null;
                }
                else
                {
                    objBanco.Email_Assinante = null;
                }

                objBanco.LoginRadius = cliente.Login;
                objBanco.SenhaRadius = cliente.Senha;

                objBanco.Cliente = clienteAniel;
                objBanco.Nome_Cliente = StringService.Truncate(clienteAniel.NomeFantasia, 25);

                objBanco.Id_PlanoProduto = view.Id_Plano_Produto;
                objBanco.Id_Cliente_PlanoProdutoConnect = view.Id_Cliente_Plano_Produto;

                var _idTrocaPlanoTmp = view?.Id_Troca_Plano > 0 ? view.Id_Troca_Plano : planoProduto?.Id_TrocaPlano;
                if (_idTrocaPlanoTmp > 0)
                {
                    var objTipoPlanoAnterior = cliente.PlanosProdutos.FirstOrDefault(x => x.Id == _idTrocaPlanoTmp);
                    objBanco.Id_PlanoProduto_Ant = objTipoPlanoAnterior.Plano?.Id;
                    objBanco.DescPlanoAnterior = objTipoPlanoAnterior.Plano?.Descricao;
                    objBanco.Id_Cliente_PlanoConnect_Ant = objTipoPlanoAnterior.Id;
                }

                objBanco.Latitude_Str = Convert.ToString(planoProduto?.Endereco_Instalacao?.Latitude);
                objBanco.Longitude_Str = Convert.ToString(planoProduto?.Endereco_Instalacao?.Longitude);

                if (view.Id_Cliente_Plano_Produto.HasValue)
                {
                    if (String.IsNullOrEmpty(objBanco.LoginRadius) || String.IsNullOrEmpty(objBanco.SenhaRadius))
                    {
                        using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                        {
                            var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == view.Id_Cliente_Plano_Produto);
                            if (loginRadius != null)
                            {
                                objBanco.LoginRadius = loginRadius.Login;
                                objBanco.SenhaRadius = loginRadius.SenhaPPPOE;
                                objBanco.Ssid_Wifi = loginRadius.SSID_WIFI;
                                objBanco.Senha_Wifi = loginRadius.Senha_WIFI;
                            }
                        }
                    }
                    using (var repoAtendimento = new Conexao<Atendimento>(sessionConnect))
                    {
                        var atendimento = view.Id_Atendimento.HasValue ? await repoAtendimento.selectFirst(p => p.Id == view.Id_Atendimento) : null;
                        var enderecoMudanca = planoProduto.Endereco_Mudanca_Cliente ?? atendimento?.Endereco_Mudanca;

                        if (enderecoMudanca != null)
                        {
                            objBanco.Bairro = enderecoMudanca.Bairro;
                            objBanco.Cep = enderecoMudanca.Cep;
                            objBanco.Cidade = StringService.Truncate(enderecoMudanca.Cidade, 20);
                            objBanco.Logradouro = StringService.Truncate(enderecoMudanca.Logradouro, 100);
                            objBanco.Numero = enderecoMudanca.Numero;
                            objBanco.Complemento = StringService.Truncate(enderecoMudanca.Complemento, 100);
                            objBanco.Observacao = StringService.Truncate(enderecoMudanca.Ponto_Referencia, 100);
                            objBanco.Observacao_Aux = view.Observacao_OS;
                            objBanco.UF = enderecoMudanca.UF;
                            objBanco.Pais = enderecoMudanca.Pais?.Nome;
                            objBanco.Cod_Tipo_Imovel = Convert.ToInt32(view.Id_Tipo_Imovel);

                            objBanco.Id_Endereco_Cliente_Antigo = planoProduto?.Endereco_Instalacao?.Id;
                            objBanco.Endereco_Bairro_Antigo = planoProduto?.Endereco_Instalacao?.Bairro;
                            objBanco.Endereco_Cep_Antigo = planoProduto?.Endereco_Instalacao?.Cep;
                            objBanco.Endereco_Cidade_Antigo = StringService.Truncate(planoProduto?.Endereco_Instalacao?.Cidade, 20);
                            objBanco.Endereco_Antigo = StringService.Truncate(planoProduto?.Endereco_Instalacao?.Logradouro, 100);
                            objBanco.Endereco_Numero_Antigo = planoProduto?.Endereco_Instalacao?.Numero;
                            objBanco.Endereco_Complemento_Antigo = StringService.Truncate(planoProduto?.Endereco_Instalacao?.Complemento, 100);
                            objBanco.Endereco_UF_Antigo = planoProduto?.Endereco_Instalacao?.UF;

                        }
                        else
                        {
                            objBanco.Bairro = planoProduto?.Endereco_Instalacao?.Bairro;
                            objBanco.Cep = planoProduto?.Endereco_Instalacao?.Cep;
                            objBanco.Cidade = StringService.Truncate(planoProduto?.Endereco_Instalacao?.Cidade, 20);
                            objBanco.Logradouro = StringService.Truncate(planoProduto?.Endereco_Instalacao?.Logradouro, 100);
                            objBanco.Numero = planoProduto?.Endereco_Instalacao?.Numero;
                            objBanco.Complemento = StringService.Truncate(planoProduto?.Endereco_Instalacao?.Complemento, 100);
                            objBanco.Observacao = StringService.Truncate(planoProduto?.Endereco_Instalacao?.Ponto_Referencia, 100);
                            objBanco.Observacao_Aux = view.Observacao_OS;
                            objBanco.UF = planoProduto?.Endereco_Instalacao?.UF;
                            objBanco.Pais = planoProduto?.Endereco_Instalacao?.Pais?.Nome;
                            objBanco.Cod_Tipo_Imovel = Convert.ToInt32(view.Id_Tipo_Imovel);
                        }
                    }
                }
                else
                {
                    objBanco.Bairro = view.Bairro;
                    objBanco.Cep = view.CEP;
                    objBanco.Cidade = StringService.Truncate(view.Cidade, 20);
                    objBanco.Logradouro = StringService.Truncate(view.Logradouro, 100);
                    objBanco.Numero = view.Numero;
                    objBanco.Complemento = StringService.Truncate(view.Complemento, 100);
                    objBanco.UF = view.Uf;
                    objBanco.Bloco = view.Bloco;
                    objBanco.Apartamento = view.Apartamento;
                    objBanco.Pais = view.Pais;
                    objBanco.Cod_Tipo_Imovel = Convert.ToInt32(view.Id_Tipo_Imovel_Manut);
                    objBanco.Observacao = StringService.Truncate(view.Ponto_Referencia, 100);
                    objBanco.Latitude_Str = StringService.Truncate(view.Latitude, 100);
                    objBanco.Longitude_Str = StringService.Truncate(view.Longitude, 100);
                }
            }
            else if (objBanco.Tipo == "M") // Manutenção
            {
                objBanco.TipoManutencaoOs = (TipoManutencaoOs?)Convert.ToInt32(view.TipoManutencao);
                objBanco.CodCt = view.Contrato;
                objBanco.Projeto = view.Projeto;
                objBanco.Num_Doc = documentoBusiness.GetNumDocManutencao();
                objBanco.OXBDPEE = "1";
                objBanco.Tarefa = "1";
                objBanco.Num_Obra = String.Format("{0}/{1}", objBanco.Numero_Obra_Original, 1);
                objBanco.Seq = 1;

                objBanco.Latitude_Str = view.Latitude;
                objBanco.Longitude_Str = view.Longitude;

                objBanco.Bairro = view.Bairro;
                objBanco.Cep = view.CEP;
                objBanco.Cidade = StringService.Truncate(view.Cidade, 20);
                objBanco.Logradouro = StringService.Truncate(view.Logradouro, 100);
                objBanco.Numero = view.Numero;
                objBanco.Complemento = StringService.Truncate(view.Complemento, 100);
                objBanco.Observacao = StringService.Truncate(view.Ponto_Referencia, 100);

                objBanco.Observacao_Aux = view.Observacao_OS;
                objBanco.UF = view.Uf;
                objBanco.Pais = view.Pais;
                objBanco.Cod_Tipo_Imovel = Convert.ToInt32(view.Id_Tipo_Imovel_Manut);

                objBanco.Id_Pop = view.Id_Pop;
                objBanco.Id_Olt = view.Id_Olt;
                objBanco.Id_Elemento_Rede = view.Id_Elemento_Rede;
            }

            objBanco.Cod_Escritorio = view.Cod_Escritorio;
            objBanco.Cod_Area = view.Cod_Area;
            objBanco.Cod_Localidade = view.Cod_Localidade;

            objBanco.Cod_Tipo_Serv = view.Id_Tipo_servico;
            objBanco.Cod_Sub_Tipo_Serv = view.Id_SubTipo_servico;
            //objBanco.Status_Cobranca = view.Status_Cobranca;
            objBanco.Observacao_Aux = view.Observacao_OS;
            objBanco.Segmento = view.Segmento;
            objBanco.Equipe = view.Id_Responsavel;
            //objBanco.Exige_Pagamento_Instalacao = String.IsNullOrEmpty(Convert.ToString(tipoServico.Exige_Pagamento_Instalacao)) ? null : Convert.ToString(tipoServico.Exige_Pagamento_Instalacao);
            view.Exige_Pagamento_Instalacao = tipoServico == null ? null : Convert.ToString(tipoServico.Exige_Pagamento_Instalacao);

            objBanco.Exige_Pagamento_Instalacao = view.Exige_Pagamento_Instalacao;
            objBanco.Id_Atendimento = view.Id_Atendimento;

            objBanco.Origem_Documento = Origem_Documento_Producao.Connect;

            objBanco.Responsabilidade = 0;
            objBanco.Roterizar = "N";

            objBanco.Data_Indicador = null;
            objBanco.Hora_Indicador = null;

            objBanco.Adicional_1 = view.Adcional_1;
            objBanco.Adicional_2 = view.Adcional_2;
            objBanco.Adicional_3 = view.Adcional_3;
            objBanco.Adicional_4 = view.Adcional_4;
            objBanco.Adicional_5 = view.Adcional_5;
            objBanco.Adicional_6 = view.Adcional_6;
            objBanco.Adicional_7 = view.Adcional_7;
            objBanco.Adicional_8 = view.Adcional_8;
            objBanco.Adicional_9 = view.Adcional_9;
            objBanco.Adicional_10 = view.Adcional_10;
            objBanco.NomeInstalacao = StringService.Truncate(view.NomeInstalacao, 255);
            objBanco.TelefoneInstalacao = StringService.Truncate(view.TelefoneInstalacao, 50);
            objBanco.Tipo_Prioridade = view.Tipo_Prioridade;
            objBanco.CpfInstalacao = StringService.Truncate(view.TelefoneInstalacao, 50);
            objBanco.Id_Venda = view.Id_Venda;

            if (objBanco.Id_Cliente_PlanoProdutoConnect != null)
            {
                using (var repoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                {
                    var porta = (await repoPorta.selectFirst(p => p.Cliente_Plano_Prod == objBanco.Id_Cliente_PlanoProdutoConnect));
                    objBanco.CaixaFTTH = porta?.Caixa?.Id != null ? Convert.ToString(porta?.Caixa?.Id) : null;
                    objBanco.PortaFTTH = porta?.Porta != null ? Convert.ToString(porta?.Porta) : null;
                }
            }

        }

        public async Task BindViewToObjMobile(ISession sessionT5, ISession sessionConnect, Documento_ProducaoAniel objBanco, LocalidadeAniel localidade,
                                                                  Cliente_PlanoProduto clientePlanoProduto, short Cod_Tipo_Serv)
        {
            ClienteAniel clienteAniel = null;
            //PlanoProdutoAniel clientePlanoProdutoAniel = null;
            Tipo_Servico_EquipeAniel tipo_Servico = (new Conexao<Tipo_Servico_EquipeAniel>(sessionT5).selectFirst(p => p.CodCt == localidade.CodCt && p.Cod_Tipo_Serv == Cod_Tipo_Serv).Result);

            clienteAniel = await GetClienteAniel(sessionT5, clientePlanoProduto.Cliente);
            //clientePlanoProdutoAniel = await GetPlanoProdutoAniel(sessionT5, clientePlanoProduto);

            objBanco.Cod_Escritorio = localidade.Cod_Escritorio;
            objBanco.Cod_Area = localidade.Cod_Area;
            objBanco.Cod_Localidade = localidade.Cod_Localidade;

            objBanco.Cliente = clienteAniel;
            objBanco.Nome_Cliente = StringService.Truncate(clienteAniel.NomeFantasia, 25);
            objBanco.Id_PlanoProduto = clientePlanoProduto.Plano != null ? clientePlanoProduto?.Plano?.Id : clientePlanoProduto?.Produto?.Id;
            objBanco.Id_Cliente_PlanoProdutoConnect = clientePlanoProduto.Id;

            objBanco.Bairro = clientePlanoProduto?.Endereco_Instalacao?.Bairro;
            objBanco.Cep = clientePlanoProduto?.Endereco_Instalacao?.Cep;
            objBanco.Cidade = clientePlanoProduto?.Endereco_Instalacao?.Cidade;
            objBanco.Logradouro = clientePlanoProduto?.Endereco_Instalacao?.Logradouro;
            objBanco.Numero = clientePlanoProduto?.Endereco_Instalacao?.Numero;
            objBanco.Complemento = clientePlanoProduto?.Endereco_Instalacao?.Complemento;
            objBanco.Observacao = StringService.Truncate(clientePlanoProduto?.Endereco_Instalacao?.Ponto_Referencia, 100);
            objBanco.UF = clientePlanoProduto?.Endereco_Instalacao?.UF;
            objBanco.Pais = HtmlEnumExtensions.GetEnumDescription<Prioridade_Atendimento>(Convert.ToString(clientePlanoProduto?.Endereco_Instalacao?.Pais));
            objBanco.Cod_Tipo_Imovel = clientePlanoProduto?.Endereco_Instalacao?.Tipo_Imovel?.Cod_Tipo_Imovel;
            objBanco.Latitude_Str = clientePlanoProduto?.Endereco_Instalacao?.Latitude?.ToString();
            objBanco.Longitude_Str = clientePlanoProduto?.Endereco_Instalacao?.Longitude?.ToString();

            objBanco.TelefonePrincipal = clientePlanoProduto?.Cliente?.Telefone_Fixo;
            objBanco.Tarefa = clientePlanoProduto?.Cliente?.Telefone_Fixo;
            objBanco.CelularCliente = clientePlanoProduto?.Cliente?.Telefone_Celular;
            objBanco.Titular = clientePlanoProduto?.Cliente?.NomeFantasia;
            objBanco.CPF_CLIENTE = clientePlanoProduto?.Cliente?.CpfCnpj;
            objBanco.Email_Assinante = clientePlanoProduto?.Cliente?.Email_Principal;
            objBanco.RG_Assinante = clientePlanoProduto?.Cliente?.Rg;

            objBanco.Origem_Documento = Origem_Documento_Producao.Ola_Cliente;
            objBanco.Cod_Tipo_Serv = tipo_Servico == null ? (Int16?)null : Convert.ToInt16(tipo_Servico.Cod_Tipo_Serv);
            objBanco.Segmento = "-";
            //objBanco.Cod_Sub_Tipo_Serv = view.Id_SubTipo_servico;
            //objBanco.Equipe = view.Id_Responsavel;

            objBanco.Id_Atendimento = null;// view.Id_Atendimento;

            objBanco.Origem_Documento = Origem_Documento_Producao.Ola_Cliente;

            objBanco.Responsabilidade = 0;
            objBanco.Roterizar = "N";

            objBanco.LoginRadius = clientePlanoProduto?.Cliente?.Login;
            objBanco.SenhaRadius = clientePlanoProduto?.Cliente?.Senha;

            if ((String.IsNullOrEmpty(objBanco.LoginRadius) || String.IsNullOrEmpty(objBanco.SenhaRadius) && clientePlanoProduto != null))
            {
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == clientePlanoProduto.Id);
                    if (loginRadius != null)
                    {
                        objBanco.LoginRadius = loginRadius.Login;
                        objBanco.SenhaRadius = loginRadius.SenhaPPPOE;
                        objBanco.Ssid_Wifi = loginRadius.SSID_WIFI;
                        objBanco.Senha_Wifi = loginRadius.Senha_WIFI;
                    }
                }
            }

            objBanco.Data_Indicador = null;
            objBanco.Hora_Indicador = null;

            //Criar os campos adicionais
        }



        [HttpGet()]
        [Route("api/Documento_ProducaoApi/Os_Criada_Atendimento")]
        public async Task<Int64> Os_Criada_Atendimento(Int64 AtendimentoId)
        {
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                {
                    var oss = await new Conexao<Documento_ProducaoAniel>(sessionT5).select(p => p.Id_Atendimento.Value == AtendimentoId);
                    return oss.Count();
                }

            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error($"BuscarPorAtendimento({AtendimentoId})", ex);
                return 1;
            }
        }

        [HttpPost(), HttpGet()]
        [Route("api/Documento_ProducaoApi/BuscarPorAtendimentoTrava")]
        public async Task<Boolean> BuscarPorAtendimentoTrava(Int64 AtendimentoId, Int64 TipoAtendimentoId)
        {
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                {
                    //var trava = new Conexao<Configuracoes_Gerais>(session).selectFirst().Result.Trava_Atendimento_OS == true;
                    var trava = new Conexao<Tipo_Atendimento>(session).selectFirst(x => x.Id == TipoAtendimentoId).Result.Trava_Atendimento_OS;
                    if (trava)
                    {
                        var oss = await new Conexao<Documento_ProducaoAniel>(sessionT5).select(p => p.Id_Atendimento.Value == AtendimentoId);

                        if (oss.Count() > 0)
                        {
                            var tmp = (from o in oss
                                       group o by new { o.Numero_Obra_Original, o.Seq, o.Status } into g
                                       where g.Key.Seq == g.Max(p => p.Seq)
                                       select new
                                       {
                                           Num_Obra = g.Key.Numero_Obra_Original,
                                           Seq = g.Max(p => p.Seq),
                                           Status = g.Key.Status
                                       }).ToList();


                            if (tmp.Find(p => p.Status != StatusExecucaoAniel.Fechada_Produtiva && p.Status != StatusExecucaoAniel.Fechada_Improdutiva && p.Status != StatusExecucaoAniel.Cancelado) != null)
                                return true;
                            else
                                return false;
                        }
                        else
                            return false;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error($"BuscarPorAtendimento({AtendimentoId})", ex);
                return false;
            }
        }

        [HttpPost()]
        [Route("api/Documento_ProducaoApi/Gravar")]
        public async Task<PostApiRetorno<Documento_ProducaoChaveModel>> Gravar(ParamWebApi<Documento_ProducaoCadastroModel> value)
        {
            PostApiRetorno<Documento_ProducaoChaveModel> retorno = new PostApiRetorno<Documento_ProducaoChaveModel>();

            try
            {
                Documento_ProducaoAniel modelo = null;

                using (var CamposObrigatoriosApi = new CamposObrigatoriosDocumentoProducaoApiController())
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                    using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
                    using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
                    {
                        if (value.Objeto.Id_Atendimento.HasValue)
                        {
                            var validacaoAtendimento = await CamposObrigatoriosApi.ValidacaoCamposObrigatoriosOS(value.Objeto.Id_Atendimento.Value);
                            if (!validacaoAtendimento.Ok)
                            {
                                retorno.Ok = false;
                                retorno.Mensagens = validacaoAtendimento.Mensagens;
                                return retorno;
                            }
                        }
                        LocalidadeAniel localidade = await new Conexao<LocalidadeAniel>(sessionT5).selectFirst(p => p.Cod_Escritorio == value.Objeto.Cod_Escritorio &&
                                                                                                                    p.Cod_Area == value.Objeto.Cod_Area &&
                                                                                                                    p.Cod_Localidade == value.Objeto.Cod_Localidade);

                        var tipoCliente = (string.IsNullOrEmpty(value.Objeto.Tipo) ? "C" : value.Objeto.Tipo) == "C";
                        if (tipoCliente && (String.IsNullOrEmpty(value.Objeto.Contrato) || String.IsNullOrEmpty(value.Objeto.Projeto)))
                        {
                            throw new Exception("CodCt/Projeto não informado na Localidade");
                        }


                        var config = await repoConfig.selectFirst();
                        IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                        var documentoBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);
                        String num_Obra = value.Objeto.Num_Obra ?? documentoBusiness.GetNum_Obra(config);

                        Tipo_Servico_EquipeAniel tipoServico = (await repoTipoServico.queryover().Where(p => p.Cod_Tipo_Serv == value.Objeto.Id_Tipo_servico).ListAsync()).FirstOrDefault();

                        String unidade_Custo = String.Empty;
                        using (var repoContrato = new Conexao<ContratoAniel>(sessionT5))
                        {
                            if (value.Objeto.Contrato != null && value.Objeto.Projeto != null)
                            {
                                unidade_Custo = (await repoContrato.selectFirst(p => p.CodCt == value.Objeto.Contrato))?.Unidade_Custo;
                            }


                            if (unidade_Custo == String.Empty && localidade != null)
                            {
                                unidade_Custo = (await repoContrato.selectFirst(p => p.CodCt == localidade.CodCt))?.Unidade_Custo;
                                if (unidade_Custo == null)
                                {
                                    throw new Exception("Nenhum contrato vinculado a localidade selecionada");
                                }
                            }
                        }

                        modelo = new Documento_ProducaoAniel
                        {
                            Status = StatusExecucaoAniel.Aberta_Aguard_Agendamento,
                            Tipo = value.Objeto.Tipo,
                            Numero_Obra_Original = num_Obra,
                            Unidade_Custo = unidade_Custo,
                            Adicional_1 = value.Objeto.Adcional_1,
                            Adicional_2 = value.Objeto.Adcional_2,
                            Adicional_3 = value.Objeto.Adcional_3,
                            Adicional_4 = value.Objeto.Adcional_4,
                            Adicional_5 = value.Objeto.Adcional_5,
                            Adicional_6 = value.Objeto.Adcional_6,
                            Adicional_7 = value.Objeto.Adcional_7,
                            Adicional_8 = value.Objeto.Adcional_8,
                            Adicional_9 = value.Objeto.Adcional_9,
                            Adicional_10 = value.Objeto.Adcional_10,
                            Usuario = value.Usuario,
                            Dia = DateTime.Now,
                            Hora = DateTime.Now,
                            CodDepto = value.Objeto.IdDepartamento,
                            Tipo_Prioridade = value.Objeto.Tipo_Prioridade,
                            Cod_Tipo_Imovel = Convert.ToInt32(value.Objeto.Id_Tipo_Imovel),
                        };

                        await BindViewToObj(session, sessionT5, repoCliente, repository, modelo, value.Objeto, localidade, tipoServico);
                        bool osJaExiste = false;
                        if (String.IsNullOrEmpty(value.Objeto.Num_Obra))
                            osJaExiste = documentoBusiness.NumeroOsJaExiste(value.Objeto.Contrato ?? localidade?.CodCt, value.Objeto.Projeto ?? localidade?.Projeto, num_Obra, modelo.Num_Doc);
                        else
                            osJaExiste = documentoBusiness.NumeroOsJaExiste(value.Objeto.Contrato ?? localidade?.CodCt, value.Objeto.Projeto ?? localidade?.Projeto, num_Obra);

                        if (osJaExiste)
                        {
                            throw new Exception("Ordem de serviço com esse número ja existe.");
                        }

                        // ******* Alterando o status dos Planos/Produtos da OS *******
                        if (tipoCliente && value.Objeto.Id_Cliente_Plano_Produto != null)
                        {
                            using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                            {
                                IDocumentoProducaoAnielRepository repo = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                                var business = new DocumentoProducaoAnielBusiness(repo);

                                Cliente_PlanoProduto planoProduto = (await repoClientePlanoProduto.selectFirst(p => p.Id == value.Objeto.Id_Cliente_Plano_Produto));
                                await business.CapturaPlanoSemOs(planoProduto.Id);

                                if (planoProduto.Status == Status_PlanoProduto.Aguardando_Criacao_OS)
                                    planoProduto.Status = Status_PlanoProduto.Aguardando_Instalacao;

                                await new Conexao<Cliente_PlanoProduto>(session).SaveOrUpdate(planoProduto);
                            }
                        }

                        // ***********************************************************

                        await repository.SaveOrUpdate(modelo);


                        // caso a os seja criada atraves do atendimento, deve ser criado uma acao no atendimento sobre a os
                        if (modelo.Id_Atendimento.HasValue)
                        {
                            var repositoryAtendimento = new Conexao<Atendimento>(session);
                            var repositoryVincular = new Conexao<VincularAtendimento>(session);
                            var atendimento = await repositoryAtendimento.selectFirst(p => p.Id == modelo.Id_Atendimento || p.Protocolo == Convert.ToString(modelo.Id_Atendimento));

                            var vincular = await repositoryVincular.selectFirst(p => p.Numero_Atendimento == atendimento.Protocolo && p.Num_Obra == Convert.ToString(modelo.Num_Obra) && p.Projeto == modelo.Projeto && p.CodCT == modelo.CodCt && p.Num_Obra == modelo.Num_Obra);



                            var travaAtendimento = atendimento?.Tipo_Atendimento?.Trava_Atendimento_OS;

                            Configuracoes_Gerais Conf_Gerais = await new Conexao<Configuracoes_Gerais>(session).selectFirst();

                            if (vincular == null)
                            {
                                vincular = new VincularAtendimento();

                                vincular.Numero_Atendimento = atendimento.Protocolo;
                                vincular.CodCT = modelo.CodCt;
                                vincular.Projeto = modelo.Projeto;
                                vincular.Num_Doc = Convert.ToString(modelo.Num_Doc);
                                vincular.Num_Obra = modelo.Num_Obra;

                                await repositoryVincular.SaveOrUpdate(vincular);
                            }

                            if (travaAtendimento.HasValue && travaAtendimento.Value)
                            //if (Conf_Gerais.Trava_Atendimento_OS)
                            {
                                atendimento.Status_Atendimento = Status_Atendimento.Aguardando;
                                atendimento.Motivo_Status = await new Conexao<Motivo_Status_Atendimento>(session).selectFirst(p => p.Id == -2);
                            }

                            atendimento.Ultima_Acao.Add(new Atendimento_Acao
                            {
                                UsuarioCriacao = value.Usuario,
                                HTML = $"OS {modelo.Numero_Obra_Original} gerada através desse atendimento.",
                                Status_Atendimento = atendimento.Status_Atendimento,
                                Atendimento = atendimento,
                                Responsavel = atendimento.Responsavel,
                            }
                            );

                            await repositoryAtendimento.SaveOrUpdate(atendimento);
                        }
                    }

                    await InsertLog_Lanc_Servico(sessionT5, modelo.CodCt, modelo.Projeto, modelo.Num_Obra, modelo.Num_Doc, "Criação do Serviço", null,
                            value.Usuario, null, null, null, null);
                }
                retorno.IdObjeto = new Documento_ProducaoChaveModel
                {
                    CodCt = modelo.CodCt,
                    Projeto = modelo.Projeto,
                    Num_Obra = modelo.Num_Obra,
                    Num_Doc = modelo.Num_Doc
                };
                retorno.Ok = true;

            }
            catch (Exception ex)
            {
                retorno.IdObjeto = null;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Documento_ProducaoApi/GetObrigatoriedadeTecnico")]
        public async Task<int> GetObrigatoriedadeTecnico(Documento_ProducaoBaixaModel value)
        {
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repositoryCodigoEncerramento = new Conexao<Codigo_EncerramentoAniel>(sessionT5))
                {
                    Codigo_EncerramentoAniel encerramentoAniel = await repositoryCodigoEncerramento.selectFirst(p => p.Cod_Encerramento == value.Codigo_Encerramento && p.CodCt == value.CodCt);
                    if (encerramentoAniel != null && encerramentoAniel.Status_Execucao == 2 && String.IsNullOrEmpty(value.Equipe))
                        return 1;
                    else
                        return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost()]
        [Route("api/Documento_ProducaoApi/BaixarOS")]
        public async Task<PostApiRetorno<Documento_ProducaoChaveModel>> BaixarOS(Documento_ProducaoBaixaModel value)
        {
            PostApiRetorno<Documento_ProducaoChaveModel> retorno = new PostApiRetorno<Documento_ProducaoChaveModel>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repositoryVinculoAt = new Conexao<VincularAtendimento>(session))
                using (var repository = new Conexao<Model.nhibernate.Atendimento>(session))
                using (var api = new NotificacoesApiController())
                {
                    IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                    INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(sessionT5);
                    ICodigoEncerramentoAnielRepository repoCodEncerr = new CodigoEncerramentoAnielRepository(sessionT5);
                    IContratoAnielRepository repoContrato = new ContratoAnielRepository(sessionT5);
                    IConfiguracoesGeraisRepository repoConfGerais = new ConfiguracoesGeraisRepository(session);
                    IClientePlanoProdutoRepository repoCliPlanProd = new ClientePlanoProdutoRepository(session);

                    var documentoBusiness = new DocumentoProducaoAnielBusiness(repoDocProd, repoNatureza, repoCodEncerr, repoContrato, repoConfGerais, repoCliPlanProd);

                    var documento = new DocProdBaixa
                    {
                        CodCt = value.CodCt,
                        Projeto = value.Projeto,
                        Num_Obra = value.Num_Obra,
                        Num_Doc = value.Num_Doc != null ? value.Num_Doc.Value : 0,
                        Observacao = value.Observacao,
                        Codigo_Encerramento = value.Codigo_Encerramento,
                        Equipe = value.Equipe,
                        Data_Ini_Atend = value.Data_Ini_Atend,
                        Data_Fim_Atend = value.Data_Fim_Atend,
                        Hora_Ini_Atend = value.Hora_Ini_Atend,
                        Hora_Fim_Atend = value.Hora_Fim_Atend,
                        CaixaFTTH = value.CaixaFTTH,
                        PortaFTTH = value.PortaFTTH,
                        Usuario = string.IsNullOrEmpty(value.Usuario) ? User.Identity.GetUserId() : value.Usuario,
                        BaixaOsProdDuplicada = value.BaixaOsProdDuplicada
                    };

                    var documentoBaixado = documentoBusiness.BaixarOrdemServico(documento, session, CONSTANTES.UsaRadiusSemContainer);

                    if (!documentoBaixado.Ok)
                        throw new Exception(message: documentoBaixado.Mensagem, documentoBaixado.InnerException);

                    var objDoc = new DocumentoProducao_GetDocumento
                    {
                        CodCt = documentoBaixado.DocProducao.CodCt,
                        Num_Obra = documentoBaixado.DocProducao.Num_Obra,
                        Num_Doc = documentoBaixado.DocProducao.Num_Doc,
                        Projeto = documentoBaixado.DocProducao.Projeto,
                        Num_Obra_Original = documentoBaixado.DocProducao.Numero_Obra_Original,
                        Status = Convert.ToInt32(documentoBaixado.DocProducao.Status),
                        Cod_Encerramento = documentoBaixado.DocProducao.Cod_Encerramento,
                        Status_Cobranca = Convert.ToInt32(documentoBaixado.DocProducao.Status_Cobranca)
                    };

                    await EncerrarAtendimentoJuntoOS(objDoc);
                    await RotinaRouter(objDoc);
                    await GerarCobrancaDocProd(objDoc);
                    if (documentoBaixado?.DocProducao?.IdLogin != null &&
                        !string.IsNullOrWhiteSpace(documento.PortaFTTH) &&
                        !string.IsNullOrWhiteSpace(documento.CaixaFTTH))
                    {
                        await new Elemento_Rede_PortaApiController().Atribuir_Porta(new Elemento_Reserva
                        {
                            Cliente_PlanoProduto = documentoBaixado.DocProducao.Id_Cliente_PlanoProdutoConnect ?? 0,
                            Id_Login = documentoBaixado?.DocProducao?.IdLogin,
                            Login = documentoBaixado?.DocProducao?.LoginRadius,
                            Porta = Convert.ToInt16(documento.PortaFTTH),
                            Id_Caixa = Convert.ToInt64(documento.CaixaFTTH),
                            Reset = false
                        });
                    }



                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = new Documento_ProducaoChaveModel { CodCt = value.CodCt, Projeto = value.Projeto, Num_Doc = value.Num_Doc, Num_Obra = value.Num_Obra };
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }
            return retorno;
        }

        [Route("api/Documento_ProducaoApi/GerarCobrancaDocProd")]
        [HttpPost()]
        public async Task<PostApiRetorno<bool>> GerarCobrancaDocProd(DocumentoProducao_GetDocumento doc)
        {
            PostApiRetorno<bool> response = new PostApiRetorno<bool>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoCliPlanProd = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoPlano = new Conexao<Plano>(session))
                using (var repoFatFormaPag = new Conexao<Model.nhibernate.Faturamento_Forma_Pagamento>(session))
                using (var repoFatCli = new Conexao<Faturamento_Cliente>(session))
                using (var repoCobServExec = new Conexao<Cobranca_Servico_Executado>(session))
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repoFormaPagamento = new Conexao<Forma_Pagamento>(session))
                {

                    IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                    FaturamentoService fatService = new FaturamentoService();
                    var docProdBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);

                    var docProd = repoDocProd.GetFirstDocProducao(doc.CodCt, doc.Projeto, doc.Num_Doc.Value, doc.Num_Obra);

                    if (docProd.Status_Cobranca == Status_CobrancaAniel.Aguardando_Avaliacao_Cobranca)
                    {
                        response.IdObjeto = false;
                        response.Ok = false;
                        response.Mensagem = "Essa OS necessita de aprovação antes de gerar a Cobrança";

                        return response;
                    }

                    if (docProd.Status_Cobranca == Status_CobrancaAniel.Reprovado)
                    {
                        response.IdObjeto = false;
                        response.Ok = false;
                        response.Mensagem = "Cobrança reprovada.";

                        return response;
                    }

                    var listaCobrancaValidado = await docProdBusiness.ValidarCobDocProd(docProd, doc.Status_Cobranca);
                    foreach (var cobValidado in listaCobrancaValidado)
                    {
                        var cliDocProd = await repoCliente.selectFirst(c => c.CpfCnpj == docProd.Cliente.CPF);
                        var objPlano = cobValidado.IdPlanoProduto > 0 ? await repoPlano.selectFirst(x => x.Id == cobValidado.IdPlanoProduto) : null;

                        var formPag = cobValidado.FormaPagamento != null ? await repoFormaPagamento.selectFirst(f => f.Id == cobValidado.FormaPagamento.Id) : null;
                        var objetoGenerico_Cliente_Plano = bindViewObjGen(cliDocProd, objPlano, null, formPag, docProd);

                        if (docProd.Status_Cobranca == Status_CobrancaAniel.Automatica || docProd.Status_Cobranca == Status_CobrancaAniel.Aprovado)
                        {
                            var cobServicoExec = await repoCobServExec.selectFirst(x => x.Num_Obra == docProd.Num_Obra && x.Plano_Cobranca.Id == objetoGenerico_Cliente_Plano.Plano.Id);
                            if (cobServicoExec == null)
                            {
                                cobServicoExec = BindCobServExec(geradoFatura: false, processado: false, null, objetoGenerico_Cliente_Plano, cobServicoExec);
                                await repoCobServExec.SaveOrUpdate(cobServicoExec);
                            }
                            else if (cobServicoExec.Processado || cobServicoExec.GeradoFatura)
                            {
                                response.IdObjeto = false;
                                response.Ok = false;
                                response.Mensagem = "Cobrança já efetuada.";

                                await InsertLog_Lanc_Servico(sessionT5, docProd.CodCt, docProd.Projeto, docProd.Num_Obra, docProd.Num_Doc,
                                "", "", "", null, null, response.Mensagem, response.Mensagem);
                            }
                            else
                            {
                                if (cobServicoExec.Plano_Cobranca.Id != objetoGenerico_Cliente_Plano.Plano.Id)
                                {
                                    cobServicoExec.Plano_Cobranca = objetoGenerico_Cliente_Plano.Plano;
                                    await repoCobServExec.SaveOrUpdate(cobServicoExec);
                                }
                            }

                            if (docProd.Status_Cobranca == Status_CobrancaAniel.Automatica)
                            {
                                await InsertLog_Lanc_Servico(sessionT5, docProd.CodCt, docProd.Projeto, docProd.Num_Obra, docProd.Num_Doc,
                                "", "", "", null, null, "Cobrança registrada.", "Cobrança registrada.");
                            }
                            else
                            {
                                await InsertLog_Lanc_Servico(sessionT5, docProd.CodCt, docProd.Projeto, docProd.Num_Obra, docProd.Num_Doc,
                                "", "", "", null, null, "Cobrança aprovada.", "Cobrança aprovada.");
                            }

                            #region [Trecho comentado - não será mais utilizado a geração automatica de cobrança para o EVA]
                            //if (cobValidado.MomentoCobranca == Momento_Cobranca.Baixa_Servico)
                            //{
                            //    var fatParcial = BindFaturamentoParcial(cobValidado, docProd, cobServicoExec);
                            //    var gerarFatParcial = await fatService.GerarFaturamento(fatParcial);

                            //    if (!gerarFatParcial.Ok)
                            //    {
                            //        response.IdObjeto = false;
                            //        response.Ok = false;
                            //        response.Mensagem = "Cobrança não efetuada. Erro ao Gerar o Faturamento";

                            //        await InsertLog_Lanc_Servico(sessionT5, docProd.CodCt, docProd.Projeto, docProd.Num_Obra, docProd.Num_Doc,
                            //        "", "", "", null, null, response.Mensagem, response.Mensagem);
                            //    }

                            //    if (response.Ok != false)
                            //    {
                            //        var fatFormaPag = await repoFatFormaPag.selectFirst(RequestAlteraOs => RequestAlteraOs.Faturamento_Protocolo.Id == gerarFatParcial.IdObjeto &&
                            //                                                            RequestAlteraOs.Forma_pagto.Id == cobValidado.FormaPagamento.Id);

                            //        var fatCliente = await repoFatCli.selectFirst(RequestAlteraOs => RequestAlteraOs.Faturamento_Forma_Pagto == fatFormaPag &&
                            //                                                          RequestAlteraOs.Cliente.Id == docProd.Cliente.Cod_Cliente);
                            //        objetoGenerico_Cliente_Plano = bindViewObjGen(cliDocProd, objPlano, fatCliente, formPag, docProd);
                            //        if (formPag.Gerar_Boleto_Automatico)
                            //        {

                            //            var fatProcessarObj = new FaturamentoProcessarRequestModels
                            //            {
                            //                Id_Faturamento_Forma = fatFormaPag?.Id,
                            //                Id_Cliente = cliDocProd.Id,
                            //                Ids_Faturamento_Cliente = new List<long?> { fatCliente.Id }
                            //            };

                            //            var gerarProcessamento = await fatService.GerarProcessamento(fatProcessarObj);

                            //            if (gerarProcessamento.Ok)
                            //            {
                            //                cobServicoExec = BindCobServExec(geradoFatura: true, processado: true, gerarFatParcial.IdObjeto, objetoGenerico_Cliente_Plano, cobServicoExec);
                            //                await repoCobServExec.SaveOrUpdate(cobServicoExec);

                            //                await InsertLog_Lanc_Servico(sessionT5, docProd.CodCt, docProd.Projeto, docProd.Num_Obra, docProd.Num_Doc,
                            //                    "", "", "", null, null, "Cobrança Gerada e Processada", "Cobrança Gerada e Processada");
                            //            }
                            //            else
                            //            {
                            //                cobServicoExec = BindCobServExec(geradoFatura: true, processado: false, gerarFatParcial.IdObjeto, objetoGenerico_Cliente_Plano, cobServicoExec);
                            //                await repoCobServExec.SaveOrUpdate(cobServicoExec);
                            //                await InsertLog_Lanc_Servico(sessionT5, docProd.CodCt, docProd.Projeto, docProd.Num_Obra, docProd.Num_Doc,
                            //                    "", "", "", null, null, "Cobrança Gerada", "Cobrança Gerada");
                            //            }
                            //        }
                            //        else
                            //        {
                            //            cobServicoExec = BindCobServExec(geradoFatura: true, processado: false, gerarFatParcial.IdObjeto, objetoGenerico_Cliente_Plano, cobServicoExec);
                            //            await repoCobServExec.SaveOrUpdate(cobServicoExec);
                            //            await InsertLog_Lanc_Servico(sessionT5, docProd.CodCt, docProd.Projeto, docProd.Num_Obra, docProd.Num_Doc,
                            //                "", "", "", null, null, "Cobrança Gerada", "Cobrança Gerada");
                            //        }
                            //    }
                            //}
                            //else if (cobValidado.MomentoCobranca == Momento_Cobranca.Fechamento_Faturamento)
                            //{

                            //    cobServicoExec = BindCobServExec(geradoFatura: false, processado: false, null, objetoGenerico_Cliente_Plano, cobServicoExec);
                            //    await repoCobServExec.SaveOrUpdate(cobServicoExec);

                            //    await InsertLog_Lanc_Servico(sessionT5, docProd.CodCt, docProd.Projeto, docProd.Num_Obra, docProd.Num_Doc,
                            //        "", "", "", null, null, "Gerado Registro da Cobranca para Faturamento", "Gerado Registro da Cobranca para Faturamento.");
                            //}
                            #endregion

                        }
                        else
                        {
                            if (docProd.Status_Cobranca == Status_CobrancaAniel.Reprovado)
                            {
                                await InsertLog_Lanc_Servico(sessionT5, docProd.CodCt, docProd.Projeto, docProd.Num_Obra, docProd.Num_Doc,
                                "", "", "", null, null, "Cobrança reprovada.", "Cobrança reprovada.");
                            }
                            else
                            {
                                await InsertLog_Lanc_Servico(sessionT5, docProd.CodCt, docProd.Projeto, docProd.Num_Obra, docProd.Num_Doc,
                                "", "", "", null, null, "Cobrança aguardando avaliação.", "Cobrança aguardando avaliação.");
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            response.Ok = true;
            response.IdObjeto = true;
            return response;
        }
        private ObjetosCobranca_Plano_Cliente bindViewObjGen(Cliente cliente, Plano plano, Faturamento_Cliente faturamento_Cliente, Forma_Pagamento forma_Pagamento, Documento_ProducaoAniel docProd)
        {
            return new ObjetosCobranca_Plano_Cliente
            {
                Cliente = cliente,
                Plano = plano,
                Faturamento_Cliente = faturamento_Cliente,
                Forma_Pagamento = forma_Pagamento,
                Documento_Producao = docProd
            };
        }

        private FaturamentoModels BindFaturamentoParcial(ValidacaoCobDocProd validacaoCob, Documento_ProducaoAniel docProd, Cobranca_Servico_Executado cobranca)
        {
            FaturamentoModels response = new FaturamentoModels();

            response.Id_Cliente = docProd.Cliente.Cod_Cliente;
            //response.Id_FormaPagamento = validacaoCob.FormaPagamento.Id;
            response.FormasPagamento = new List<long?>()
                            {
                {
                    validacaoCob.FormaPagamento.Id
                            }
            };
            response.MesCompetenciaInicio = DateTime.Now.ToString("MM/yyyy");
            response.MesCompetenciaFim = DateTime.Now.ToString("MM/yyyy");
            response.IdsPlanoProdutoCobranca = new List<long>();
            response.Usuario = User.Identity.GetUserId();

            if (cobranca != null)
                response.IdsPlanoProdutoCobranca.Add(cobranca.Id);

            return response;
        }

        private Cobranca_Servico_Executado BindCobServExec(bool geradoFatura, bool processado, long? idFatProtocolo, ObjetosCobranca_Plano_Cliente objeto_generico, Cobranca_Servico_Executado cobranca)
        {
            if (cobranca == null)
            {
                cobranca = new Cobranca_Servico_Executado
                {
                    Codct = objeto_generico.Documento_Producao.CodCt,
                    Projeto = objeto_generico.Documento_Producao.Projeto,
                    Num_Obra = objeto_generico.Documento_Producao.Num_Obra,
                    Num_Doc = objeto_generico.Documento_Producao.Num_Doc,
                    Forma_Pagamento = objeto_generico.Forma_Pagamento,
                    Id_FaturamentoProtocolo = idFatProtocolo != null ? idFatProtocolo.Value : (Int64?)null,
                    Plano_Cobranca = objeto_generico.Plano,
                    Faturamento_Cliente = objeto_generico.Faturamento_Cliente,
                    GeradoFatura = geradoFatura,
                    Processado = processado,
                    Cliente = objeto_generico.Cliente
                };
            }
            else
            {
                cobranca.Codct = objeto_generico.Documento_Producao.CodCt;
                cobranca.Projeto = objeto_generico.Documento_Producao.Projeto;
                cobranca.Num_Obra = objeto_generico.Documento_Producao.Num_Obra;
                cobranca.Num_Doc = objeto_generico.Documento_Producao.Num_Doc;
                cobranca.Forma_Pagamento = objeto_generico.Forma_Pagamento;
                cobranca.Id_FaturamentoProtocolo = idFatProtocolo != null ? idFatProtocolo.Value : (Int64?)null;
                cobranca.Plano_Cobranca = objeto_generico.Plano;
                cobranca.Faturamento_Cliente = objeto_generico.Faturamento_Cliente;
                cobranca.GeradoFatura = geradoFatura;
                cobranca.Processado = processado;
                cobranca.Cliente = objeto_generico.Cliente;
            }

            return cobranca;
        }

        [HttpPost()]
        [Route("api/Documento_ProducaoApi/ReabrirOS")]
        public async Task<PostApiRetorno<Documento_ProducaoChaveModel>> ReabrirOS(Documento_ProducaoChaveModel value)
        {
            PostApiRetorno<Documento_ProducaoChaveModel> retorno = new PostApiRetorno<Documento_ProducaoChaveModel>();

            try
            {

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (ITransaction transaction = sessionT5.BeginTransaction())
                {
                    Documento_ProducaoAniel documento = await repository.selectFirst(p => p.Num_Obra == value.Num_Obra && p.Num_Doc == value.Num_Doc && p.Projeto == value.Projeto && p.CodCt == value.CodCt);
                    if (documento == null)
                    {
                        throw new Exception("Ordem de Servico não foi encontrada");
                    }

                    if (documento.Status != StatusExecucaoAniel.Fechada_Improdutiva &&
                        documento.Status != StatusExecucaoAniel.Fechada_Produtiva &&
                        documento.Status != StatusExecucaoAniel.Atendimento_Finalizado)
                    {
                        throw new Exception("Ordem de Servico não se encontrada fechada");
                    }

                    documento.Data_Prod = null;
                    documento.Hora_Prod = null;
                    documento.Cod_Encerramento = null;

                    //decidido nao apagar esses campos por motivos de historico
                    //documento.DataHora_Deslocamento = null;
                    //documento.DataHora_Fim_Atend = null;
                    //documento.DataHora_Fim_Atend = null;
                    //documento.DataHora_Pausou = null;
                    //documento.DataHora_Reinicio = null;

                    documento.Modulo_Baixa = null;
                    documento.Roterizar = "N";
                    documento.Responsabilidade = 0;
                    documento.Status_Auditoria = null;
                    documento.Status_Cobranca = null;
                    if (documento.Equipe == null)
                    {
                        if (documento.Data_Maxima == null)
                            documento.Status = StatusExecucaoAniel.Aberta_Aguard_Agendamento;
                        else
                            documento.Status = StatusExecucaoAniel.Aberta_Aguard_Responsavel;
                    }
                    else
                    {
                        documento.Status = StatusExecucaoAniel.Aberta_Aguard_Atendimento;
                    }

                    await repository.Update(documento);
                    await InsertLog_Lanc_Servico(sessionT5, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc, "Reabriu OS", null,
                               User.Identity.GetUserId(), documento.Data_Maxima, documento.Cod_Tipo_Serv, null, null, transaction);

                    transaction.Commit();

                    var notificacao = new NotificacoesApiController().EnviaNotificacoesServico(20, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc, null);
                }

                retorno.Ok = true;

            }

            catch (Exception ex)
            {
                retorno.IdObjeto = new Documento_ProducaoChaveModel { CodCt = value.CodCt, Projeto = value.Projeto, Num_Doc = value.Num_Doc, Num_Obra = value.Num_Obra };
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Documento_ProducaoApi/GetEquipeOS")]
        public async Task<PostApiRetorno<String>> GetEquipeOS(Documento_ProducaoChaveModel value)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
                {
                    Documento_ProducaoAniel documento = await repository.selectFirst(p => p.Num_Obra == value.Num_Obra && p.Num_Doc == value.Num_Doc && p.Projeto == value.Projeto && p.CodCt == value.CodCt);
                    retorno.IdObjeto = documento.Equipe;
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = "";
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        public async Task<List<DocumentoProducaoModel>> GetDocumentoAgendamento(String CodCt, String Projeto, String Num_Os, Int32 Num_Doc)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                Boolean Exibe_RDOs = (await new Conexao<Configuracoes_Gerais>(session).selectFirst()).ExibeRDOs;

                List<DocumentoProducaoModel> retorno = new List<DocumentoProducaoModel>();

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder("SELECT A.NUM_OBRA_ORIGINAL, A.NUM_OBRA, A.CODCT, A.PROJETO, A.NUM_DOC, A.DATA_ABERTURA AS DATAHORA_ABERTURA_OS, A.STATUS_EXECUCAO AS STATUS_OS, ")
                    .AppendLine("A.DATAHORA_AGENDAMENTO AS DATAHORA_AGENDAMENTO_OS, A.DATA_PROD AS DATAHORA_ENCERRAMENTO_OS, ")
                    .AppendLine("A.COD_ESCRITORIO, C.DESCRICAO AS DESCRICAO_ESCRITORIO, A.COD_LOCALIDADE,F.COD_NATUREZA,")
                    .AppendLine("A.COD_AREA, D.DESCRICAO AS DESCRICAO_AREA, E.LOCALIDADE, F.DESCRICAO AS DESCRICAO_TIPO_SERV,F.EXIGE_PAGAMENTO_INSTALACAO, ")
                    .AppendLine("G.DESCRICAO AS DESCRICAO_SUBTIPO_SERV, H.RAZAO_SOCIAL, ")
                    .AppendLine("A.ENDERECO AS ENDERECO_CLIENTE, A.TEL_CIENTE AS CELULAR_CLIENTE, ")
                    .AppendLine("A.ENDERECO_COMPL AS ENDERECO_COMPL, A.TEL_COMERCIAL AS TELEFONE_PRINCIPAL_CLIENTE, ")
                    .AppendLine("A.ENDERECO_TIPO AS ENDERECO_CIDADE, A.ENDERECO_BAIRRO AS ENDERECO_BAIRRO, A.ENDERECO_UF AS ENDERECO_UF, ")
                    .AppendLine("A.ENDERECO_CPL AS ENDERECO_CEP, 'Brasil' AS ENDERECO_PAIS, A.ENDERECO_NUMERO, ")
                    .AppendLine("J.DESCRICAO AS TIPO_IMOVEL, L.DESCRICAO AS NATUREZA_OS, A.PARPRIM_PARA AS ID_TIPO_IMOVEL, A.COD_TIPO_SERV, ")

                    //PEGA TEMPO DE EXECUÇÃO INDIFERENTE OU
                    .AppendLine("(SELECT FIRST 1 M.HORAS ")
                    .AppendLine("FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M ")
                    .AppendLine("WHERE ( (M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO ")

                    .AppendLine("   FROM TB_DOCUMENTO_PRODUCAO A ")
                    .AppendLine("LEFT JOIN TB_ESCRITORIO C ")
                    .AppendLine("on(A.COD_ESCRITORIO = C.COD_ESCRITORIO) ")
                    .AppendLine("LEFT JOIN TB_AREA D ")
                    .AppendLine("on(D.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA) ")
                    .AppendLine("LEFT JOIN TB_LOCALIDADE E ")
                    .AppendLine("on(E.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND ")
                    .AppendLine("A.COD_LOCALIDADE = E.COD_LOCALIDADE) ")
                    .AppendLine("INNER JOIN TB_TIPO_SERVICO_EQUIPE F ")
                    .AppendLine("on(A.COD_TIPO_SERV = F.COD_TIPO_SERV) ") //AND F.CODCT = A.CODCT
                    .AppendLine("LEFT JOIN TB_SUB_TIPO_SERVICO G ")
                    .AppendLine("on(A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV) ")
                    .AppendLine("LEFT JOIN TB_CLIENTE H ") // ID CLIENTE
                    .AppendLine("on(A.ID_CLIENTE = H.COD_CLIENTE) ")
                    .AppendLine("LEFT JOIN TB_TIPO_IMOVEL J ")
                    .AppendLine("on(A.PARPRIM_PARA = J.COD_TIPO_IMOVEL) ")
                    .AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO L")
                    .AppendLine("on(L.ID = A.NATUREZA) ")

                    .AppendLine("  WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA = @NUM_OBRA AND A.NUM_DOC = @CONTRATO ");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    parametros.Add("@CODCT", CodCt);
                    parametros.Add("@PROJETO", Projeto);
                    parametros.Add("@NUM_OBRA", Num_Os);
                    parametros.Add("@CONTRATO", Num_Doc);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new DocumentoProducaoModel
                            {
                                Numero_OS = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]),
                                Nome_Cliente = Convert.ToString(reader["RAZAO_SOCIAL"]),
                                Endereco_Cliente = Convert.ToString(reader["ENDERECO_CLIENTE"]),
                                Endereco_Numero = Convert.ToString(reader["ENDERECO_NUMERO"]),
                                Endereco_UF = Convert.ToString(reader["ENDERECO_UF"]),
                                Endereco_Cidade = Convert.ToString(reader["ENDERECO_CIDADE"]),
                                Endereco_Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]),
                                Endereco_Complemento = Convert.ToString(reader["ENDERECO_COMPL"]),
                                Telefone_Principal_Cliente = Convert.ToString(reader["TELEFONE_PRINCIPAL_CLIENTE"]),
                                Celular_Cliente = Convert.ToString(reader["CELULAR_CLIENTE"]),
                                Cod_Tipo_Serv = reader["COD_TIPO_SERV"] != DBNull.Value ? Convert.ToInt16(reader["COD_TIPO_SERV"]) : (Int16?)null,
                                Tipo_Servico = Convert.ToString(reader["DESCRICAO_TIPO_SERV"]),
                                SubTipo_Servico = Convert.ToString(reader["DESCRICAO_SUBTIPO_SERV"]),
                                Exige_Pagamento_Instalacao = Convert.ToString(reader["EXIGE_PAGAMENTO_INSTALACAO"]),
                                Status_OS = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(reader["STATUS_OS"])),
                                //Status_Cobranca = HtmlEnumExtensions.GetEnumDescription<Status_CobrancaAniel>(Convert.ToString(reader["STATUS_COBRANCA"])),
                                Id_Tipo_Imovel = reader["ID_TIPO_IMOVEL"] != DBNull.Value ? Convert.ToInt32(reader["ID_TIPO_IMOVEL"]) : (Int32?)null,
                                Tipo_Imovel = Convert.ToString(reader["TIPO_IMOVEL"]),
                                Cod_Natureza = reader["COD_NATUREZA"] != DBNull.Value ? Convert.ToInt32(reader["COD_NATUREZA"]) : (Int32?)null,
                                Cod_Grupo_Area = Convert.ToString(reader["COD_ESCRITORIO"]),
                                Grupo_Area = Convert.ToString(reader["DESCRICAO_ESCRITORIO"]),
                                Cod_Area = Convert.ToString(reader["COD_AREA"]),
                                Area = Convert.ToString(reader["DESCRICAO_AREA"]),
                                Cod_Localidade = reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToInt32(reader["COD_LOCALIDADE"]) : (Int32?)null,
                                Localidade = Convert.ToString(reader["LOCALIDADE"]),
                                TempoExecucao = reader["TEMPO_EXECUCAO"] != DBNull.Value ? Convert.ToDateTime(((TimeSpan)(reader["TEMPO_EXECUCAO"])).ToString(@"hh\:mm")) : (DateTime?)null,
                                Num_Obra = Convert.ToString(reader["NUM_OBRA"]),
                                CodCt = Convert.ToString(reader["CODCT"]),
                                Projeto = Convert.ToString(reader["PROJETO"]),
                                Num_Doc = reader["NUM_DOC"] != DBNull.Value ? Convert.ToInt64(reader["NUM_DOC"]) : 0,
                            });
                        }
                    }

                    return retorno;

                }
            }
        }


        public async Task<Boolean> InsertLog_Lanc_Servico(ISession session, String CodCt, String Projeto, String Num_Os, Int32 Num_Doc,
            String Alteracao, String Valor_Anterior, String Usuario, DateTime? Data_Maxima, Int16? Cod_Tipo_Serv, String Valor_Atualizado, String Obs_Cobranca_Status, ITransaction transaction = null)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            //Select
            using (var com = session.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder("INSERT INTO TB_LOG_LANC_SERVICO (CODCT, PROJETO, NUM_OBRA, NUM_DOC, ALTERACAO, VALOR_ANT, VALOR_ATUALIZADO,OBS_STATUS_COBRANCA, ")
                            .AppendLine(" USUARIO, DIA, HORA, DATA_MAXIMA, COD_TIPO_SERV) ")
                            .AppendLine(" VALUES(@CODCT, @PROJETO, @NUM_OBRA, @NUM_DOC, @ALTERACAO, @VALOR_ANT,@VALOR_ATUALIZADO,@OBS_STATUS_COBRANCA, ")
                            .AppendLine(" @USUARIO, CURRENT_DATE, CURRENT_TIME, @DATA_MAXIMA, @COD_TIPO_SERV) ");

                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUM_OBRA", Num_Os);
                parametros.Add("@NUM_DOC", Num_Doc);

                parametros.Add("@ALTERACAO", Alteracao);
                parametros.Add("@VALOR_ANT", Valor_Anterior);
                parametros.Add("@VALOR_ATUALIZADO", Valor_Atualizado);
                parametros.Add("@OBS_STATUS_COBRANCA", Obs_Cobranca_Status);
                parametros.Add("@USUARIO", Usuario);
                parametros.Add("@DATA_MAXIMA", Data_Maxima);
                parametros.Add("@COD_TIPO_SERV", Cod_Tipo_Serv);

                if (transaction != null)
                {
                    transaction.Enlist(com);
                }

                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, com.Transaction != null ? com.Transaction : null);

                return true;

            }

        }

        public async Task<Boolean> InsertLog_Lanc_ServicoReagendamento(ISession session, String CodCt, String Projeto, String Num_Os, Int32 Num_Doc,
            String Alteracao, String Valor_Anterior, String Usuario, DateTime? Data_Maxima, Int16? Cod_Tipo_Serv, String Valor_Atualizado,
            String Obs_Cobranca_Status,
            long? CodMotivoReagendamento,
            string ObsMotivoReagendamento,
            ITransaction transaction = null)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            //Select
            using (var com = session.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder("INSERT INTO TB_LOG_LANC_SERVICO (CODCT, PROJETO, NUM_OBRA, NUM_DOC, ALTERACAO, VALOR_ANT, VALOR_ATUALIZADO,OBS_STATUS_COBRANCA, ")
                            .AppendLine(" USUARIO, DIA, HORA, DATA_MAXIMA, COD_TIPO_SERV,COD_MOTIVO_REAGENDAMENTO,OBSERVACAO_MOTIVO_REAGENDAMENTO) ")
                            .AppendLine(" VALUES(@CODCT, @PROJETO, @NUM_OBRA, @NUM_DOC, @ALTERACAO, @VALOR_ANT,@VALOR_ATUALIZADO,@OBS_STATUS_COBRANCA, ")
                            .AppendLine(" @USUARIO, CURRENT_DATE, CURRENT_TIME, @DATA_MAXIMA, @COD_TIPO_SERV,@COD_MOTIVO_REAGENDAMENTO,@OBSERVACAO_MOTIVO_REAGENDAMENTO) ");

                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUM_OBRA", Num_Os);
                parametros.Add("@NUM_DOC", Num_Doc);

                parametros.Add("@ALTERACAO", Alteracao);
                parametros.Add("@VALOR_ANT", Valor_Anterior);
                parametros.Add("@VALOR_ATUALIZADO", Valor_Atualizado);
                parametros.Add("@OBS_STATUS_COBRANCA", Obs_Cobranca_Status);
                parametros.Add("@USUARIO", Usuario);
                parametros.Add("@DATA_MAXIMA", Data_Maxima);
                parametros.Add("@COD_TIPO_SERV", Cod_Tipo_Serv);
                parametros.Add("@COD_MOTIVO_REAGENDAMENTO", CodMotivoReagendamento);
                parametros.Add("@OBSERVACAO_MOTIVO_REAGENDAMENTO", ObsMotivoReagendamento);

                if (transaction != null)
                {
                    transaction.Enlist(com);
                }

                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, com.Transaction != null ? com.Transaction : null);

                return true;

            }

        }

        [AllowAnonymous]
        [AcceptVerbs("Post", "Get")]
        [Route("api/DocumentoProducao/EncerrarAtendimentoJuntoOS")]
        public async Task<PostApiRetorno<Int64>> EncerrarAtendimentoJuntoOS(DocumentoProducao_GetDocumento docProd)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<long>
            {
                Ok = true
            };

            Documento_ProducaoAniel Doc_Prod = new Documento_ProducaoAniel();

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                //var teste = await new BuscaGenericaRepository<Documento_ProducaoAniel>(sessionT5, null).FindByWhere(p => p.CodCt == docProd.CodCt && p.Projeto == docProd.Projeto && p.Num_Obra == docProd.Num_Obra && p.Num_Doc == docProd.Num_Doc);

                using (var repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5))
                {
                    Doc_Prod = await repoDocProd.selectFirst(p => p.CodCt == docProd.CodCt && p.Projeto == docProd.Projeto && p.Num_Obra == docProd.Num_Obra && p.Num_Doc == docProd.Num_Doc);
                }
            }

            if (Doc_Prod == null)
            {
                return retorno;
            }

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession session = HibernateUtil.GetSession())
            {
                using (var repoDocs = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repoConfGerais = new Conexao<Configuracoes_Gerais>(session))
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                using (var repoMotivoStatus = new Conexao<Motivo_Status_Atendimento>(session))
                using (var repositoryVinculoAt = new Conexao<VincularAtendimento>(session))
                using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                using (var roteirizacaoApi = new Painel_RoteirizacaoApiController())
                using (var repoFluxo = new Conexao<FluxoAtendimento>(session))
                using (var repoEncerramentoAtendimento = new Conexao<EncerramentoAtendimento>(session))
                using (var repoTipo_Atendimento = new Conexao<Tipo_Atendimento>(session))
                using (var api = new NotificacoesApiController())
                {
                    Configuracoes_Gerais Conf_Gerais = await repoConfGerais.selectFirst();
                    IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                    INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(sessionT5);
                    ICodigoEncerramentoAnielRepository repositoryCodEncerramento = new CodigoEncerramentoAnielRepository(sessionT5);
                    IContratoAnielRepository repoContrato = new ContratoAnielRepository(sessionT5);
                    IConfiguracoesGeraisRepository repoConfigGerais = new ConfiguracoesGeraisRepository(session);
                    IClientePlanoProdutoRepository repoCliPlanProd = new ClientePlanoProdutoRepository(session);


                    var documentoBusiness = new DocumentoProducaoAnielBusiness(repoDocProd, repoNatureza, repositoryCodEncerramento,
                                                                               repoContrato, repoConfigGerais, repoCliPlanProd);

                    if (Doc_Prod?.Id_Atendimento > 0 || Doc_Prod?.Id_Cliente_PlanoProdutoConnect > 0)
                    {
                        var objAtendimento = await repoAtendimento.selectFirst(x => x.Id == Doc_Prod.Id_Atendimento || x.Protocolo == Convert.ToString(Doc_Prod.Id_Atendimento));
                        var objClientePP = await repoClientePlanoProduto.selectFirst(x => x.Id == Doc_Prod.Id_Cliente_PlanoProdutoConnect && x.Endereco_Mudanca_Cliente != null);
                        //Fechada Produtiva - 2
                        if (Doc_Prod.Status == StatusExecucaoAniel.Fechada_Produtiva)
                        {
                            documentoBusiness.HabilitarPlano(session, Doc_Prod, CONSTANTES.UsaRadiusSemContainer);

                            if (objAtendimento != null)
                            {
                                if (objAtendimento?.Tipo_Atendimento?.Encerrar_Atendimento_Junto_OS ?? false)
                                {
                                    objAtendimento.Status_Atendimento = Status_Atendimento.Concluido;
                                    objAtendimento.Motivo_Status = await repoMotivoStatus.selectFirst(x => x.Id == -1);
                                }

                                if (objAtendimento.EtapaFluxo.Baixar_OS)
                                {
                                    var encerra_atendimento = await repoEncerramentoAtendimento.selectFirst(x => x.FluxoAtendimento.Id == objAtendimento.EtapaFluxo.Id &&
                                                                                                                 x.Cod_Encerramento == Doc_Prod.Cod_Encerramento &&
                                                                                                                 x.Contrato == Doc_Prod.CodCt);
                                    if (encerra_atendimento != null)
                                    {
                                        var proxima_etapa = await repoFluxo.selectFirst(x => x.Id == encerra_atendimento.IdFluxoEtapa && x.Status == Status_Default.Ativo);
                                        objAtendimento.Ultima_Acao.Add(new Atendimento_Acao()
                                        {
                                            Atendimento = objAtendimento,
                                            HTML = $"A baixa produtiva da OS {Doc_Prod.Numero_Obra_Original} avançou a etapa do atendimento de {objAtendimento.EtapaFluxo.Descricao} para {proxima_etapa.Descricao}.",
                                            Responsavel = objAtendimento.Responsavel,
                                            Status_Atendimento = objAtendimento.Status_Atendimento,
                                            UsuarioCriacao = Doc_Prod.Usuario,
                                        });
                                        objAtendimento.EtapaFluxo = proxima_etapa;
                                    }
                                }

                                if (objAtendimento.Endereco_Mudanca != null)
                                {
                                    objAtendimento.PlanoProduto.Endereco_Instalacao = objAtendimento.Endereco_Mudanca;
                                    objAtendimento.Endereco_Mudanca = null;
                                    objAtendimento.Ultima_Acao.Add(new Atendimento_Acao()
                                    {
                                        Atendimento = objAtendimento,
                                        HTML = "Alteração do endereço de instalação após conclusão do atendimento.",
                                        Responsavel = objAtendimento.Responsavel,
                                        Status_Atendimento = objAtendimento.Status_Atendimento,
                                        UsuarioCriacao = Doc_Prod.Usuario,
                                    });
                                }
                            }

                            if (objClientePP != null)
                            {
                                objClientePP.Endereco_Mudanca_Cliente = null;
                            }


                            



                        }//Fechada Improdutiva - 1
                        else if (Doc_Prod.Status == StatusExecucaoAniel.Fechada_Improdutiva)
                        {
                            using (var repoCodEncerramento = new Conexao<Codigo_EncerramentoAniel>(sessionT5))
                            {
                                var codigo_encerramento = repoCodEncerramento.selectFirst(x => x.CodCt == Doc_Prod.CodCt && x.Cod_Encerramento == Doc_Prod.Cod_Encerramento).Result;
                                if (codigo_encerramento != null)
                                {
                                    if (codigo_encerramento.Encerra_OS_Apos_Baixa == "N")
                                    {
                                        if (objAtendimento != null)
                                        {
                                            objAtendimento.Ultima_Acao.Add(new Atendimento_Acao
                                            {
                                                UsuarioCriacao = User.Identity.GetUserId(),
                                                HTML = $"OS {Doc_Prod.Numero_Obra_Original} para troca de endereço de instalação reagendada.",
                                                Status_Atendimento = objAtendimento.Status_Atendimento,
                                                Atendimento = objAtendimento,
                                                Responsavel = objAtendimento.Responsavel,
                                            });
                                        }
                                    }
                                    else if (codigo_encerramento.Encerra_OS_Apos_Baixa == "S")
                                    {
                                        if (objAtendimento != null)
                                        {
                                            objAtendimento.Endereco_Mudanca = null;
                                            objAtendimento.Ultima_Acao.Add(new Atendimento_Acao()
                                            {
                                                Atendimento = objAtendimento,
                                                HTML = "Alteração de endereço de instalação abortada! Ordem de Serviço fechada improdutiva.",
                                                Responsavel = objAtendimento.Responsavel,
                                                Status_Atendimento = objAtendimento.Status_Atendimento,
                                                UsuarioCriacao = User.Identity.GetUserId(),
                                            });
                                        }

                                        if (objClientePP != null)
                                        {
                                            objClientePP.Endereco_Mudanca_Cliente = null;
                                        }
                                    }
                                }
                            }
                        }

                        if (Doc_Prod.Status == StatusExecucaoAniel.Fechada_Produtiva || Doc_Prod.Status == StatusExecucaoAniel.Fechada_Improdutiva)
                        {
                            var atendimentosvinculadosOS = (await repositoryVinculoAt.select(p =>
                             p.CodCT == Doc_Prod.CodCt &&
                             p.Projeto == Doc_Prod.Projeto &&
                             p.Num_Obra == Doc_Prod.Num_Obra &&
                             p.Num_Doc == Convert.ToString(Doc_Prod.Num_Doc)
                             )).ToList();
                            List<Atendimento> atendimentosVinculados = atendimentosvinculadosOS.Where(p => p.Atendimento != null).Select(p => p.Atendimento).Count() > 0 ?
                                atendimentosvinculadosOS.Select(p => p.Atendimento).ToList() : new List<Atendimento>();

                            if (atendimentosVinculados.Count == 0)
                            {
                                var resultado = session.QueryOver<Atendimento>()
                                                       .WhereRestrictionOn(e => e.Protocolo)
                                                       .IsIn(atendimentosvinculadosOS.Select(y => y.Numero_Atendimento).ToList())
                                                       .List<Atendimento>();
                                atendimentosVinculados.AddRange(resultado);
                            }


                            if (objAtendimento != null)
                            {
                                atendimentosVinculados.Add(objAtendimento);
                            }

                            var statusViaveisNotificacao = new List<StatusExecucaoAniel?>
                            {
                                StatusExecucaoAniel.Fechada_Produtiva,
                                StatusExecucaoAniel.Fechada_Improdutiva
                            };

                            if (statusViaveisNotificacao.Contains(Doc_Prod.Status) && atendimentosVinculados.Count > 0 &&
                                atendimentosVinculados.Any(p => (p.Status_Atendimento == Status_Atendimento.Em_Atendimento || p.Status_Atendimento == Status_Atendimento.Aguardando) && p.Responsavel != null))
                            {
                                await api.EnviaNotificacoesServico(45, Doc_Prod.CodCt, Doc_Prod.Projeto, Doc_Prod.Num_Obra, Doc_Prod.Num_Doc, null);
                            }
                        }

                        if (objAtendimento != null)
                        {
                            await repoAtendimento.SaveOrUpdate(objAtendimento);
                        }

                        if (objClientePP != null)
                        {
                            await repoClientePlanoProduto.SaveOrUpdate(objClientePP);
                        }
                    }


                    Cobranca_Servico cobrancaEncontrada = documentoBusiness.BuscarCobrancaDocumento(Doc_Prod);

                    if (cobrancaEncontrada != null)
                    {
                        if (cobrancaEncontrada.Escolha_Cobranca == Escolha_Cobranca.Aguarda_Avaliacao)
                            Doc_Prod.Status_Cobranca = Status_CobrancaAniel.Aguardando_Avaliacao_Cobranca;
                        else
                            Doc_Prod.Status_Cobranca = Status_CobrancaAniel.Automatica;

                        await repoDocs.Update(Doc_Prod);
                    }

                    if (Doc_Prod.Cliente?.Cod_Cliente != null)
                    {
                        var pesquisa_disparo_auto_servico = new Conexao<Pesquisa_Satisfacao>(session).select(p => p.DisparoAuto == PesquisaSatisfacaoDisparoAuto.Envio_Automatico_Servico && p.Status == Status_Default.Ativo).Result;
                        if (pesquisa_disparo_auto_servico.Count != 0)
                        {
                            await new Pesquisa_SatisfacaoApiController().EnviarEmMassa(new Pesquisa_Satisfacao_PesquisaEmMassaModel
                            {
                                ids = new List<long> { Doc_Prod.Cliente.Cod_Cliente },
                                ids_pesquisa = pesquisa_disparo_auto_servico.Select(p => p.Id).ToList(),
                                usuario = User.Identity.GetUserId()
                            });
                        }
                    }

                    var atendimento = await repoAtendimento.selectFirst(p => p.Id == Doc_Prod.Id_Atendimento);
                    if (atendimento != null)
                    {
                        var notificacao = api.EnviaNotificacoesAtendimento(6, atendimento, docProd.Num_Obra);
                    }

                    retorno.Mensagem = "Operação bem sucedida";
                    retorno.IdObjeto = Doc_Prod.Num_Doc;
                }
            }
            return retorno;
        }

        [AllowAnonymous]
        [HttpPost()]
        [Route("api/DocumentoProducao/RotinaRouter")]
        public async Task<PostApiRetorno<long>> RotinaRouter(DocumentoProducao_GetDocumento docProd)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<long>
            {
                Ok = true
            };

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repoDocs = new Conexao<Documento_ProducaoAniel>(sessionT5))
                    using (var repoConfGerais = new Conexao<Configuracoes_Gerais>(session))
                    using (var repoAtendimento = new Conexao<Atendimento>(session))
                    using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                    using (var roteirizacaoApi = new Painel_RoteirizacaoApiController())
                    using (var repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5))
                    {
                        Configuracoes_Gerais Conf_Gerais = await repoConfGerais.selectFirst();
                        if (Conf_Gerais.UtilizaRouter)
                        {
                            var Doc_Prod = await repoDocProd.selectFirst(p => p.CodCt == docProd.CodCt && p.Projeto == docProd.Projeto && p.Num_Obra == docProd.Num_Obra && p.Num_Doc == docProd.Num_Doc);

                            if (Doc_Prod == null)
                            {
                                throw new Exception("Serviço não Encontrado");
                            }

                            roteirizacaoApi.HoraRoteirizacao = Doc_Prod.Data_Prod.Value.Add(Doc_Prod.Hora_Prod.Value.TimeOfDay);
                            await roteirizacaoApi.CompactarRota(new CompactarRotaModel
                            {
                                CodCt = docProd.CodCt,
                                Projeto = docProd.Projeto,
                                NumDoc = docProd.Num_Doc,
                                NumObra = docProd.Num_Obra,
                                SalvarRota = true
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.Ok = false;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/DocumentoProducao/ValidaProjetoByLocalidade")]
        public async Task<PostApiRetorno<Boolean>> ValidaProjeto(String Cod_Escritorio, String Cod_Area, String Cod_Localidade)
        {
            var retorno = new PostApiRetorno<Boolean>();
            try
            {

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<ProjetoAniel>(sessionT5))
                {
                    LocalidadeAniel localidade = await new Conexao<LocalidadeAniel>(sessionT5).selectFirst(p => p.Cod_Escritorio == Cod_Escritorio &&
                                                                                                                p.Cod_Area == Cod_Area &&
                                                                                                                p.Cod_Localidade == Convert.ToInt32(Cod_Localidade));


                    if (localidade == null)
                    {
                        throw new Exception("Localidade informada não foi encontrada");
                    }

                    ProjetoAniel projetoAniel = await repository.selectFirst(p => p.CodCt == localidade.CodCt && p.Codigo == localidade.Projeto);
                    if (projetoAniel == null)
                        retorno.Ok = false;
                    else
                    {
                        if (projetoAniel.Situacao != "A")
                            retorno.Ok = false;
                        else
                            retorno.Ok = true;
                    }
                }

            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }
            return retorno;

        }

        [HttpGet()]
        [Route("api/DocumentoProducao/ValidaProjeto")]
        public async Task<PostApiRetorno<Boolean>> ValidaProjeto(String CodCt, String Projeto)
        {
            var retorno = new PostApiRetorno<Boolean>();
            try
            {

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<ProjetoAniel>(sessionT5))
                {
                    ProjetoAniel projetoAniel = await repository.selectFirst(p => p.CodCt == CodCt && p.Codigo == Projeto);

                    if (projetoAniel == null)
                    {
                        throw new Exception("Contrato/Projeto informado não foi encontrada");
                    }


                    if (projetoAniel == null)
                        retorno.Ok = false;
                    else
                    {
                        if (projetoAniel.Situacao != "A")
                            retorno.Ok = false;
                        else
                            retorno.Ok = true;
                    }
                }

            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/Documento_Producaoapi/GetComboVincularONU")]
        public async Task<ObjCombo<String>> GetComboVincularONU(RequestObjComboPai<String, String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MATERIAL_TAREFA A ")
                        .AppendLine("INNER JOIN TB_MATERIAL B ")
                        .AppendLine("ON(A.CODMAT = B.CODMAT) ")
                        .AppendLine("INNER JOIN TB_CODIGO_COMPLEMENTAR C ")
                        .AppendLine("ON(A.CODCT = C.CODCT AND A.PROJETO = C.PROJETO AND A.CODMAT = C.CODMAT AND A.CODCPL = C.CODCPL) ")
                        .AppendLine("INNER JOIN TB_DOCUMENTO_PRODUCAO D ")
                        .AppendLine("ON(A.CODCT = D.CODCT AND A.PROJETO = D.PROJETO AND A.NUM_OBRA = D.NUM_OBRA AND A.NUM_DOC = D.NUM_DOC) ")
                        .AppendLine("INNER JOIN TB_EQUIPE E ")
                        .AppendLine("ON(A.EQUIPE = E.EQUIPE) ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        //condicoes.Add($"A.STATUS = 1");
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"C.LOCALIZACAO = @MAC");
                        parametros.Add("@MAC", request.idpai);
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.NUM_OBRA = @NUM_OBRA");
                        parametros.Add("@NUM_OBRA", request.id);
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.NUM_OBRA) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.CODMAT) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, A.EQUIPE, E.NOME AS NOME_EQUIPE, A.CODMAT, A.CODCPL, A.DIA, D.ID_CLIENTE, D.ID_PLANOPRODUTO, D.ID_CLIENTE_PLANOPRODUTOCONNECT ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NUM_OBRA)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new DocumentoProducao_VinculoONUComboResult
                                {
                                    Id = Convert.ToString(reader["NUM_OBRA"]),
                                    Text = $"{reader["NUM_OBRA"]}",
                                    CodCt = $"{reader["CODCT"]}",
                                    Projeto = $"{reader["PROJETO"]}",
                                    Num_Obra = $"{reader["NUM_OBRA"]}",
                                    Num_Doc = reader["NUM_DOC"] != DBNull.Value ? Convert.ToInt32(reader["NUM_DOC"]) : (Int32?)null,
                                    CodMat = $"{reader["CODMAT"]}",
                                    CodCpl = $"{reader["CODCPL"]}",
                                    Equipe = $"{reader["EQUIPE"]} - {reader["NOME_EQUIPE"]}",
                                    Id_Cliente = reader["ID_CLIENTE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE"]) : (Int64?)null,
                                    Id_PlanoProduto = reader["ID_PLANOPRODUTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_PLANOPRODUTO"]) : (Int64?)null,
                                    Id_Cliente_PlanoProduto = reader["ID_CLIENTE_PLANOPRODUTOCONNECT"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE_PLANOPRODUTOCONNECT"]) : (Int64?)null
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

        [HttpGet()]
        [Route("api/Documento_Producaoapi/OrdemServicoReincidente")]
        public async Task<GetApiRetorno<Boolean>> OrdemServicoReincidente(Int64 id_cliente, Int64 id_plano_produto, Int64 id_tipo_servico)
        {
            GetApiRetorno<Boolean> retorno = new GetApiRetorno<bool>();
            retorno.Objeto = false;
            using (ISession session = HibernateUtil.GetSession())
            using (var repositoryConf = new Conexao<Configuracoes_Gerais>(session))
            using (var repositoryClientePlanoProduto = new Conexao<Cliente>(session))
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repositoryDodProd = new Conexao<Documento_ProducaoAniel>(sessionT5))
            using (var repositoryTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
            using (var repositoryNatureza = new Conexao<Natureza_ProducaoAniel>(sessionT5))
            {
                try
                {

                    Int64? id_natureza = (await repositoryTipoServico.selectFirst(p => p.Cod_Tipo_Serv == id_tipo_servico)).Cod_Natureza;

                    if (id_natureza.HasValue)
                    {
                        Natureza_ProducaoAniel natureza = await repositoryNatureza.selectFirst(p => p.Id == id_natureza);


                        if (natureza?.QtdDias_GeraReincidencia.GetValueOrDefault(0) > 0)
                        {


                            Cliente cliente = await repositoryClientePlanoProduto.selectFirst(p => p.Id == id_cliente);
                            Cliente_PlanoProduto cliente_planoProduto = cliente.PlanosProdutos.ToList().Find(p => p.Plano?.Id == id_plano_produto || p.Produto?.Id == id_plano_produto);

                            DateTime data_param = DateTime.Now.Date.AddDays(-natureza.QtdDias_GeraReincidencia.GetValueOrDefault(0));
                            IList<Documento_ProducaoAniel> lista_reincidente = await repositoryDodProd.select(p => p.Id_Cliente_PlanoProdutoConnect == cliente_planoProduto.Id
                                                                                                                   && p.Dia >= data_param);

                            lista_reincidente = lista_reincidente.Where(p => p.Cliente?.Cod_Cliente == id_cliente).ToList();

                            List<(Documento_ProducaoAniel documento, Tipo_Servico_EquipeAniel tipo_servico)> lista_tipo_serv = lista_reincidente.Select(p => (p, repositoryTipoServico.selectFirst(y => y.Cod_Tipo_Serv == p.Cod_Tipo_Serv).Result)).ToList();
                            lista_reincidente = lista_tipo_serv.Where(p => p.tipo_servico?.Cod_Natureza == id_natureza).Select(p => p.documento).ToList();

                            if (lista_reincidente.Count() > 0)
                            {
                                retorno.Objeto = true;
                                retorno.Mensagem = $"Já foram criadas outras ordens de serviço recentemente para o mesmo cliente,produto e natureza de serviço. Ordem de serviço será considerada reincidente.";
                            }


                        }
                    }

                    retorno.Ok = true;

                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/Documento_Producaoapi/GetComboCodigoEncerramento")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboAvo<String, String, String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_CODIGO_ENCERRAMENTO A");



                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.ATIVO = 'S'");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"A.CODCT = '{request.idpai}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (String.IsNullOrEmpty(request.idavo) == false)
                    {
                        sqlFrom.AppendLine(" LEFT JOIN TB_VINC_ENCERRAMENTO_NATUREZA B ON(B.COD_ENCERRAMENTO = A.COD_ENCERRAMENTO AND B.CODCT = A.CODCT) ");
                        condicoes.Add($"( (COALESCE(A.VINCULA_NATUREZA, 'N') = 'N') OR (A.VINCULA_NATUREZA = 'S' AND B.COD_NATUREZA = '{request.idavo}'))");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.Append(" WHERE ");
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    sqlFrom.Append(" GROUP BY A.COD_ENCERRAMENTO,A.CODCT, A.DESCRICAO");

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(*) AS CO FROM (");
                        sql.AppendLine("SELECT A.COD_ENCERRAMENTO,A.CODCT, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(")");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.COD_ENCERRAMENTO,A.CODCT, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new OjCombCodigoEncerramento
                                {
                                    Id = Convert.ToString(reader["COD_ENCERRAMENTO"]),
                                    Text = $"{Convert.ToString(reader["COD_ENCERRAMENTO"])} - {Convert.ToString(reader["DESCRICAO"])}",
                                    CodCt = Convert.ToString(reader["CODCT"])
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
        [Route("api/Documento_Producaoapi/GetComboNumDoc")]
        public async Task<ObjCombo<String>> GetComboNumDoc(RequestObjComboAvo<String, String, String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_DOCUMENTO_PRODUCAO A");

                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.NUM_OBRA = {request.id}");
                    }

                    if (!String.IsNullOrEmpty(request.idavo))
                    {
                        condicoes.Add($"A.CODCT = '{request.idavo}'");
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"A.PROJETO = '{request.idpai}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( A.NUM_OBRA LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.Append(" WHERE ");
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT (DISTINCT A.NUM_OBRA) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.NUM_OBRA ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new OjCombCodigoEncerramento
                                {
                                    Id = Convert.ToString(reader["NUM_OBRA"]),
                                    Text = $"{Convert.ToString(reader["NUM_OBRA"])}",
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
        [Route("api/DocumentoProducao/ServicosImprodutivos")]
        public async Task<DataTableResponse<dynamic>> ServicosImprodutivos(ServicosImprodutivos_DataTableRequest request)
        {
            var retorno = new List<dynamic>();


            if (String.IsNullOrEmpty(request.Data_Inicio) || String.IsNullOrEmpty(request.Data_Fim))
            {
                throw new Exception("nenhuma data informada");
            }

            if (String.IsNullOrEmpty(request.Tipo_Data))
            {
                throw new Exception("nenhum tipo de data informado");
            }


            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {

                using (var com = sessionT5.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder(@"SELECT A.NUM_OBRA,A.DATA_MAXIMA + COALESCE(A.HORA_MAXIMA, CAST('00:00' AS TIME)) AS DATAHORA_AGENDAMENTO, B.DESCRICAO AS TIPO_SERVICO, C.NOME AS NOME_EQUIPE, D.NOME AS SUPERVISOR, E.CNPJ,E.RAZAO_SOCIAL AS CLIENTE,COALESCE(G.DESCRICAO,A.DESCRICAO_PLANO_ASSINANTE) AS PLANO_PRODUTO,
                                                            A.ENDERECO_TIPO,A.ENDERECO_BAIRRO,ENDERECO_UF, A.DATA_PROD + COALESCE(A.HORA_PROD, CAST('00:00' AS TIME)) AS DATAHORA_PROD, A.COD_ENCERRAMENTO,F.DESCRICAO AS NOME_ENCERRAMENTO,A.STATUS_COBRANCA

                                                            FROM TB_DOCUMENTO_PRODUCAO A

                                                            LEFT JOIN TB_TIPO_SERVICO_EQUIPE B 
                                                            ON(A.COD_TIPO_SERV = B.COD_TIPO_SERV) 

                                                            LEFT JOIN TB_EQUIPE C 
                                                            ON(A.EQUIPE = C.EQUIPE)

                                                            LEFT JOIN TB_FUNCIONARIO D 
                                                            ON(D.NUM_FRE = C.COD_SUPERV)

                                                            LEFT JOIN TB_CLIENTE E 
                                                            ON(A.ID_CLIENTE = E.COD_CLIENTE)

                                                            LEFT JOIN TB_CODIGO_ENCERRAMENTO F 
                                                            ON(A.COD_ENCERRAMENTO = F.COD_ENCERRAMENTO AND A.CODCT = F.CODCT)

                                                            LEFT JOIN TB_PLANO_PRODUTO G
                                                            ON(G.ID = A.ID_PLANOPRODUTO)

                                                            WHERE F.STATUS_EXECUCAO = 1

                    ");

                    if (!String.IsNullOrEmpty(request.Data_Inicio) && !String.IsNullOrEmpty(request.Data_Fim))
                    {
                        if (request.Tipo_Data == "agendamento")
                            sql.AppendLine("AND (A.DATA_MAXIMA BETWEEN @DATA_INICIO AND @DATA_FIM)");
                        else
                            sql.AppendLine("AND (A.DATA_PROD BETWEEN @DATA_INICIO AND @DATA_FIM)");

                        parametros.Add("@DATA_INICIO", Convert.ToDateTime(request.Data_Inicio).ToString("dd/MM/yyyy 00:00"));
                        parametros.Add("@DATA_FIM", Convert.ToDateTime(request.Data_Fim).ToString("dd/MM/yyyy 23:59"));
                    }

                    if (!String.IsNullOrEmpty(request.Equipe))
                    {
                        sql.AppendLine("AND (A.EQUIPE = @EQUIPE)");
                        parametros.Add("@EQUIPE", request.Equipe);
                    }
                    if (!String.IsNullOrEmpty(request.Supervisor))
                    {
                        sql.AppendLine("AND (D.NUM_FRE = @SUPERVISOR)");
                        parametros.Add("@SUPERVISOR", request.Supervisor);
                    }
                    if (request.Id_Cliente.HasValue)
                    {
                        sql.AppendLine("AND (A.ID_CLIENTE = @CLIENTE)");
                        parametros.Add("@CLIENTE", request.Id_Cliente);
                    }


                    if (!String.IsNullOrEmpty(request.Codigo_Encerramento) && !String.IsNullOrEmpty(request.CodCt_Encerramento))
                    {
                        sql.AppendLine("AND (F.COD_ENCERRAMENTO = @COD_ENCERRAMENTO AND F.CODCT = @CODCT_ENCERRAMENTO)");
                        parametros.Add("@COD_ENCERRAMENTO", request.Codigo_Encerramento);
                        parametros.Add("@CODCT_ENCERRAMENTO", request.CodCt_Encerramento);
                    }

                    if (request.Cod_Tipo_Serv.HasValue)
                    {
                        sql.AppendLine("AND (A.COD_TIPO_SERV = @COD_TIPO_SERV)");
                        parametros.Add("@COD_TIPO_SERV", request.Cod_Tipo_Serv);
                    }

                    if (request.Cod_Cliente.HasValue)
                    {
                        sql.AppendLine("AND (A.ID_CLIENTE = @ID_CLIENTE)");
                        parametros.Add("@ID_CLIENTE", request.Cod_Cliente);
                    }
                    if (request.Id_Categoria_Cliente.HasValue)
                    {
                        sql.AppendLine("AND (E.ID_CATEGORIA = @ID_CATEGORIA)");
                        parametros.Add("@ID_CATEGORIA", request.Id_Categoria_Cliente);
                    }
                    if (!String.IsNullOrEmpty(request.UF))
                    {
                        sql.AppendLine("AND (A.ENDERECO_UF = @UF)");
                        parametros.Add("@UF", request.UF);
                    }
                    if (!String.IsNullOrEmpty(request.Cidade))
                    {
                        sql.AppendLine("AND (UPPER(A.ENDERECO_TIPO) = @CIDADE)");
                        parametros.Add("@CIDADE", request.Cidade.ToUpperInvariant());
                    }
                    if (request.Id_PlanoProduto.HasValue)
                    {
                        sql.AppendLine("AND (G.ID = @PLANO_PRODUTO)");
                        parametros.Add("@PLANO_PRODUTO", request.Id_PlanoProduto);
                    }


                    sql.AppendLine("ORDER BY F.DESCRICAO,DATAHORA_PROD DESC");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new
                            {
                                numero_obra = Convert.ToString(reader["NUM_OBRA"]).Split('/')[0],
                                datahora_agendamento = reader["DATAHORA_AGENDAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_AGENDAMENTO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                tipo_servico = Convert.ToString(reader["TIPO_SERVICO"]),
                                equipe = Convert.ToString(reader["NOME_EQUIPE"]),
                                supervisor = Convert.ToString(reader["SUPERVISOR"]),
                                cpf_cliente = Convert.ToString(reader["CNPJ"]),
                                nome_cliente = Convert.ToString(reader["CLIENTE"]),
                                plano_produto = Convert.ToString(reader["PLANO_PRODUTO"]),
                                uf = Convert.ToString(reader["ENDERECO_UF"]),
                                cidade = Convert.ToString(reader["ENDERECO_TIPO"]),
                                bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]),
                                data_baixa = reader["DATAHORA_PROD"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_PROD"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                encerramento = Convert.ToString(reader["NOME_ENCERRAMENTO"]),
                                Status_Cobranca = reader["STATUS_COBRANCA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_CobrancaAniel>(Convert.ToString(reader["STATUS_COBRANCA"])) : ""
                            });
                        }
                    }


                }


            }

            return new DataTableResponse<dynamic>
            {
                data = retorno,
                draw = request.draw,
                recordsFiltered = retorno.Count()
            };

        }

        [HttpPost()]
        [Route("api/DocumentoProducao/RelatorioAcompanhamentoServico")]
        public async Task<DataTableResponse<dynamic>> AcompanhamentoServico(ServicosImprodutivos_DataTableRequest request)
        {
            var retorno = new List<dynamic>();


            if (String.IsNullOrEmpty(request.Data_Inicio) || String.IsNullOrEmpty(request.Data_Fim))
            {
                throw new Exception("nenhuma data informada");
            }

            if (String.IsNullOrEmpty(request.Tipo_Data))
            {
                throw new Exception("nenhum tipo de data informado");
            }

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {

                using (var com = sessionT5.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder(@"SELECT A.CODCT,A.PROJETO,A.NUM_DOC,A.NUM_OBRA, B.DESCRICAO AS TIPO_SERVICO, C.NOME AS NOME_EQUIPE,A.DATA_MAXIMA + COALESCE(A.HORA_MAXIMA, CAST('00:00' AS TIME)) AS DATAHORA_AGENDAMENTO, 
                                                            A.DATA_PROD + A.HORA_PROD AS DATAHORA_PROD,A.DATAHORA_INI_ATEND,A.DATAHORA_FIM_ATEND,A.DATAHORA_DESLOCAMENTO,
                                                            A.DATA_INDICADOR + A.HORA_INDICADOR AS DATAHORA_INDICADOR, A.DIA + A.HORA AS DATAHORA_CRIACAO

                                                            FROM TB_DOCUMENTO_PRODUCAO A

                                                            LEFT JOIN TB_TIPO_SERVICO_EQUIPE B 
                                                            ON(A.COD_TIPO_SERV = B.COD_TIPO_SERV) 

                                                            LEFT JOIN TB_EQUIPE C 
                                                            ON(A.EQUIPE = C.EQUIPE)

                                                            LEFT JOIN TB_FUNCIONARIO D 
                                                            ON(D.NUM_FRE = C.COD_SUPERV)

                                                            LEFT JOIN TB_CLIENTE E 
                                                            ON(A.ID_CLIENTE = COD_CLIENTE)

                                                            LEFT JOIN TB_CODIGO_ENCERRAMENTO F 
                                                            ON(A.COD_ENCERRAMENTO = F.COD_ENCERRAMENTO AND A.CODCT = F.CODCT)

                                                            LEFT JOIN TB_PLANO_PRODUTO G
                                                            ON(G.ID = A.ID_PLANOPRODUTO)


                    ");



                    if (!String.IsNullOrEmpty(request.Data_Inicio) && !String.IsNullOrEmpty(request.Data_Fim))
                    {
                        if (request.Tipo_Data == "agendamento")
                            sql.AppendLine("WHERE (A.DATA_MAXIMA BETWEEN @DATA_INICIO AND @DATA_FIM)");
                        else
                            sql.AppendLine("WHERE (A.DATA_PROD BETWEEN @DATA_INICIO AND @DATA_FIM)");

                        parametros.Add("@DATA_INICIO", Convert.ToDateTime(request.Data_Inicio).ToString("dd/MM/yyyy 00:00"));
                        parametros.Add("@DATA_FIM", Convert.ToDateTime(request.Data_Fim).ToString("dd/MM/yyyy 23:59"));
                    }

                    if (!String.IsNullOrEmpty(request.Equipe))
                    {
                        sql.AppendLine("AND (A.EQUIPE = @EQUIPE)");
                        parametros.Add("@EQUIPE", request.Equipe);
                    }
                    if (!String.IsNullOrEmpty(request.Supervisor))
                    {
                        sql.AppendLine("AND (D.NUM_FRE = @SUPERVISOR)");
                        parametros.Add("@SUPERVISOR", request.Supervisor);
                    }

                    if (!String.IsNullOrEmpty(request.Codigo_Encerramento) && !String.IsNullOrEmpty(request.CodCt_Encerramento))
                    {
                        sql.AppendLine("AND (F.COD_ENCERRAMENTO = @COD_ENCERRAMENTO AND F.CODCT = @CODCT_ENCERRAMENTO)");
                        parametros.Add("@COD_ENCERRAMENTO", request.Codigo_Encerramento);
                        parametros.Add("@CODCT_ENCERRAMENTO", request.CodCt_Encerramento);
                    }

                    if (request.Cod_Tipo_Serv.HasValue)
                    {
                        sql.AppendLine("AND (A.COD_TIPO_SERV = @COD_TIPO_SERV)");
                        parametros.Add("@COD_TIPO_SERV", request.Cod_Tipo_Serv);
                    }

                    if (request.Cod_Cliente.HasValue)
                    {
                        sql.AppendLine("AND (A.ID_CLIENTE = @ID_CLIENTE)");
                        parametros.Add("@ID_CLIENTE", request.Cod_Cliente);
                    }
                    if (request.Id_Categoria_Cliente.HasValue)
                    {
                        sql.AppendLine("AND (E.ID_CATEGORIA = @ID_CATEGORIA)");
                        parametros.Add("@ID_CATEGORIA", request.Id_Categoria_Cliente);
                    }
                    if (!String.IsNullOrEmpty(request.UF))
                    {
                        sql.AppendLine("AND (A.ENDERECO_UF = @UF)");
                        parametros.Add("@UF", request.UF);
                    }
                    if (!String.IsNullOrEmpty(request.Cidade))
                    {
                        sql.AppendLine("AND (UPPER(A.ENDERECO_TIPO) = @CIDADE)");
                        parametros.Add("@CIDADE", request.Cidade.ToUpperInvariant());
                    }
                    if (request.Id_PlanoProduto.HasValue)
                    {
                        sql.AppendLine("AND (G.ID = @PLANO_PRODUTO)");
                        parametros.Add("@PLANO_PRODUTO", request.Id_PlanoProduto);
                    }


                    sql.AppendLine("ORDER BY A.NUM_OBRA");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {

                            DateTime? datahora_indicador = reader["DATAHORA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INDICADOR"]) : (DateTime?)null;
                            DateTime? datahora_ini_atend = reader["DATAHORA_INI_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INI_ATEND"]) : (DateTime?)null;
                            DateTime? datahora_fim_atend = reader["DATAHORA_FIM_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM_ATEND"]) : (DateTime?)null;
                            DateTime? datahora_encerramento = reader["DATAHORA_PROD"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_PROD"]) : (DateTime?)null;
                            DateTime? datahora_criacao = reader["DATAHORA_CRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_CRIACAO"]) : (DateTime?)null;
                            DateTime? datahora_agendamento = reader["DATAHORA_AGENDAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_AGENDAMENTO"]) : (DateTime?)null;


                            var sla = "Sem Indicador de Sla";
                            if (datahora_indicador.HasValue)
                            {
                                sla = GetDescricaoSla(datahora_indicador, datahora_criacao, datahora_encerramento);
                            }

                            if (!String.IsNullOrEmpty(request.Tipo_SLA))
                            {
                                if (sla == RsCadastros.DENTRO_PRAZO && request.Tipo_SLA != "dentro_prazo")
                                    continue;
                                if (sla == RsCadastros.ATENCAO && request.Tipo_SLA != "atencao")
                                    continue;
                                if (sla == RsCadastros.VENCIMENTO_PROXIMO && request.Tipo_SLA != "vencimento_proximo")
                                    continue;
                                if (sla == RsCadastros.RISCO_PERDA_PRAZO && request.Tipo_SLA != "risco_perda_prazo")
                                    continue;
                                if (sla == RsCadastros.PRAZO_VENCIDO && request.Tipo_SLA != "prazo_vencido")
                                    continue;
                            }

                            string codct = Convert.ToString(reader["CODCT"]);
                            string projeto = Convert.ToString(reader["PROJETO"]);
                            string num_obra = Convert.ToString(reader["NUM_OBRA"]);
                            int num_doc = Convert.ToInt32(reader["NUM_DOC"]);

                            var temposStatus = GetTempoStatus(codct, projeto, num_obra, num_doc, datahora_encerramento);

                            var TotalTempoAtendimento = temposStatus.tempoAtendimento + temposStatus.tempoParalisacao;

                            retorno.Add(new
                            {
                                numero_obra = Convert.ToString(reader["NUM_OBRA"]),
                                datahora_agendamento = datahora_agendamento.HasValue ? datahora_agendamento?.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                tipo_servico = Convert.ToString(reader["TIPO_SERVICO"]),
                                equipe = Convert.ToString(reader["NOME_EQUIPE"]),
                                status_sla = sla,
                                deslocamento = reader["DATAHORA_DESLOCAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_DESLOCAMENTO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                inicio_atendimento = datahora_ini_atend.HasValue ? datahora_ini_atend?.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                finalizacao_atendimento = datahora_fim_atend.HasValue ? datahora_fim_atend?.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                tempo_deslocamento = temposStatus.tempoDeslocamento.ToString(@"hh\:mm"),
                                tempo_execucao = temposStatus.tempoAtendimento.ToString(@"hh\:mm"),
                                tempo_paralisacao = temposStatus.tempoParalisacao.ToString(@"hh\:mm"),
                                tempo_total_atendimento = TotalTempoAtendimento.ToString(@"hh\:mm"),
                                km_percorrido = String.Empty,
                                data_baixa = datahora_encerramento.HasValue ? datahora_encerramento?.ToString("dd/MM/yyyy HH:mm") : String.Empty
                            });
                        }
                    }
                }


            }

            return new DataTableResponse<dynamic>
            {
                data = retorno,
                draw = request.draw,
                recordsFiltered = retorno.Count()
            };

        }

        [HttpGet()]
        [Route("api/DocumentoProducao/GetOrdemServico")]
        public async Task<Boolean> GetOrdemServico(Int64 IdAtendimento)
        {
            var retorno = false;
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
                {
                    var lista = await repository.select(x => x.Id_Atendimento == IdAtendimento && x.Status != StatusExecucaoAniel.Cancelado && x.Status != StatusExecucaoAniel.Fechada_Produtiva);
                    if (lista.Count > 0)
                    {
                        retorno = true;
                    }
                }
            }
            catch (Exception)
            {
                retorno = false;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/DocumentoProducao/RelatorioProducaoServicoAnalitico")]
        public async Task<DataTableResponse<dynamic>> ProducaoServicoAnalitico(Filtros_ProducaoServico_DataTableRequest request)
        {
            var retorno = new List<dynamic>();


            if (String.IsNullOrEmpty(request.Data_Inicio) || String.IsNullOrEmpty(request.Data_Fim))
            {
                throw new Exception("nenhuma data informada");
            }

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {

                using (var com = sessionT5.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder
                    (@"
                        SELECT A.RCODCT, A.RPROJETO, A.REQUIPE, A.RNOME, A.RDATA, A.RCONTRATO, A.ROS, A.RMANIFESTO, A.RSENHA_BX, A.RBAIRRO,
                               A.RTIPO_IMOVEL, A.RCOD_TIPO_SERV, A.RDESC_TIPO_SERV, A.RSEGMENTO, A.RUNIDADE,
                               A.RCOD_ENCERRAMENTO, A.RDESCRICAO_ENCERRAMENTO, A.RDEVOLVIDO,  A.RCERTIFICADO,  A.RVEICULO,  A.RCODIGO,  A.RDESC_SERV,
                               A.RCOD_PRECO, A.RPONTO, A.RACUM_PONTO, A.RVALOR_PROD,A.RVALOR_UNIDADE_CUSTO, A.RDESC_FAIXA, A.RINI_FAIXA, A.RFIM_FAIXA, A.RCOEFICIENTE, A.RSUBTOTAL
                        FROM SP_REL_PRODUCAO_FAIXA_CONNECT(
                          @PCODCT,
                          @PPROJETO,
                          @PEQUIPE,
                          @PCOD_SUPERV,
                          @PCOD_TIPO_SERV,
                          @PCOD_NATUREZA,
                          @PDATAINI,
                          @PDATAFIM,
                          @PTIPODATA,
                          @PCERTIFICADO,
                          @PDEVOLVIDO,
                          @PDATACRIACAO,
                          @PQUANT_PREV,
                          @PBAIXAMOBILE) A
                        ORDER BY A.REQUIPE");

                    //Filtros
                    parametros.Add("@PCODCT", !String.IsNullOrEmpty(request.Contrato) ? request.Contrato : "-");
                    parametros.Add("@PPROJETO", !String.IsNullOrEmpty(request.Projeto) ? request.Projeto : "-");
                    parametros.Add("@PEQUIPE", !String.IsNullOrEmpty(request.Equipe) ? request.Equipe : "-");
                    parametros.Add("@PCOD_SUPERV", !String.IsNullOrEmpty(request.Supervisor) ? request.Supervisor : "-");
                    parametros.Add("@PCOD_TIPO_SERV", !String.IsNullOrEmpty(request.Tipo_Servico) ? request.Tipo_Servico : "0");
                    parametros.Add("@PCOD_NATUREZA", !String.IsNullOrEmpty(request.Natureza_Servico) ? request.Natureza_Servico : "0");
                    parametros.Add("@PDATAINI", Convert.ToDateTime(request.Data_Inicio));
                    parametros.Add("@PDATAFIM", Convert.ToDateTime(request.Data_Fim));
                    parametros.Add("@PTIPODATA", request.TipoData);
                    parametros.Add("@PCERTIFICADO", "N");
                    parametros.Add("@PDEVOLVIDO", request.Servico_Doc_Fisico_Devolvido ? "S" : "N");
                    parametros.Add("@PDATACRIACAO", "N");
                    parametros.Add("@PQUANT_PREV", request.Producao_Prevista ? "S" : "N");
                    parametros.Add("@PBAIXAMOBILE", request.Servico_Baixado_Mobile ? "S" : "N");


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {

                            var vqtd_produzida = Convert.ToString(reader["RPONTO"]);
                            var vvalor_prod = reader["RVALOR_PROD"] != DBNull.Value ? Convert.ToDouble(reader["RVALOR_PROD"]) : 0;
                            var vvalor_classe = reader["RVALOR_UNIDADE_CUSTO"] != DBNull.Value ? Convert.ToDouble(reader["RVALOR_UNIDADE_CUSTO"]) : 0;


                            retorno.Add(new
                            {
                                contrato = Convert.ToString(reader["RCODCT"]),
                                projeto = Convert.ToString(reader["RPROJETO"]),
                                tecnico = Convert.ToString(reader["REQUIPE"]),
                                nome = Convert.ToString(reader["RNOME"]),
                                data_servico = reader["RDATA"] != DBNull.Value ? Convert.ToDateTime(reader["RDATA"]).ToString("dd/MM/yyyy") : String.Empty,
                                id_cliente = Convert.ToString(reader["RCONTRATO"]),
                                os = Convert.ToString(reader["ROS"]).Split('/')[0],
                                cod_tipo_servico = Convert.ToString(reader["RCOD_TIPO_SERV"]),
                                desc_tipo_servico = Convert.ToString(reader["RDESC_TIPO_SERV"]),
                                tipo_imovel = Convert.ToString(reader["RTIPO_IMOVEL"]),
                                segmento = Convert.ToString(reader["RSEGMENTO"]),
                                cod_encerramento = Convert.ToString(reader["RCOD_ENCERRAMENTO"]),
                                desc_encerramento = Convert.ToString(reader["RDESCRICAO_ENCERRAMENTO"]),
                                doc_devolvido = Convert.ToString(reader["RDEVOLVIDO"]) == "N" ? RsBase.NAO : RsBase.SIM,
                                cod_servico = Convert.ToString(reader["RCODIGO"]),
                                desc_servico = Convert.ToString(reader["RDESC_SERV"]),
                                unidade = Convert.ToString(reader["RUNIDADE"]),
                                qtd_produzida = vqtd_produzida,
                                producao_acumulada = Convert.ToString(reader["RACUM_PONTO"]),
                                cod_preco = Convert.ToString(reader["RCOD_PRECO"]),
                                valor_classe = vvalor_classe.ToString("N2"),
                                valor_producao = vvalor_prod.ToString("N2"),
                                faixa_aplicada = Convert.ToString(reader["RDESC_FAIXA"]),
                                inicio_faixa = reader["RINI_FAIXA"] != DBNull.Value ? Convert.ToDouble(reader["RINI_FAIXA"]).ToString("N3") : "0,00",
                                final_faixa = reader["RFIM_FAIXA"] != DBNull.Value ? Convert.ToDouble(reader["RFIM_FAIXA"]).ToString("N3") : "0,00",
                                coeficiente_faixa = reader["RCOEFICIENTE"] != DBNull.Value ? Convert.ToDouble(reader["RCOEFICIENTE"]).ToString("N2") : "0,00",
                                valor_final_producao = String.IsNullOrEmpty(Convert.ToString(reader["RDESC_FAIXA"])) == false ? (reader["RSUBTOTAL"] != DBNull.Value ? Convert.ToDouble(reader["RSUBTOTAL"]).ToString("N2") : "0,00") : (vvalor_prod.ToString("N2"))
                            });
                        }
                    }
                }


            }

            return new DataTableResponse<dynamic>
            {
                data = retorno,
                draw = request.draw,
                recordsFiltered = retorno.Count()
            };

        }

        [HttpPost()]
        [Route("api/DocumentoProducao/RelatorioProducaoServicoSintetico")]
        public async Task<DataTableResponse<Relatorio_Producao_SinteticoModel>> ProducaoServicoSintetico(Filtros_ProducaoServico_DataTableRequest request)
        {

            List<Relatorio_Producao_SinteticoModel> retorno = new List<Relatorio_Producao_SinteticoModel>();
            using (ISession session = HibernateUtil.GetSessionT5())
            {

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        StringBuilder sql = new StringBuilder();
                        StringBuilder sqlWhere = new StringBuilder();

                        //Filtros
                        parametros.Add("@PCODCT", !String.IsNullOrEmpty(request.Contrato) ? request.Contrato : "-");
                        parametros.Add("@PPROJETO", !String.IsNullOrEmpty(request.Projeto) ? request.Projeto : "-");
                        parametros.Add("@PEQUIPE", !String.IsNullOrEmpty(request.Equipe) ? request.Equipe : "-");
                        parametros.Add("@PCOD_SUPERV", !String.IsNullOrEmpty(request.Supervisor) ? request.Supervisor : "-");
                        parametros.Add("@PCOD_TIPO_SERV", !String.IsNullOrEmpty(request.Tipo_Servico) ? request.Tipo_Servico : "0");
                        parametros.Add("@PCOD_NATUREZA", !String.IsNullOrEmpty(request.Natureza_Servico) ? request.Natureza_Servico : "0");
                        parametros.Add("@PDATAINI", Convert.ToDateTime(request.Data_Inicio));
                        parametros.Add("@PDATAFIM", Convert.ToDateTime(request.Data_Fim));
                        parametros.Add("@PTIPODATA", request.TipoData);
                        parametros.Add("@PCERTIFICADO", "N");
                        parametros.Add("@PDEVOLVIDO", request.Servico_Doc_Fisico_Devolvido ? "S" : "N");
                        parametros.Add("@PDATACRIACAO", "N");
                        parametros.Add("@PQUANT_PREV", request.Producao_Prevista ? "S" : "N");
                        parametros.Add("@PBAIXAMOBILE", request.Servico_Baixado_Mobile ? "S" : "N");

                        var groupby = String.Empty;
                        var select_group = String.Empty;
                        switch (request.Agrupamento)
                        {
                            case "contrato_aniel":

                                groupby = "A.RCODCT";
                                select_group = "A.RCODCT";
                                break;
                            case "projeto":
                                groupby = "A.RPROJETO";
                                select_group = "A.RPROJETO";
                                break;
                            case "supervisor":
                                groupby = "A.RSUPERVISOR";
                                select_group = "A.RSUPERVISOR";
                                break;
                            case "desc_tipo_serv":
                                groupby = "A.RDESC_TIPO_SERV";
                                select_group = "A.RDESC_TIPO_SERV";
                                break;
                            case "mes_ano":
                                groupby = "EXTRACT(MONTH FROM A.RDATA), EXTRACT(YEAR FROM A.RDATA)";
                                select_group = "EXTRACT(MONTH FROM A.RDATA) AS MES, EXTRACT(YEAR FROM A.RDATA) AS ANO";
                                break;
                        }

                        sql.AppendLine($@"SELECT A.REQUIPE, A.RNOME, A.RDESC_FAIXA,{select_group},
                                        MAX(A.RACUM_PONTO) AS PRODUCAO_ACUMULADA,
                                        SUM(COALESCE(A.RVALOR_PROD,0)) AS VALOR_PRODUCAO, 
                                        SUM(COALESCE(A.RSUBTOTAL,0)) AS VALOR_FINAL_PROD
                                          FROM SP_REL_PRODUCAO_FAIXA_CONNECT(
                                              @PCODCT,
                                              @PPROJETO,
                                              @PEQUIPE,
                                              @PCOD_SUPERV,
                                              @PCOD_TIPO_SERV,
                                              @PCOD_NATUREZA,
                                              @PDATAINI,
                                              @PDATAFIM,
                                              @PTIPODATA,
                                              @PCERTIFICADO,
                                              @PDEVOLVIDO,
                                              @PDATACRIACAO,
                                              @PQUANT_PREV,
                                              @PBAIXAMOBILE) A
                                        GROUP BY REQUIPE, RNOME, RDESC_FAIXA,{groupby}
                                        ORDER BY {groupby}"
                        );

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new Relatorio_Producao_SinteticoModel
                                {

                                    equipe = Convert.ToString(reader["REQUIPE"]),
                                    nome_equipe = Convert.ToString(reader["RNOME"]),
                                    producao_acumulada = Convert.ToString(reader["PRODUCAO_ACUMULADA"]),
                                    valor_producao = reader["VALOR_PRODUCAO"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_PRODUCAO"]).ToString("N2") : "0,00",
                                    faixa_aplicada = Convert.ToString(reader["RDESC_FAIXA"]),

                                };

                                if (String.IsNullOrEmpty(Convert.ToString(reader["RDESC_FAIXA"])))
                                {
                                    obj.valor_final_producao = reader["VALOR_PRODUCAO"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_PRODUCAO"]).ToString("N2") : "0,00";
                                }
                                else
                                {
                                    obj.valor_final_producao = reader["VALOR_FINAL_PROD"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_FINAL_PROD"]).ToString("N2") : "0,00";
                                }

                                switch (request.Agrupamento)
                                {
                                    case "contrato_aniel":
                                        obj.contrato_aniel = Convert.ToString(reader["RCODCT"]);
                                        break;
                                    case "projeto":
                                        obj.projeto = Convert.ToString(reader["RPROJETO"]);
                                        break;
                                    case "supervisor":
                                        obj.supervisor = Convert.ToString(reader["RSUPERVISOR"]);
                                        break;
                                    case "desc_tipo_serv":
                                        obj.desc_tipo_serv = Convert.ToString(reader["RDESC_TIPO_SERV"]);
                                        break;
                                    case "mes_ano":

                                        var mes = reader["MES"] != DBNull.Value ? Convert.ToInt16(reader["MES"]) : (Int16?)null;
                                        var ano = reader["ANO"] != DBNull.Value ? Convert.ToInt16(reader["ANO"]) : (Int16?)null;

                                        if (mes.HasValue && ano.HasValue)
                                        {
                                            obj.mes_ano = $"{HtmlEnumExtensions.GetEnumDescription<Mes_Simplicado>(mes.ToString())}/{ano}";
                                        }
                                        break;
                                }

                                retorno.Add(obj);
                            }
                        }
                    }


                }

            }

            return new DataTableResponse<Relatorio_Producao_SinteticoModel>
            {
                data = retorno,
                draw = request.draw,
                recordsFiltered = retorno.Count()
            };
        }

        [HttpPost()]
        [Route("api/DocumentoProducao/GetMotivoParalizacao")]
        public async Task<List<MotivoParalizacaoResponse>> GetMotivoParalizacao(MotivoParalizacaoRequest request)
        {
            using ISession session = HibernateUtil.GetSessionT5();
            using ConexaoMongo conexaoMongo = new ConexaoMongo();
            using var repository = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo);

            var response = new List<MotivoParalizacaoResponse>();

            var collection = repository.GetCollection();
            var documentoProducao = await collection.Aggregate()
                                            .Match(p => p.codCt == request.CodCt
                                                        && p.numObra == request.NumObra
                                                        && p.contrato == request.Contrato
                                                        && p.projeto == request.Projeto
                                                        )
                                            .FirstOrDefaultAsync();

            if (documentoProducao == null)
            {
                return response;
            }

            var statusDocumento = documentoProducao.status.Where(p => p.statusExecucao == 10).ToList();

            if (statusDocumento == null)
            {
                return response;
            }

            foreach (var item in statusDocumento)
            {
                var statusResponse = new MotivoParalizacaoResponse()
                {
                    Data = item.dataHoraServidor.Date.ToString("dd/MM/yyyy"),
                    Hora = item.dataHoraServidor.ToString(@"HH:mm"),
                    MotivoParalizacao = String.IsNullOrEmpty(item.justificativaPausaMongo) ? await GetMotivoParalizacaoDocumentoProducao(request) : item.justificativaPausaMongo,
                    NumObra = request.NumObra,
                    TecnicoOs = request.Tecnico,
                    Tecnico = item.nome,
                };

                response.Add(statusResponse);
            }

            return response;
        }

        #region Mobile
        [AllowAnonymous]
        [HttpGet()]
        [Route("api/DocumentoProducao/getNumObra")]
        public String getNumObra()
        {
            String num_Obra = "";
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
            using (var repositoryConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                var config = repositoryConfig.selectFirst().Result;
                IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                var documentoBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);

                num_Obra = documentoBusiness.GetNum_Obra(config);
            }

            return num_Obra;
        }

        [HttpGet()]
        [Route("api/DocumentoProducao/GetDadosOS")]
        public async Task<GetApiRetorno<List<DadosOSMobile>>> GetDadosOS(Int64? IdVenda)
        {
            try
            {
                if (!IdVenda.HasValue)
                {
                    throw new Exception("ID da venda não informado");
                }

                GetApiRetorno<List<DadosOSMobile>> retorno = new GetApiRetorno<List<DadosOSMobile>>();

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoVenda = new Conexao<Venda>(session))
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repoTipoServ = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
                using (var repoTipo_Imovel = new Conexao<Tipo_Imovel>(session))
                {
                    Venda venda = null;
                    Cliente cliente = null;

                    venda = await repoVenda.selectFirst(p => p.Id == IdVenda.Value);
                    cliente = await repoCliente.selectFirst(p => p.Id == venda.Id_Cliente.Value);

                    var listaPlanosProdutos = cliente.PlanosProdutos.Where(p => p.Id_Venda.HasValue && p.Id_Venda.Value == venda.Id && (p.Plano != null ? p.Plano.StatusInicial == Status_PlanoProduto.Aguardando_Criacao_OS : p.Produto.Status_Inicial == Status_PlanoProduto.Aguardando_Criacao_OS));
                    var dadosOS = new List<DadosOSMobile>();

                    foreach (var planoProduto in listaPlanosProdutos)
                    {
                        Documento_ProducaoAniel documento = await repository.selectFirst(p => p.Id_Cliente_PlanoProdutoConnect == planoProduto.Id && p.Cliente.Cod_Cliente == planoProduto.Cliente.Id);

                        var desc_Planoproduto = planoProduto.Plano != null ? planoProduto.Plano.Descricao : planoProduto.Produto.Descricao;
                        var tipo_Imovel = await repoTipo_Imovel.selectFirst(p => p.Cod_Tipo_Imovel == (documento != null ? documento.Cod_Tipo_Imovel : -1));
                        var tipo_Servico = await repoTipoServ.selectFirst(p => p.Cod_Tipo_Serv == (documento != null ? documento.Cod_Tipo_Serv : -1));
                        var dataAgendamento = documento != null ? documento.Data_Maxima != null ? documento.Data_Maxima.Value.ToString("yyyy-MM-dd") : String.Empty : String.Empty;
                        var horaAgendamento = documento != null ? documento.Hora_Maxima != null ? documento.Hora_Maxima.Value.ToString("HH:mm") : String.Empty : String.Empty;
                        var dataHoraAgendamento = String.Empty;
                        if (dataAgendamento != String.Empty && horaAgendamento != String.Empty)
                        {
                            dataHoraAgendamento = $"{dataAgendamento} {horaAgendamento}";
                        }

                        dadosOS.Add(new DadosOSMobile
                        {
                            Id_PlanoProduto_Venda = planoProduto.Id,
                            Descricao_PlanoProduto_Venda = desc_Planoproduto,
                            CodCt_OS = documento?.CodCt,
                            Projeto_OS = documento?.Projeto,
                            Num_Obra_OS = documento?.Num_Obra,
                            Num_Doc_OS = documento?.Num_Doc,
                            Numero_OS = documento?.Numero,
                            Status_OS = documento != null ? HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(documento.Status.Value) : null,
                            Tipo_Serv_OS = tipo_Servico?.Descricao,
                            PlanoProduto_OS = documento != null ? desc_Planoproduto : null,
                            Endereco_Cliente_OS = documento != null ? GetEnderecoDadosOS(documento, tipo_Imovel.Descricao) : null,
                            DataHora_Agendamento_OS = dataHoraAgendamento,
                        });
                    }

                    retorno.Objeto = dadosOS;
                    retorno.Ok = true;
                }
                return retorno;
            }
            catch (Exception ex)
            {
                return new GetApiRetorno<List<DadosOSMobile>>
                {
                    Ok = false,
                    Mensagem = ex.Message,
                    InnerException = ex.InnerException?.Message
                };
            }
        }

        private String GetEnderecoDadosOS(Documento_ProducaoAniel documento, String tipo_Imovel)
        {
            return $"{documento.Logradouro}, {documento.Numero}, {documento.Pais}, {documento.Cidade}, {documento.UF}, " +
                $"{documento.Bairro}, {tipo_Imovel}, {documento.Complemento} - CEP: {documento.Cep}";
        }

        [AllowAnonymous]
        [HttpPost()]
        [Route("api/DocumentoProducao/CriarOS")]
        public async Task<GetApiRetorno<Documento_ProducaoChaveModel>> CriarOSMobile(ParamWebApi<CriarOSMobileRequest> request)
        {
            GetApiRetorno<Documento_ProducaoChaveModel> retorno = new GetApiRetorno<Documento_ProducaoChaveModel>();

            try
            {
                if (!request.Objeto.IdVenda.HasValue && !request.Objeto.IdPlanoProduto.HasValue)
                {
                    throw new Exception("ID da venda ou do plano/produto não informado");
                }

                Documento_ProducaoAniel modelo = null;
                Documento_ProducaoCriarOSMobile objeto = null;

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (ITransaction transaction = sessionT5.BeginTransaction())
                    {
                        using (var repoVenda = new Conexao<Venda>(session))
                        using (var repoCliente = new Conexao<Cliente>(session))
                        using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                        using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(session))
                        using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
                        using (var repoLocalidade = new Conexao<LocalidadeAniel>(sessionT5))
                        using (var repoClientePlanProd = new Conexao<Cliente_PlanoProduto>(session))
                        {
                            Venda venda = null;
                            Cliente cliente = null;
                            Cliente_PlanoProduto planoProduto = null;

                            venda = await repoVenda.selectFirst(p => p.Id == request.Objeto.IdVenda);
                            cliente = (await repoCliente.queryover().Where(p => p.Id == venda.Id_Cliente).ListAsync()).FirstOrDefault();
                            planoProduto = cliente.PlanosProdutos.Where(p => p.Id_Venda.HasValue && p.Id_Venda.Value == venda.Id && p.Id == request.Objeto.IdPlanoProduto).FirstOrDefault();

                            var config = await repoConfig.selectFirst();
                            IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                            var documentoBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);


                            objeto = new Documento_ProducaoCriarOSMobile
                            {
                                Cod_Escritorio = planoProduto.Cod_Escritorio,
                                Cod_Area = planoProduto.Cod_Area,
                                Cod_Localidade = planoProduto.Cod_Localidade,
                                Id_Plano_Produto = planoProduto.Id,
                                Id_Cliente = cliente.Id,
                                Id_Venda = venda.Id,
                                Id_Tipo_servico = planoProduto.Plano.Id_Tipo_Servico != null ? planoProduto.Plano.Id_Tipo_Servico : planoProduto.Produto.Id_Tipo_Servico,
                                Id_SubTipo_servico = planoProduto.Plano.Id_Subtipo_Servico != null ? planoProduto.Plano.Id_Subtipo_Servico : planoProduto.Produto.Id_Subtipo_Servico,
                                Id_Responsavel = request.Usuario
                            };

                            LocalidadeAniel localidade = await repoLocalidade.selectFirst(p => p.Cod_Escritorio == objeto.Cod_Escritorio &&
                                                                                                                        p.Cod_Area == objeto.Cod_Area &&
                                                                                                                        p.Cod_Localidade == objeto.Cod_Localidade);
                            if (localidade == null)
                            {
                                throw new Exception(RsCadastros.LOCALIDADE_NAO_ENCONTRADA);
                            }
                            if (String.IsNullOrEmpty(localidade.CodCt) || String.IsNullOrEmpty(localidade.Projeto))
                            {
                                throw new Exception("CodCt/Projeto não informado na Localidade");
                            }

                            String num_Obra = objeto.Num_Obra ?? documentoBusiness.GetNum_Obra(config);
                            Int64 _seq = documentoBusiness.GerarSeq(localidade.CodCt, localidade.Projeto, num_Obra, Convert.ToInt32(cliente.Numero_Contrato));

                            String unidade_Custo = String.Empty;
                            using (var repoContrato = new Conexao<ContratoAniel>(sessionT5))
                            {
                                unidade_Custo = (await repoContrato.selectFirst(p => p.CodCt == localidade.CodCt))?.Unidade_Custo;
                            }

                            modelo = new Documento_ProducaoAniel
                            {
                                Status = StatusExecucaoAniel.Aberta_Aguard_Agendamento,
                                CodCt = localidade.CodCt,
                                Projeto = localidade.Projeto,
                                Num_Obra = String.Format("{0}/{1}", num_Obra, _seq),
                                Seq = Convert.ToInt32(_seq),
                                Numero_Obra_Original = num_Obra,
                                Num_Doc = Convert.ToInt32(cliente.Numero_Contrato),
                                OXBDPEE = cliente.Numero_Contrato,
                                Tarefa = cliente.Telefone_Fixo,
                                Unidade_Custo = unidade_Custo,
                                Usuario = request.Usuario,
                                Dia = DateTime.Now,
                                Hora = DateTime.Now,
                                //Status_Cobranca= cliente.Status_Cobranca
                            };

                            await BindViewToObjCriarOS(sessionT5, session, modelo, objeto, localidade, cliente, planoProduto);

                            if (planoProduto.Status == Status_PlanoProduto.Aguardando_Criacao_OS)
                                planoProduto.Status = Status_PlanoProduto.Aguardando_Instalacao;

                            await repoClientePlanProd.SaveOrUpdate(planoProduto);

                            await repository.SaveOrUpdate(modelo);
                        }

                        transaction.Commit();
                        await InsertLog_Lanc_Servico(sessionT5, modelo.CodCt, modelo.Projeto, modelo.Num_Obra, modelo.Num_Doc, "Criação do Serviço", null,
                                request.Usuario, null, null, null, null);
                    }
                    retorno.Objeto = new Documento_ProducaoChaveModel
                    {
                        CodCt = modelo.CodCt,
                        Projeto = modelo.Projeto,
                        Num_Obra = modelo.Num_Obra,
                        Num_Doc = modelo.Num_Doc
                    };
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Objeto = null;
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        private async Task BindViewToObjCriarOS(ISession sessionT5, ISession sessionConnect, Documento_ProducaoAniel objBanco, Documento_ProducaoCriarOSMobile view, LocalidadeAniel localidade, Cliente cliente, Cliente_PlanoProduto planoProduto)
        {
            ClienteAniel clienteAniel = await GetClienteAniel(sessionT5, cliente);
            PlanoProdutoAniel clientePlanoProduto = (await GetPlanoProdutoAniel(sessionT5, planoProduto)).FirstOrDefault();

            objBanco.Cod_Escritorio = localidade.Cod_Escritorio;
            objBanco.Cod_Area = localidade.Cod_Area;
            objBanco.Cod_Localidade = localidade.Cod_Localidade;

            objBanco.Cliente = clienteAniel;
            objBanco.Nome_Cliente = StringService.Truncate(clienteAniel.NomeFantasia, 25);

            objBanco.Id_PlanoProduto = clientePlanoProduto.Id;
            objBanco.Id_Cliente_PlanoProdutoConnect = planoProduto.Id;

            objBanco.Latitude_Str = Convert.ToString(planoProduto?.Endereco_Instalacao?.Latitude);
            objBanco.Longitude_Str = Convert.ToString(planoProduto?.Endereco_Instalacao?.Longitude);

            objBanco.Bairro = planoProduto?.Endereco_Instalacao?.Bairro;
            objBanco.Cep = planoProduto?.Endereco_Instalacao?.Cep;
            objBanco.Cidade = StringService.Truncate(planoProduto?.Endereco_Instalacao?.Cidade, 20);
            objBanco.Logradouro = planoProduto?.Endereco_Instalacao?.Logradouro;
            objBanco.Numero = planoProduto?.Endereco_Instalacao?.Numero;
            objBanco.Complemento = planoProduto?.Endereco_Instalacao?.Complemento;
            objBanco.Observacao = StringService.Truncate(planoProduto?.Endereco_Instalacao?.Ponto_Referencia, 100);
            objBanco.UF = planoProduto?.Endereco_Instalacao?.UF;
            objBanco.Pais = HtmlEnumExtensions.GetEnumDescription<Prioridade_Atendimento>(Convert.ToString(planoProduto?.Endereco_Instalacao?.Pais));
            objBanco.Cod_Tipo_Imovel = planoProduto?.Endereco_Instalacao?.Tipo_Imovel?.Cod_Tipo_Imovel;

            objBanco.TelefonePrincipal = cliente.Telefone_Fixo;

            objBanco.Tarefa = String.IsNullOrEmpty(cliente.Telefone_Fixo) ? "1" : cliente.Telefone_Fixo;
            objBanco.CelularCliente = cliente.Telefone_Celular;
            objBanco.CPF_CLIENTE = cliente.CpfCnpj;
            objBanco.Titular = cliente.NomeFantasia;
            objBanco.RG_Assinante = cliente.Rg;
            objBanco.Email_Assinante = cliente.Email_Principal;

            short? Cod_Tipo_Serv = Convert.ToInt16(view.Id_Tipo_servico);
            if (Cod_Tipo_Serv == 0) { Cod_Tipo_Serv = null; }
            objBanco.Cod_Tipo_Serv = Cod_Tipo_Serv;
            String Cod_Sub_Tipo_Serv = Convert.ToString(view.Id_SubTipo_servico);
            if (Cod_Sub_Tipo_Serv == String.Empty) { Cod_Sub_Tipo_Serv = null; }
            objBanco.Cod_Sub_Tipo_Serv = Cod_Sub_Tipo_Serv;
            objBanco.Observacao_Aux = view.Observacao_OS;
            objBanco.Segmento = view.Segmento;
            objBanco.Equipe = view.Id_Responsavel;

            objBanco.Origem_Documento = Origem_Documento_Producao.Premier;

            objBanco.Responsabilidade = 0;
            objBanco.Roterizar = "N";

            objBanco.LoginRadius = cliente.Login;
            objBanco.SenhaRadius = cliente.Senha;

            if ((String.IsNullOrEmpty(objBanco.LoginRadius) || String.IsNullOrEmpty(objBanco.SenhaRadius)) && planoProduto != null)
            {
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == planoProduto.Id);
                    if (loginRadius != null)
                    {
                        objBanco.LoginRadius = loginRadius.Login;
                        objBanco.SenhaRadius = loginRadius.SenhaPPPOE;
                        objBanco.Ssid_Wifi = loginRadius.SSID_WIFI;
                        objBanco.Senha_Wifi = loginRadius.Senha_WIFI;
                    }
                }
            }

            objBanco.Data_Indicador = null;
            objBanco.Hora_Indicador = null;

            objBanco.Id_Venda = view.Id_Venda;
        }

        [AllowAnonymous]
        [HttpPost()]
        [Route("api/DocumentoProducao/CriarOSVenda")]
        public async Task<GetApiRetorno<Int32>> CriarOSVendaMobile(ParamWebApi<CriarOSMobileRequest> request)
        {
            GetApiRetorno<Int32> retorno = new GetApiRetorno<Int32>();

            try
            {
                if (!request.Objeto.IdVenda.HasValue)
                {
                    throw new Exception("ID da venda não informado");
                }

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoVenda = new Conexao<Venda>(session))
                using (var repoCliente = new Conexao<Cliente>(session))
                {
                    Venda venda = null;
                    Cliente cliente = null;

                    venda = await repoVenda.selectFirst(p => p.Id == request.Objeto.IdVenda);
                    cliente = (await repoCliente.queryover().Where(p => p.Id == venda.Id_Cliente).ListAsync()).FirstOrDefault();
                    var listaPlanosProdutos = cliente.PlanosProdutos.Where(p => p.Id_Venda.HasValue && p.Id_Venda.Value == venda.Id);

                    var req = request;

                    foreach (var planoProduto in listaPlanosProdutos)
                    {
                        if (planoProduto.Plano != null ? planoProduto.Plano.StatusInicial == Status_PlanoProduto.Aguardando_Criacao_OS : planoProduto.Produto.Status_Inicial == Status_PlanoProduto.Aguardando_Criacao_OS)
                        {
                            req.Objeto.IdPlanoProduto = planoProduto.Id;
                            await CriarOSMobile(req);
                        }
                    }

                    retorno.Objeto = 1;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Objeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        #endregion

        [AllowAnonymous]
        [OlaAuthorize]
        [HttpPost()]
        [Route("api/Documento_ProducaoApi/GetOSAbertas")]
        public async Task<List<string>> GetOSAbertas(String idCliente)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                Boolean Exibe_RDOs = (await new Conexao<Configuracoes_Gerais>(session).selectFirst()).ExibeRDOs;

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                List<string> retorno = new List<string>();
                string mensagemRetorno = "";

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder(@"SELECT
                                                               A.CODCT,
                                                               A.NUM_OBRA,
                                                               A.NUM_DOC,
                                                               B.DESCRICAO AS TIPO_SERVICO,
                                                               A.STATUS_EXECUCAO,
                                                               A.DATA_MAXIMA + COALESCE(A.HORA_MAXIMA, CAST('00:00' AS TIME)) AS DATAHORA_AGENDAMENTO,
                                                               G.NOME AS NOME_EQUIPE
                                                        FROM TB_DOCUMENTO_PRODUCAO A
                                                        INNER JOIN TB_TIPO_SERVICO_EQUIPE B ON (A.COD_TIPO_SERV = B.COD_TIPO_SERV)
                                                        LEFT JOIN TB_TURNO_SERVICO F ON (A.COD_TURNO_SERV = F.ID)
                                                        INNER JOIN TB_EQUIPE G ON (G.EQUIPE = A.EQUIPE)
                                                        WHERE A.ID_CLIENTE = @CODCT
                                                        AND A.STATUS_EXECUCAO IN (0,6,3)
                                                        AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO'");

                    // Aberto Aguardando Atendimento(0), Enviado ao Técnico(), OS em Deslocamento, Atendimento Iniciado/Reiniciado (3) e Aberta Aguardando Fechamento ().
                    sql.AppendLine("AND A.STATUS_EXECUCAO IN (0,6,3)");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    sql.AppendLine(@" GROUP BY
                                         A.CODCT,
                                         A.PROJETO,
                                         A.NUM_OBRA,
                                         A.NUM_DOC,
                                         B.DESCRICAO,
                                         A.STATUS_EXECUCAO,
                                         A.DATA_MAXIMA + COALESCE(A.HORA_MAXIMA, CAST('00:00' AS TIME)),
                                         G.NOME");


                    parametros.Add("@CODCT", idCliente);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var numOS = reader["NUM_OBRA"] != DBNull.Value ? Convert.ToString(reader["NUM_OBRA"]) : string.Empty;
                            var tipoServico = reader["TIPO_SERVICO"] != DBNull.Value ? Convert.ToString(reader["TIPO_SERVICO"]) : "Tipo de Serviço não atribuído";
                            var dataAgendamento = reader["DATAHORA_AGENDAMENTO"] != DBNull.Value
                                ? Convert.ToDateTime(reader["DATAHORA_AGENDAMENTO"]).ToString("dd/MM/yyyy HH:mm")
                                : "Sem agendamento";
                            var statusExecucao = reader["STATUS_EXECUCAO"] != DBNull.Value
                                ? HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(reader["STATUS_EXECUCAO"]))
                                : "Status indisponível";
                            var tecnicoResponsavel = reader["NOME_EQUIPE"] != DBNull.Value ? Convert.ToString(reader["NOME_EQUIPE"]) : "Equipe não atribuída";

                            if (!string.IsNullOrWhiteSpace(numOS))
                            {
                                mensagemRetorno += $"\r\n *Nº OS:* {numOS}" +
                                                   $"\r\n  *Data e Hora:* {dataAgendamento}" +
                                                   $"\r\n  *Tipo de Serviço:* {tipoServico}" +
                                                   $"\r\n  *Situação:* {statusExecucao}" +
                                                   $"\r\n  *Téc. Responsável:* {tecnicoResponsavel}" +
                                                   "\r\n\r\n";
                            }
                        }


                        if (String.IsNullOrEmpty(mensagemRetorno))
                        {
                            mensagemRetorno = ("Não foram encontradas solicitações para este cliente em aberto.");
                        }

                        retorno.Add(mensagemRetorno);
                    }
                    return retorno;
                }
            }

        }

        [HttpPost()]
        [Route("api/DocumentoProducao/GetMotivoParalizacaoDocumentoProducao")]
        public async Task<string> GetMotivoParalizacaoDocumentoProducao(MotivoParalizacaoRequest request)
        {
            string retorno = null;
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder(@" SELECT A.OBSERVACAO_PAUSA FROM TB_DOCUMENTO_PRODUCAO A 
                                                             WHERE A.NUM_OBRA = @NUM_OBRA AND A.NUM_DOC = @NUM_DOC AND A.PROJETO = @PROJETO AND A.CODCT = @CODCT");

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    parametros.Add("@NUM_OBRA", request.NumObra);
                    parametros.Add("@NUM_DOC", request.Contrato);
                    parametros.Add("@PROJETO", request.Projeto);
                    parametros.Add("@CODCT", request.CodCt);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno = Convert.ToString(reader["OBSERVACAO_PAUSA"]);
                        }
                    }


                }

                return retorno;
            }


        }

        [HttpPost()]
        [Route("api/DocumentoProducao/AlterarStatusCobrancaServico")]
        public async Task<PostApiRetorno<Int64>> AlterarStatusCobrancaServico(ServicoCobrancaAlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var planoProdutoClienteRepo = new Conexao<Cliente_PlanoProduto>(sessionConnect))
                using (var planoRepor = new Conexao<Plano>(sessionConnect))
                using (var produtoRepor = new Conexao<Produto>(sessionConnect))
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder($" UPDATE TB_DOCUMENTO_PRODUCAO SET  STATUS_COBRANCA = {value.status} WHERE CODCT = '{value.CodCt}' AND PROJETO = '{value.Projeto}' AND NUM_OBRA = '{value.Num_Obra}' AND NUM_DOC = {value.Num_Doc} ");


                        if (session.Transaction != null)
                        {
                            session.Transaction.Enlist(com);
                        }

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);


                    }
                    retorno.Ok = true;

                    var planoProdutoCliente = await planoProdutoClienteRepo.selectFirst(x => x.Id == value.IdCliente);
                    var planoFaturamento = await planoRepor.selectFirst(y => y.Id == value.IdPlanoProduto);
                    var produtoFaturamento = await produtoRepor.selectFirst(y => y.Id == value.IdPlanoProduto);

                    if (value.status == 2)//aprovado
                    {
                        await InsertLog_Lanc_Servico(session, value.CodCt, value.Projeto, value.Num_Obra, value.Num_Doc.Value, "Avaliação de cobrança de serviço", "Aguardando Avaliação", "", null, null, "Aprovado", value.Motivo);

                        if (planoFaturamento != null)
                        {
                            var docProd = new DocumentoProducao_GetDocumento { CodCt = value.CodCt, Num_Doc = value.Num_Doc, Num_Obra = value.Num_Obra, Projeto = value.Projeto, Status_Cobranca = value.status };

                            await GerarCobrancaDocProd(docProd);
                        }
                    }
                    else if (value.status == 3)
                    {
                        await InsertLog_Lanc_Servico(session, value.CodCt, value.Projeto, value.Num_Obra, value.Num_Doc.Value, "Avaliação de cobrança de serviço", "Aguardando Avaliação", User.Identity.GetUserId(), null, null, "Reprovado", value.Motivo);
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

        [HttpPost()]
        [Route("api/DocumentoProducao/AlterarStatusAuditoria")]
        public async Task<PostApiRetorno<Int64>> AlterarStatusAuditoria(ServicoCobrancaAlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var planoProdutoClienteRepo = new Conexao<Cliente_PlanoProduto>(sessionConnect))
                using (var docPrdRepo = new Conexao<Documento_ProducaoAniel>(session))
                using (var confGeraisRepo = new Conexao<Configuracoes_Gerais>(sessionConnect))
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        var doc = await docPrdRepo.selectFirst(p => p.CodCt == value.CodCt && p.Projeto == value.Projeto && p.Num_Obra == value.Num_Obra && p.Num_Doc == value.Num_Doc);
                        var configGeral = await confGeraisRepo.selectFirst();

                        if (doc.Status.Value == StatusExecucaoAniel.Fechada_Improdutiva)
                        {
                            if (!configGeral.PermitirAuditoriaOSImprod)
                            {
                                retorno.Mensagem = "Não é possível realizar Auditoria para OS Improdutivas";
                                retorno.Ok = false;
                                return retorno;
                            }
                        }

                        doc.Status_Auditoria = (Status_AuditoriaAniel)value.status;
                        doc.UsuarioAuditoria = value.Usuario;

                        await docPrdRepo.SaveOrUpdate(doc);
                    }
                    retorno.Ok = true;

                    if (value.status == 0)//REPROVADO
                    {
                        await InsertLog_Lanc_Servico(session, value.CodCt, value.Projeto, value.Num_Obra, value.Num_Doc.Value, "Avaliação Auditor", "Aguardando Avaliação", User.Identity.GetUserId(), null, null, "Reprovado", StringService.Truncate(value.Motivo, 800));
                        retorno.Mensagem = RsBase.AUDITORIA_REPROVADA;
                    }
                    else if (value.status == 1)//APROVADO
                    {
                        await InsertLog_Lanc_Servico(session, value.CodCt, value.Projeto, value.Num_Obra, value.Num_Doc.Value, "Avaliação Auditor", "Aguardando Avaliação", User.Identity.GetUserId(), null, null, "Aprovado", StringService.Truncate(value.Motivo, 800));
                        retorno.Mensagem = RsBase.AUDITORIA_APROVADA;
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





        [HttpPost()]
        [Route("api/DocumentoProducao/AutorizarBaixaServico")]
        public async Task<GetApiRetorno<string>> AutorizarBaixaServico(AutorizarBaixaModel value)
        {
            GetApiRetorno<string> retorno = new GetApiRetorno<string>();

            try
            {
                Documento_ProducaoChaveModel chave_documento = value.ids.Select(chave =>
                {

                    String[] _chaves = chave.Split('|');
                    String CodCt = _chaves[0];
                    String Projeto = _chaves[1];
                    String Num_Obra = _chaves[2];
                    Int32 Num_Doc = Convert.ToInt32(_chaves[3]);

                    return new Documento_ProducaoChaveModel
                    {
                        CodCt = CodCt,
                        Num_Doc = Num_Doc,
                        Num_Obra = Num_Obra,
                        Projeto = Projeto
                    };

                }).ToList().FirstOrDefault();

                var senhaExistente = await GetCodigoAutorizacaoBaixaServico(chave_documento);

                if (!String.IsNullOrEmpty(senhaExistente))
                {
                    retorno.Objeto = senhaExistente;
                }
                else
                {
                    string senhaAutorizacao = this.GerarCodigoAutorizacaoOS();

                    using (ISession session = HibernateUtil.GetSessionT5())
                    {


                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        using (var com = session.Connection.CreateCommand())
                        {
                            StringBuilder sql = new StringBuilder($" UPDATE TB_DOCUMENTO_PRODUCAO SET SENHA_AUTORIZACAO_BAIXA = @SENHA_AUTORIZACAO_BAIXA ");
                            sql.AppendLine(" WHERE CODCT = @CODCT AND PROJETO = @PROJETO AND NUM_OBRA = @NUM_OBRA AND NUM_DOC = @NUM_DOC ");

                            parametros.Add("@CODCT", chave_documento.CodCt);
                            parametros.Add("@PROJETO", chave_documento.Projeto);
                            parametros.Add("@NUM_OBRA", chave_documento.Num_Obra);
                            parametros.Add("@NUM_DOC", chave_documento.Num_Doc);
                            parametros.Add("@SENHA_AUTORIZACAO_BAIXA", senhaAutorizacao);

                            await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, com.Transaction);
                        }

                        retorno.Objeto = senhaAutorizacao;

                    }
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        public async Task<string> GetCodigoAutorizacaoBaixaServico(Documento_ProducaoChaveModel value)
        {
            string retorno = null;

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder($" SELECT SENHA_AUTORIZACAO_BAIXA FROM TB_DOCUMENTO_PRODUCAO ");
                    sql.AppendLine(" WHERE CODCT = @CODCT AND PROJETO = @PROJETO AND NUM_OBRA = @NUM_OBRA AND NUM_DOC = @NUM_DOC ");

                    parametros.Add("@CODCT", value.CodCt);
                    parametros.Add("@PROJETO", value.Projeto);
                    parametros.Add("@NUM_OBRA", value.Num_Obra);
                    parametros.Add("@NUM_DOC", value.Num_Doc);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, com.Transaction))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno = reader["SENHA_AUTORIZACAO_BAIXA"] != DBNull.Value ? Convert.ToString(reader["SENHA_AUTORIZACAO_BAIXA"]) : null;
                        }
                    }
                }
            }

            return retorno;
        }

        /// <summary>
        /// Gera um código alfanumérico de 6 dígitos para autorização da Baixa de Serviço.
        /// </summary>
        /// <returns></returns>
        private string GerarCodigoAutorizacaoOS()
        {
            int length = 6;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder result = new StringBuilder(length);
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }

        private async Task<bool> GerarFaturamentoInstalacao(Cliente_PlanoProduto planoProduto, ISession session)
        {

            String URLPortal = CONSTANTES.URLFinanceiro;

            using (var repositoryPlano = new Conexao<Plano>(session))
            using (var repoClientePlano = new Conexao<Cliente_PlanoProduto>(session))
            using (var repoPlanoParcela = new Conexao<Cliente_PlanoProduto_Parcela>(session))
            {
                var plano = await repositoryPlano.selectFirst(x => x.Id == planoProduto.Plano.Id);

                var fat = await BindFaturaObj(plano, planoProduto.Cliente.Id);

                var criarFaturamento = await PostAsync(URLPortal, $"api/Faturamentoapi/CriarParcial", fat); //cria o faturamento do Cliente no EVA

                if (!criarFaturamento.Ok)
                    throw new Exception("Não foi possível fazer o faturamento");

                Int64 formaFaturamentoId = await GetFormaPagmento(criarFaturamento.IdObjeto); //obtem a forma de pagamento

                Int64 faturamentoCliId = await GetFaturamentoCliente(formaFaturamentoId); //obtem faturamento cliente

                var faturamentoProcessar = new FaturamentoProcessarRequestModels
                {
                    Id_Faturamento_Forma = formaFaturamentoId,
                    Ids_Faturamento_Cliente = new List<Int64?>() { faturamentoCliId },
                    ProcessarBoletoInstall = true
                };

                var processarFaturamento = await PostAsyncString(URLPortal, $"api/Faturamentoapi/Processar", faturamentoProcessar); //processa o fat e gera o boleto (caso formato boleto)

                if (!processarFaturamento.Ok)
                    throw new Exception($"Não foi possível fazer o Processamento do Faturamento. Confira o EVA {faturamentoCliId}");

                planoProduto.ContaReceberInstalacao = true;

                await repoClientePlano.SaveOrUpdate(planoProduto);

                var planoParcelaCli = new Cliente_PlanoProduto_Parcela
                {
                    PlanoProduto = planoProduto,
                    Parcela = 1,
                    TotParcelas = plano.QtdParcelas,
                    ValorTotal = plano.ValorInstalacaoFinal,
                    Faturamento = new Faturamento_Cliente { Id = faturamentoCliId }
                };

                await repoPlanoParcela.SaveOrUpdate(planoParcelaCli);
            }

            return true;
        }

        private async Task<Int64> GetFormaPagmento(Int64 fatProtocol)
        {
            Int64 formPagmentoId = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine("SELECT FORMA.ID ");
                sql.AppendLine("FROM TB_FATURAMENTO_FORMA_PAGAMENTO FORMA ");
                sql.AppendLine("WHERE FORMA.ID_FATURAMENTO_PROTOCOLO = @IDPROTOCOL ");
                parametros.Add("@IDPROTOCOL", fatProtocol);

                using (var com = session.Connection.CreateCommand())
                {
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            formPagmentoId = Convert.ToInt64(reader["ID"]);
                        }
                    }
                }
            }

            return formPagmentoId;
        }

        private async Task<Int64> GetFaturamentoCliente(Int64 formaFatId)
        {
            Int64 faturamentoCliId = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine("SELECT FATU.ID ");
                sql.AppendLine("FROM TB_FATURAMENTO_CLIENTE FATU ");
                sql.AppendLine("WHERE FATU.ID_FATURAMENTO_FORMA = @IDFORMA ");
                parametros.Add("@IDFORMA", formaFatId);

                using (var com = session.Connection.CreateCommand())
                {
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            faturamentoCliId = Convert.ToInt64(reader["ID"]);
                        }
                    }
                }
            }

            return faturamentoCliId;
        }

        private async Task<FaturamentoParcial> BindFaturaObj(Plano plano, Int64 clientId)
        {
            var date = DateTime.Now;
            var diasVencBoleto = plano.DiasBoleto;

            FaturamentoParcial fatura = new FaturamentoParcial
            {
                Id_FormaPagamento = null,
                Id_Cliente = clientId,
                Data_Inicial = date.ToString("dd/MM/yyyy"),
                Data_Final = date.ToString("dd/MM/yyyy"),
                Data_Vencimento = date.AddDays(diasVencBoleto).ToString("dd/MM/yyyy"),
                Valor_Avulso = $"{plano.ValorInstalacao}",
                Fat_PlanosProdutos = false,
                Id_ContaFinanceira = plano.Conta_FinanceiraInstalacao.Id,
                Id_Forma_Pagamento_Cliente = plano.FormaPagamentoCobranca,
                Automatico = false,
                Recorrente = false,
                Quantidade_Meses = null,
                Periodo_Recorrencia = null,
                IdsClientePlanoProduto = null,
                Usuario = "SISTEMA"
            };

            return fatura;
        }
        private async Task GerarBoleto(ISession session, Cliente_PlanoProduto clientePlanoProduto, Cliente cliente, double valor)
        {

            var objetoRequisicao = new
            {
                cobranca = new
                {
                    Vencimento = DateTime.Now.AddDays(7),
                    Email_Principal = cliente.Email_Principal,
                    Endereco_Cliente = cliente.Endereco_Principal,
                    Telefone_Fixo = cliente.Telefone_Fixo,
                    faturamento_Cliente = new
                    {
                        Faturamento_Forma_Pagto = new
                        {
                            Forma_pagto = clientePlanoProduto.Forma_Pagamento_Cliente.Forma_Pagamento
                        }
                    },
                    valor = valor,
                },
                tipo = 1
            };

            //chamar api do eva
            var client = new RestClient("http://localhost/Aniel.Financeiro/api/Cobrancaapi/enviarcobranca");
            var request = new RestRequest(Method.POST);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(objetoRequisicao);

            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            object retorno = JsonService.DeserializeObject<object>(response.Content);
            Console.WriteLine();

        }

        public async Task<TimeSpan?> GetTempoExecucao(Configuracoes_Gerais config, String CodCt, String Projeto, String Num_Os, Int32 Num_Doc)
        {
            TimeSpan? retorno = config.Tempo_Execucao_Padrao.HasValue ? (TimeSpan?)config.Tempo_Execucao_Padrao.Value.TimeOfDay : null;
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.AppendLine("SELECT (SELECT FIRST 1 M.HORAS ")
                .AppendLine("FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M ")
                .AppendLine("WHERE ( (M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO ")
                .AppendLine("   FROM TB_DOCUMENTO_PRODUCAO A ");

                sql.AppendLine($"WHERE (A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA = @NUMOBRA AND A.NUM_DOC = @NUMDOC)  ");
                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUMOBRA", Num_Os);
                parametros.Add("@NUMDOC", Num_Doc);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        return reader["TEMPO_EXECUCAO"] != DBNull.Value ? (TimeSpan?)(reader["TEMPO_EXECUCAO"]) : retorno;
                    }
                }
            }

            return retorno;
        }
    }
}







