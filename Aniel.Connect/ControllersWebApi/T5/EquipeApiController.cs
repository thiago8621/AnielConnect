using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Attributes;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.Equipe;
using Aniel.Connect.Repository.T5.TipoServico;
using Aniel.Connect.Repository.Usuario;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Hql.Ast;
using OpenXmlPowerTools;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class EquipeApiController : BaseApiController
    {
        //[HttpPost()]
        [AcceptVerbs("GET", "POST")]
        [Route("api/EquipeApi/GetAllAcompanhamentoEquipe")]
        public async Task<DataTableResponse<Acompanhamento_EquipeListaModel>> GetAllAcompanhamentoEquipe(Acompanhamento_Equipes_DataTableRequest request)
        {
            List<Acompanhamento_EquipeListaModel> retorno = new List<Acompanhamento_EquipeListaModel>();
            IOrderedEnumerable<Acompanhamento_EquipeListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession sessionT5NH = HibernateUtil.GetSessionT5())
            using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(sessionT5NH))
            using (var repositoryPerfilAgendamento = new Conexao<PerfilAgendamento>(sessionT5NH))
            using (var repositoryFeriado = new Conexao<Feriado>(sessionT5NH))
            {
                IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                ITipoServicoAnielRepository repoTipoServico = new TipoServicoAnielRepository(sessionT5NH, sessionT5);
                IDocumentoProducaoAnielRepository repoDocumento = new DocumentoProducaoAnielRepository(sessionT5NH, sessionT5);
                IEquipeAnielRepository repoEquipe = new EquipeAnielRepository(sessionT5);
                EquipeAnielBusiness equipeAnielBusiness = new EquipeAnielBusiness(repoEquipe, repoTipoServico, repoDocumento);
                var usuarioBusiness = new UsuarioBusiness(repoUsuario);

                IdentityUser user = usuarioBusiness.GetById(User.Identity.GetUserId());
                Dictionary<long, PerfilAgendamento> cachePerfilAgendamento = new Dictionary<long, PerfilAgendamento>();
                var config = await new Conexao<Configuracoes_Gerais>(session).selectFirst();
                Boolean Exibe_RDOs = config.ExibeRDOs;

                bool feriado = repositoryFeriado.selectFirst(p => p.Data == DateTime.Now.Date).Result != null;
                List<Horario_Connect> horarios_Connect = (await new Conexao<Horario_Connect>(sessionT5).select(p => p.Tipo == Tipo_Hora_Connect.Horario_Servico)).ToList();
                Horario_Connect horario_ConnectPadrao = horarios_Connect.Find(p => p.Padrao == true);

                if (horario_ConnectPadrao == null)
                {
                    throw new Exception("O Horário de Serviço Padrão não foi definido em Configurações Gerais > Horários ");
                }

                DayOfWeek diaSemana = DateTime.Now.DayOfWeek;

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(EQUIPE) CO FROM TB_EQUIPE A WHERE A.EQUIPE_CONNECT = 'S' ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    var sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.EQUIPE, A.NOME, A.NUM_FRE, A.USUARIO_MONITOR,A.ID_PERFIL_AGENDAMENTO, B.CEP, B.LOGRADOURO, B.BAIRRO, B.CIDADE, ");
                    sql.AppendLine("        B.NUMERO, B.PRECISAO, B.BATERIA, B.CARREGANDO, B.STATUSGPS, B.DISPOSITIVO, B.VERSAO_ANDROID, ");
                    sql.AppendLine("        B.VERSAO_APP, B.TIPO, B.DATAHORACRIACAO, B.DATAHORAALTERACAO, COUNT(C.NUM_OBRA) AS TOTAL, ");

                    sql.AppendLine("       SUM(CASE WHEN C.STATUS_EXECUCAO = 9 THEN 1 ELSE 0 END) AS CANCELADOS, ");

                    sql.AppendLine("       (SELECT FIRST 1 F.ID ");
                    sql.AppendLine("        FROM TB_HORARIO_EQUIPE_CONNECT F ");
                    sql.AppendLine("        LEFT JOIN TB_HORARIO_CONNECT G ");
                    sql.AppendLine("        ON(F.ID_HORARIO = G.ID) ");
                    sql.AppendLine("        WHERE F.EQUIPE = A.EQUIPE AND(((F.APENAS_NO_DIA = 1) AND F.DATA = @DATA) OR ");
                    sql.AppendLine("                                       (F.APENAS_NO_DIA = 0 AND F.DATA <= @DATA)) ");
                    sql.AppendLine("        ORDER BY F.DATA DESC) AS ID_HORARIO_EQUIPE, ");

                    sql.AppendLine("       SUM(CASE WHEN D.STATUS_EXECUCAO = 2 AND C.STATUS_EXECUCAO <> 9 THEN 1 ELSE 0 END) AS PRODUTIVO, ");
                    sql.AppendLine("       SUM(CASE WHEN D.STATUS_EXECUCAO = 1 AND C.STATUS_EXECUCAO <> 9  THEN 1 ELSE 0 END) AS IMPRODUTIVO, ");
                    sql.AppendLine("       COALESCE(SUM(E.RPONTOS_PREVISTOS),0.0) QTDE_PREV, ");
                    sql.AppendLine("       COALESCE(SUM(E.RPONTOS_PRODUZIDOS),0.0) QTDE_PROD ");

                    sql.AppendLine(" FROM TB_EQUIPE A ");

                    sql.AppendLine(" INNER JOIN TB_FUNCIONARIO F ");
                    sql.AppendLine(" ON(A.NUM_FRE = F.NUM_FRE) ");

                    sql.AppendLine(" LEFT JOIN TB_EQUIPE_STATUS B ");
                    sql.AppendLine(" ON(A.EQUIPE = B.EQUIPE) ");

                    sql.AppendLine(" LEFT JOIN TB_DOCUMENTO_PRODUCAO C ");
                    sql.AppendLine(" ON(C.EQUIPE = A.EQUIPE");

                    sql.AppendLine("    AND (");
                    // Aberto Aguardando Atendimento(0), Enviado ao Técnico(), OS em Deslocamento(6), Atendimento Iniciado/Reiniciado (3) e Aberta Aguardando Fechamento ().
                    sql.AppendLine("         ((C.DATA_MAXIMA BETWEEN @DataAbertoIni AND @DataAbertoFim) AND C.STATUS_EXECUCAO IN (0,6,3))");

                    // Atendimento Paralisado(10)
                    sql.AppendLine("          OR ((C.DATA_MAXIMA BETWEEN @DataParalisadosIni AND @DataParalisadosFim) AND C.STATUS_EXECUCAO = 10)");

                    // Atendimento Finalizado(8), Fechada Improdutiva(1), Fechada Produtiva(2) e Cancelado(9)
                    sql.AppendLine("          OR ((C.DATA_MAXIMA BETWEEN @DataEncerradosIni AND @DataEncerradosFim) AND C.STATUS_EXECUCAO IN (8,1,2,9))");

                    sql.AppendLine("    )");

                    parametros.Add("@DataAbertoIni", request.DataAbertoIni);
                    parametros.Add("@DataAbertoFim", request.DataAbertoFim);

                    parametros.Add("@DataParalisadosIni", request.DataParalisadosIni);
                    parametros.Add("@DataParalisadosFim", request.DataParalisadosFim);

                    parametros.Add("@DataEncerradosIni", request.DataEncerradosIni);
                    parametros.Add("@DataEncerradosFim", request.DataEncerradosFim);

                    if (request.Situacao.HasValue)
                    {
                        switch (request.Situacao)
                        {
                            case 1: //Em Andamento
                                sql.AppendLine("    AND (C.STATUS_EXECUCAO NOT IN(1,2,9,8)) "); //1 = IMPRODUTIVA, 2 = PRODUTIVA, 9 = CANCELADA,8 = ATENDIMENTO FINALIZADO
                                break;
                        }

                        switch (request.Situacao)
                        {
                            case 2: //Concluídas Produtivas
                                sql.AppendLine("    AND (SELECT FIRST 1 STATUS_EXECUCAO FROM TB_CODIGO_ENCERRAMENTO WHERE COD_ENCERRAMENTO = C.COD_ENCERRAMENTO) = 2 "); //2 = PRODUTIVA
                                break;
                            case 3: //Concluídas Improdutivas
                                sql.AppendLine("    AND (SELECT FIRST 1 STATUS_EXECUCAO FROM TB_CODIGO_ENCERRAMENTO WHERE COD_ENCERRAMENTO = C.COD_ENCERRAMENTO) = 1 "); //1 = IMPRODUTIVA
                                break;
                        }

                    }

                    if (!Exibe_RDOs)
                        sql.AppendLine("    AND COALESCE(C.SIGLA_MODULO, '') <> 'MCO' ");

                    sql.AppendLine(" ) ");

                    sql.AppendLine(" LEFT JOIN TB_CODIGO_ENCERRAMENTO D ");
                    sql.AppendLine(" ON(D.COD_ENCERRAMENTO = C.COD_ENCERRAMENTO AND D.CODCT = C.CODCT) ");

                    sql.AppendLine(" LEFT JOIN SP_CONNECT_TOTAL_PONTOS(C.CODCT,C.PROJETO, C.NUM_OBRA, C.NUM_DOC) E ");
                    sql.AppendLine(" ON (C.CODCT = E.RCODCT AND C.PROJETO = E.RPROJETO AND C.NUM_OBRA = E.RNUM_OBRA AND C.NUM_DOC = E.RNUM_DOC) ");

                    sql.AppendLine(" WHERE A.EQUIPE_CONNECT = 'S' ");

                    parametros.Add("@DATA", DateTime.Now.Date);

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("   AND ((UPPER(A.EQUIPE) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }
                    if (request.Supervisor != null)
                    {
                        sql.AppendLine("   AND (A.COD_SUPERV = @ID_SUPERVISOR) ");
                        parametros.Add("@ID_SUPERVISOR", request.Supervisor);
                    }
                    if (request.Monitor != null)
                    {
                        sql.AppendLine("   AND UPPER(A.USUARIO_MONITOR) = @ID_MONITOR ");
                        parametros.Add("@ID_MONITOR", request.Monitor.ToUpper());
                    }

                    if (!String.IsNullOrEmpty(request.Tecnico_Ativos))
                    {
                        sql.AppendLine("   AND ( F.SITUACAO = 0 )  ");
                    }
                    if (!String.IsNullOrEmpty(request.Tecnico_Em_Execucao))
                    {
                        sql.AppendLine("   AND EXISTS(SELECT FIRST 1 C.NUM_OBRA FROM TB_DOCUMENTO_PRODUCAO C ");
                        sql.AppendLine("              WHERE C.EQUIPE = A.EQUIPE AND C.DATA_MAXIMA = CURRENT_DATE ) ");
                    }

                    #region Filtro Perfil Visualização de Serviço
                    if (request.Tecnico_Em_Execucao == "true")
                    {
                        if (user.PerfilVisualizacaoServico != null)
                        {
                            if (user.PerfilVisualizacaoServico.ParamsContratoProjeto.Count() > 0)
                            {
                                var filtrosContratoProj = new List<string>();

                                foreach (var paramContratoProj in user.PerfilVisualizacaoServico.ParamsContratoProjeto)
                                {
                                    if (!String.IsNullOrEmpty(paramContratoProj.CodCt) && !String.IsNullOrEmpty(paramContratoProj.Projeto))
                                    {
                                        filtrosContratoProj.Add($"(C.CODCT = '{paramContratoProj.CodCt}' AND C.PROJETO = '{paramContratoProj.Projeto}')");
                                    }
                                    else
                                    {
                                        filtrosContratoProj.Add($"(C.CODCT = '{paramContratoProj.CodCt}')");
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
                                        filtrosArea.Add($"(C.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND C.COD_AREA = '{paramArea.CodArea}' AND C.COD_LOCALIDADE = {paramArea.CodLocalidade.Value})");
                                    }
                                    else if (!String.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                             !String.IsNullOrEmpty(paramArea.CodArea))
                                    {
                                        filtrosArea.Add($"(C.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND C.COD_AREA = '{paramArea.CodArea}')");
                                    }
                                    else
                                    {
                                        filtrosArea.Add($"(C.COD_ESCRITORIO = '{paramArea.CodEscritorio}')");
                                    }
                                }

                                sql.Append($" AND ({String.Join(" OR ", filtrosArea)})");
                            }

                            if (user.PerfilVisualizacaoServico.ParamsSetorDepto.Count() > 0)
                            {
                                var filtrosSetor = new List<string>();

                                foreach (var paramSetor in user.PerfilVisualizacaoServico.ParamsSetorDepto)
                                {
                                    filtrosSetor.Add($"(C.COD_DEPTO = {paramSetor.CodDepto})");
                                }

                                sql.Append($" AND ({String.Join(" OR ", filtrosSetor)})");
                            }

                            if (user.PerfilVisualizacaoServico.ParamsSupervisor.Count() > 0)
                            {
                                var filtrosSupervisor = new List<string>();

                                foreach (var paramSupervisor in user.PerfilVisualizacaoServico.ParamsSupervisor)
                                {
                                    filtrosSupervisor.Add($"(C.COD_SUPERV = '{paramSupervisor.CodSuperv}')");
                                }

                                sql.Append($" AND ({String.Join(" OR ", filtrosSupervisor)})");
                            }

                            if (user.PerfilVisualizacaoServico.ParamsMonitor.Count() > 0)
                            {
                                var filtrosMonitor = new List<string>();

                                foreach (var paramMonitor in user.PerfilVisualizacaoServico.ParamsMonitor)
                                {
                                    filtrosMonitor.Add($"(C.USUARIO_MONITOR = '{paramMonitor.UsuarioMonitor}')");
                                }

                                sql.Append($" AND ({String.Join(" OR ", filtrosMonitor)})");
                            }
                        }
                    }
                    #endregion

                    sql.AppendLine(" GROUP BY A.EQUIPE, A.NOME, A.NUM_FRE, A.USUARIO_MONITOR,A.ID_PERFIL_AGENDAMENTO, B.CEP, B.LOGRADOURO, B.BAIRRO, B.CIDADE, ");
                    sql.AppendLine("          B.NUMERO, B.PRECISAO, B.BATERIA, B.CARREGANDO, B.STATUSGPS, B.DISPOSITIVO, B.VERSAO_ANDROID, ");
                    sql.AppendLine("          B.VERSAO_APP, B.TIPO, B.DATAHORACRIACAO, B.DATAHORAALTERACAO  ");

                    //Se o filtro estiver como "não" retorna todos os funcionários que possuem OS
                    if (String.IsNullOrEmpty(request.Tecnico_Ativos))
                    {
                        sql.AppendLine(" HAVING COUNT(C.NUM_OBRA) <> 0");
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        Agendamento_EquipeApiController agendamentoApi = new Agendamento_EquipeApiController();

                        while (await reader.ReadAsync())
                        {
                            String endereco = String.Empty;
                            if (reader["LOGRADOURO"] != DBNull.Value)
                                endereco = $@"{reader["LOGRADOURO"]} {reader["NUMERO"]}, {reader["BAIRRO"]} - {reader["CIDADE"]}";

                            Int32 Total = Convert.ToInt32(reader["TOTAL"]);
                            Int32 Cancelados = Convert.ToInt32(reader["CANCELADOS"]);

                            var obj = new Acompanhamento_EquipeListaModel
                            {
                                Equipe = Convert.ToString(reader["EQUIPE"]).ToUpper(),
                                Nome = Convert.ToString(reader["NOME"]).ToUpper(),
                                Ultimo_Endereço = endereco.ToUpper(),
                                Atualizacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm:ss") : "-",
                                BaixadasProdutivas = Convert.ToString(reader["PRODUTIVO"]),
                                BaixadasImprodutivas = Convert.ToString(reader["IMPRODUTIVO"]),
                                TotalVisitas = (Total - Cancelados).ToString(),
                                Bateria = Convert.ToString(reader["BATERIA"]).PadLeft(3, '0') + "%",
                                Cor_Bateria = GetCorIcone(Convert.ToString(reader["BATERIA"]))
                            };



                            var carregando = TipoCarregamentoBateria.Sem_Carregar;

                            if (reader["CARREGANDO"] != DBNull.Value)
                                carregando = (TipoCarregamentoBateria)Enum.Parse(typeof(TipoCarregamentoBateria), Convert.ToString(reader["CARREGANDO"]));

                            obj.Icone_Bateria = GetIcone(Convert.ToString(reader["BATERIA"]), carregando);
                            EquipeStatusTipo tipo = reader["TIPO"] != DBNull.Value ? (EquipeStatusTipo)Enum.Parse(typeof(EquipeStatusTipo), Convert.ToString(reader["TIPO"])) : EquipeStatusTipo.Sem_Status;

                            Horario_Connect horario_Equipe = horario_ConnectPadrao;
                            Horario_Equipe horarioEquipeConnect = null;
                            PerfilAgendamento perfilAgendamento = null;

                            if (reader["ID_HORARIO_EQUIPE"] != DBNull.Value)
                            {
                                horarioEquipeConnect = await repositoryHorarioEquipe.selectFirst(p => p.Id == Convert.ToInt64(reader["ID_HORARIO_EQUIPE"]));

                                if (horarioEquipeConnect?.Indisponivel == true)
                                {
                                    tipo = EquipeStatusTipo.Em_Intervalo;
                                }


                                long? horario_connect = horarioEquipeConnect?.Horario?.Id;
                                var tmp = horarios_Connect.Find(p => p.Id == Convert.ToInt64(horario_connect));
                                if (tmp != null)
                                    horario_Equipe = tmp;
                            }

                            if (horarioEquipeConnect?.PerfilAgendamento != null)
                            {
                                perfilAgendamento = horarioEquipeConnect.PerfilAgendamento;
                            }
                            else
                            {
                                if (reader["ID_PERFIL_AGENDAMENTO"] != DBNull.Value)
                                {
                                    if (cachePerfilAgendamento.TryGetValue(Convert.ToInt64(reader["ID_PERFIL_AGENDAMENTO"]), out perfilAgendamento) == false)
                                    {
                                        perfilAgendamento = await repositoryPerfilAgendamento.selectFirst(p => p.Id == Convert.ToInt64(reader["ID_PERFIL_AGENDAMENTO"]));
                                        if (perfilAgendamento != null)
                                            cachePerfilAgendamento.Add(perfilAgendamento.Id, perfilAgendamento);
                                    }
                                }

                            }


                            tipo = equipeAnielBusiness.GetStatusPersonalizado(DateTime.Now.Date,
                                                                              horario_ConnectPadrao,
                                                                              feriado,
                                                                              horarioEquipeConnect,
                                                                              tipo,
                                                                              perfilAgendamento,
                                                                              obj.Equipe,
                                                                              config.Tempo_Execucao_Padrao.Value.TimeOfDay);

                            obj.Cor_StatusEquipe = GetCorStatusEquipe(tipo);
                            obj.Restantes = Convert.ToString(Convert.ToInt32(obj.TotalVisitas) - (Convert.ToInt32(obj.BaixadasProdutivas) + Convert.ToInt32(obj.BaixadasImprodutivas)));

                            obj.Pontos_Previstos = reader["QTDE_PREV"] != DBNull.Value ? Convert.ToDouble(reader["QTDE_PREV"]).ToString("N2") : "0,00";
                            obj.Pontos_Realizados = reader["QTDE_PROD"] != DBNull.Value ? Convert.ToDouble(reader["QTDE_PROD"]).ToString("N2") : "0,00";

                            retorno.Add(obj);
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
                    case 1:     // Equipe
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Nome);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Nome);
                            break;
                        }
                        //case 2:     // Nome
                        //    {
                        //        if (sortDirection == "asc")
                        //            listaOrdenada = retorno.OrderBy(p => p.Nome);
                        //        else
                        //            listaOrdenada = retorno.OrderByDescending(p => p.Nome);
                        //        break;
                        //    }
                }
            }

            if (listaOrdenada == null)
            {
                listaOrdenada = retorno.OrderBy(p => p.Nome);
            }

            recordsTotal = listaOrdenada.Count();

            //Page e retornando DataTable
            return new DataTableResponse<Acompanhamento_EquipeListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        public async Task<Boolean> Afastado(ISession session, String equipe, DateTime? Data)
        {
            Boolean retorno = false;

            try
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(@"SELECT COALESCE(B.SITUACAO,0) AS SITUACAO,A.ENCERRAMENTO,B.DATA_DEMISSAO
                                        FROM TB_EQUIPE A
                                        INNER JOIN TB_FUNCIONARIO B
                                            ON(A.NUM_FRE = B.NUM_FRE)
                                        WHERE A.EQUIPE = @EQUIPE");

                    parametros.Add("@EQUIPE", equipe);

                    if (Data.HasValue)
                    {
                        sql.AppendLine(@" AND ((B.DATA_DEMISSAO <= @DATA) 
                                            or (B.SITUACAO<>0) or(A.ENCERRAMENTO IS NOT NULL) )");
                        parametros.Add("@DATA", Data);
                    }

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            Int32 _situacao = reader["SITUACAO"] != DBNull.Value ? Convert.ToInt32(reader["SITUACAO"]) : 1;
                            DateTime? _encerramento = reader["ENCERRAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["ENCERRAMENTO"]) : (DateTime?)null;
                            DateTime? _demissao = reader["DATA_DEMISSAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_DEMISSAO"]) : (DateTime?)null;

                            if (!(new List<Int32>() { 0, 3 }.Exists(p => p == _situacao)) || _encerramento.HasValue || _demissao.HasValue)
                                retorno = true;
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

        public async Task<Boolean> VerifFechtoEquipe(ISession session, String tiposaldo, String codct, String projeto, String equipe, DateTime? Data_Movto, Boolean GerarException)
        {
            Boolean retorno = false;

            try
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(@"Select rDATA_FINAL as DATA_FINAL From SP_VERIF_DATA_FECHTO_EQUIPE(@pTIPOSALDO,@pCODCT,@pPROJETO,@pEQUIPE)");

                    parametros.Add("@pTIPOSALDO", tiposaldo);
                    parametros.Add("@pCODCT", codct);
                    parametros.Add("@pPROJETO", projeto);
                    parametros.Add("@pEQUIPE", equipe);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            var data_Final = reader["DATA_FINAL"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_FINAL"]) : (DateTime?)null;

                            if (Data_Movto <= data_Final)
                            {
                                retorno = true;
                                if (GerarException)
                                    throw new Exception(String.Format(RsCadastros.EX_EQUIPE_INVENTARIO_PERIODO, equipe, data_Final.Value.ToString("dd/MM/yyyy")));
                            }
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

        public bool EquipeEstaEmIntervalo(AgendamentoDiaModel configDoDia, DateTime data, EquipeStatusTipo? tipo)
        {

            if (tipo.HasValue)
            {
                if (configDoDia != null && configDoDia.HoraInicioIntervalo.HasValue)
                {
                    if (data >= configDoDia.HoraInicioIntervalo && data <= configDoDia.HoraTerminoIntervalo && (tipo != EquipeStatusTipo.Em_Atendimento && tipo != EquipeStatusTipo.Em_Deslocamento))
                        return true;
                }
            }
            return false;
        }


        public bool EquipeSemAgenda(AgendamentoDiaModel configDoDia, DateTime data, EquipeStatusTipo? tipo)
        {

            if (tipo.HasValue)
            {
                if (configDoDia != null && configDoDia.HoraInicio.HasValue)
                {
                    if (DateTime.Now <= configDoDia.HoraInicio && (tipo != EquipeStatusTipo.Em_Atendimento && tipo != EquipeStatusTipo.Em_Deslocamento
                        && tipo != EquipeStatusTipo.Disponivel && tipo != EquipeStatusTipo.Em_Intervalo))
                        return true;
                }
            }
            return false;
        }

        //[HttpPost()]
        [AcceptVerbs("GET", "POST")]
        [Route("api/EquipeApi/GetAllSimple")]
        public async Task<DataTableResponse<EquipeListaModel>> GetAllSimple(DefinirTecnicosRoteirizacaoDataTableRequest request)
        {
            List<EquipeListaModel> retorno = new List<EquipeListaModel>();
            IOrderedEnumerable<EquipeListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                StringBuilder sqlFrom = new StringBuilder();
                sqlFrom.AppendLine(" FROM TB_EQUIPE A ");
                sqlFrom.AppendLine(" LEFT JOIN TB_FUNCIONARIO D ");
                sqlFrom.AppendLine("    ON(A.NUM_FRE = D.NUM_FRE) ");
                sqlFrom.AppendLine(" WHERE D.SITUACAO IN ('0','4') AND A.EQUIPE_CONNECT = 'S' ");

                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (request.search.value.Length <= 10)
                    {
                        sqlFrom.AppendLine(" AND ((UPPER(A.EQUIPE) LIKE UPPER(@SEARCH)) OR (UPPER(A.NOME) LIKE UPPER(@SEARCH))) ");
                    }
                    else
                    {
                        sqlFrom.AppendLine(" AND (UPPER(A.NOME) LIKE UPPER(@SEARCH)) ");
                    }
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }

                if (!String.IsNullOrEmpty(request.Empresa))
                {
                    sqlFrom.AppendLine("AND (D.CODEMPRESA = @EMPRESA)");
                    parametros.Add("@EMPRESA", request.Empresa);

                    if (!String.IsNullOrEmpty(request.Filial))
                    {
                        sqlFrom.AppendLine("AND (D.CODFILIAL = @FILIAL)");
                        parametros.Add("@FILIAL", request.Filial);
                    }
                }

                if (request.Supervisor.Count > 0)
                {
                    var condicoesSupervisores = new List<string>();
                    for (int i = 1; i <= request.Supervisor.Count; i++)
                    {
                        condicoesSupervisores.Add($"(A.COD_SUPERV = @COD_SUPERV{i})");
                        parametros.Add($"@COD_SUPERV{i}", request.Supervisor[i - 1].ToUpper());
                    }

                    sqlFrom.AppendLine($" AND ({String.Join(" OR ", condicoesSupervisores)})");
                }
                if (request.Monitor.Count > 0)
                {
                    var condicoesMonitores = new List<string>();
                    for (int i = 1; i <= request.Monitor.Count; i++)
                    {
                        condicoesMonitores.Add($"(A.USUARIO_MONITOR = @ID_MONITOR{i})");
                        parametros.Add($"@ID_MONITOR{i}", request.Monitor[i - 1].ToUpper());
                    }

                    sqlFrom.AppendLine($" AND ({String.Join(" OR ", condicoesMonitores)})");
                }
                if (request.IdPerfilAgendamentoRoteirizacao.Count > 0)
                {
                    var condicoesPerfisAgendamento = new List<string>();
                    for (int i = 1; i <= request.IdPerfilAgendamentoRoteirizacao.Count; i++)
                    {
                        condicoesPerfisAgendamento.Add($"(A.ID_PERFIL_AGENDAMENTO = @ID_PERFIL_AGENDAMENTO{i})");
                        parametros.Add($"@ID_PERFIL_AGENDAMENTO{i}", request.IdPerfilAgendamentoRoteirizacao[i - 1].ToString());
                    }

                    sqlFrom.AppendLine($" AND ({String.Join(" OR ", condicoesPerfisAgendamento)})");
                }

                if (request.Departamento.Count > 0)
                {
                    var condicoesDepartamento = new List<string>();
                    for (int i = 1; i <= request.Departamento.Count; i++)
                    {
                        condicoesDepartamento.Add($"(D.COD_DEPTO = @COD_DEPTO{i})");
                        parametros.Add($"@COD_DEPTO{i}", request.Departamento[i - 1].ToString());
                    }

                    sqlFrom.AppendLine($" AND ({String.Join(" OR ", condicoesDepartamento)})");
                }

                List<string> configuradosParaRoteirizacao = new List<string>();
                //CONFIGURACAO DE ROTEIRIZACAO
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT EQUIPE FROM TB_USUARIO_TECNICOROTEIRIZACAO WHERE USUARIO = @USUARIO");
                    Dictionary<String, Object> parametrosUsuarioRoteirizacao = new Dictionary<string, object>();
                    parametrosUsuarioRoteirizacao.Add("@USUARIO", request.Usuario);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametrosUsuarioRoteirizacao, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            configuradosParaRoteirizacao.Add(Convert.ToString(reader["EQUIPE"]));
                        }
                    }
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.EQUIPE) CO ");
                    sql.AppendLine(sqlFrom.ToString());

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


                    sql.AppendLine(" SELECT A.EQUIPE,  A.NOME ");
                    sql.AppendLine(sqlFrom.ToString());


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new EquipeListaModel
                            {
                                Equipe = Convert.ToString(reader["EQUIPE"]),
                                Nome = Convert.ToString(reader["NOME"]),
                            });
                        }
                    }

                }

                foreach (var item in configuradosParaRoteirizacao)
                {
                    var equipeConfiguradaRoteirizacao = retorno.Find(p => p.Equipe == item);
                    if (equipeConfiguradaRoteirizacao != null)
                    {
                        equipeConfiguradaRoteirizacao.ConfiguradoRoteirizacao = true;
                    }
                }

            }

            //Ordenacao
            listaOrdenada = retorno.OrderByDescending(p => p.ConfiguradoRoteirizacao).ThenBy(p => p.Nome);

            //Page e retornando DataTable
            return new DataTableResponse<EquipeListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }


        //[HttpPost()]
        [AcceptVerbs("GET", "POST")]
        [Route("api/EquipeApi/GetAllMonitor")]
        public async Task<DataTableResponse<EquipeListaModel>> GetAllMonitor(Acompanhamento_Equipes_DataTableRequest request)
        {
            List<EquipeListaModel> retorno = new List<EquipeListaModel>();
            IOrderedEnumerable<EquipeListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<EquipeAniel>(session))
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                StringBuilder sqlWhere = new StringBuilder(" WHERE C.SITUACAO != 2 AND A.EQUIPE_CONNECT = 'S' ");

                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlWhere.AppendLine(" AND ((UPPER(A.NOME) LIKE UPPER(@SEARCH)))");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }
                if (!String.IsNullOrEmpty(request.Monitor))
                {

                    if (request.Monitor == "vinculado")
                        sqlWhere.AppendLine(" AND B.NOME IS NOT NULL  ");
                    else if (request.Monitor == "desvinculado")
                        sqlWhere.AppendLine(" AND B.NOME IS NULL  ");

                }
                if (!String.IsNullOrEmpty(request.Pesquisa))
                {
                    sqlWhere.AppendLine(" AND A.USUARIO_MONITOR = @MONITOR  ");
                    parametros.Add("@MONITOR", request.Pesquisa);

                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.EQUIPE) CO FROM TB_EQUIPE A  ");
                    sql.AppendLine(" LEFT JOIN TB_SENHA B ");
                    sql.AppendLine(" ON A.USUARIO_MONITOR = B.USUARIO ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO C ");
                    sql.AppendLine(" ON(A.NUM_FRE = C.NUM_FRE) ");
                    sql.AppendLine(sqlWhere.ToString());

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
                    sql.AppendLine(" SELECT A.EQUIPE,  A.NOME , B.NOME AS USUARIO_MONITOR ");
                    sql.AppendLine(" FROM TB_EQUIPE A ");
                    sql.AppendLine(" LEFT JOIN TB_SENHA B ");
                    sql.AppendLine(" ON A.USUARIO_MONITOR = B.USUARIO ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO C ");
                    sql.AppendLine(" ON(A.NUM_FRE = C.NUM_FRE) ");
                    sql.AppendLine(sqlWhere.ToString());


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new EquipeListaModel
                            {
                                Equipe = Convert.ToString(reader["EQUIPE"]),
                                Nome = Convert.ToString(reader["NOME"]),
                                Monitor = Convert.ToString(reader["USUARIO_MONITOR"])
                            });

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
                                listaOrdenada = retorno.OrderBy(p => p.Nome);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Nome);
                            break;
                        }

                    case 2:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Monitor);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Monitor);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<EquipeListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }
        //[HttpPost()]
        [AcceptVerbs("GET", "POST")]
        [Route("api/EquipeApi/GetLogVincularMonitorTecnico")]
        public async Task<DataTableResponse<LogVincularMonitorTecnicoListaModel>> GetLogVincularMonitorTecnico(LogVincularMonitorTecnico_DatatableRequest request)
        {
            IOrderedEnumerable<LogVincularMonitorTecnicoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;
            List<LogVincularMonitorTecnicoListaModel> retorno = new List<LogVincularMonitorTecnicoListaModel>();

            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (var repository = new Conexao<EquipeAniel>(sessionLog))
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = sessionLog.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_LOG_VINCULAR_MONITOR_TECNICO A ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionLog.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = sessionLog.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.USUARIOCRIACAO,A.DATAHORACRIACAO,A.TIPOOPERACAO,A.TECNICO,A.MONITOR ");
                    sql.AppendLine(" FROM TB_LOG_VINCULAR_MONITOR_TECNICO A ");
                    sql.AppendLine("  WHERE A.ID <>0 ");

                    #region Filtros

                    if (!String.IsNullOrEmpty(request.Tecnico) && request.Tecnico != "Selecione\n")
                    {
                        sql.AppendLine(" AND A.TECNICO = @TECNICO  ");
                        parametros.Add("@TECNICO", request.Tecnico);
                    }
                    if ((!String.IsNullOrEmpty(request.Dt_Inicial_Filtro)) && (!String.IsNullOrEmpty(request.Dt_Final_Filtro)))
                    {
                        sql.AppendLine(" AND (A.DATAHORACRIACAO BETWEEN @DATAINI AND @DATAFIM) ");
                        parametros.Add("@DATAINI", DateTime.Parse(String.Format("{0} 00:00", request.Dt_Inicial_Filtro)));
                        parametros.Add("@DATAFIM", DateTime.Parse(String.Format("{0} 23:59", request.Dt_Final_Filtro)));
                    }
                    #endregion

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionLog.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new LogVincularMonitorTecnicoListaModel
                            {
                                Tecnico = reader["TECNICO"] != DBNull.Value ? Convert.ToString(reader["TECNICO"]) : "N/D",
                                Monitor = reader["MONITOR"] != DBNull.Value ? Convert.ToString(reader["MONITOR"]) : "N/D",
                                UsuarioCriacao = reader["USUARIOCRIACAO"] != DBNull.Value ? Convert.ToString(reader["USUARIOCRIACAO"]) : "N/D",
                                DataAlteracao = Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm"),
                                TipoOperacao = reader["TIPOOPERACAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(reader["TIPOOPERACAO"])) : "N/D"

                            });
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
                                listaOrdenada = retorno.OrderBy(p => p.Tecnico);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Tecnico);
                            break;
                        }

                    case 2:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Monitor);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Monitor);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<LogVincularMonitorTecnicoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }


        [HttpGet]
        [Route("api/EquipeApi/RemoverVinculoMonitor")]
        public async Task<PostApiRetorno<Int64>> RemoverVinculoMonitor(string ids)
        {
            List<String> ArgumentsIds = JsonConvert.DeserializeObject<List<String>>(ids);
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repository = new Conexao<EquipeAniel>(session))
                using (var repositoryLog = new Conexao<LogVincularMonitorTecnico>(sessionLog))
                {
                    EquipeAniel modelo = null;
                    EquipeAniel modeloAnterior = null;

                    List<EquipeListaModel> lista = new List<EquipeListaModel>();
                    List<EquipeAniel> equipe = (await repository.select(p => p.Matricula.IsIn(ArgumentsIds))).ToList();

                    foreach (var item in equipe)
                    {
                        modelo = (await repository.select(p => p.Matricula == item.Matricula)).FirstOrDefault();
                        modeloAnterior = (EquipeAniel)modelo.Clone();

                        item.Monitor = null;
                        item.DataHoraAlteracao = DateTime.Now;
                        item.UsuarioAlteracao = User.Identity.GetUserId();

                        try
                        {
                            var usuarioAlteracao = User.Identity.GetUserId();

                            var logs = modelo.GeraLogVincularMonitorTecnico(modeloAnterior, usuarioAlteracao, modelo.Nome, Sinapse.ModelLog.TipoOperLog.Exclusao);

                            await repository.Update(modelo);
                            await repositoryLog.Save(logs);

                        }
                        catch (Exception ex)
                        {
                            retorno.Mensagem = ex.Message;
                            if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                        }

                        var notificacoes = new NotificacoesApiController();
                        var retornoNotificacao = notificacoes.EnviaNotificacoesServico(12, null, null, null, null, modeloAnterior.Monitor).Result;


                        await repository.SaveOrUpdate(item);
                    }
                    //var lista_modificada = equipe.Select(p=>{ p.Monitor = null;p.DataHoraAlteracao = DateTime.Now; p.UsuarioAlteracao = User.Identity.GetUserId(); return p; }).ToList();

                    retorno.IdObjeto = lista.Count();
                    retorno.Ok = true;
                }

            }
            catch (Exception e)
            {

                retorno.IdObjeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = e.Message;
                retorno.InnerException = e.InnerException?.Message;

            }

            return retorno;
        }




        [HttpGet()]
        [Route("api/EquipeApi/buscamonitor")]
        public async Task<List<ObjBusca<String>>> buscaMonitor(String term)
        {
            List<ObjBusca<String>> retorno = new List<ObjBusca<String>>();

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

                        sql.AppendLine("SELECT A.ID, A.USERNAME");
                        sql.AppendLine("FROM ASPNETUSERS A");


                        sql.AppendLine($"WHERE (UPPER(A.USERNAME) LIKE UPPER('%{term}%')) AND A.SECAO_CONNECT = 2 AND A.TIPO_USUARIO = 4  ");

                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<String>
                                {
                                    Value = Convert.ToString(reader["ID"]),
                                    Label = Convert.ToString(reader["USERNAME"])
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

        [AcceptVerbs("GET", "POST")]
        [Route("api/EquipeApi/GetAllSimpleArea")]
        public async Task<DataTableResponse<EquipeListaModel>> GetAllSimpleArea(Acompanhamento_Equipes_DataTableRequest request)
        {
            List<EquipeListaModel> retorno = new List<EquipeListaModel>();
            IOrderedEnumerable<EquipeListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(EQUIPE) CO FROM TB_EQUIPE A WHERE A.EQUIPE_CONNECT = 'S' ");

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

                    sql.AppendLine(" SELECT A.EQUIPE,  A.NOME , COUNT (B.COD_AREA) COUNT_AREA ");
                    sql.AppendLine(" FROM TB_EQUIPE A ");

                    sql.AppendLine(" LEFT JOIN TB_EQUIPE_LOCALIDADE B ");
                    sql.AppendLine(" ON A.EQUIPE = B.EQUIPE ");

                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO C ");
                    sql.AppendLine(" ON(A.NUM_FRE = C.NUM_FRE) ");
                    sql.AppendLine(" WHERE C.SITUACAO != 2 AND A.EQUIPE_CONNECT = 'S' ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        if (request.search.value.Length <= 10)
                            sql.AppendLine(" AND ((UPPER(A.EQUIPE) LIKE UPPER(@SEARCH)) OR (UPPER(A.NOME) LIKE UPPER(@SEARCH))) ");
                        else
                            sql.AppendLine(" AND (UPPER(A.NOME) LIKE UPPER(@SEARCH)) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (!String.IsNullOrEmpty(request.GrupoArea))
                    {
                        sql.AppendLine(" AND B.COD_ESCRITORIO = @GRUPOAREA ");
                        parametros.Add("@GRUPOAREA", request.GrupoArea);
                    }

                    if (!String.IsNullOrEmpty(request.Area))
                    {
                        sql.AppendLine(" AND B.COD_AREA = @AREA ");
                        parametros.Add("@AREA", request.Area);
                    }

                    if (!String.IsNullOrEmpty(request.Localidade))
                    {
                        sql.AppendLine(" AND B.COD_LOCALIDADE = @LOCALIDADE ");
                        parametros.Add("@LOCALIDADE", request.Localidade);
                    }




                    sql.AppendLine(" GROUP BY  A.EQUIPE,  A.NOME ");
                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new EquipeListaModel
                            {
                                Equipe = Convert.ToString(reader["EQUIPE"]),
                                Nome = Convert.ToString(reader["NOME"]),
                                Descricao = Convert.ToInt32(reader["COUNT_AREA"]) > 0 ? Resources.RsBase.SIM : Resources.RsBase.NAO
                            });
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
                                listaOrdenada = retorno.OrderBy(p => p.Equipe);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Equipe);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<EquipeListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/EquipeApi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboConfig<string, EquipeComboConfig> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_EQUIPE A ")
                        .AppendLine("LEFT JOIN TB_FUNCIONARIO B ")
                        .AppendLine("ON (A.NUM_FRE = B.NUM_FRE) ");

                    List<String> condicoes = new List<String>();



                    if (request.soativos && !Convert.ToBoolean(request.id_usuario))
                    {
                        condicoes.Add($"B.SITUACAO IN (0,4)");
                    }


                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.EQUIPE) = '{request.id.ToUpperInvariant()}'");
                    }
                    else
                    {
                        condicoes.Add($" (A.EQUIPE_CONNECT = 'S') ");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.EQUIPE) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 10)}%' OR " +
                                      $" UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (request.config != null && request.config.ExibirApenasTecnicosSupervisorMonitor)
                    {
                        using (ISession session = HibernateUtil.GetSession())
                        {
                            IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                            var usuarioBusiness = new UsuarioBusiness(repoUsuario);

                            string userId = User.Identity.GetUserId();
                            IdentityUser user = usuarioBusiness.GetById(userId);

                            condicoes.Add($"(A.COD_SUPERV = @COD_SUPERV OR A.USUARIO_MONITOR = @USUARIO_MONITOR)");

                            parametros.Add("@USUARIO_MONITOR", userId);
                            parametros.Add("@COD_SUPERV", user.Funcionario?.Num_Fre);
                        }
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.EQUIPE) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.EQUIPE, A.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, sessionT5.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["EQUIPE"]),
                                    Text = Convert.ToString(reader["NOME"])
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
        [Route("api/EquipeApi/GetComboAtribuirTecnico")]
        public async Task<ObjCombo<String>> GetComboAtribuirTecnico(RequestObjCombo<String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session_connect = HibernateUtil.GetSession())
            using (ISession session = HibernateUtil.GetSessionT5())
            {

                try
                {
                    Configuracoes_Gerais config = await new Conexao<Configuracoes_Gerais>(session_connect).selectFirst();

                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_EQUIPE A ")
                        .AppendLine("LEFT JOIN TB_FUNCIONARIO B ")
                        .AppendLine("ON (A.NUM_FRE = B.NUM_FRE) ")

                        .AppendLine("LEFT JOIN TB_EQUIPE_STATUS C ")
                        .AppendLine("ON(A.EQUIPE = C.EQUIPE) ")

                        .AppendLine("LEFT JOIN TB_PONTO_PARTIDA D ")
                        .AppendLine("ON(A.ID_PONTO_PARTIDA = D.ID) ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"B.SITUACAO IN (0,4) AND C.TIPO <> 1");
                    }



                    if (config.UtilizaPoint_RoteirizacaoAgendamento == false)
                    {
                        condicoes.Add($@" (NOT EXISTS(SELECT FIRST 1 H.ID
                                        FROM TB_HORARIO_EQUIPE_CONNECT H
                                        LEFT JOIN TB_HORARIO_CONNECT I
                                        ON(H.ID_HORARIO = I.ID)
                                        WHERE H.EQUIPE = A.EQUIPE AND H.APENAS_NO_DIA = 1
                                        AND H.DATA = '{DateTime.Today.ToString("dd.MM.yyyy")}' AND H.INDISPONIVEL = 1))");
                    }
                    else
                    {
                        condicoes.Add($@" (NOT EXISTS (SELECT FIRST 1 H.ID FROM TB_FUNCIONARIO H
                                     INNER JOIN TB_PT_DIA_FUNCIONARIO I ON(H.NUM_FRE = I.NUM_FRE)
                                     LEFT JOIN TB_PT_CLASSIFICACAO_ESCALA J ON(I.ID_EVENTO = J.ID)
                                     WHERE J.PERMITIR_MARCAR = 0 AND H.NUM_FRE = A.NUM_FRE AND I.DATA = '{DateTime.Today.ToString("dd.MM.yyyy")}'))");
                    }



                    //Fim

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.EQUIPE) = '{request.id.ToUpperInvariant()}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.EQUIPE) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 10)}%' OR " +
                                      $" UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.EQUIPE) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.EQUIPE, A.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["EQUIPE"]),
                                    Text = Convert.ToString(reader["NOME"])
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
        [Route("api/EquipeApi/GetComboAtribuirTecnicoCriterios")]
        public async Task<ObjCombo<String>> GetComboAtribuirTecnicoCriterios(RequestObjComboAtribuirTecnicoCrit<String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            List<AreaModuloModel> areas = new List<AreaModuloModel>();

            using (var acessoApi = new AcessoApiController())
            using (ISession session_connect = HibernateUtil.GetSession())
            using (ISession session = HibernateUtil.GetSessionT5())
            {

                try
                {
                    areas = acessoApi.GetArea_ModuloUsuario(User.Identity.GetUserId()).Result.Objeto;
                    Configuracoes_Gerais config = await new Conexao<Configuracoes_Gerais>(session_connect).selectFirst();

                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_EQUIPE A ")
                        .AppendLine("LEFT JOIN TB_FUNCIONARIO B ")
                        .AppendLine("ON (A.NUM_FRE = B.NUM_FRE) ")

                        .AppendLine("LEFT JOIN TB_EQUIPE_STATUS C ")
                        .AppendLine("ON(A.EQUIPE = C.EQUIPE) ")

                        .AppendLine("LEFT JOIN TB_PONTO_PARTIDA D ")
                        .AppendLine("ON(A.ID_PONTO_PARTIDA = D.ID) ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"B.SITUACAO IN (0,4) AND COALESCE(C.TIPO, 0) <> 1");
                    }

                    //199 - Atribuir Técnico com regra (área de atuação, natureza e habilidade)
                    if (!areas.Exists(p => p.Cod_Area == "199"))
                    {
                        Boolean _temCriterioArea = config.Criterio_Area;
                        Boolean _temCriterioHabilidades = config.Criterio_Habilidade;
                        Boolean _temCriterioSLA_Execucao = config.Criterio_SLA_Execucao;

                        if (_temCriterioArea)
                        {
                            condicoes.Add("   A.EQUIPE IN(SELECT C.EQUIPE FROM TB_EQUIPE_LOCALIDADE C " +
                               $"                    WHERE C.COD_ESCRITORIO = @GRUPO_AREA " +
                               $"                      AND C.COD_AREA = @AREA " +
                               $"                      AND (C.COD_LOCALIDADE = @LOCALIDADE OR C.TODAS_LOCALIDADES = 1)) ");

                            parametros.Add("@GRUPO_AREA", request.config?.Grupo_Area);
                            parametros.Add("@AREA", request.config?.Area);
                            parametros.Add("@LOCALIDADE", request.config?.Cod_Localidade);

                        }

                        if (_temCriterioHabilidades && !String.IsNullOrEmpty(request.config?.Tipo_serv))
                        {
                            condicoes.Add("    A.EQUIPE IN(SELECT D.EQUIPE " +
                               "                      FROM TB_EQUIPE_NATUREZA D " +
                               "                     INNER JOIN TB_TIPO_SERVICO_EQUIPE E " +
                               "                        ON (D.ID_NATUREZA = E.COD_NATUREZA) " +
                               $"                     WHERE E.COD_TIPO_SERV = @TIPO_SERV) ");

                            parametros.Add("@TIPO_SERV", request.config?.Tipo_serv);
                        }
                    }

                    //188 - Permitir agendamentos fora de escala e independente de disponibilidade do técnico
                    //Se não possuir a área de acesso, faz as validações de escala e disponibilidade de atendimento
                    if (!areas.Exists(p => p.Cod_Area == "188"))
                    {
                        if (config.UtilizaPoint_RoteirizacaoAgendamento == false)
                        {
                            condicoes.Add($@" (NOT EXISTS(SELECT FIRST 1 H.ID
                                        FROM TB_HORARIO_EQUIPE_CONNECT H
                                        LEFT JOIN TB_HORARIO_CONNECT I
                                        ON(H.ID_HORARIO = I.ID)
                                        WHERE H.EQUIPE = A.EQUIPE AND H.APENAS_NO_DIA = 1
                                        AND H.DATA = '{DateTime.Today.ToString("dd.MM.yyyy")}' AND H.INDISPONIVEL = 1))");
                        }
                        else
                        {
                            condicoes.Add($@" (NOT EXISTS (SELECT FIRST 1 H.ID FROM TB_FUNCIONARIO H
                                     INNER JOIN TB_PT_DIA_FUNCIONARIO I ON(H.NUM_FRE = I.NUM_FRE)
                                     LEFT JOIN TB_PT_CLASSIFICACAO_ESCALA J ON(I.ID_EVENTO = J.ID)
                                     WHERE J.PERMITIR_MARCAR = 0 AND H.NUM_FRE = A.NUM_FRE AND I.DATA = '{DateTime.Today.ToString("dd.MM.yyyy")}'))");
                        }
                    }




                    condicoes.Add($" (A.EQUIPE_CONNECT = 'S') ");

                    //Fim

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.EQUIPE) = '{request.id.ToUpperInvariant()}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.EQUIPE) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 10)}%' OR " +
                                      $" UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.EQUIPE) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.EQUIPE, A.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["EQUIPE"]),
                                    Text = Convert.ToString(reader["NOME"])
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
        [Route("api/EquipeApi/GetComboNumFre")]
        public async Task<ObjCombo<String>> GetComboNumFre(RequestObjCombo<String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_EQUIPE A ")
                        .AppendLine("LEFT JOIN TB_FUNCIONARIO B ")
                        .AppendLine("ON (A.NUM_FRE = B.NUM_FRE) ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"B.SITUACAO IN (0,4)");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.NUM_FRE) = '{request.id.ToUpperInvariant()}'");
                    }
                    else
                    {
                        condicoes.Add($" (A.EQUIPE_CONNECT = 'S') ");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.EQUIPE) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 10)}%' OR " +
                                      $" UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.EQUIPE) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.EQUIPE, A.NOME, A.TIPO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["EQUIPE"]),
                                    Text = Convert.ToString(reader["NOME"])
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

        [HttpPost]
        [Route("api/EquipeApi/GetComboHabilidade")]
        public async Task<ObjCombo<String>> GetComboHabilidade(RequestObjCombo<String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    StringBuilder sqlFrom = new StringBuilder("  FROM TB_NATUREZA_PRODUCAO A ");

                    if (!String.IsNullOrEmpty(request.q))
                    {
                        sqlFrom.AppendLine(" WHERE UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)  ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.q));
                    }


                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID AS ID, A.DESCRICAO DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"])
                                };

                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/EquipeApi/GetComboCriterios")]
        public async Task<ObjCombo<String>> GetComboCriterios(RequestObjComboCriterios<String, String, String, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            //List<AreaModuloModel> areas = new List<AreaModuloModel>();

            //using (var acessoApi = new AcessoApiController())
            using (ISession session_connect = HibernateUtil.GetSession())
            using (ISession session = HibernateUtil.GetSessionT5())
            using (ISession sessionNH = HibernateUtil.GetSessionT5())
            using (var repoDocumento = new Conexao<Documento_ProducaoAniel>(sessionNH))
            {
                try
                {
                    //areas = acessoApi.GetArea_ModuloUsuario(User.Identity.GetUserId()).Result.Objeto;
                    Configuracoes_Gerais config = await new Conexao<Configuracoes_Gerais>(session_connect).selectFirst();
                    var doc = await repoDocumento.selectFirst(p => p.CodCt == request.CodCt && p.Projeto == request.Projeto && p.Num_Doc == request.NumDoc && p.Num_Obra == request.NumObra);

                    Boolean _temCriterioArea = config.Criterio_Area;
                    Boolean _temCriterioHabilidades = config.Criterio_Habilidade;
                    Boolean _temCriterioSLA_Execucao = config.Criterio_SLA_Execucao;


                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_EQUIPE A ")
                        .AppendLine("LEFT JOIN TB_FUNCIONARIO B ")
                        .AppendLine("ON (A.NUM_FRE = B.NUM_FRE) ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"B.SITUACAO IN (0,4)");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.NUM_FRE) = '{request.id.ToUpperInvariant()}'");
                    }
                    else
                    {


                        condicoes.Add($" (A.EQUIPE_CONNECT = 'S') ");


                        if (request != null && !String.IsNullOrEmpty(request.q))
                        {
                            condicoes.Add($"( UPPER(A.EQUIPE) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 10)}%' OR " +
                                          $" UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                        }

                        if (_temCriterioArea)
                        {
                            condicoes.Add("   A.ID_PERFIL_AGENDAMENTO IN(SELECT C.ID_PERFIL_AGENDAMENTO FROM TB_PERFIL_AGENDAMENTO_AREA C " +
                               $"                    WHERE C.COD_ESCRITORIO = @GRUPO_AREA " +
                               $"                      AND C.COD_AREA = @AREA " +
                               $"                      AND (C.LOCALIDADE = @LOCALIDADE OR C.TODAS_LOCALIDADES = 1)) ");

                            parametros.Add("@GRUPO_AREA", request.Grupo_Area);
                            parametros.Add("@AREA", request.Area);
                            parametros.Add("@LOCALIDADE", request.Cod_Localidade);

                        }

                        if (_temCriterioHabilidades && !String.IsNullOrEmpty(request.Tipo_serv))
                        {
                            condicoes.Add("    A.ID_PERFIL_AGENDAMENTO IN(SELECT D.ID_PERFIL_AGENDAMENTO " +
                               "                      FROM TB_PERFIL_AGENDAMENTO_NATUREZA D " +
                               "                     INNER JOIN TB_TIPO_SERVICO_EQUIPE E " +
                               "                        ON (D.ID_NATUREZA = E.COD_NATUREZA) " +
                               $"                     WHERE E.COD_TIPO_SERV = @TIPO_SERV) ");

                            parametros.Add("@TIPO_SERV", request.Tipo_serv);
                        }
                    }


                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.EQUIPE) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.EQUIPE, A.NOME,B.HORARIO_ATRIBUIDO, ");
                        sql.AppendLine("(SELECT FIRST 1 H.ID");
                        sql.AppendLine("FROM TB_HORARIO_EQUIPE_CONNECT H");
                        sql.AppendLine("LEFT JOIN TB_HORARIO_CONNECT I");
                        sql.AppendLine("ON(H.ID_HORARIO = I.ID)");
                        sql.AppendLine("WHERE H.EQUIPE = A.EQUIPE AND H.APENAS_NO_DIA = 1");
                        sql.AppendLine("AND H.DATA = @DATA AND H.INDISPONIVEL = 1)");
                        sql.AppendLine(" AS INDISPONIVEL_CONNECT, ");

                        sql.AppendLine("(SELECT FIRST 1 A.NUM_FRE FROM TB_FUNCIONARIO J");
                        sql.AppendLine("INNER JOIN TB_PT_DIA_FUNCIONARIO K ON(J.NUM_FRE = K.NUM_FRE)");
                        sql.AppendLine("LEFT JOIN TB_PT_CLASSIFICACAO_ESCALA M ON(K.ID_EVENTO = M.ID)");
                        sql.AppendLine("WHERE M.PERMTITE_ROTEIRIZAR = 0 AND J.NUM_FRE = B.NUM_FRE AND B.DATA = @DATA)");
                        sql.AppendLine(" AS INDISPONIVEL_POINT ");

                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME) ");
                        parametros.Add("@DATA", doc.Data_Maxima);

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                if (String.IsNullOrEmpty(request.id) && doc.Data_Maxima.HasValue)
                                {
                                    HorarioAtribuido horarioAtribuido = reader["HORARIO_ATRIBUIDO"] != DBNull.Value ?
                                                                                                                        (HorarioAtribuido)Enum.Parse(typeof(HorarioAtribuido), Convert.ToString(reader["HORARIO_ATRIBUIDO"]))
                                                                                                                    :
                                                                                                                        (config.UtilizaPoint_RoteirizacaoAgendamento ? HorarioAtribuido.AnielPoint : HorarioAtribuido.AnielConnect);
                                    if (horarioAtribuido == HorarioAtribuido.AnielPoint)
                                    {
                                        var indisponivelPoint = reader["INDISPONIVEL_POINT"] != DBNull.Value ? Convert.ToString(reader["INDISPONIVEL_POINT"]) : null;
                                        if (String.IsNullOrEmpty(indisponivelPoint) == false)
                                            continue;

                                    }
                                    else
                                    {
                                        var indisponivelConnect = reader["INDISPONIVEL_CONNECT"] != DBNull.Value ? Convert.ToBoolean(reader["INDISPONIVEL_CONNECT"]) : false;
                                        if (indisponivelConnect)
                                            continue;
                                    }
                                }

                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["EQUIPE"]),
                                    Text = Convert.ToString(reader["NOME"])
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
        [Route("api/EquipeApi/GetStatusEquipe")]
        public async Task<GetApiRetorno> GetStatusEquipe(String Equipe)
        {
            GetApiRetorno<EquipeStatusTipo> retorno = new GetApiRetorno<EquipeStatusTipo>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.TIPO FROM TB_EQUIPE_STATUS  A ")
                        .AppendLine("WHERE A.EQUIPE = @EQUIPE");

                        parametros.Add("@EQUIPE", Equipe);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                retorno.Objeto = (EquipeStatusTipo)Enum.Parse(typeof(EquipeStatusTipo), Convert.ToString(reader["TIPO"]));

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    retorno.InnerException = ex.InnerException?.Message;
                }
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/EquipeApi/GetEquipeAgendamento")]
        public async Task<GetApiRetorno> GetStatusEquipe(EquipeAgendamentoModel model)
        {
            GetApiRetorno<EquipeStatusTipo> retorno = new GetApiRetorno<EquipeStatusTipo>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Agendamento_EquipeAniel>(session))
            using (var repositoryEquipe = new Conexao<EquipeAniel>(session))
            using (var api = new AcessoApiController())
            {
                GetApiRetorno<List<AreaModuloModel>> areas = api.GetArea_ModuloUsuario(model.Usuario).Result;
                bool temAcesso188 = AcessoHelper.TemAcesso(areas.Objeto, "188");

                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    //Select
                    Agendamento_EquipeAniel agendamento = await repository.selectFirst(p => p.Num_Obra == model.Numero_Os && p.Num_Doc == model.Numero_Doc && p.CodCt == model.CodCt && p.Projeto == model.Projeto);
                    EquipeAniel equipeAniel = await repositoryEquipe.selectFirst(p => p.Matricula == model.Equipe);

                    if (agendamento != null)
                    {

                        using (var com = session.Connection.CreateCommand())
                        {


                            StringBuilder sql = new StringBuilder(@"SELECT COALESCE(A.EQUIPE, A.EQUIPE_TMP) AS EQUIPE,B.NUM_OBRA    FROM TB_AGENDAMENTO_EQUIPEANIEL A
                                                         INNER JOIN TB_DOCUMENTO_PRODUCAO B ON (A.NUM_OBRA = B.NUM_OBRA AND A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_DOC = B.NUM_DOC)
                                                         WHERE A.DATAHORA_INICIO BETWEEN @INICIO_DATA_INICIO AND @INICIO_DATA_FIM
                                                         AND B.COD_ENCERRAMENTO IS NULL
                                                         AND A.EQUIPE = @EQUIPE   
                                                         
                                                         UNION

                                                         SELECT COALESCE(A.EQUIPE, A.EQUIPE_TMP) AS EQUIPE,B.NUM_OBRA    FROM TB_AGENDAMENTO_EQUIPEANIEL A
                                                         INNER JOIN TB_DOCUMENTO_PRODUCAO B ON (A.NUM_OBRA = B.NUM_OBRA AND A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_DOC = B.NUM_DOC)
                                                         WHERE A.DATAHORA_FIM BETWEEN @FIM_DATA_INICIO AND @FIM_DATA_FIM
                                                         AND B.COD_ENCERRAMENTO IS NULL
                                                         AND A.EQUIPE = @EQUIPE");

                            parametros.Add("@INICIO_DATA_INICIO", agendamento.DataHora_Inicio);
                            parametros.Add("@INICIO_DATA_FIM", agendamento.DataHora_Fim.AddSeconds(-1));

                            parametros.Add("@FIM_DATA_INICIO", agendamento.DataHora_Inicio.AddSeconds(1));
                            parametros.Add("@FIM_DATA_FIM", agendamento.DataHora_Fim);
                            parametros.Add("@EQUIPE", model.Equipe);
                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                            {
                                if (await reader.ReadAsync())
                                {

                                    if (String.IsNullOrEmpty(Convert.ToString(reader["EQUIPE"])))
                                    {
                                        return new GetApiRetorno
                                        {
                                            Ok = true
                                        };
                                    }
                                    else
                                    {

                                        string numeroOS = Convert.ToString(reader["NUM_OBRA"]);
                                        return new GetApiRetorno
                                        {
                                            Ok = false,
                                            Mensagem = temAcesso188 ?
                                                    $"O técnico {equipeAniel.Nome} já possui o serviço {numeroOS} agendado para a mesma faixa de horário. Deseja realmente realizar esta atribuição?"
                                                    : $"Não será possível atribuir o técnico {equipeAniel.Nome} para este serviço, pois ele já possui o serviço {numeroOS} agendado para a mesma faixa de horário."
                                        };
                                    }



                                }
                            }

                            return new GetApiRetorno
                            {
                                Ok = true
                            };
                        }
                    }
                    else
                    {
                        return new GetApiRetorno
                        {
                            Ok = true
                        };
                    }
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    retorno.InnerException = ex.InnerException?.Message;
                }
            }
            return retorno;

        }

        public static String GetCorStatusEquipe(EquipeStatusTipo? Status)
        {
            try
            {
                if (!Status.HasValue)
                    return "#38761d";

                switch (Status)
                {
                    case EquipeStatusTipo.Disponivel:
                        return "#38761d";
                    case EquipeStatusTipo.Ausente:
                        return "#d6915c";
                    case EquipeStatusTipo.Em_Atendimento:
                        return "#1b61e3";
                    case EquipeStatusTipo.Sem_Agenda:
                        return "#ff001d";
                    case EquipeStatusTipo.Em_Deslocamento:
                        return "#6bc6fa";
                    case EquipeStatusTipo.Em_Intervalo:
                        return "#989a9e";
                    case EquipeStatusTipo.Atendimento_Fora_Horario:
                        return "#652E80";
                    case EquipeStatusTipo.Fora_Horario_Trabalho:
                        return "#FF00B8";
                    default:
                        return "#38761d"; //Sem Status está a mesma cor (verde) que disponível.
                }
            }
            catch
            {
                return "#38761d";
            }
        }

        private String GetCorIcone(String Porcentagem)
        {
            try
            {
                var Tmp = Convert.ToInt32(Porcentagem);
                if (Tmp >= 90)
                    return "#f8b317"; //LARANJA
                else if (Tmp >= 40)
                    return "#0cff00"; //VERDE
                else if (Tmp >= 20)
                    return "#ffd000"; //LARANJA
                else if (Tmp >= 1)
                    return "#ff001d"; //VERMELHO
                else
                    return "#ffffff"; //BRANCO
            }
            catch
            {
                return "battery_unknown"; //PRETO COM INTERROGAÇÂO
            }
        }

        private String GetIcone(String Porcentagem, TipoCarregamentoBateria Carregamento)
        {
            try
            {
                var Tmp = Convert.ToInt32(Porcentagem);

                if (Carregamento == TipoCarregamentoBateria.Sem_Carregar)
                {
                    if (Tmp >= 90)
                        return "battery_std"; //VERDE 100 % - Carga Completa
                    else if (Tmp >= 80)
                        return "battery_80"; //VERDE 80 % - Carga Incompleta
                    else if (Tmp >= 60)
                        return "battery_60"; //LARANJA 60 % - Carga Incompleta
                    else if (Tmp >= 50)
                        return "battery_50"; //LARANJA 50 % - Carga Incompleta
                    else if (Tmp >= 30)
                        return "battery_30"; //LARANJA 30 % - Carga Incompleta
                    else if (Tmp >= 20)
                        return "battery_20"; //VERNELHO 20 % - Carga Incompleta
                    else if (Tmp >= 1)
                        return "battery_alert"; //PRETO COM EXCLAMAÇÂO - 0% Carga Incompleta
                    else
                        return "battery_unknown"; //PRETO COM INTERROGAÇÂO
                }
                else
                {
                    if (Tmp >= 90)
                        return "battery_charging_full"; //VERDE COM RAIO 100 % - Carregando ...
                    else if (Tmp >= 80)
                        return "battery_charging_80"; //VERDE COM RAIO  80 % - Carregando ...
                    else if (Tmp >= 60)
                        return "battery_charging_60"; //LARANJA COM RAIO  60 % - Carregando ...
                    else if (Tmp >= 50)
                        return "battery_charging_50"; //LARANJA COM RAIO  50 % - Carregando ...
                    else if (Tmp >= 30)
                        return "battery_charging_30"; //LARANJA COM RAIO  30 % - Carregando ...
                    else if (Tmp >= 20)
                        return "battery_charging_20"; //VERMELHO COM RAIO  20 % - Carregando ...
                    else
                        return "battery_unknown"; //PRETO COM INTERROGAÇÂO
                }

            }
            catch
            {
                return "battery_unknown"; //PRETO COM INTERROGAÇÂO
            }
        }


        [HttpGet()]
        [Route("api/equipeapi/busca")]
        public async Task<List<ObjBusca<String>>> busca(String term)
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

                        sql.AppendLine("SELECT A.EQUIPE, A.NOME");
                        sql.AppendLine("FROM TB_EQUIPE A");
                        sql.AppendLine("WHERE ((UPPER(A.EQUIPE) LIKE @VALOR) OR (UPPER(A.NOME) LIKE @VALOR)) AND A.EQUIPE_CONNECT = 'S'");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<String>
                                {
                                    Value = Convert.ToString(reader["EQUIPE"]),
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

        [HttpGet()]
        [Route("api/equipeapi/buscaCriterios")]
        public async Task<List<ObjBusca<String>>> buscaEquipeCriterios(String term, String Tipo_serv, String grupoarea, String area, String localidade)
        {
            List<ObjBusca<String>> retorno = new List<ObjBusca<String>>();

            Boolean _temCriterioArea = false;
            //Boolean _temCriterioDistancia = false;
            Boolean _temCriterioHabilidades = false;
            Boolean _temCriterioSLA_Execucao = false;

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                try
                {
                    Configuracoes_Gerais config = await new Conexao<Configuracoes_Gerais>(session).selectFirst();

                    _temCriterioArea = config.Criterio_Area;
                    //_temCriterioDistancia = criterios.Find(p => p.Criterio == Criterio_RoteirizacaoTipoAniel.Distancia) != null;
                    _temCriterioHabilidades = config.Criterio_Habilidade;
                    _temCriterioSLA_Execucao = config.Criterio_SLA_Execucao;

                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {


                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.EQUIPE, A.NOME");
                        sql.AppendLine("FROM TB_EQUIPE A");
                        sql.AppendLine("WHERE ((UPPER(A.EQUIPE) LIKE @VALOR) OR (UPPER(A.NOME) LIKE @VALOR)) AND A.EQUIPE_CONNECT = 'S' ");

                        if (_temCriterioArea)
                        {
                            sql.AppendLine("   AND A.EQUIPE IN(SELECT C.EQUIPE FROM TB_EQUIPE_LOCALIDADE C ")
                               .AppendLine($"                    WHERE C.COD_ESCRITORIO = @GRUPO_AREA ")
                               .AppendLine($"                      AND C.COD_AREA = @AREA ")
                               .AppendLine($"                      AND (C.COD_LOCALIDADE = @LOCALIDADE OR C.TODAS_LOCALIDADES = 1)) ");
                        }

                        if (_temCriterioHabilidades)
                        {
                            sql.AppendLine("   AND A.EQUIPE IN(SELECT D.EQUIPE ")
                               .AppendLine("                      FROM TB_EQUIPE_NATUREZA D ")
                               .AppendLine("                     INNER JOIN TB_TIPO_SERVICO_EQUIPE E ")
                               .AppendLine("                        ON (D.ID_NATUREZA = E.COD_NATUREZA) ")
                               .AppendLine($"                     WHERE E.COD_TIPO_SERV = @TIPO_SERV) ");
                        }

                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");
                        parametros.Add("@GRUPO_AREA", grupoarea);
                        parametros.Add("@AREA", area);
                        parametros.Add("@LOCALIDADE", localidade);
                        parametros.Add("@TIPO_SERV", Tipo_serv);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<String>
                                {
                                    Value = Convert.ToString(reader["EQUIPE"]),
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


        [HttpGet()]
        [Route("api/equipeapi/buscaEquipe")]
        public async Task<List<ObjBusca<String>>> buscaEquipe(String term)
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

                        sql.AppendLine("SELECT A.EQUIPE, A.NOME");
                        sql.AppendLine("FROM TB_EQUIPE A");
                        sql.AppendLine($"WHERE ((UPPER(A.NOME) LIKE @VALOR) OR (UPPER(A.EQUIPE) LIKE @VALOR)) AND A.EQUIPE_CONNECT = 'S'");

                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<String>
                                {
                                    Value = Convert.ToString(reader["EQUIPE"]),
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
        [Route("api/equipeapi/buscaEquipeComCriterioPontoPartida")]
        public async Task<ObjCombo<String>> buscaEquipeComCriterioPontoPartida(RequestObjCombo<String> request)
        {

            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sqlFrom = new StringBuilder();

                    sqlFrom.AppendLine("FROM TB_EQUIPE A");
                    sqlFrom.AppendLine($"WHERE A.EQUIPE_CONNECT = 'S'");


                    if (!String.IsNullOrEmpty(request.q))
                    {
                        sqlFrom.AppendLine(" AND (UPPER(A.NOME) LIKE @VALOR) ");
                        parametros.Add("@VALOR", $"%{request.q.ToUpperInvariant()}%");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.EQUIPE) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.EQUIPE, A.NOME");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["EQUIPE"]),
                                    Text = Convert.ToString(reader["NOME"])
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
        [Route("api/equipeapi/buscaEquipeSemMonitor")]
        public async Task<List<ObjBusca<String>>> buscaEquipeSemMonitor(String term)
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

                        sql.AppendLine("SELECT A.EQUIPE, A.NOME");
                        sql.AppendLine("FROM TB_EQUIPE A");


                        sql.AppendLine("WHERE (UPPER(A.NOME) LIKE @VALOR) AND A.ID_PONTO_PARTIDA IS NULL AND A.EQUIPE_CONNECT = 'S' AND A.USUARIO_MONITOR IS NULL");

                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<String>
                                {
                                    Value = Convert.ToString(reader["EQUIPE"]),
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

        public async Task<GetApiRetorno<EquipeStatusModel>> GetEquipeStatus(String Equipe)
        {
            GetApiRetorno<EquipeStatusModel> retorno = new GetApiRetorno<EquipeStatusModel>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<EquipeStatusAniel>(session))
                    {
                        var lista = (await repository.select(p => p.Equipe.Matricula == Equipe)).Select(p => new EquipeStatusModel
                        {
                            Equipe = Convert.ToString(p.Equipe),
                            Atualizado_Em = p.DataHoraCriacao != null ? p.DataHoraCriacao.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                            DataHora_Login = p.DataHora_Login != null ? Convert.ToDateTime(p.DataHora_Login).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                            Bateria = String.Format("{0:N2}", p.Bateria),
                            Dispositivo = p.Dispositivo,
                            Endereco = String.IsNullOrEmpty(p.Logradouro) ? String.Empty : String.Format("{0} nº {1}, {2} {3}", p.Logradouro, p.Numero, p.Bairro, p.Cidade),
                            IMEI = p.IMEI,
                            InternetMovel = p.InternetMovel,
                            Precisao = String.Format("{0:N2}", p.Precisao),
                            StatusGPS = p.StatusGPS,
                            Versao_Android = p.Versao_Android,
                            Versao_App = p.Versao_App,
                            WiFi = p.WiFi ? Resources.RsBase.LIGADO : Resources.RsBase.DESLIGADO,
                            Status_App = p.DataHoraAlteracao != null ? p.DataHoraAlteracao.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                            Tipo = p.Logado != null ? (p.Logado.Value ? "LOGADO EM" : "DESLOGADO EM") : String.Empty,
                            Justificativa_Ausencia = p.Justificativa_Ausencia,
                            Motivo_Paralisacao = GetMotivoParalisacao(Equipe).Result

                        }).ToList();


                        if (lista.Count() > 0)
                            retorno.Objeto = lista[0];
                        else
                            retorno.Objeto = new EquipeStatusModel();

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        public async Task<String> GetMotivoParalisacao(String equipe)
        {
            String retorno = "";
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    parametros.Add("@EQUIPE", equipe);
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT FIRST 1 B.OBSERVACAO_PAUSA FROM TB_EQUIPE_STATUS A");
                        sql.AppendLine("LEFT JOIN TB_DOCUMENTO_PRODUCAO B");
                        sql.AppendLine("ON(A.EQUIPE = B.EQUIPE)");
                        sql.AppendLine("WHERE A.EQUIPE = @EQUIPE");
                        sql.AppendLine("ORDER BY B.DIA DESC,B.OBSERVACAO_PAUSA DESC");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                retorno = !String.IsNullOrEmpty(Convert.ToString(reader["OBSERVACAO_PAUSA"])) ? Convert.ToString(reader["OBSERVACAO_PAUSA"]) : "";
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

        [HttpGet()]
        [Route("api/Equipeapi/GetNome")]
        public async Task<String> GetNome(String equipe)
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


                        sql.AppendLine("SELECT A.NOME");
                        sql.AppendLine("FROM   TB_EQUIPE A");
                        sql.AppendLine("WHERE A.EQUIPE = @EQUIPE");

                        parametros.Add("@EQUIPE", equipe);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                return Convert.ToString(reader["NOME"]);
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

        public async Task<String> GetNome(ISession session, String equipe)
        {
            try
            {
                //Filtros
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();


                    sql.AppendLine("SELECT A.NOME");
                    sql.AppendLine("FROM   TB_EQUIPE A");
                    sql.AppendLine("WHERE A.EQUIPE = @EQUIPE");

                    parametros.Add("@EQUIPE", equipe);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            return Convert.ToString(reader["NOME"]);
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


        [Route("api/Equipeapi/ExcluirGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(String value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<EquipeAniel>(session))
                    {

                        EquipeAniel modelo = null;
                        if (value != null || value != String.Empty)
                        {
                            modelo = (await repository.select(p => p.Matricula == value)).FirstOrDefault();
                            modelo.Ponto_Partida = null;
                            await repository.SaveOrUpdate(modelo);

                            retorno.IdObjeto = Convert.ToInt64(modelo.Matricula);
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }


        [Route("api/Equipeapi/ExcluirGridMonitoramento")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridMonitoramento(String value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<EquipeAniel>(session))
                    {

                        EquipeAniel modelo = null;
                        if (value != null || value != String.Empty)
                        {
                            modelo = (await repository.select(p => p.Matricula == value)).FirstOrDefault();
                            modelo.Monitor = null;
                            await repository.SaveOrUpdate(modelo);

                            retorno.IdObjeto = Convert.ToInt64(modelo.Matricula);
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/EquipeApi/FindById_HabilitarServico")]
        public async Task<GetApiRetorno<Habilitar_ServicoCadastroModel>> FindById_HabilitarServico(String Equipe)
        {
            GetApiRetorno<Habilitar_ServicoCadastroModel> retorno = new GetApiRetorno<Habilitar_ServicoCadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    EquipeAniel _equipe = await new BuscaGenericaRepository<EquipeAniel>(session, null).FindByWhere(p => p.Matricula == Equipe);

                    retorno.Objeto = new Habilitar_ServicoCadastroModel
                    {

                        Id = _equipe.Matricula,

                        Naturezas = new List<NaturezasListaModel>(_equipe.Naturezas.Select(p => new NaturezasListaModel
                        {
                            Id = Convert.ToInt32(p.Id),
                            Descricao = p.Natureza.Descricao,
                            Id_Natureza = p.Natureza.Id
                        }))
                    };

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
        [Route("api/EquipeApi/FindById_HabilitarArea")]
        public async Task<GetApiRetorno<Habilitar_AreaCadastroModel>> FindById_HabilitarArea(String Equipe)
        {
            GetApiRetorno<Habilitar_AreaCadastroModel> retorno = new GetApiRetorno<Habilitar_AreaCadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    EquipeAniel _equipe = await new BuscaGenericaRepository<EquipeAniel>(session, null).FindByWhere(p => p.Matricula == Equipe);

                    retorno.Objeto = new Habilitar_AreaCadastroModel
                    {

                        Id = _equipe.Matricula,
                        Areas = new List<AreasListaModel>(_equipe.Localidades.Select(p => new AreasListaModel
                        {
                            Id = Convert.ToInt32(p.Id),
                            Cod_Escritorio = p.Cod_Escritorio,
                            GrupoAreaStr = (new Conexao<EscritorioAniel>(session).selectFirst(x => x.Cod_Escritorio == p.Cod_Escritorio)).Result?.Descricao,

                            Cod_Area = p.Cod_Area,
                            AreaStr = (new Conexao<AreaAniel>(session).selectFirst(x => x.Cod_Area == p.Cod_Area &&
                                                                                        x.Cod_Escritorio == p.Cod_Escritorio)).Result?.Descricao,

                            Cod_Localidade = p.Cod_Localidade,
                            LocalidadeStr = (new Conexao<LocalidadeAniel>(session).selectFirst(x => x.Cod_Escritorio == p.Cod_Escritorio &&
                                                                                                        x.Cod_Area == p.Cod_Area &&
                                                                                                        x.Cod_Localidade == p.Cod_Localidade)).Result?.LocalidadeStr,
                            Todas_Localidades = p.Todas_Localidades,
                            Nivel = p.Nivel
                        }))
                    };

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


        [HttpPost()]
        [Route("api/EquipeApi/Gravar")]
        public async Task<PostApiRetorno<String>> Gravar(ParamWebApi<Habilitar_ServicoCadastroModel> value)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();

            try
            {
                EquipeAniel modelo = null;
                List<EquipeAniel> modelos = new List<EquipeAniel>();

                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession session_log = HibernateUtil.GetSessionLog())
                using (var repository_log = new Conexao<LogEquipe_NaturezaAniel>(session_log))
                using (var repository = new Conexao<EquipeAniel>(session))
                {
                    if (value.Objeto.Ids == null)
                    {
                        if (!String.IsNullOrEmpty(value.Objeto.Id))
                            modelo = (await repository.select(p => p.Matricula == value.Objeto.Id)).FirstOrDefault();


                        else
                        {
                            modelo = new EquipeAniel();
                        }
                        var log = new List<LogEquipe_NaturezaAniel>();
                        BindViewToObj(session, modelo, log, value);

                        await repository.SaveOrUpdate(modelo);
                        await repository_log.Save(log);

                    }
                    else
                    {
                        String[] equipesStr = value.Objeto.Ids.Split(',');

                        foreach (String equipeStr in equipesStr)
                        {

                            EquipeAniel equipe = await repository.selectFirst(p => p.Matricula == equipeStr);
                            List<Equipe_NaturezaAniel> lista_NovasNaturezas = new List<Equipe_NaturezaAniel>();
                            List<Equipe_NaturezaAniel> naturezasEquipe = equipe.Naturezas.ToList();
                            foreach (var item in naturezasEquipe)
                            {
                                if (item == null)
                                {
                                    item.Natureza = new Natureza_ProducaoAniel();
                                }
                            }

                            foreach (NaturezasListaModel naturezaView in value.Objeto.Naturezas)
                            {

                                if (naturezasEquipe.Find(p => p.Natureza != null) == null)
                                {
                                    lista_NovasNaturezas.Add(new Equipe_NaturezaAniel
                                    {
                                        Equipe = equipe,
                                        Natureza = new Natureza_ProducaoAniel
                                        {
                                            Id = naturezaView.Id_Natureza.Value,
                                            Descricao = naturezaView.Descricao,
                                        },
                                        UsuarioCriacao = value.Usuario
                                    });


                                }
                                else
                                {
                                    if (naturezasEquipe.Find(p => p.Natureza.Id == naturezaView.Id_Natureza) == null)
                                    {
                                        lista_NovasNaturezas.Add(new Equipe_NaturezaAniel
                                        {
                                            Equipe = equipe,
                                            Natureza = await new Conexao<Natureza_ProducaoAniel>(session).selectFirst(p => p.Id == naturezaView.Id_Natureza),
                                            UsuarioCriacao = value.Usuario
                                        });

                                    }


                                }

                            }
                            if (lista_NovasNaturezas.Count > 0)
                            {
                                naturezasEquipe.AddRange(lista_NovasNaturezas);
                                equipe.Naturezas = naturezasEquipe;
                                await repository.Save(equipe);

                                var log = lista_NovasNaturezas.Select(p => new LogEquipe_NaturezaAniel
                                {
                                    Equipe = p.Equipe.Matricula,
                                    Id_Natureza_ProducaoAniel = p.Natureza.Id,
                                    TipoOperacao = TipoOperacao.Insercao,
                                    UsuarioCriacao = value.Usuario,
                                    Nome_Equipe = p.Equipe?.Nome,
                                    Descricao_Natureza = p.Natureza?.Descricao
                                }).ToList();

                                await repository_log.Save(log);
                            }
                        }




                    }
                    retorno.IdObjeto = modelo != null ? modelo.Matricula : String.Empty;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = String.Empty;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }



        [HttpPost()]
        [Route("api/EquipeApi/GravarArea")]
        public async Task<PostApiRetorno<String>> GravarArea(ParamWebApi<Habilitar_AreaCadastroModel> value)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();

            try
            {
                EquipeAniel modelo = null;
                List<EquipeAniel> modelos = new List<EquipeAniel>();

                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession session_log = HibernateUtil.GetSessionLog())
                using (var repository = new Conexao<EquipeAniel>(session))
                using (var repository_log = new Conexao<LogEquipe_LocalidadeAniel>(session_log))
                {

                    {

                        if (value.Objeto.Ids == null)
                        {
                            if (!String.IsNullOrEmpty(value.Objeto.Id))
                                modelo = (await repository.select(p => p.Matricula == value.Objeto.Id)).FirstOrDefault();
                            else
                            {
                                modelo = new EquipeAniel();
                            }


                            List<Equipe_LocalidadeAniel> localidadesTmp = modelo.Localidades.ToList();
                            List<LogEquipe_LocalidadeAniel> log = new List<LogEquipe_LocalidadeAniel>();


                            foreach (var localidade in value.Objeto.Areas.FindAll(p => p.Id == 0))
                            {
                                localidadesTmp.Add(new Equipe_LocalidadeAniel
                                {
                                    Cod_Escritorio = localidade.Cod_Escritorio,
                                    Cod_Area = localidade.Cod_Area,
                                    Cod_Localidade = localidade.Cod_Localidade,
                                    UsuarioCriacao = value.Usuario,
                                    Equipe = modelo,
                                    Todas_Localidades = localidade.Todas_Localidades,
                                    Nivel = Convert.ToInt16(localidade.Nivel)
                                });
                            }

                            modelo.Localidades = localidadesTmp;

                            var logs = await getLogEquipe_Localidade(localidadesTmp.Where(p => p.Id == 0).ToList(), value.Usuario, TipoOperacao.Insercao);
                            await repository.SaveOrUpdate(modelo);
                            await repository_log.Save(logs);
                        }
                        else
                        {
                            String[] equipesStr = value.Objeto.Ids.Split(',');

                            foreach (String equipeStr in equipesStr)
                            {
                                Boolean novaLocalidade = false;
                                EquipeAniel equipe = await repository.selectFirst(p => p.Matricula == equipeStr);
                                List<Equipe_LocalidadeAniel> localidadesEquipe = equipe.Localidades.ToList();
                                foreach (AreasListaModel localidadeView in value.Objeto.Areas)
                                {
                                    if (localidadesEquipe.Find(p => p.Cod_Escritorio == localidadeView.Cod_Escritorio &&
                                            p.Cod_Area == localidadeView.Cod_Area &&
                                           (p.Todas_Localidades || p.Cod_Localidade == localidadeView.Cod_Localidade)) == null)
                                    {
                                        localidadesEquipe.Add(new Equipe_LocalidadeAniel
                                        {
                                            Equipe = equipe,
                                            Cod_Escritorio = localidadeView.Cod_Escritorio,
                                            Cod_Area = localidadeView.Cod_Area,
                                            Cod_Localidade = localidadeView.Cod_Localidade,
                                            Todas_Localidades = localidadeView.Todas_Localidades,
                                            UsuarioCriacao = value.Usuario,
                                            Nivel = Convert.ToInt16(localidadeView.Nivel)
                                        });
                                        novaLocalidade = true;
                                    }
                                }
                                if (novaLocalidade)
                                {
                                    equipe.Localidades = localidadesEquipe;
                                    var logs = await getLogEquipe_Localidade(localidadesEquipe.ToList().Where(p => p.Id == 0).ToList(), value.Usuario, TipoOperacao.Insercao);
                                    await repository.SaveOrUpdate(equipe);
                                    await repository_log.Save(logs);

                                }
                            }

                        }


                    }
                    retorno.IdObjeto = modelo != null ? modelo.Matricula : String.Empty;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = String.Empty;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }



        private void BindViewToObj(ISession session, EquipeAniel objBanco, List<LogEquipe_NaturezaAniel> log, ParamWebApi<Habilitar_ServicoCadastroModel> value)
        {
            Equipe_NaturezaAniel obj = new Equipe_NaturezaAniel();
            var view = value.Objeto;
            //obj.Natureza = view.Id_Natureza != null ? new BuscaGenericaRepository<Natureza_ProducaoAniel>(session, null).FindById(view.Id_Natureza.Value).Result : null;

            List<Equipe_NaturezaAniel> naturezasTmp = objBanco.Naturezas.ToList();
            foreach (var naturezas in view.Naturezas.FindAll(p => p.Id == 0))
            {
                naturezasTmp.Add(new Equipe_NaturezaAniel
                {
                    Natureza = new Conexao<Natureza_ProducaoAniel>(session).selectFirst(p => p.Id == naturezas.Id_Natureza).Result,
                    Equipe = objBanco,
                    UsuarioCriacao = value.Usuario
                });

                log.Add(new LogEquipe_NaturezaAniel
                {
                    Id_Natureza_ProducaoAniel = naturezas.Id_Natureza.Value,
                    Equipe = objBanco.Matricula,
                    Nome_Equipe = objBanco?.Nome,
                    Descricao_Natureza = naturezas?.Descricao,
                    TipoOperacao = TipoOperacao.Insercao,
                    UsuarioCriacao = value.Usuario
                });

            }

            objBanco.Naturezas = naturezasTmp;

        }

        [Route("api/EquipeNaturezaapi/ExcluirGridEquipeNatureza")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridEquipeNatureza(String value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession session_log = HibernateUtil.GetSessionLog())
                using (var repository = new Conexao<Equipe_NaturezaAniel>(session))
                using (var repository_log = new Conexao<LogEquipe_NaturezaAniel>(session_log))
                {
                    Equipe_NaturezaAniel modelo = new Equipe_NaturezaAniel();
                    if (!String.IsNullOrEmpty(value))
                    {
                        modelo = await repository.selectFirst(p => p.Id == Convert.ToInt64(value));
                        var log = new LogEquipe_NaturezaAniel
                        {
                            Id_Natureza_ProducaoAniel = modelo.Natureza.Id,
                            Equipe = modelo.Equipe.Matricula,
                            TipoOperacao = TipoOperacao.Exclusao,
                            UsuarioCriacao = User.Identity.GetUserId(),
                            Nome_Equipe = modelo.Equipe?.Nome,
                            Descricao_Natureza = modelo.Natureza?.Descricao
                        };

                        await repository.Remove(modelo);
                        await repository_log.Save(log);

                        retorno.IdObjeto = 0;
                        retorno.Ok = true;
                    }
                    else
                    {
                        throw new Exception(Resources.RsBase.NAOENCONTRADO);
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/EquipeNaturezaapi/ExcluirGridEquipeNaturezaEmMassa")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridEquipeNaturezaEmMassa(String value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            String[] Ids = value.Split(',');

            try
            {

                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession session_log = HibernateUtil.GetSessionLog())
                using (var repositoryEquipe = new Conexao<EquipeAniel>(session))
                using (var repository = new Conexao<Equipe_NaturezaAniel>(session))
                using (var repository_log = new Conexao<LogEquipe_NaturezaAniel>(session))
                {

                    List<Equipe_NaturezaAniel> modelo = new List<Equipe_NaturezaAniel>();
                    if (value.Length != 0)
                    {
                        var equipe = (await repositoryEquipe.select(p => p.Matricula.IsIn(Ids))).ToList();
                        var matriculas = equipe.Select(p => p.Matricula).ToList();

                        modelo = (await repository.select(p => p.Equipe.Matricula.IsIn(matriculas))).ToList();

                        await repository.Remove(modelo);

                        var log = modelo.Select(p => new LogEquipe_NaturezaAniel
                        {
                            Id_Natureza_ProducaoAniel = p.Natureza.Id,
                            Equipe = p.Equipe.Matricula,
                            TipoOperacao = TipoOperacao.Exclusao,
                            Descricao_Natureza = p.Natureza?.Descricao,
                            Nome_Equipe = p.Equipe.Nome,
                            UsuarioCriacao = User.Identity.GetUserId()
                        }).ToList();

                        await repository_log.Save(log);

                        retorno.IdObjeto = Convert.ToInt64(Ids.Last());
                        retorno.Ok = true;
                    }
                    else
                    {
                        throw new Exception(Resources.RsBase.NAOENCONTRADO);
                    }
                }

            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/EquipeLocalidadeapi/ExcluirGridEquipeLocalidade")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridEquipeLocalidade(String value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession session_log = HibernateUtil.GetSessionLog())
                using (var repository = new Conexao<Equipe_LocalidadeAniel>(session))
                using (var repository_escritorio = new Conexao<EscritorioAniel>(session))
                using (var repository_area = new Conexao<AreaAniel>(session))
                using (var repository_localidade = new Conexao<LocalidadeAniel>(session))
                using (var repository_log = new Conexao<LogEquipe_LocalidadeAniel>(session_log))
                {
                    Equipe_LocalidadeAniel modelo = new Equipe_LocalidadeAniel();
                    if (!String.IsNullOrEmpty(value))
                    {
                        modelo = await repository.selectFirst(p => p.Id == Convert.ToInt64(value));
                        await repository.Remove(modelo);

                        var escritorio = await repository_escritorio.selectFirst(p => p.Cod_Escritorio == modelo.Cod_Escritorio);
                        var area = await repository_area.selectFirst(p => p.Cod_Escritorio == modelo.Cod_Escritorio && p.Cod_Area == modelo.Cod_Area);
                        var localidade = await repository_localidade.selectFirst(p => p.Cod_Escritorio == modelo.Cod_Escritorio && p.Cod_Area == modelo.Cod_Area
                                                                                && p.Cod_Localidade == modelo.Cod_Localidade);


                        await repository_log.Save(await getLogEquipe_Localidade(new List<Equipe_LocalidadeAniel> { modelo }, User.Identity.GetUserId(), TipoOperacao.Exclusao));

                        retorno.IdObjeto = 0;
                        retorno.Ok = true;
                    }
                    else
                    {
                        throw new Exception(Resources.RsBase.NAOENCONTRADO);
                    }
                }

            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/EquipeLocalidadeapi/ExcluirGridEquipeLocalidadeEmMassa")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridEquipeLocalidadeEmMassa(String value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            String[] Ids = value.Split(',');

            try
            {
                using (ISession session_log = HibernateUtil.GetSessionLog())
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository_log = new Conexao<LogEquipe_LocalidadeAniel>(session_log))
                    using (var repository = new Conexao<Equipe_LocalidadeAniel>(session))
                    using (var repository_escritorio = new Conexao<EscritorioAniel>(session))
                    using (var repository_area = new Conexao<AreaAniel>(session))
                    using (var repository_localidade = new Conexao<LocalidadeAniel>(session))
                    using (var repositoryEquipe = new Conexao<EquipeAniel>(session))
                    {
                        List<Equipe_LocalidadeAniel> modelo = new List<Equipe_LocalidadeAniel>();
                        if (value.Length != 0)
                        {
                            var equipe = (await repositoryEquipe.select(p => p.Matricula.IsIn(Ids))).ToList();
                            var matriculas = equipe.Select(p => p.Matricula).ToList();

                            modelo = (await repository.select(p => p.Equipe.Matricula.IsIn(matriculas))).ToList();

                            await repository.Remove(modelo);

                            await repository_log.Save(await getLogEquipe_Localidade(modelo, User.Identity.GetUserId(), TipoOperacao.Exclusao));

                            retorno.IdObjeto = Convert.ToInt64(Ids.Last());
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        #region Mobile
        [AllowAnonymous]
        [OlaAuthorize]
        [HttpGet]
        [Route("api/EquipeLocalidadeapi/GetEquipeMobile")]
        public async Task<GetApiRetorno<Equipe_Status_Mobile>> GetEquipeMobile(String idEquipe)
        {
            var retorno = new GetApiRetorno<Equipe_Status_Mobile>();
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        var sql = new StringBuilder();


                        sql.Append(" SELECT ");
                        sql.Append("     B.TELEFONE, B.FOTO, ");
                        sql.Append("     C.USUARIOCRIACAO, C.LATITUDE, C.LONGITUDE, C.JUSTIFICATIVA ");
                        sql.Append(" FROM ");
                        sql.Append("     TB_EQUIPE A ");
                        sql.Append(" INNER JOIN ");
                        sql.Append("     TB_FUNCIONARIO B ");
                        sql.Append("         ON B.NUM_FRE = A.NUM_FRE ");
                        sql.Append(" INNER JOIN ");
                        sql.Append("     TB_EQUIPE_STATUS C ");
                        sql.Append("         ON A.EQUIPE = C.EQUIPE ");
                        sql.Append(" WHERE ");
                        sql.Append("     A.EQUIPE = @ID_EQUIPE ");

                        parametros.Add("@ID_EQUIPE", idEquipe);

                        var _reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null);

                        while (await _reader.ReadAsync())
                        {

                            var modeloTemp = new Equipe_Status_Mobile
                            {
                                Nome = _reader["USUARIOCRIACAO"].ToString(),
                                Telefone = _reader["TELEFONE"].ToString(),
                                Foto = _reader["FOTO"] != DBNull.Value ? ImageService.ImageToBase64(ImageService.BytesToImage((byte[])_reader["FOTO"]), ImageFormat.Png) : String.Empty,
                                Latitude = _reader["LATITUDE"] != DBNull.Value ? Convert.ToString(_reader["LATITUDE"]) : null,
                                Longitude = _reader["LONGITUDE"] != DBNull.Value ? Convert.ToString(_reader["LONGITUDE"]) : null
                            };
                            retorno.Objeto = modeloTemp;
                        }

                    }
                    retorno.Ok = true;

                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
                return retorno;
            }

        }
        #endregion
        private async Task<List<LogEquipe_LocalidadeAniel>> getLogEquipe_Localidade(List<Equipe_LocalidadeAniel> localidade_equipe, String Usuario, TipoOperacao tipo)
        {
            var logs = await Task.WhenAll(localidade_equipe.Select(async p =>
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var repository_escritorio = new Conexao<EscritorioAniel>(session))
                using (var repository_area = new Conexao<AreaAniel>(session))
                using (var repository_localidade = new Conexao<LocalidadeAniel>(session))
                {

                    var escritorio = await repository_escritorio.selectFirst(y => y.Cod_Escritorio == p.Cod_Escritorio);
                    var area = await repository_area.selectFirst(y => y.Cod_Escritorio == p.Cod_Escritorio && y.Cod_Area == p.Cod_Area);
                    var localidade = await repository_localidade.selectFirst(y => y.Cod_Escritorio == p.Cod_Escritorio && y.Cod_Area == p.Cod_Area
                                                                            && y.Cod_Localidade == p.Cod_Localidade);

                    LogEquipe_LocalidadeAniel log = new LogEquipe_LocalidadeAniel
                    {
                        Cod_Escritorio = p.Cod_Escritorio,
                        Cod_Area = p.Cod_Area,
                        Cod_Localidade = p.Cod_Localidade,
                        Todas_Localidades = p.Todas_Localidades,
                        Equipe = p.Equipe.Matricula,
                        Descricao_Escritorio = escritorio?.Descricao,
                        Descricao_Area = area?.Descricao,
                        Descricao_Localidade = localidade?.LocalidadeStr,
                        Nome_Equipe = p.Equipe.Nome,
                        UsuarioCriacao = Usuario,
                        TipoOperacao = tipo,
                    };

                    return log;
                }
            }));

            return logs.ToList();
        }

        [Route("api/Equipeapi/ServicosExecutadosPorTecnico")]
        [HttpPost()]
        public async Task<DataTableResponse<dynamic>> ServicosExecutadosPorTecnico(ServicosExecutadosPorTecnico_DataTableRequest request)
        {
            var retorno = new List<dynamic>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();

                        string campodata = request.Tipo_Data == "agendamento" ? "A.DATA_MAXIMA" : "A.DATA_PROD";
                        StringBuilder sql = new StringBuilder($@"SELECT {campodata},C.EQUIPE,C.NOME,COUNT(F.COD_ENCERRAMENTO) IMPRODUTIVAS ,COUNT(G.COD_ENCERRAMENTO) PRODUTIVAS

                            FROM TB_DOCUMENTO_PRODUCAO A

                            LEFT JOIN TB_EQUIPE C 
                            ON(A.EQUIPE = C.EQUIPE)

                            LEFT JOIN TB_FUNCIONARIO D 
                            ON(D.NUM_FRE = C.COD_SUPERV)

                            LEFT JOIN TB_CODIGO_ENCERRAMENTO F 
                            ON(A.COD_ENCERRAMENTO = F.COD_ENCERRAMENTO AND A.CODCT = F.CODCT AND F.STATUS_EXECUCAO = 1)

                            LEFT JOIN TB_CODIGO_ENCERRAMENTO G 
                            ON(A.COD_ENCERRAMENTO = G.COD_ENCERRAMENTO AND A.CODCT = G.CODCT AND G.STATUS_EXECUCAO = 2)

                            LEFT JOIN TB_EQUIPE_NATUREZA H
                            ON(H.EQUIPE = A.EQUIPE)

                            LEFT JOIN TB_TIPO_SERVICO_EQUIPE I 
                            ON(A.COD_TIPO_SERV = I.COD_TIPO_SERV)  

                            LEFT JOIN TB_NATUREZA_PRODUCAO J 
                            ON(I.COD_NATUREZA = J.ID) 


                             WHERE {campodata} BETWEEN @DATAINI AND @DATAFIM AND (A.STATUS_EXECUCAO = 2 OR A.STATUS_EXECUCAO = 1) 

                            ");

                        parametros.Add("DATAINI", request.Data_Inicio);
                        parametros.Add("DATAFIM", request.Data_Fim);

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
                        if (!String.IsNullOrEmpty(request.Centro))
                        {
                            sql.AppendLine("AND (D.COD_CCUSTO = @CENTRO)");
                            parametros.Add("CENTRO", request.Centro);
                        }
                        if (!String.IsNullOrEmpty(request.Empresa))
                        {
                            sql.AppendLine("AND (D.CODEMPRESA = @EMPRESA)");
                            parametros.Add("EMPRESA", request.Empresa);
                        }
                        if (!String.IsNullOrEmpty(request.Filial))
                        {
                            sql.AppendLine("AND (D.CODFILIAL = @FILIAL)");
                            parametros.Add("FILIAL", request.Filial);
                        }
                        if (!String.IsNullOrEmpty(request.GrupoArea))
                        {
                            sql.AppendLine("AND (A.cod_escritorio = @GRUPO_AREA)");
                            parametros.Add("GRUPO_AREA", request.GrupoArea);
                        }
                        if (!String.IsNullOrEmpty(request.Area))
                        {
                            sql.AppendLine("AND (A.cod_area = @AREA)");
                            parametros.Add("AREA", request.Area);
                        }
                        if (!String.IsNullOrEmpty(request.Localidade))
                        {
                            sql.AppendLine("AND (A.cod_localidade = @LOCALIDADE)");
                            parametros.Add("LOCALIDADE", request.Localidade);
                        }
                        if (!String.IsNullOrEmpty(request.Monitor))
                        {
                            sql.AppendLine("AND (C.USUARIO_MONITOR = @MONITOR)");
                            parametros.Add("MONITOR", request.Monitor);
                        }
                        if (!String.IsNullOrEmpty(request.Habilidade))
                        {
                            sql.AppendLine("AND (I.COD_NATUREZA = @HABILIDADE)");
                            parametros.Add("HABILIDADE", request.Habilidade);
                        }


                        sql.AppendLine($@"GROUP BY {campodata}, C.NOME, C.EQUIPE 
                                         ORDER BY C.EQUIPE, C.NOME, {campodata}");


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {

                                var produtivas = Convert.ToDouble(reader["PRODUTIVAS"]);
                                var improdutivas = Convert.ToDouble(reader["IMPRODUTIVAS"]);
                                var totais = produtivas + improdutivas;

                                retorno.Add(new
                                {
                                    tecnico = $"{Convert.ToString(reader["EQUIPE"])} - {Convert.ToString(reader["NOME"])}",
                                    data = Convert.ToDateTime(reader[request.Tipo_Data == "agendamento" ? "DATA_MAXIMA" : "DATA_PROD"]).ToString("dd/MM/yyyy"),
                                    produtivas = produtivas.ToString(),
                                    improdutivas = improdutivas.ToString(),
                                    percentualProdutivas = !Double.IsNaN(Math.Round((produtivas / totais) * 100)) ? Math.Round((produtivas / totais) * 100) : 0,
                                    percentualImprodutivas = !Double.IsNaN(Math.Round((improdutivas / totais) * 100)) ? Math.Round((improdutivas / totais) * 100) : 0,

                                });
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return new DataTableResponse<dynamic>
            {
                data = retorno,
                draw = request.draw,
                recordsFiltered = retorno.Count()
            };
        }

        [HttpPost()]
        [Route("api/EquipeApi/GetComboMobile")]
        public async Task<ObjCombo<String>> GetComboMobile(RequestObjCombo<string> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_EQUIPE A ")
                        .AppendLine("LEFT JOIN TB_FUNCIONARIO B ")
                        .AppendLine("ON (A.NUM_FRE = B.NUM_FRE) ");

                    List<String> condicoes = new List<String>();

                    condicoes.Add("A.MOBILE = 'S'");

                    if (request.soativos && !Convert.ToBoolean(request.id_usuario))
                    {
                        condicoes.Add($"B.SITUACAO IN (0,4)");
                    }


                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.EQUIPE) = '{request.id.ToUpperInvariant()}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.EQUIPE) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 10)}%' OR " +
                                      $" UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.EQUIPE) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.EQUIPE, A.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, sessionT5.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["EQUIPE"]),
                                    Text = Convert.ToString(reader["NOME"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    throw ex;
                }
            }
            return retorno;

        }

    }

    public class RemoveUsuario
    {
        public string[] Ids { get; set; }
    }
}


