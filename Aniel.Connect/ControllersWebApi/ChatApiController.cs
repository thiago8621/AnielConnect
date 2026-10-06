using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Socket.ChatBackOfficeHub;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using MongoDB.Driver.Linq;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class ChatApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/ChatApi/GetMensagens")]
        public async Task<List<MensagemListaModel>> GetMensagens()
        {
            List<MensagemListaModel> retorno = new List<MensagemListaModel>();

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

                        sql.AppendLine(" SELECT A.ID AS ID_MENSAGEM, A.MENSAGEM, A.NUM_IDENTIFICADOR, A.REMETENTE, A.SALA AS ID_SALA, A.DATAHORA ");
                        sql.AppendLine(" FROM TB_XMPP_MENSAGEM A ");

                        sql.AppendLine(" LEFT JOIN TB_XMPP_SALA B ");
                        sql.AppendLine(" ON(B.ID = A.SALA) ");

                        sql.AppendLine(" LEFT JOIN TB_DOCUMENTO_PRODUCAO C ");
                        //sql.AppendLine(" ON(C.USUARIO = @USUARIO ");
                        sql.AppendLine(" ON(C.USUARIO = 'ADMIN' ");
                        sql.AppendLine("    AND C.CODCT = B.CODCT ");
                        sql.AppendLine("    AND C.PROJETO = B.PROJETO ");
                        sql.AppendLine("    AND C.NUM_DOC = B.NUM_DOC ");
                        sql.AppendLine("    AND C.NUM_OBRA = B.NUM_OBRA) ");

                        sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO D ");
                        sql.AppendLine(" ON(D.EQUIPE = C.EQUIPE) ");

                        sql.AppendLine(" LEFT JOIN TB_XMPP_GRUPO E ");
                        sql.AppendLine(" ON(E.CPF = D.CPF) ");

                        parametros.Add("@USUARIO", User.Identity.GetUserId());

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new MensagemListaModel
                                {
                                    Id_Mensagem = Convert.ToInt64(reader["ID_MENSAGEM"]),
                                    Id_Sala = Convert.ToInt64(reader["ID_SALA"] == DBNull.Value ? null : reader["ID_SALA"]),

                                    Num_Identificador = Convert.ToString(reader["NUM_IDENTIFICADOR"]),
                                    Mensagem = Convert.ToString(reader["MENSAGEM"]),
                                    Remetente = Convert.ToString(reader["REMETENTE"]),
                                    Data_Envio = Convert.ToString(reader["DATAHORA"])
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
        [Route("api/ChatApi/GravarEquipe")]
        public async Task<PostApiRetorno<Int64>> GravarEquipe(String grupo, String numObra)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            numObra = numObra.Replace("'", "");

            try
            {
                Documento_ProducaoAniel modelo = null;

                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(sessionConnect))
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    using (var api_Roteirizacao = new Painel_RoteirizacaoApiController())
                    using (var api = new Documento_ProducaoApiController())
                    using (var repositoryAgendamento = new Conexao<Agendamento_EquipeAniel>(session))
                    using (var conexaoMongo = new ConexaoMongo())
                    using (var repositoryChat = new RepositoryMongo<ServicoChatMongo>(conexaoMongo))
                    {
                        Agendamento_EquipeAniel agendamentoEquipe = await repositoryAgendamento.selectFirst(p => p.Num_Obra == numObra);

                        Configuracoes_Gerais config = await repoConfiguracoes_Gerais.selectFirst();

                        using (var repository = new Conexao<Documento_ProducaoAniel>(session))
                        {
                            //Busca informações de acordo com o Num_OS
                            modelo = await repository.selectFirst(p => p.Num_Obra == numObra);

                            var status_old = modelo.Status;

                            if (modelo.Equipe != grupo)
                            {
                                if (!new List<StatusExecucaoAniel> { StatusExecucaoAniel.Aberta_Aguard_Agendamento,
                                StatusExecucaoAniel.Aberta_Aguard_Atendimento,
                                StatusExecucaoAniel.Atendimento_Paralizado,
                                StatusExecucaoAniel.Aberta_Aguard_Responsavel}.Contains(modelo.Status.Value))
                                {
                                    throw new Exception($"Não foi possível alterar equipe devido ao fato do Documento ter sido alterado para o status {HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(modelo.Status.Value)}.");
                                }

                                if (modelo.Responsabilidade == ResponsabilidadeOS.TECNICO)
                                {
                                    throw new Exception($"Não foi possível alterar equipe devido ao fato do Documento estar na responsabilidade do técnico.");
                                }
                            }

                            if (modelo.Status == StatusExecucaoAniel.Fechada_Produtiva ||
                                modelo.Status == StatusExecucaoAniel.Fechada_Improdutiva ||
                                modelo.Status == StatusExecucaoAniel.Atendimento_Finalizado)
                            {
                                throw new Exception("Não foi possível alterar o agendamento do serviço, pois o mesmo se encontra fechado.");
                            }

                            if (modelo == null)
                            {
                                throw new Exception("Documento Produção não encontrado");
                            }


                            await api.InsertLog_Lanc_Servico(session, modelo.CodCt, modelo.Projeto, modelo.Num_Obra, modelo.Num_Doc,
                                                             String.Format("Agendamento do serviço {0} {1}", modelo.Dia, modelo.Hora),
                                                             modelo.Data_Maxima.HasValue ? modelo.Data_Maxima.Value.ToString("dd/MM/yyyy") : "",
                                                             User.Identity.GetUserId(), Convert.ToDateTime(modelo.Dia), null, modelo.Dia.ToString() + modelo.Hora.ToString(), null, transaction);
                            modelo.Equipe = grupo;

                            if (config.Considerar_Agendamento_Periodo)
                            {
                                modelo.CodTurnoServ = PeriodoApiController.BuscarTurno(session, (modelo.Data_Maxima.Value.Date + modelo.Hora_Maxima.Value.TimeOfDay), Convert.ToDateTime(modelo.DataHora_Fim_Atend).TimeOfDay - Convert.ToDateTime(modelo.DataHora_Ini_Atend).TimeOfDay)?.Id;
                            }

                            await repository.SaveOrUpdate(modelo);

                        }

                        using (var repository = new Conexao<Agendamento_EquipeAniel>(session))
                        {
                            List<Agendamento_EquipeAniel> agendamentos = (await repository.select(p => p.CodCt == modelo.CodCt && p.Projeto == modelo.Projeto &&
                                                                                                    p.Num_Obra == modelo.Num_Obra && p.Num_Doc == modelo.Num_Doc)).ToList();

                            foreach (var agendamento in agendamentos)
                            {
                                agendamento.Equipe = await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == grupo);
                                await repository.SaveOrUpdate(agendamento);

                                if (agendamento.Equipe != null)
                                {
                                    if (!String.IsNullOrEmpty(agendamento.Equipe.Device_Id))
                                    {
                                        MobileService.NotificarSmartphone(agendamento.Equipe.Device_Id, String.Empty, "Houve mudanças em sua rotina.", "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS", modelo.CodCt, modelo.Projeto, modelo.Num_Doc.ToString(), modelo.Num_Obra);
                                    }
                                }
                            }
                        }

                        using (var repoFuncionario = new Conexao<FuncionarioAniel>(session))
                        {
                            var equipeFuncionario = await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == grupo);
                            FuncionarioAniel funcionario = (await repoFuncionario.selectFirst(p => p.Num_Fre == equipeFuncionario.Num_Fre));

                            ServicoChatMongo servicoChatMongo = await repositoryChat.Find(p => p.num_os == numObra);
                            await ChatBackOfficeHub.GetChatSrategy(servicoChatMongo).InativaAtivaUsuario(grupo, numObra, funcionario);
                        }

                        transaction.Commit();

                        if (transaction.WasCommitted == false)
                            transaction.Commit();


                    }
                }

                retorno.IdObjeto = 1;
                retorno.Ok = true;
            }

            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/ChatApi/GetQtdMensagens")]
        public async Task<QtdMensagensModel> GetQtdMensagens()
        {
            QtdMensagensModel retorno = new QtdMensagensModel();

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

                        sql.AppendLine(" SELECT COUNT(A.ID) CO ");
                        sql.AppendLine(" FROM TB_XMPP_MENSAGEM A ");

                        sql.AppendLine(" LEFT JOIN TB_XMPP_SALA B ");
                        sql.AppendLine(" ON(B.ID = A.SALA) ");

                        sql.AppendLine(" LEFT JOIN TB_DOCUMENTO_PRODUCAO C ");
                        sql.AppendLine(" ON(C.USUARIO = @USUARIO ");
                        sql.AppendLine("    AND C.CODCT = B.CODCT ");
                        sql.AppendLine("    AND C.PROJETO = B.PROJETO ");
                        sql.AppendLine("    AND C.NUM_DOC = B.NUM_DOC ");
                        sql.AppendLine("    AND C.NUM_OBRA = B.NUM_OBRA) ");

                        sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO D ");
                        sql.AppendLine(" ON(D.EQUIPE = C.EQUIPE) ");

                        sql.AppendLine(" LEFT JOIN TB_XMPP_GRUPO E ");
                        sql.AppendLine(" ON(E.CPF = D.CPF) ");

                        sql.AppendLine(" WHERE A.DATAHORA > E.ULTIMA_VISUALIZACAO ");
                        sql.AppendLine(" AND A.REMETENTE != @USUARIO ");

                        parametros.Add("@USUARIO", User.Identity.GetUserId());

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno = new QtdMensagensModel
                                {
                                    Qtd_Mensagens = Convert.ToInt64(reader["CO"])
                                };
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
        [Route("api/ChatApi/GetAllConversas")]
        public async Task<DataTableResponse<Conversas_ListModel>> GetAllConversas(Conversas_DataTableRequest request)
        {

          List<Conversas_ListModel> retorno = new List<Conversas_ListModel>();
            IOrderedEnumerable<Conversas_ListModel> listaOrdenada = null;

            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder("SELECT A.CODCT, A.PROJETO, A.NUM_DOC, A.NUM_OBRA, A.EQUIPE, A.HORA_MAXIMA, A.DATA_MAXIMA ")
                           .AppendLine("        FROM TB_DOCUMENTO_PRODUCAO A ")
                           .AppendLine("        LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL B ON(A.CODCT = B.CODCT")
                           .AppendLine("                AND A.PROJETO = B.PROJETO ")
                           .AppendLine("                AND A.NUM_OBRA = B.NUM_OBRA")
                           .AppendLine("                AND A.NUM_DOC = B.NUM_DOC ")
                           .AppendLine("                AND A.DATA_MAXIMA = CAST(B.DATAHORA_INICIO AS DATE)) ");

                    List<String> filtros = new List<string>();

                    if (String.IsNullOrEmpty(request.search.value) == false)
                    {
                        filtros.Add(" A.NUM_OBRA LIKE @SEARCH ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }

                    if (request.temAcesso == "False")
                    {
                        filtros.Add("  A.EQUIPE IN (SELECT EQUIPE FROM TB_EQUIPE WHERE COD_SUPERV = (SELECT FIRST 1 NUM_FRE FROM TB_SENHA WHERE USUARIO = @MATRICULA) UNION " +
                            "                       SELECT EQUIPE FROM TB_EQUIPE WHERE USUARIO_MONITOR = @MATRICULA UNION " +
                            "                       SELECT EQUIPE FROM TB_EQUIPE WHERE EQUIPE = (SELECT FIRST 1 EQUIPE FROM TB_EQUIPE WHERE NUM_FRE = (SELECT FIRST 1 NUM_FRE FROM TB_SENHA WHERE USUARIO = @MATRICULA))) ");

                        parametros.Add("@MATRICULA", request.usuario);

                    }

                    if (!String.IsNullOrEmpty(request.dataInicio))
                    {
                        filtros.Add(" A.DATA_MAXIMA BETWEEN @DATAINICIO AND @DATAFIM ");
                        parametros.Add("@DATAINICIO", request.dataInicio);
                        parametros.Add("@DATAFIM", request.dataFim);
                    }

                    if (filtros.Count > 0)
                    {
                        sql.AppendLine(" WHERE " + String.Join(" AND ", filtros));
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            var ultimaMensagem = await GetDataUltimaMensagem(Convert.ToString(reader["NUM_OBRA"]));
                            retorno.Add(new Conversas_ListModel
                            {
                                numOS = Convert.ToString(reader["NUM_OBRA"]),
                                codCt = Convert.ToString(reader["CODCT"]),
                                projeto = Convert.ToString(reader["PROJETO"]),
                                numDoc = Convert.ToString(reader["NUM_DOC"]),
                                dataConversa = ultimaMensagem.descricao,
                                totalDiasUltimaMensagem = ultimaMensagem.totalDias
                            });
                        }
                    }

                }
            }

            //Ordenacao
            listaOrdenada = retorno.OrderByDescending(p => p.totalDiasUltimaMensagem.HasValue).ThenBy(p => p.totalDiasUltimaMensagem);

            //Page e retornando DataTable
            return new DataTableResponse<Conversas_ListModel>
            {
                draw = request.draw,
                recordsFiltered = listaOrdenada.Count(),
                data = listaOrdenada.ToList(),
                error = ""
            };

        }


        [AcceptVerbs]
        [Route("api/ChatApi/GetDadosOS")]
        public async Task<GetApiRetorno<List<DadosOS_ListModel>>> GetDadosOS(String NumOS)
        {
            GetApiRetorno<List<DadosOS_ListModel>> retorno = new GetApiRetorno<List<DadosOS_ListModel>>();
            retorno.Objeto = new List<DadosOS_ListModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder("SELECT A.NUM_OBRA, A.NUM_OBRA_ORIGINAL, A.DATA_MAXIMA, A.HORA_MAXIMA, A.CODCT, A.PROJETO, A.NUM_DOC,")
                               .AppendLine(" A.STATUS_AUDITORIA, A.STATUS_EXECUCAO, A.EQUIPE, A.COD_TURNO_SERV ")
                               .AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ")
                               .AppendLine(" LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL B ")
                               .AppendLine(" ON(A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_OBRA = B.NUM_OBRA AND ")
                               .AppendLine("     A.NUM_DOC = B.NUM_DOC AND A.DATA_MAXIMA = CAST(B.DATAHORA_INICIO AS DATE)) ")
                               .AppendLine(" LEFT JOIN TB_CODIGO_ENCERRAMENTO C ")
                               .AppendLine(" ON(A.COD_ENCERRAMENTO = C.COD_ENCERRAMENTO AND A.CODCT = C.CODCT) ")
                               .AppendLine(" LEFT JOIN TB_TIPO_SERVICO_EQUIPE D ")
                               .AppendLine(" ON (A.COD_TIPO_SERV = D.COD_TIPO_SERV) ")
                               .AppendLine(" LEFT JOIN TB_SUB_TIPO_SERVICO E ")
                               .AppendLine(" ON (A.COD_TIPO_SERV = E.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = E.COD_SUB_TIPO_SERV) ");

                        List<String> filtros = new List<string>();
                        if (!String.IsNullOrEmpty(NumOS))
                        {
                            filtros.Add("A.NUM_OBRA = @NUM_OBRA");
                            parametros.Add("@NUM_OBRA", NumOS);
                        }

                        if (filtros.Count > 0)
                        {
                            sql.AppendLine(" WHERE " + String.Join(" AND ", filtros));
                        }

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {

                                var usuario = await retornaUsuarioLogado(Convert.ToString(reader["NUM_OBRA"]));

                                retorno.Objeto.Add(new DadosOS_ListModel
                                {
                                    numOS = Convert.ToString(reader["NUM_OBRA"]),
                                    dataAgendamento = reader["DATA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_MAXIMA"]).ToString("dd/MM/yyyy") : null,
                                    horaInicio = reader["HORA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["HORA_MAXIMA"].ToString()).ToString("HH:mm") : String.Empty,
                                    tecnico = Convert.ToString(reader["EQUIPE"]),
                                    status_auditoria = HtmlEnumExtensions.GetEnumDescription<Status_AuditoriaAniel>(Convert.ToString(reader["STATUS_AUDITORIA"])),
                                    status_execucao = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(reader["STATUS_EXECUCAO"])),
                                    turno = Convert.ToString(reader["COD_TURNO_SERV"]),
                                    codct = Convert.ToString(reader["CODCT"]),
                                    projeto = Convert.ToString(reader["PROJETO"]),
                                    num_doc = Convert.ToString(reader["NUM_DOC"]),
                                    usuario_ativo = usuario?.ativo ?? true,
                                    tipo_usuario = Convert.ToString(usuario?.tipo_usuario)
                                });

                            }
                        }
                    }
                }

                retorno.Ok = true;
            }
            catch(Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }

            //Page e retornando DataTable
            return retorno;
        }


        public async Task<ServicoUsuario> retornaUsuarioLogado(string numObra)
        {
            string numFre = null;
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                using (var repoAniel = new Conexao<SenhaAniel>(sessionT5))
                using (var repoFuncionarioAniel = new Conexao<FuncionarioAniel>(sessionT5))
                {
                    SenhaAniel usuario = await repoAniel.selectFirst(p => p.Usuario == User.Identity.GetUserId());
                    numFre = usuario?.Funcionario?.Num_Fre;
                }
            }

            //1) Busca os dados da OS no MongoDb
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<ServicoChatMongo>(conexaoMongo))
            {
                ServicoUsuario usuario = new ServicoUsuario();
                ServicoChatMongo chat = await repository.Find(p => p.num_os == numObra);
                if (chat != null)
                {
                    if (chat.usuarios != null)
                    {

                        //2) Buscar tipo de usuário atual vinculado à OS para verificar se a permissão é a mesma
                        var retornoOS = await GetPermissaoOS(numFre, numObra,chat.codct,chat.projeto,Convert.ToString(chat.num_doc));

                        if (retornoOS.Count > 0)
                        {
                            //3) Verificar se usuário logado possui acesso ao mongo
                            usuario = chat.usuarios.Find(p => p.num_fre == User.Identity.GetUserId() && p.tipo_usuario == retornoOS[0].tipo_usuario);
                        }
                        else
                        {
                            usuario = chat.usuarios.Find(p => p.num_fre == User.Identity.GetUserId());
                        }
                    }
                }
                else
                {
                    usuario.ativo = true;
                }
                return usuario;
            }
        }

        public async Task<String> GetHistoricoMensagensChat(String NumOs)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repositoryServico = new Conexao<Servico>(session))
            {
                var servico = await repositoryServico.selectFirst(p => p.NumOs == NumOs);
                var conversas = String.Join("", "Não há mensagens.");
                return conversas;
            }
        }

        public async Task<(String descricao, int? totalDias)> GetDataUltimaMensagem(string num_os)
        {
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<ServicoChatMongo>(conexaoMongo))
            {
                (String descricao, int? totalDias) dataSeparador = (null, null);
                ServicoChatMongo chat = await repository.Find(p => p.num_os == num_os);
                

                    if (chat != null)
                    {
                        if (chat.mensagens == null)
                        {
                            chat.mensagens = new List<Mensagem_ChatBackOfficeModels>();
                        }

                        if (chat.mensagens.Count > 0)
                        {                        
                          DateTime dataMensagem = chat.mensagens.Max(p => p.dataHora).Value.ToLocalTime().Date;
                          int totalDias = (DateTime.Today - dataMensagem).Days;
                          dataSeparador.totalDias = totalDias;

                        if (dataSeparador.totalDias == 0)
                            {
                                dataSeparador.descricao = "Hoje";
                            }
                            else if (dataSeparador.totalDias == 1)
                            {
                                dataSeparador.descricao = "Ontem";
                            }
                            else
                            {
                                DateTime dt = DateTime.Parse(String.Format("{0:dd/MM/yyyy}", dataMensagem), new CultureInfo("pt-BR"));
                                dataSeparador.descricao = String.Format("{0:dd/MM/yyyy}", dt);
                            }
                        }
                    }
                

                return dataSeparador;
            }
        }


        [HttpGet()]
        [Route("api/ChatApi/GetChatAtivo")]
        public async Task<bool> GetChatAtivo(String numOs)
        {
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<ServicoChat>(conexaoMongo))
            using (var repositoryChat = new RepositoryMongo<ServicoChatMongo>(conexaoMongo))
            {
                ServicoChatMongo servico = await repositoryChat.Find(p => p.num_os == numOs);

                if (servico != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [HttpGet()]
        [Route("api/ChatApi/DownloadAnexo")]
        public async Task DownloadAnexo(String numOs, String id_mensagem)
        {
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<ServicoChatMongo>(conexaoMongo))
            {
                var caminhoArquivo = "";
                var servicos = await repository.Find(p => p.num_os == numOs);
                var mensagens = new List<Mensagem_ChatBackOfficeModels>();

                mensagens.AddRange(servicos.mensagens);

                foreach (var item in servicos.mensagens)
                {
                    if (item.anexo != null && item.id == id_mensagem)
                    {
                        var arquivo = await GetAnexoChatBytes(numOs, item.id, item.anexo.fileExtension);

                        string nomeArquivo = $"{item.anexo.name}.{item.anexo.fileExtension}";

                        if (Directory.Exists($@"{CONSTANTES.CaminhoFisico}\Arquivos-Chat") == false)
                        {
                            Directory.CreateDirectory($@"{CONSTANTES.CaminhoFisico}\Arquivos-Chat");
                        }

                        caminhoArquivo = $@"{CONSTANTES.CaminhoFisico}\Arquivos-Chat\{nomeArquivo}";
                        if (File.Exists(caminhoArquivo) == false)
                            File.WriteAllBytes(caminhoArquivo, arquivo);
                    }
                }
            }
        }

        public async Task<byte[]> GetAnexoChatBytes(string num_os, string idMensagem, string fileExtension)
        {
            using (var client = new ConexaoMongo())
            using (var repository = new RepositoryMongo<ServicoChatMongo>(client))
            {
                if (String.IsNullOrEmpty(fileExtension))
                {
                    IMongoQueryable<ServicoChatMongo> query = repository.GetQueryable();
                    query = query.Where(p => p.dataEncerramento == null && p.num_os == num_os);
                    var chat = query.SingleOrDefault();

                    var mensagem = chat.mensagens.Find(p => p.id == idMensagem);
                    if (mensagem != null)
                    {
                        fileExtension = mensagem.anexo.fileExtension;
                    }
                }

                string nomeArquivo = $"{idMensagem}.{fileExtension}";
                GridFSBucket bucket = new GridFSBucket(client.GetConnection(), new GridFSBucketOptions
                {
                    BucketName = "MensagensChatAnexo",
                    ReadPreference = ReadPreference.Secondary
                });

                return await bucket.DownloadAsBytesByNameAsync(nomeArquivo);
            }
        }

        private String GerarLog(List<String> Lista_Forca_Trabalho, List<String> Lista_Ja_Agendados, Configuracoes_Gerais config)
        {
            return $"Força de Trabalho: {String.Join(",", Lista_Forca_Trabalho)}\nJa Agendados:{String.Join(",", Lista_Ja_Agendados)}\n" +
                   $"Exige Agendamento Para Criação dos Serviços ? {config.Exige_Agendamento_Servico}\n" +
                   $"Empresa Utilizará Roteirização de Serviços ? {config.Empresa_Roteirizacao_Servico}\n" +
                   $"Criterio Habilidade:{config.Criterio_Habilidade}\nCriterio Area: {config.Criterio_Area}\nCriterio SLA:{config.Criterio_SLA_Execucao}";
        }

        public String FormataData(String Data)
        {
            var dataretorno = "";



            return dataretorno;
        }

        [HttpPost]
        [Route("api/ChatApi/AdicionarAnexoChatConnect")]
        public async Task<Response<Mensagem_ChatBackOfficeModels>> AdicionarAnexoChatConnect(String m)
        {
            Response<Mensagem_ChatBackOfficeModels> resposta = new Response<Mensagem_ChatBackOfficeModels>();
            try
            {
                var mensagem = Newtonsoft.Json.JsonConvert.DeserializeObject<Mensagem_ChatBackOfficeModels>(m);

                HttpContext context = HttpContext.Current;

                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                var provider = new MultipartMemoryStreamProvider();

                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (var file in provider.Contents.Take(1))
                {
                    var buffer = await file.ReadAsByteArrayAsync();
                    var stream = new MemoryStream(buffer);

                    using (ISession session = HibernateUtil.GetSession())
                    using (var repositoryConfigGerais = new Conexao<Configuracoes_Gerais>(session))
                    {
                        var config_gerais = await repositoryConfigGerais.selectFirst();

                        if (config_gerais.Utiliza_Armazenamento_Nuvem) //Cloud Storage
                        {
                            CloudStorageService gcloud = new CloudStorageService(config_gerais.CredentialJson_Cloud_Storage, config_gerais.ProjectId_Cloud_Storage);
                            var nomeArquivo = $"{mensagem.id}.{mensagem.anexo.fileExtension}";

                            using (var client = new ConexaoMongo())
                            using (var repository = new RepositoryMongo<ServicoChatMongo>(client))
                            {
                                string url_anexo = await gcloud.UploadArquivoEmNuvem(nomeArquivo, mensagem.anexo.mimeType, stream, CloudStorageBucketEnum.SERVICO_CHAT_ANEXO);

                                mensagem.anexo.url = url_anexo;
                                mensagem.anexo.isUrl = true;

                                IMongoQueryable<ServicoChatMongo> query = repository.GetQueryable();
                                query = query.Where(p => p.dataEncerramento == null && p.num_os == mensagem.num_os);
                                var chat = query.SingleOrDefault();

                                chat.mensagens.Add(mensagem);

                                await repository.Update(p => p.num_os == chat.num_os, chat);
                            }
                        }
                        else //Salva a estrutura no MongDb
                        {
                            var sucesso = await SalvarAnexoBytes(mensagem.num_os, mensagem, stream);
                        }
                    }
                }

                resposta.result = true;
                resposta.valor = mensagem;
            }
            catch (Exception ex)
            {
                resposta.result = false;
                resposta.message = ex.Message;
            }

            return resposta;
        }

        [AcceptVerbs("Get", "Post")]
        [Route("api/ChatApi/GetPermissaoOS")]
        public async Task<List<ServicoUsuario>> GetPermissaoOS(string Num_fre, string NumOS, string Cod_Ct, string Projeto, string Num_Doc)
        {
            List<ServicoUsuario> retorno = new List<ServicoUsuario>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    if (!String.IsNullOrEmpty(NumOS))
                    {
                        sql.Append(" SELECT C.NUM_FRE, C.NOME, C.FOTO, 0 AS TIPO FROM TB_DOCUMENTO_PRODUCAO A ")
                       .AppendLine("   INNER JOIN TB_EQUIPE B ON B.EQUIPE = A.EQUIPE ")
                       .AppendLine("   INNER JOIN TB_FUNCIONARIO C ON C.NUM_FRE = B.EQUIPE ")
                       .AppendLine("    WHERE C.NUM_FRE = @NUM_FRE AND (A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_DOC = @NUMDOC AND A.NUM_OBRA = @NUM_OBRA) ")
                       .AppendLine("   UNION ")
                       .AppendLine("  SELECT C.NUM_FRE, C.NOME, C.FOTO, 1 AS TIPO FROM TB_DOCUMENTO_PRODUCAO A ")
                       .AppendLine("   INNER JOIN TB_EQUIPE B ON B.EQUIPE = A.EQUIPE ")
                       .AppendLine("   INNER JOIN TB_FUNCIONARIO C ON C.NUM_FRE = B.COD_SUPERV ")
                       .AppendLine("   WHERE C.NUM_FRE = @NUM_FRE AND (A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_DOC = @NUMDOC AND A.NUM_OBRA = @NUM_OBRA) ")
                       .AppendLine("   UNION  ")
                       .AppendLine("  SELECT C.NUM_FRE, C.NOME, C.FOTO, 2 AS TIPO FROM TB_DOCUMENTO_PRODUCAO A ")
                       .AppendLine("   INNER JOIN TB_EQUIPE B ON B.EQUIPE = A.EQUIPE   ")
                       .AppendLine("   INNER JOIN TB_FUNCIONARIO C ON C.NUM_FRE = B.USUARIO_MONITOR ")
                       .AppendLine("   WHERE C.NUM_FRE = @NUM_FRE AND (A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_DOC = @NUMDOC AND A.NUM_OBRA = @NUM_OBRA) ");

                        parametros.Add("@NUM_FRE", Num_fre);
                        parametros.Add("@NUM_OBRA", NumOS);
                        parametros.Add("@CODCT", Cod_Ct);
                        parametros.Add("@PROJETO", Projeto);
                        parametros.Add("@NUMDOC", Num_Doc);

                    }
                    else
                    {
                        sql.Append(" SELECT C.NUM_FRE, C.NOME, C.FOTO, 0 AS TIPO FROM TB_DOCUMENTO_PRODUCAO A ")
                        .AppendLine("   INNER JOIN TB_EQUIPE B ON B.EQUIPE = A.EQUIPE ")
                        .AppendLine("   INNER JOIN TB_FUNCIONARIO C ON C.NUM_FRE = B.EQUIPE ")
                        .AppendLine("   WHERE C.NUM_FRE = @NUM_FRE ")
                        .AppendLine("   UNION ")
                        .AppendLine("  SELECT C.NUM_FRE, C.NOME, C.FOTO, 1 AS TIPO FROM TB_DOCUMENTO_PRODUCAO A ")
                        .AppendLine("   INNER JOIN TB_EQUIPE B ON B.EQUIPE = A.EQUIPE ")
                        .AppendLine("   INNER JOIN TB_FUNCIONARIO C ON C.NUM_FRE = B.COD_SUPERV ")
                        .AppendLine("   WHERE C.NUM_FRE = @NUM_FRE ")
                        .AppendLine("   UNION  ")
                        .AppendLine("  SELECT C.NUM_FRE, C.NOME, C.FOTO, 2 AS TIPO FROM TB_DOCUMENTO_PRODUCAO A ")
                        .AppendLine("   INNER JOIN TB_EQUIPE B ON B.EQUIPE = A.EQUIPE   ")
                        .AppendLine("   INNER JOIN TB_FUNCIONARIO C ON C.NUM_FRE = B.USUARIO_MONITOR ")
                        .AppendLine("   WHERE C.NUM_FRE = @NUM_FRE ");

                        parametros.Add("@NUM_FRE", Num_fre);
                    }



                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var equipe = new ServicoUsuario
                            {
                                ativo = true,
                                num_fre = Convert.ToString(reader["NUM_FRE"]),
                                nome = Convert.ToString(reader["NOME"]),
                                //foto_usuario = Convert.ToString(reader["FOTO"]),
                                tipo_usuario = reader["TIPO"] != DBNull.Value ? (TipoUsuarioChatEnum)Enum.Parse(typeof(TipoUsuarioChatEnum), Convert.ToString(reader["TIPO"])) : TipoUsuarioChatEnum.AGENTE
                            };

                            retorno.Add(equipe);
                        }
                    }
                }
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/ChatApi/GravarDadosAgendamento")]
        public async Task<PostApiRetorno<String>> GravarDadosAgendamento(AgendamentoChat value)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();
            try
            {
                Documento_ProducaoAniel modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    var repositoryAgendamento = new Conexao<Agendamento_EquipeAniel>(session);

                    using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(session))
                    using (var repoSubTipoServico = new Conexao<Sub_Tipo_ServicoAniel>(session))
                    using (var repository = new Conexao<Documento_ProducaoAniel>(session))
                    using (var repositoryTurno = new Conexao<Turno_ServicoAniel>(session))
                    {
                        //if (!String.IsNullOrEmpty(value.Id))
                        modelo = (await repository.select(p => p.CodCt == value.CodCt && p.Projeto == value.projeto &&
                                                               p.Num_Doc == Convert.ToInt32(value.num_doc) && p.Num_Obra == value.num_obra)).FirstOrDefault();

                        var data_old = modelo.Data_Maxima;
                        var hora_old = modelo.Hora_Maxima;
                        var lat_old = modelo.Latitude_Str;
                        var long_old = modelo.Longitude_Str;
                        var status_old = modelo.Status;
                        var turnoOld = modelo.CodTurnoServ;

                        if (modelo == null)
                        {
                            modelo = new Documento_ProducaoAniel
                            {

                            };
                        }
                        else
                        {
                            DateTime temp;
                            if (DateTime.TryParse(value.data, out temp))
                            {
                                //Convert.ToDateTime(String.Format(value.Data, "dd/MM/yyyy") + ' ' + String.Format(value.Hora, "hh:mm"));
                                modelo.Data_Maxima = Convert.ToDateTime(String.Format(value.data, "dd/MM/yyyy"));

                                if (!String.IsNullOrEmpty(value.hora))
                                    modelo.Hora_Maxima = Convert.ToDateTime(String.Format(value.hora, "hh:mm"));
                            }

                            if (modelo.Data_Maxima != data_old || modelo.Hora_Maxima != hora_old)
                            {
                                if (new List<StatusExecucaoAniel> { StatusExecucaoAniel.Fechada_Produtiva,
                                StatusExecucaoAniel.Fechada_Improdutiva,
                                StatusExecucaoAniel.Atendimento_Finalizado}.Contains(modelo.Status.Value))
                                {
                                    throw new Exception($"Não será possível alterar o agendamento do serviço, pois o mesmo se encontra fechado.");
                                }
                            }

                            if (!string.IsNullOrEmpty(value.turno))
                            {
                                if (new List<StatusExecucaoAniel> { StatusExecucaoAniel.Fechada_Produtiva,
                                StatusExecucaoAniel.Fechada_Improdutiva,
                                StatusExecucaoAniel.Atendimento_Finalizado}.Contains(modelo.Status.Value))
                                {
                                    throw new Exception($"Não será possível alterar o turno do serviço, pois o mesmo se encontra fechado.");
                                }

                                modelo.CodTurnoServ = Convert.ToInt32(value.turno);
                                modelo.TurnoServico = await repositoryTurno.selectFirst(p => p.Id == Convert.ToInt32(value.turno));
                            }


                            if (modelo.Data_Maxima != null)
                            {
                                if (modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Agendamento
                                    || modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Responsavel
                                    || modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Atendimento)
                                    modelo.Status = modelo.Equipe != null ? StatusExecucaoAniel.Aberta_Aguard_Atendimento : StatusExecucaoAniel.Aberta_Aguard_Responsavel;

                                if (modelo.Data_Maxima != data_old)
                                    modelo.Usuario_Agendamento = User.Identity.GetUserId();
                            }


                            using (var documento_api = new Documento_ProducaoApiController())
                            {

                                if (status_old != modelo.Status)
                                {
                                    ValidaAlteracaoDocumento(modelo);
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                "Mudança de Status",
                                                                                HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(status_old.Value),
                                                                                User.Identity.GetUserId(),
                                                                                null,
                                                                                null,
                                                                                HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(modelo.Status.Value), null);
                                }
                                if (lat_old != modelo?.Latitude_Str || long_old != modelo?.Longitude_Str)
                                {
                                    ValidaAlteracaoDocumento(modelo);
                                    List<String> lista_latlng = new List<string>();
                                    lista_latlng.Add(lat_old);
                                    lista_latlng.Add(long_old);

                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                "Latitude/Longitude",
                                                                                String.Join(",", lista_latlng),
                                                                                User.Identity.GetUserId(),
                                                                                null,
                                                                                null,
                                                                                $"{modelo.Latitude_Str},{modelo.Longitude_Str}", null);
                                }

                                if (turnoOld != modelo?.CodTurnoServ)
                                {
                                    ValidaAlteracaoDocumento(modelo);;

                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                "Turno",
                                                                                null,
                                                                                User.Identity.GetUserId(),
                                                                                null,
                                                                                null,
                                                                                $"{modelo.Latitude_Str},{modelo.Longitude_Str}", null);
                                }

                                if (data_old != modelo?.Data_Maxima)
                                {
                                    ValidaAlteracaoDocumento(modelo);
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                "Agendamento",
                                                                                String.Concat(data_old?.ToString("dd/MM/yyyy"), " ", hora_old?.ToString(@"HH:mm")),
                                                                                User.Identity.GetUserId(),
                                                                                null,
                                                                                null,
                                                                                modelo.Data_Maxima.HasValue ? modelo?.Data_Maxima.Value.Add(modelo.Hora_Maxima.Value.TimeOfDay).ToString() : null, null);
                                }
                                else if (hora_old?.TimeOfDay != modelo?.Hora_Maxima?.TimeOfDay)
                                {
                                    ValidaAlteracaoDocumento(modelo);
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                "Agendamento",
                                                                                String.Concat(data_old?.ToString("dd/MM/yyyy"), " ", hora_old?.ToString(@"HH:mm")),
                                                                                User.Identity.GetUserId(),
                                                                                null,
                                                                                null,
                                                                                modelo.Hora_Maxima.HasValue ? modelo?.Hora_Maxima.ToString() : null, null);
                                }
                            }

                            await repository.SaveOrUpdate(modelo);

                            if (modelo.Equipe != null && (hora_old != modelo?.Hora_Maxima || data_old != modelo.Data_Maxima) && modelo.Equipe != "FALSE")
                            {
                                var equipe = await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == modelo.Equipe);
                                if (!String.IsNullOrEmpty(equipe.Device_Id))
                                {
                                    MobileService.NotificarSmartphone(equipe.Device_Id, String.Empty, "Houve mudanças em sua rotina.", "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS", modelo.CodCt, modelo.Projeto, modelo.Num_Doc.ToString(), modelo.Num_Obra);
                                }
                            }

                        }
                    }
                    retorno.IdObjeto = "";
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = "";
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        private void ValidaAlteracaoDocumento(Documento_ProducaoAniel modelo)
        {
            if (new List<StatusExecucaoAniel> { StatusExecucaoAniel.Fechada_Produtiva,
                                                                        StatusExecucaoAniel.Fechada_Improdutiva,
                                                                        StatusExecucaoAniel.Atendimento_Finalizado}.Contains(modelo.Status.Value))
            {
                throw new Exception($"Não será possível gravar dados do serviço, pois o mesmo se encontra fechado.");
            }

            if (new List<StatusExecucaoAniel> { StatusExecucaoAniel.Cancelado }.Contains(modelo.Status.Value))
            {
                throw new Exception($"Não será possível gravar dados do serviço, pois o mesmo se encontra cancelado.");
            }

            if (modelo.Responsabilidade == ResponsabilidadeOS.TECNICO)
            {
                throw new Exception($"Não será possível gravar dados do serviço, pois o mesmo se esta na responsabilidade do técnico.");
            }
        }



        //[HttpPost]
        //[Route("api/ChatApi/verificaPermissaoAcesso")]
        //public async Task<Response<ServicoChat>> verificaPermissaoAcesso(String num_fre, String numOs)
        //{

        //    var resposta = new Response<ServicoChat>>();

        //    using (var session = HibernateUtil.GetSession())
        //    using (var cliente_api = new ClienteApiController())
        //    using (var gestao_api = new Gestao_EquipeApiController())
        //    {
        //        //verifica se possui acesso à area


        //        //caso não tenha acesso, verifica se possui vínculo a essa OS

        //        var usuariosComAacesso = await gestao_api.GetParticipantes(numOS);
        //        if (usuariosComAacesso.FindAll(p => p.num_fre == num_fre).Count > 0)
        //        {
        //            //pode fazer tudo
        //        }
        //        else if (temareaacesso)
        //        {
        //            // pode ler
        //        }
        //        else
        //        {
        //            throw new Exception("VAI EMBORA USUARIO SEM VERGONHA");
        //        }
        //    }


        //    return resposta;
        //}

        #region Mobile Touch
        [AllowAnonymous]
        [HttpPost]
        [Route("api/ChatApi/SalvarAnexoChat")]
        public async Task<Response<Mensagem_ChatBackOfficeModels>> SalvarAnexoChat()
        {
            Response<Mensagem_ChatBackOfficeModels> resposta = new Response<Mensagem_ChatBackOfficeModels>();
            try
            {
                HttpContext context = HttpContext.Current;
                String objeto = context.Request.Form["json"];

                ParamWebApi<ChatBackTecnico_SalvarAnexoModel> fotoParams =
                    JsonService.DeserializeJasonMapper<ParamWebApi<ChatBackTecnico_SalvarAnexoModel>>(objeto);

                using (ISession session = HibernateUtil.GetSession())
                using (var repositoryConfigGerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    var config_gerais = await repositoryConfigGerais.selectFirst();

                    if (config_gerais.Utiliza_Armazenamento_Nuvem) //Cloud Storage
                    {
                        //var credentialJson = $@"{CONSTANTES.CaminhoFisico}\anielconnect-356113-5aff4e8b189a.json";
                        //var projectId = CloudStorageService.getProjectIdDebug();

                        CloudStorageService gcloud = new CloudStorageService(config_gerais.CredentialJson_Cloud_Storage, config_gerais.ProjectId_Cloud_Storage);
                        var nomeArquivo = $"{fotoParams.Objeto.mensagem.id}.{fotoParams.Objeto.mensagem.anexo.fileExtension}";

                        MemoryStream arquivo = new MemoryStream();

                        if (context.Request.Files.Count > 0)
                            context.Request.Files[0].InputStream.CopyTo(arquivo);

                        if (arquivo.Length > 0)
                        {
                            using (var client = new ConexaoMongo())
                            using (var repository = new RepositoryMongo<ServicoChatMongo>(client))
                            {
                                fotoParams.Objeto.file = arquivo;

                                string url_anexo = await gcloud.UploadArquivoEmNuvem(nomeArquivo, fotoParams.Objeto.mensagem.anexo.mimeType, arquivo, CloudStorageBucketEnum.SERVICO_CHAT_ANEXO);

                                fotoParams.Objeto.mensagem.anexo.url = url_anexo;
                                fotoParams.Objeto.mensagem.anexo.isUrl = true;

                                IMongoQueryable<ServicoChatMongo> query = repository.GetQueryable();
                                query = query.Where(p => p.dataEncerramento == null && p.num_os == fotoParams.Objeto.num_os);
                                var chat = query.SingleOrDefault();

                                chat.mensagens.Add(fotoParams.Objeto.mensagem);

                                await repository.Update(p => p.num_os == chat.num_os, chat);
                            }
                        }
                    }
                    else //Armazenamento no MongoDB
                    {
                        MemoryStream arquivo = new MemoryStream();

                        if (context.Request.Files.Count > 0)
                            context.Request.Files[0].InputStream.CopyTo(arquivo);

                        if (arquivo.Length > 0)
                        {

                            fotoParams.Objeto.file = arquivo;
                            //fotoParams.Objeto.mensagem.anexo.arquivoBytes = arquivo.ToArray();

                            var sucesso = await SalvarAnexoBytes(fotoParams.Objeto.num_os, fotoParams.Objeto.mensagem, arquivo);

                        }

                    }
                }

                resposta.result = true;
                resposta.valor = fotoParams.Objeto.mensagem;
            }
            catch (Exception ex)
            {
                resposta.result = false;
                resposta.message = ex.Message;
            }

            return resposta;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/ChatApi/GetAnexoChatBytes")]
        public async Task<Response<String>> GetAnexoChatBytes(ParamWebApi<ChatBackTecnico_GetAnexoBytesModel> value)
        {
            Response<String> retorno = new Response<String>();
            try
            {
                String CaminhoProjeto = CONSTANTES.CaminhoFisico;
                String CaminhoProjetoWeb = CONSTANTES.URLPortal;

                if (String.IsNullOrEmpty(CaminhoProjeto))
                {
                    throw new Exception("Parâmetro CaminhoProjeto não informado no Config.");
                }

                if (String.IsNullOrEmpty(CaminhoProjetoWeb))
                {
                    throw new Exception("CaminhoProjetoWebApi não definido no web.config");
                }


                var bytes = await GetAnexoChatBytes(value.Objeto.num_os, value.Objeto.idMensagem, value.Objeto.fileExtension);

                string nomeArquivo = $"{value.Objeto.idMensagem}_{value.Objeto.equipe}.{value.Objeto.fileExtension}";
                String diretorio = String.Format("{0}\\Arquivos-Chat", CaminhoProjeto);
                String caminho = String.Format("{0}\\Arquivos-Chat\\{1}", CaminhoProjeto, nomeArquivo);

                if (!Directory.Exists(diretorio))
                {
                    Directory.CreateDirectory(diretorio);
                }

                if (!File.Exists(caminho))
                {
                    File.WriteAllBytes(caminho, bytes);
                }

                retorno.valor = String.Format("{0}/Arquivos-Chat/{1}", CaminhoProjetoWeb, nomeArquivo);


                retorno.result = true;
            }

            catch (Exception ex)
            {
                retorno.result = false;
                retorno.message = ex.Message;
            }

            return retorno;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/ChatApi/DeleteTempFileAnexoChat")]
        public Response DeleteTempFileAnexoChat(ParamWebApi<ChatBackTecnico_GetAnexoBytesModel> value)
        {
            Response retorno = new Response();
            try
            {

                String CaminhoProjeto = CONSTANTES.CaminhoFisico;
                String CaminhoProjetoWeb = CONSTANTES.URLPortal;

                if (String.IsNullOrEmpty(CaminhoProjeto))
                {
                    throw new Exception("Parâmetro CaminhoProjeto não informado no Config.");
                }

                if (String.IsNullOrEmpty(CaminhoProjetoWeb))
                {
                    throw new Exception("CaminhoProjetoWebApi não definido no web.config");
                }

                string nomeArquivo = $"{value.Objeto.idMensagem}_{value.Objeto.equipe}.{value.Objeto.fileExtension}";
                String caminho = String.Format("{0}\\Arquivos-Chat\\{1}", CaminhoProjeto, nomeArquivo);

                if (File.Exists(caminho))
                {
                    File.Delete(caminho);
                }

                retorno.result = true;
            }

            catch (Exception ex)
            {
                retorno.result = false;
                retorno.message = ex.Message;
            }

            return retorno;
        }

        public async Task<bool> SalvarAnexoBytes(string num_os, Mensagem_ChatBackOfficeModels mensagem, MemoryStream arquivo)
        {
            bool retorno = false;

            using (var client = new ConexaoMongo())
            using (var repository = new RepositoryMongo<ServicoChatMongo>(client))
            {

                int chunkSize = arquivo.Length <= 1048576 ? 51200 : 1048576;
                string nomeArquivo = $"{mensagem.id}.{mensagem.anexo.fileExtension}";

                GridFSBucket bucket = new GridFSBucket(client.GetConnection(), new GridFSBucketOptions
                {
                    BucketName = "MensagensChatAnexo",
                    ChunkSizeBytes = chunkSize, // 1MB ou 512KB
                    WriteConcern = WriteConcern.WMajority,
                    ReadPreference = ReadPreference.Secondary
                });

                await bucket.UploadFromBytesAsync(nomeArquivo, arquivo.ToArray());

                IMongoQueryable<ServicoChatMongo> query = repository.GetQueryable();
                query = query.Where(p => p.dataEncerramento == null && p.num_os == num_os);
                var chat = query.SingleOrDefault();

                if (!chat.mensagens.Exists(x => x.id == mensagem.id))
                {
                    chat.mensagens.Add(mensagem);
                }

                await repository.Update(p => p.num_os == chat.num_os, chat);
                retorno = true;
            }

            return retorno;
        }

        //public async Task<byte[]> GetAnexoChatBytes(string num_os, string idMensagem, string fileExtension)
        //{
        //    using (var client = new ConexaoMongo())
        //    using (var repository = new RepositoryMongo<ServicoChatMongo>(client))
        //    {
        //        if (String.IsNullOrEmpty(fileExtension))
        //        {
        //            IMongoQueryable<ServicoChatMongo> query = repository.GetQueryable();
        //            query = query.Where(p => p.dataEncerramento == null && p.num_os == num_os);
        //            var chat = query.SingleOrDefault();

        //            var mensagem = chat.mensagens.Find(p => p.id == idMensagem);
        //            if (mensagem != null)
        //            {
        //                fileExtension = mensagem.anexo.fileExtension;
        //            }
        //        }

        //        string nomeArquivo = $"{idMensagem}.{fileExtension}";
        //        GridFSBucket bucket = new GridFSBucket(client.GetConnection(), new GridFSBucketOptions
        //        {
        //            BucketName = "MensagensChatAnexo",
        //            ReadPreference = ReadPreference.Secondary
        //        });

        //        return await bucket.DownloadAsBytesByNameAsync(nomeArquivo);
        //    }
        //}
        #endregion
    }
}