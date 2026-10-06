
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Hubsoft;
using Aniel.T5.DAO.map;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Utils;
using Microsoft.AspNet.SignalR;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Impl;
using OpenXmlPowerTools;
using Resources;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Twilio;
using Twilio.Rest.Conversations.V1;

namespace Aniel.Connect.Socket.ChatHub
{
    public class ChatHub : Hub
    {
        public static List<(ConcurrentQueue<AtendimentoChat> fila, Int64? Id_CategoriaCliente, Escritorio_Area? Escritorio_Area)> FilasEsperaCliente
        {
            get
            {
                var atendimentos = getAtendimentos();
                var filas = atendimentos.Values.Where(p => p?.suporte?.idSuporte == null && p.bot == false).GroupBy(x => new { x?.cliente?.idCategoriaCliente, x.cliente?.Escritorio_Area })
                    .Select(g => (fila: new ConcurrentQueue<AtendimentoChat>(g.ToList().OrderBy(p => Convert.ToDateTime(p.dataAbertura))), g.Key.idCategoriaCliente, g.Key.Escritorio_Area))
                    .ToList();
                return filas;
            }
        }
        public static string IdConexaoClienteOla = null;

        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new ConcurrentDictionary<string, SemaphoreSlim>();

        public static List<SuporteChatMongo> SuporteDisponiveis
        {
            get
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
                {

                    IMongoQueryable<SuporteChatMongo> query = repository.GetQueryable();
                    var suporte = query.ToList();
                    return suporte;
                }
            }
        }

        public static List<SuporteChatMongo> SuporteDisponiveisAtualizado
        {
            get
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
                {

                    IMongoQueryable<SuporteChatMongo> query = repository.GetQueryable();
                    var suporte = query.ToList();

                    using (ISession session = HibernateUtil.GetSession())
                    using (var repoAgente = new Conexao<Perfil_Agente>(session))
                    using (var repoUsuario = new Conexao<IdentityUser>(session))
                    {
                        Dictionary<long, Perfil_Agente> perfis = new Dictionary<long, Perfil_Agente>();
                        foreach (var item in suporte)
                        {
                            var usuario = repoUsuario.selectFirst(p => p.Id == item.idSuporte).Result;
                            if (usuario != null)
                            {
                                Perfil_Agente agente = null;

                                if (usuario.Id_Perfil_Agente != null && !perfis.ContainsKey(usuario.Id_Perfil_Agente.Value))
                                {
                                    agente = repoAgente.selectFirst(p => p.Id == usuario.Id_Perfil_Agente).Result;
                                    perfis.Add(usuario.Id_Perfil_Agente.Value, agente);
                                }
                                else if (usuario.Id_Perfil_Agente != null)
                                {
                                    agente = perfis[usuario.Id_Perfil_Agente.Value];
                                }

                                item.Categorias = agente == null ? new List<long?>() : agente.Categorias.Select(p => p.Categoria_Cliente?.Id).ToList(); ;
                                item.Localidades = agente == null ? new List<Escritorio_Area>() : agente.Localidades.Select(p => new Escritorio_Area { Cod_Area = p.Cod_Area, Cod_Escritorio = p.Cod_Escritorio }).ToList(); ;
                                item.qtdMaximaAtendimentos = agente == null ? 0 : agente.Numero_Max_Atendi_Simultaneo_Por_Agente.GetValueOrDefault(0);
                                item.nomeAgente = usuario?.UserName;
                                item.inicioAtendimento = agente?.Inicio_Atendimento;
                                item.terminoAtendimento = agente?.Termino_Atendimento;


                            }
                        }



                    }
                    return suporte;
                }


            }
        }


        public static List<AtendimentoChat> AtendimentosAguardandoConfirmacao = new List<AtendimentoChat>();
        public static ConcurrentDictionary<string, AtendimentoChat> Atendimentos
        {
            get
            {
                return getAtendimentos();
            }
        }


        public static IChat GetChatSrategy(AtendimentoChat chat, string ConnectionId = null)
        {

            if (chat.conversationSid != null)
            {
                var chatStrategy = new ChatConversation(chat.TwilioAccountId, chat.TwilioAuthToken);
                chatStrategy.HubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                return chatStrategy;

            }
            else if (chat.telegramChatId != null)
            {
                var chatStrategy = new ChatBotTelegram(chat.telegramToken, chat.telegramChatId, ConnectionId);
                chatStrategy.HubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                return chatStrategy;
            }
            else if (chat.messengerChatId != null)
            {
                var chatStrategy = new ChatBotMessenger(chat.messengerToken, chat.messengerChatId);
                chatStrategy.HubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                return chatStrategy;
            }
            else if (chat.whatsAppChatId != null)
            {
                var chatStrategy = new ChatBotWhatsApp(chat.whatsAppToken, chat.whatsAppChatId, ConnectionId);
                chatStrategy.HubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                return chatStrategy;
            }
            else
            {
                var chatStrategy = new ChatOLA();
                //if(!String.IsNullOrEmpty(IdConexaoClienteOla))
                // chatStrategy.ConnectionId = IdConexaoClienteOla;

                chatStrategy.HubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                return chatStrategy;
            }

        }

        public async Task IniciarChat(AtendimentoChat chat)
        {
            String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Atendimento\Chat\Teste";

            if (!Directory.Exists(_pasta))
            {
                Directory.CreateDirectory(_pasta);
            }

            File.WriteAllText($"{_pasta}/teste.txt", "textoteste");
            await GetChatSrategy(chat, Context.ConnectionId).StartChat(chat);
        }

        public async Task IniciarChatV2(String chat)
        {
            String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Atendimento\Chat\Teste";

            if (!Directory.Exists(_pasta))
            {
                Directory.CreateDirectory(_pasta);
            }
            IdConexaoClienteOla = Context.ConnectionId;
            var chatObject = JsonConvert.DeserializeObject<AtendimentoChat>(chat);
            File.WriteAllText($"{_pasta}/teste.txt", "textoteste");
            await GetChatSrategy(chatObject, Context.ConnectionId).StartChat(chatObject);
        }

        public async Task buscaAtivaAtendimento(string suporte)
        {
            try
            {
                if (SuporteDisponiveis.FindAll(x => x.idSuporte == suporte).Count() > 0)
                {
                    using (var conexaoMongo = new ConexaoMongo())
                    using (var repository = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
                    {
                        var suporteObj = SuporteDisponiveis.FirstOrDefault(x => x.idSuporte == suporte);
                        suporteObj.datahoraAlive = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        await repository.Update(p => p.idSuporte == suporte, suporteObj);
                        await FindClienteNecessitaSuporte(suporteObj);
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
        }
        public async Task iniciarConexaoSuporte(SuporteChatModels suporte)
        {
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
            {

                if (SuporteDisponiveis.FindAll(x => x.idSuporte == suporte.idSuporte).Count() <= 0)
                {
                    using (var agente_api = new AgenteApiController())
                    {
                        suporte.status = StatusAtendimento.DISPONIVEL;
                        //suporte.foto = String.IsNullOrEmpty(suporte.foto) ? await agente_api.getFotoAgente(suporte.idSuporte) : suporte.foto;

                        try
                        {
                            SuporteChatMongo suporteMongo = new SuporteChatMongo
                            {
                                terminoAtendimento = suporte.terminoAtendimento,
                                idSuporte = suporte.idSuporte,
                                Categorias = suporte.Categorias,
                                foto = suporte.foto,
                                idConexaoChat = suporte.idConexaoChat,
                                inicioAtendimento = suporte.inicioAtendimento,
                                Localidades = suporte.Localidades,
                                MotivoAusencia = suporte.MotivoAusencia,
                                nomeAgente = suporte.nomeAgente,
                                qtdMaximaAtendimentos = suporte.qtdMaximaAtendimentos,
                                status = suporte.status,
                                datahoraAlive = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                            };

                            await repository.Save(suporteMongo);
                        }
                        catch (Exception)
                        {

                        }
                    }

                    await FindClienteNecessitaSuporte(suporte);
                }

                else
                {
                    var tecnico = SuporteDisponiveis.Find(x => x.idSuporte == suporte.idSuporte);
                    tecnico.Categorias = suporte.Categorias;
                    tecnico.Localidades = suporte.Localidades;
                    tecnico.idConexaoChat = suporte.idConexaoChat;
                    tecnico.terminoAtendimento = suporte.terminoAtendimento;
                    tecnico.idSuporte = suporte.idSuporte;
                    tecnico.foto = suporte.foto;
                    tecnico.idConexaoChat = suporte.idConexaoChat;
                    tecnico.inicioAtendimento = suporte.inicioAtendimento;
                    tecnico.MotivoAusencia = suporte.MotivoAusencia;
                    tecnico.nomeAgente = suporte.nomeAgente;
                    tecnico.qtdMaximaAtendimentos = suporte.qtdMaximaAtendimentos;
                    tecnico.status = suporte.status;
                    tecnico.datahoraAlive = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");


                    if (tecnico.MotivoAusencia == "Realizou Logoff")
                    {
                        tecnico.MotivoAusencia = null;
                        tecnico.status = StatusAtendimento.DISPONIVEL;
                    }

                    await repository.Update(p => p.idSuporte == tecnico.idSuporte, tecnico);

                    switch (tecnico.status)
                    {
                        case StatusAtendimento.EM_ATENDIMENTO:
                            foreach (var item in Atendimentos.Values.Where(p => p.suporte?.idSuporte == suporte.idSuporte))
                            {
                                item.suporte = suporte;
                                await EntrarChatAtendimento(item.protocolo, suporte.idSuporte, Context.ConnectionId);
                            }

                            break;

                        case StatusAtendimento.DISPONIVEL:
                            await FindClienteNecessitaSuporte(suporte);
                            break;
                        case StatusAtendimento.AGUARD_CONFIRMACAO:
                            suporte.status = StatusAtendimento.DISPONIVEL;
                            await FindClienteNecessitaSuporte(suporte);
                            break;
                    }



                }

                Clients.Client(suporte.idConexaoChat).statusUsuario(getSuporteStatus(suporte.idSuporte).status != StatusAtendimento.AUSENTE);

            }
        }

        public async Task finalizarConexaoSuporte(String id_suporte)
        {
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
            using (var API = new AtendimentoApiController())
            {
                var responseStatus = getSuporteStatus(id_suporte);

                if (responseStatus.status != StatusAtendimento.AUSENTE)
                {
                    await API.AlterarStatusAgente(new AlterarStatusModels
                    {
                        online = false,
                        idSuporte = id_suporte,
                        motivoAusencia = "Realizou Logoff"
                    });
                }
            }

        }

        public async Task finalizarConexaoSuporte(AtendimentoChat atendimento, String idsuporte)
        {
            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
                {
                    await repository.Delete(p => p.idSuporte == idsuporte);

                    if (Atendimentos.ContainsKey(atendimento.protocolo))
                    {
                        await NegarAtendimento(atendimento, idsuporte);
                        await finalizarConexaoSuporte(idsuporte);

                    }
                }

            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
        }

        public static async Task EnviarNotificacaoSuporte(AtendimentoChat chat, Boolean reincidente = false, String SuporteReincidente = null)
        {
            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
                {


                    if (Atendimentos.ContainsKey(chat.protocolo))
                    {
                        await GetChatSrategy(chat).EnviarNotificacaoSuporte(chat, reincidente, SuporteReincidente);


                    }
                }

            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
        }


        public async Task AceitarAtendimento(AtendimentoChat atendimento, string username)
        {
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            using (var repositorySuporte = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
            using (var repositoryConvidado = new RepositoryMongo<UsuarioConvidado>(conexaoMongo))
            {
                //AtendimentoChatMongo chatMongo = await repository.Find(p => p.protocolo == atendimento.protocolo);
                //List<UsuarioConvidado> listaConvidados = chatMongo.usuariosConvidados;
                //var convidado = listaConvidados.Find(x => x.usuarioAceite == atendimento.suporte.idSuporte);

                //convidado.connectionid = Context.ConnectionId;
                //convidado.aceite = true;
                //convidado.dataAceite = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                //await repositorySuporte.Update(p => p.usuarioAceite == convidado.usuarioAceite, convidado);

                //await Clients.Group(atendimento.protocolo, Context.ConnectionId);

                if (atendimento.suporte != null)
                {

                    var index = AtendimentosAguardandoConfirmacao.FindIndex(x => x.protocolo == atendimento.protocolo);

                    if (index != -1)
                        AtendimentosAguardandoConfirmacao.RemoveAt(index);

                    var suporte = SuporteDisponiveis.Find(x => x.idSuporte == atendimento.suporte.idSuporte);
                    suporte.status = StatusAtendimento.EM_ATENDIMENTO;
                    await repositorySuporte.Update(p => p.idSuporte == suporte.idSuporte, suporte);

                    atendimento.suporte = suporte;
                    await Clients.Group(atendimento.protocolo, Context.ConnectionId).suporteAceito(atendimento.suporte);


                    if (Atendimentos.ContainsKey(atendimento.protocolo))
                    {
                        await EntrarChatAtendimento(atendimento.protocolo, username, Context.ConnectionId);
                        Atendimentos[atendimento.protocolo].suporte = atendimento.suporte;
                        using (var atendimento_api = new AtendimentoApiController())
                        {
                            await atendimento_api.StatusEmAtendimento(atendimento.protocolo, atendimento.suporte.idSuporte);
                        }

                        foreach (var item in FilasEsperaCliente)
                        {
                            var fila = item.fila.Where(x => x.protocolo == atendimento.protocolo);
                            if (fila.Count() > 0)
                            {
                                item.fila.TryDequeue(out _);
                            }
                        }

                        IMongoQueryable<AtendimentoChatMongo> query = repository.GetQueryable();
                        var atendimentos = query.ToList().FindAll(p => p.protocolo == atendimento.protocolo).ToList();
                        AtendimentoChatMongo atendimentoChatMongo = atendimentos.FirstOrDefault();

                        if (atendimentoChatMongo != null)
                        {
                            ConvertMongoAtendimentoChat(atendimento, atendimentoChatMongo);
                            await repository.Update(p => p.protocolo == atendimento.protocolo, atendimentoChatMongo);
                        }

                    }
                }
            }
        }

        public async Task AlterarDepartamento(String protocolo, String Departamento, String username)
        {
            var chat = Atendimentos[protocolo];
            var suporte = SuporteDisponiveis.Find(p => p.idSuporte == username);

            using (ISession session = HibernateUtil.GetSession())
            {
                var departamento = new DepartamentoApiController().FindById(Convert.ToInt64(Departamento)).Result;

                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                {
                    await GetChatSrategy(chat, Context.ConnectionId).EnviarMensagem(protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                    {
                        dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                        enviadoPeloSuporte = true,
                        usuario = suporte?.nomeAgente,
                        fluxoChat = true,
                        statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                        mensagem = "Chat transferido para o setor " + departamento[0].Label,
                        conversasId = chat.conversas != null ? chat.conversas.FirstOrDefault()?.id : null,
                        matriculaUsuario = chat?.suporte?.idSuporte,
                    }));
                }
            }
        }

        public async Task AceitarConviteAtendimento(String protocolo, String usernameConvidado, string idConexao, bool aceite)
        {
            var chat = Atendimentos[protocolo];
            var suporte = SuporteDisponiveis.Find(p => p.idSuporte == usernameConvidado);

            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            {
                //Aceitou o convite
                if (aceite == true)
                {
                    if (idConexao != null)
                    {
                        await Groups.Add(idConexao, protocolo);
                    }

                    await GetChatSrategy(chat, Context.ConnectionId).EnviarMensagem(protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                    {
                        dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                        enviadoPeloSuporte = true,
                        usuario = suporte.nomeAgente,
                        fluxoChat = true,
                        statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                        mensagem = "{NOME DO AGENTE} aceitou a sua solicitação para o chat",
                        conversasId = chat.conversas != null ? chat.conversas.FirstOrDefault()?.id : null,
                        matriculaUsuario = chat?.suporte?.idSuporte,
                    }));

                }
                else
                {
                    await GetChatSrategy(chat, Context.ConnectionId).EnviarMensagem(protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                    {
                        dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                        enviadoPeloSuporte = true,
                        usuario = suporte.nomeAgente,
                        fluxoChat = true,
                        statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                        mensagem = "{NOME DO AGENTE} recusou a sua solicitação para o chat",
                        conversasId = chat.conversas != null ? chat.conversas.FirstOrDefault()?.id : null,
                        matriculaUsuario = chat?.suporte?.idSuporte,
                    }));
                }
            }
        }


        public async Task EntrarChatAtendimento(String protocolo, String username, string idConexao)
        {
            if (Atendimentos.ContainsKey(protocolo))
            {
                var chat = Atendimentos[protocolo];
                var usuario = chat.usuarios.Find(p => p.userid == username);
                if (usuario?.userid == null)
                {
                    chat.usuarios.Add(new AtendimentoUsuario { userid = username, connectionid = idConexao });
                    var suporte = SuporteDisponiveis.Find(p => p.idSuporte == username);

                    if (chat.usuariosConvidados.Find(p => p.usuarioAceite == username) != null)
                    {
                        using (var conexaoMongo = new ConexaoMongo())
                        using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                        {
                            AtendimentoChatMongo chatMongo = await repository.Find(p => p.protocolo == protocolo);
                            var convite = chatMongo.usuariosConvidados.Find(p => p.usuarioAceite == username);
                            convite.aceite = true;
                            convite.dataAceite = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                            await repository.Update(p => p.protocolo == protocolo, chatMongo);
                        }
                    }

                    if (suporte != null)
                    {
                        using (var conexaoMongo = new ConexaoMongo())
                        using (ISession session = HibernateUtil.GetSession())
                        using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                        using (var repositoryFunc = new Conexao<IdentityUser>(session))
                        {
                            AtendimentoChatMongo chatMongo = await repository.Find(p => p.protocolo == protocolo);
                            chatMongo.usuarios.Add(new AtendimentoUsuario { userid = username, connectionid = idConexao });
                            chat.bot = false;
                            chatMongo.bot = false;



                            if (chatMongo.suporte != null)
                            {
                                //Recupera o nome do suporte
                                if (string.IsNullOrEmpty(suporte.nomeAgente))
                                {
                                    var agente = await repositoryFunc.selectFirst(p => p.Id == suporte.idSuporte);
                                    if (!string.IsNullOrEmpty(agente?.Nome))
                                    {
                                        chatMongo.suporte.nomeAgente = agente?.Nome;
                                    }
                                }
                                else
                                {
                                    chatMongo.suporte.nomeAgente = suporte.nomeAgente;
                                }
                            }

                            if (chatMongo.suporte != null)
                                chatMongo.suporte.idConexaoChat = idConexao;

                            await repository.Update(p => p.protocolo == protocolo, chatMongo);

                            if (String.IsNullOrEmpty(suporte?.inicioAtendimento) == false && chatMongo.suporte?.idSuporte == null)
                            {
                                await GetChatSrategy(chat, Context.ConnectionId).EnviarMensagem(protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                                {
                                    dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                    enviadoPeloSuporte = true,
                                    usuario = suporte.nomeAgente,
                                    fluxoChat = true,
                                    statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                                    mensagem = "{NOME DO AGENTE} entrou na conversa",
                                    conversasId = chat.conversas != null ? chat.conversas.FirstOrDefault()?.id : null,
                                    matriculaUsuario = chat?.suporte?.idSuporte,
                                }));

                                await GetChatSrategy(chat, Context.ConnectionId).EnviarMensagem(protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                                {
                                    dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                    enviadoPeloSuporte = true,
                                    usuario = suporte.nomeAgente,
                                    statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                                    mensagem = suporte.inicioAtendimento,
                                    conversasId = chat.conversas != null ? chat.conversas.FirstOrDefault()?.id : null,
                                    matriculaUsuario = chat?.suporte?.idSuporte,
                                }));
                            }
                            else if (String.IsNullOrEmpty(suporte?.inicioAtendimento) == false && chatMongo.suporte.idSuporte != username)
                            {
                                await GetChatSrategy(chat, Context.ConnectionId).EnviarMensagem(protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                                {
                                    dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                    enviadoPeloSuporte = true,
                                    usuario = suporte.nomeAgente,
                                    fluxoChat = true,
                                    statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                                    mensagem = "{NOME DO AGENTE} entrou na conversa",
                                    conversasId = chat.conversas != null ? chat.conversas.FirstOrDefault()?.id : null,
                                    matriculaUsuario = chat?.suporte?.idSuporte,
                                }));

                                await GetChatSrategy(chat, Context.ConnectionId).EnviarMensagem(protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                                {
                                    dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                    enviadoPeloSuporte = true,
                                    usuario = suporte.nomeAgente,
                                    statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                                    mensagem = suporte.inicioAtendimento,
                                    conversasId = chat.conversas != null ? chat.conversas.FirstOrDefault()?.id : null,
                                    matriculaUsuario = chat?.suporte?.idSuporte,
                                }));
                            }
                        }
                    }

                    if (idConexao != null)
                    {
                        await Groups.Add(idConexao, protocolo);
                    }

                }
                else if (usuario.connectionid != idConexao)
                {
                    try
                    {
                        using (var conexaoMongo = new ConexaoMongo())
                        using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                        {
                            var atendimento_chat = await repository.Find(x => x.protocolo == protocolo);
                            foreach (var item in atendimento_chat.usuarios)
                            {
                                if (item.connectionid == usuario.connectionid)
                                {
                                    item.connectionid = idConexao;
                                }
                            }
                            await repository.Update(x => x.protocolo == protocolo, atendimento_chat);
                        }

                        usuario.connectionid = idConexao;
                        if (idConexao != null)
                        {
                            await Groups.Add(idConexao, protocolo);
                        }
                    }
                    catch (Exception ex)
                    {
                        CONSTANTES.log4Net.Error(ex.Message, ex);
                    }
                }
            }


        }

        public async Task EscutarAtendimento(String protocolo, String username, string idConexao)
        {
            if (Atendimentos.ContainsKey(protocolo))
            {
                var chat = Atendimentos[protocolo];
                if (idConexao != null)
                {
                    await Groups.Add(idConexao, protocolo);
                }
            }


        }

        public async Task SairChatAtendimento(String protocolo, String idsuporte)
        {
            if (Atendimentos.ContainsKey(protocolo))
            {
                var atendimento = Atendimentos[protocolo];
                var usuarios = atendimento.usuarios.FindAll(p => p.userid == idsuporte);

                atendimento.usuarios.RemoveAll(p => p.userid == idsuporte);

                if (atendimento.suporte?.idSuporte == idsuporte)
                {
                    atendimento.suporte = null;
                }

                await GetChatSrategy(atendimento, Context.ConnectionId).EnviarMensagem(protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                {
                    dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                    enviadoPeloSuporte = true,
                    usuario = atendimento?.suporte?.nomeAgente == null ? idsuporte : atendimento?.suporte?.nomeAgente,
                    fluxoChat = true,
                    statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                    mensagem = "{NOME DO AGENTE} saiu da conversa",
                    conversasId = atendimento.conversas != null ? atendimento.conversas.FirstOrDefault()?.id : null,
                    matriculaUsuario = atendimento?.suporte?.idSuporte,
                }));

                foreach (var item in usuarios)
                {
                    try
                    {
                        //await Clients.Client(atendimento?.cliente?.idConexaoChat).NotificarTransferenciaChat(protocolo);
                        await Groups.Remove(item.connectionid, protocolo);
                    }
                    catch (Exception ex)
                    {
                        CONSTANTES.log4Net.Error(ex.Message, ex);
                    }
                }


            }
        }


        public async Task<Response> EnviarMensagem(String protocolo, String m)
        {
            var mensagem = JsonConvert.DeserializeObject<Mensagem_ChatModels>(m);
            var response = new Response();

            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                {
                    var atendimento = await repository.Find(x => x.protocolo == protocolo);
                    if (atendimento != null)
                    {
                        /*Reabrir chat*/
                        if (!String.IsNullOrEmpty(atendimento.dataEncerramento))
                        {
                            atendimento.dataEncerramento = null;
                            atendimento.conversas.Add(new Conversas_Chat
                            {
                                id = Convert.ToString(Guid.NewGuid()),
                                data_inicio = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                            });

                            await repository.Update(x => x.protocolo == atendimento.protocolo, atendimento);
                        }
                    }

                    response = await GetChatSrategy(Atendimentos[protocolo], Context.ConnectionId).EnviarMensagem(protocolo, m);
                }

                return response;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                if (mensagem?.arquivos != null)
                {
                    foreach (var arq in mensagem.arquivos)
                    {
                        if (File.Exists(arq.url))
                        {
                            File.Delete(arq.url);
                        }
                    }
                }

                return response;
            }

        }

        public async Task MensagemLida(String protocolo)
        {
            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                using (var repositorySuporte = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
                using (var repositoryConvidado = new RepositoryMongo<UsuarioConvidado>(conexaoMongo))
                using (var repositoryMensagens = new RepositoryMongo<Mensagem_ChatModels>(conexaoMongo))
                {
                    AtendimentoChatMongo atendimentoChatMongo = await repository.Find(p => p.protocolo == protocolo);

                    List<Mensagem_ChatModels> listaMensagem = new List<Mensagem_ChatModels>();
                    var mensagensNaoLidas = atendimentoChatMongo.mensagens.FindAll(x => x.statusEnvio == Model.enumerado.StatusEnvioEnum.ENVIADO).ToList();

                    if (mensagensNaoLidas.Count > 0)
                    {
                        foreach (var item in atendimentoChatMongo.mensagens)
                        {

                            if (item.statusEnvio == Model.enumerado.StatusEnvioEnum.ENVIADO)
                                item.statusEnvio = Model.enumerado.StatusEnvioEnum.LIDO;

                        }

                        await repository.Update(p => p.protocolo == protocolo, atendimentoChatMongo);
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
        }


        public async Task<Response> FinalizarAtendimento(String protocolo, bool StatusAtendimentoFinalizado, String Usuario, Int64? IdMotivoStatus)
        {
            var response = new Response();
            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                using (var repositorySuporte = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
                {
                    await MensagemLida(protocolo);
                    if (Atendimentos.ContainsKey(protocolo))
                    {
                        var atendimento = Atendimentos[protocolo];
                        var suporte = atendimento.suporte;

                        if (atendimento.suporte.idSuporte != Usuario)
                        {
                            response.message = "{ERROR_NOT_USER}";
                            return response;
                        }

                        if (String.IsNullOrEmpty(atendimento?.suporte?.terminoAtendimento) == false)
                        {
                            await GetChatSrategy(atendimento, Context.ConnectionId).EnviarMensagem(atendimento.protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                            {
                                dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                enviadoPeloSuporte = true,
                                usuario = atendimento?.suporte?.nomeAgente,
                                statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                                mensagem = atendimento?.suporte?.terminoAtendimento,
                                conversasId = atendimento.conversas != null ? atendimento.conversas.FirstOrDefault()?.id : null,
                                matriculaUsuario = atendimento?.suporte?.idSuporte
                            }));
                        }

                        await GetChatSrategy(atendimento, Context.ConnectionId).EnviarMensagem(atendimento.protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                        {
                            dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                            enviadoPeloSuporte = true,
                            statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                            mensagem = "Concluiu chat",
                            usuario = atendimento.suporte?.nomeAgente,
                            fluxoChat = true,
                            conversasId = atendimento.conversas != null ? atendimento.conversas.FirstOrDefault()?.id : null,
                            matriculaUsuario = atendimento?.suporte?.idSuporte
                        }));

                        atendimento = Atendimentos[protocolo];
                        if (String.IsNullOrEmpty(atendimento.cliente.idConexaoChat) == false)
                        {
                            await Clients.Client(atendimento.cliente.idConexaoChat).FinalizarAtendimento("");
                        }

                        //try
                        //{
                        //    if (atendimento.suporte?.idConexaoChat != null)
                        //        await Groups.Remove(atendimento.suporte?.idConexaoChat, protocolo);

                        //    if (atendimento.cliente?.idConexaoChat != null)
                        //        await Groups.Remove(atendimento.cliente?.idConexaoChat, protocolo);
                        //}
                        //catch (Exception ex)
                        //{
                        //    CONSTANTES.log4Net.Error(ex.Message, ex);
                        //}

                        string conversasId = String.Empty;
                        using (var atendimento_api = new AtendimentoApiController())
                        {
                            if (StatusAtendimentoFinalizado)
                            {
                                await atendimento_api.FinalizarAtendimento(protocolo, Usuario, IdMotivoStatus.Value);
                            }

                            conversasId = atendimento.conversas != null ? atendimento.conversas[atendimento.conversas.Count - 1].id : null;
                            await atendimento_api.GravarHistoricoMensagensChat(protocolo, atendimento.mensagens.FindAll(p => (p.statusEnvio == Model.enumerado.StatusEnvioEnum.ENVIADO || p.statusEnvio == Model.enumerado.StatusEnvioEnum.LIDO) && p.conversasId == conversasId));

                        }

                        if (String.IsNullOrEmpty(atendimento.conversationSid) == false)
                        {
                            TwilioClient.Init(atendimento.TwilioAccountId, atendimento.TwilioAuthToken);
                            ConversationResource.Delete(pathSid: atendimento.conversationSid);
                        }

                        Atendimentos.TryRemove(protocolo, out _);


                        AtendimentoChatMongo atendimentoChatMongo = await repository.Find(x => x.protocolo == atendimento.protocolo);
                        if (atendimentoChatMongo != null)
                        {
                            //atendimento.dataEncerramento = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                            //ConvertMongoAtendimentoChat(atendimento, atendimentoChatMongo);
                            //await repository.Update(p => p.protocolo == atendimento.protocolo, atendimentoChatMongo);

                            AtendimentoChatMongo chatMongo = await repository.Find(x => x.protocolo == atendimento.protocolo);
                            chatMongo.bot = false;
                            chatMongo.dataEncerramento = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                            if (chatMongo.conversas != null && chatMongo.conversas.Count > 0)
                            {
                                chatMongo.conversas[chatMongo.conversas.Count - 1].data_final = chatMongo.dataEncerramento;
                            }
                            await repository.Update(x => x.protocolo == atendimento.protocolo, chatMongo);
                        }
                        try
                        {
                            //await Groups.Remove(atendimento.suporte?.idConexaoChat, protocolo);
                            //await Groups.Remove(atendimento.cliente?.idConexaoChat, protocolo);
                            if (atendimento?.cliente?.idConexaoChat != null)
                            {
                                await Clients.Client(atendimento.cliente?.idConexaoChat).FinalizarAtendimento();
                            }
                        }
                        catch (Exception ex)
                        {
                            CONSTANTES.log4Net.Error(ex.Message, ex);
                        }


                        if (SuporteDisponiveis.Find(x => x.idSuporte == suporte.idSuporte) != null)
                        {
                            AtendimentoChatMongo chatMongoSuporte = await repository.Find(x => x.dataEncerramento == null &&
                                                                                               x.suporte.idSuporte == suporte.idSuporte);


                            if (chatMongoSuporte == null)
                            {
                                var suporteAtualizar = SuporteDisponiveis.Find(x => x.idSuporte == suporte.idSuporte);
                                suporteAtualizar.status = StatusAtendimento.DISPONIVEL;
                                await repositorySuporte.Update(p => p.idSuporte == suporteAtualizar.idSuporte, suporteAtualizar);
                            }

                        }

                        if (suporte.idConexaoChat != null)
                            await FindClienteNecessitaSuporte(suporte);

                        response.result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return response;
        }

        public async Task<Response> FinalizarChat(String protocolo, bool StatusAtendimentoFinalizado, String Usuario, Int64? IdMotivoStatus)
        {
            var response = new Response();
            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                {
                    if (Atendimentos.ContainsKey(protocolo))
                    {
                        var atendimento = Atendimentos[protocolo];
                        var suporte = atendimento.suporte;

                        if (atendimento.suporte.idSuporte != Usuario)
                        {
                            response.message = "{ERROR_NOT_USER}";
                            return response;
                        }

                        if (String.IsNullOrEmpty(atendimento?.suporte?.terminoAtendimento) == false)
                        {
                            await GetChatSrategy(atendimento, Context.ConnectionId).EnviarMensagem(atendimento.protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                            {
                                dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                enviadoPeloSuporte = true,
                                usuario = atendimento?.suporte?.nomeAgente,
                                statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                                mensagem = atendimento?.suporte?.terminoAtendimento,
                                conversasId = atendimento.conversas != null ? atendimento.conversas.FirstOrDefault()?.id : null,
                                matriculaUsuario = atendimento?.suporte?.idSuporte,
                            }));
                        }

                        await GetChatSrategy(atendimento, Context.ConnectionId).EnviarMensagem(atendimento.protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                        {
                            dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                            enviadoPeloSuporte = true,
                            statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                            mensagem = "Concluiu chat",
                            usuario = atendimento.suporte?.nomeAgente,
                            fluxoChat = true,
                            conversasId = atendimento.conversas != null ? atendimento.conversas.FirstOrDefault()?.id : null,
                            matriculaUsuario = atendimento?.suporte?.idSuporte
                        }));

                        atendimento = Atendimentos[protocolo];
                        if (String.IsNullOrEmpty(atendimento.cliente.idConexaoChat) == false)
                        {
                            await Clients.Client(atendimento.cliente.idConexaoChat).FinalizarAtendimento();
                        }

                        string conversasId = String.Empty;
                        using (var atendimento_api = new AtendimentoApiController())
                        {
                            conversasId = atendimento.conversas != null ? atendimento.conversas[atendimento.conversas.Count - 1].id : null;
                            await atendimento_api.GravarHistoricoMensagensChat(protocolo, atendimento.mensagens.FindAll(p => (p.statusEnvio == Model.enumerado.StatusEnvioEnum.ENVIADO || p.statusEnvio == Model.enumerado.StatusEnvioEnum.LIDO) && p.conversasId == conversasId));
                        }


                        AtendimentoChatMongo atendimentoChatMongo = await repository.Find(x => x.protocolo == atendimento.protocolo);

                        if (atendimentoChatMongo != null)
                        {
                            AtendimentoChatMongo chatMongo = await repository.Find(x => x.protocolo == atendimento.protocolo);
                            chatMongo.bot = false;
                            chatMongo.dataEncerramento = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff");
                            if (chatMongo.conversas != null && chatMongo.conversas.Count > 0)
                            {
                                chatMongo.conversas[chatMongo.conversas.Count - 1].data_final = chatMongo.dataEncerramento;
                            }
                            await repository.Update(x => x.protocolo == atendimento.protocolo, chatMongo);
                        }

                        response.result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return response;
        }

        public static async Task AtualizaNotificacoesSino(String id)
        {
            var suporte = ChatHub.SuporteDisponiveis.Find(p => p.idSuporte == id);
            if (suporte?.idConexaoChat != null)
            {
                await GlobalHost.ConnectionManager.GetHubContext<ChatHub>().Clients.Client(suporte.idConexaoChat).AtualizarNotificacoes();
            }
        }

        public async Task NegarAtendimento(AtendimentoChat atendimento, String idsuporte)
        {
            try
            {
                if (Atendimentos.ContainsKey(atendimento.protocolo))
                {
                    using (var conexaoMongo = new ConexaoMongo())
                    using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                    {
                        if (Atendimentos[atendimento.protocolo].usuariosConvidados != null && Atendimentos[atendimento.protocolo].usuariosConvidados.Find(p => p.usuarioAceite == idsuporte) != null)
                        {

                            var chat = Atendimentos[atendimento.protocolo];
                            var convite = chat.usuariosConvidados.Find(p => p.usuarioAceite == idsuporte);
                            convite.aceite = false;

                            AtendimentoChatMongo atendimentoChatMongo = new AtendimentoChatMongo();
                            ConvertMongoAtendimentoChat(chat, atendimentoChatMongo);
                            await repository.Update(p => p.protocolo == atendimento.protocolo, atendimentoChatMongo);



                        }

                        if (Atendimentos[atendimento.protocolo].usuariosEnvioNotificao != null && Atendimentos[atendimento.protocolo].usuariosEnvioNotificao.Find(p => p.userid == idsuporte) != null)
                        {

                            var chatMongo = await repository.Find(p => p.protocolo == atendimento.protocolo);
                            if (chatMongo != null)
                            {
                                var envio = chatMongo.usuariosEnvioNotificao.Find(p => p.userid == idsuporte);
                                envio.recusado = true;
                                await repository.Update(p => p.protocolo == chatMongo.protocolo, chatMongo);
                            }
                        }

                    }


                    if (Atendimentos[atendimento.protocolo].suporte?.idSuporte == null)
                    {
                        await GetChatSrategy(atendimento, Context.ConnectionId).EnviarNotificacaoSuporte(atendimento, true, idsuporte);
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
        }


        private async Task SendFromClient(AtendimentoChat chat, String mensagem)
        {
            var m = new Mensagem_ChatModels()
            {
                dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                enviadoPeloSuporte = true,
                mensagem = mensagem,
                statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIANDO

            };
            chat.mensagens.Add(m);
            Atendimentos[chat.protocolo] = chat;
            await Clients.Client(chat.cliente.idConexaoChat).MensagemCliente(m);
        }

        public static Int64 GetPosicaoAtendimento(AtendimentoChat chat)
        {
            var findFilaEspera = ChatHub.FilasEsperaCliente?
                            .Find(p =>
                                (p.Id_CategoriaCliente, p.Escritorio_Area?.Cod_Escritorio, p.Escritorio_Area?.Cod_Area) ==
                                (chat.cliente?.idCategoriaCliente, chat.cliente?.Escritorio_Area?.Cod_Escritorio, chat.cliente?.Escritorio_Area?.Cod_Area));

            var filaEspera = findFilaEspera?.fila;
            if (filaEspera == null)
                return 1;
            else
            {
                if (filaEspera.Count > 0)
                {
                    return filaEspera.Select((item, idx) => new { Item = item, Index = idx })
                            .Where(x => x.Item.protocolo == chat.protocolo)
                            .Select(x => x.Index).FirstOrDefault() + 1;
                }
                else
                    return 1;
            }
        }


        public async Task<String> MensagensChat(String protocolo)
        {
            var mensagens = new List<Mensagem_ChatModels>();
            if (Atendimentos.ContainsKey(protocolo))
            {
                var atendimento = Atendimentos[protocolo];
                mensagens.AddRange(atendimento.mensagens);

                var mensagensFiltradas = mensagens
                    .Where(p => p.statusEnvio == Model.enumerado.StatusEnvioEnum.ENVIADO ||
                                p.statusEnvio == Model.enumerado.StatusEnvioEnum.LIDO)
                    .OrderBy(p => string.IsNullOrEmpty(p.dataHoraServidor) ? DateTime.MinValue : Convert.ToDateTime(p.dataHoraServidor))
                    .ToList();

                return JsonConvert.SerializeObject(mensagensFiltradas);
            }
            else
            {
                using (var atendimentoapi = new AtendimentoApiController())
                {
                    var conversas = await atendimentoapi.GetHistoricoMensagensChat(protocolo);
                    return conversas;
                }
            }


        }



        public static bool MensagemEnviada(AtendimentoChat chat, string idSuporte)
        {
            bool mensagemEnviada = false;

            var ultimaNotificacaoProximo = chat.usuariosEnvioNotificao.Find(p => p.userid == idSuporte);
            if (ultimaNotificacaoProximo != null)
            {
                mensagemEnviada = true;
            }


            return mensagemEnviada;
        }

        public async Task FindClienteNecessitaSuporte(SuporteChatModels suporte)
        {
            var filas = FilasEsperaCliente.Select(p =>
            {
                bool atendeCriterio = true;
                if (suporte.Categorias != null && suporte.Categorias.Count > 0 && suporte.Categorias.Contains(p.Id_CategoriaCliente))
                {
                    atendeCriterio = false;
                }

                if (suporte.Localidades != null && suporte.Localidades.Count() > 0)
                {
                    if (suporte.Localidades.Any(y => y.Cod_Area == p.Escritorio_Area?.Cod_Area && y.Cod_Escritorio == p.Escritorio_Area?.Cod_Escritorio) == false)
                    {
                        atendeCriterio = false;
                    }
                }

                if (atendeCriterio == true)
                    return p;
                else
                    return (null, 0, null);


            }).ToList().Where(p => p.fila != null).ToList();

            if (filas.Count > 0)
            {
                var fila = filas.FirstOrDefault().fila;
                if (fila.Count() > 0)
                {
                    var novaFila = new ConcurrentQueue<AtendimentoChat>(fila);

                    while (novaFila.Count > 0)
                    {
                        AtendimentoChat atendimentofila = null;
                        novaFila.TryPeek(out atendimentofila);
                        var atendimento = Atendimentos[atendimentofila.protocolo];

                        if (atendimento != null && (atendimento.dataHoraEnviadoAtendente == null || (DateTime.Now - Convert.ToDateTime(atendimento.dataHoraEnviadoAtendente)).TotalSeconds > 30))
                        {
                            var retornoEnviarNotificacaoSuporte = await GetChatSrategy(atendimento, Context.ConnectionId).EnviarNotificacaoSuporte(atendimento);
                            //if (!retornoEnviarNotificacaoSuporte.notificacaoEnviada)
                            //    break;
                        }

                        break;
                        //novaFila.TryDequeue(out _);
                    }
                }
            }
        }

        public async Task AlterarStatusAgente(AlterarStatusModels alterarStatusRequest)
        {
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
            {
                var suporte = SuporteDisponiveis.Find(p => p.idSuporte == alterarStatusRequest.idSuporte);
                if (suporte != null)
                {
                    if (alterarStatusRequest.online)
                    {
                        suporte.status = StatusAtendimento.DISPONIVEL;
                        suporte.MotivoAusencia = "";
                    }
                    else
                    {
                        suporte.status = StatusAtendimento.AUSENTE;
                        suporte.MotivoAusencia = alterarStatusRequest.motivoAusencia;
                    }
                }

                await repository.Update(p => p.idSuporte == suporte.idSuporte, suporte);
            }
        }

        public static GetStatusResponse getSuporteStatus(string idSuporte)
        {
            var suporte = SuporteDisponiveis.Find(p => p.idSuporte == idSuporte);
            if (suporte != null)
            {
                return new GetStatusResponse
                {
                    status = suporte.status,
                    motivoAusencia = suporte.MotivoAusencia,
                    qtdAtendimento = Atendimentos.Values.Where(p => p.suporte?.idSuporte == idSuporte).Count()
                };
            }
            else
                return new GetStatusResponse
                {
                    status = StatusAtendimento.AUSENTE
                };

        }

        public static bool ChatActive(string protocolo)
        {
            if (Atendimentos.ContainsKey(protocolo))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task EnviarConvite(string protocolo, string idSuporte)
        {
            if (Atendimentos.ContainsKey(protocolo))
            {
                var chat = Atendimentos[protocolo];
                string idConexaoChat = SuporteDisponiveis.Find(p => p.idSuporte == idSuporte)?.idConexaoChat;
                if (idConexaoChat != null)
                    await GlobalHost.ConnectionManager.GetHubContext<ChatHub>().Clients.Client(idConexaoChat).clienteAguardandoAtendimento($"Voce foi convidado para um novo atendimento ... \n Deseja aceitar o convite?", chat);
            }
        }

        public static string GetChatSuporte(string protocolo)
        {
            if (Atendimentos.ContainsKey(protocolo))
            {
                return Atendimentos[protocolo].suporte?.idSuporte;
            }
            else
            {
                return null;
            }
        }

        public static string GetMensagemReplace(AtendimentoChat chat, String mensagem, string usuario)
        {
            return mensagem.Replace("{NOME DO AGENTE}", usuario).Replace("{NOME DO CLIENTE}", chat.cliente.nome);
        }

        private static ConcurrentDictionary<string, AtendimentoChat> getAtendimentos()
        {
            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                {
                    IMongoQueryable<AtendimentoChatMongo> query = repository.GetQueryable();
                    query = query.Where(p => p.dataEncerramento == null);
                    var atendimentos = query.ToList().Select(p => ConvertSeriableAtendimentoChat(p)).Where(p => p.protocolo != null).ToList();
                    if (atendimentos != null && atendimentos.Count > 0)
                        return new ConcurrentDictionary<string, AtendimentoChat>(atendimentos.ToDictionary(p => p.protocolo, p => p));
                    else
                        return new ConcurrentDictionary<string, AtendimentoChat>();

                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return new ConcurrentDictionary<string, AtendimentoChat>();
            }
        }

        public static AtendimentoChat ConvertSeriableAtendimentoChat(AtendimentoChatMongo p)
        {
            return new AtendimentoChat
            {
                cliente = p.cliente,
                conversationSid = p.conversationSid,
                dataAbertura = p.dataAbertura,
                dataEncerramento = p.dataEncerramento,
                descricaoProblema = p.descricaoProblema,
                mensagens = p.mensagens,
                motivo = p.motivo,
                protocolo = p.protocolo,
                suporte = p.suporte,
                usuarios = p.usuarios,
                idCanal = p.idCanal,
                bot = p.bot,
                idUltimoEvento = p.idUltimoEvento,
                TwilioAccountId = p.TwilioAccountId,
                TwilioAuthToken = p.TwilioAuthToken,
                telegramChatId = p.telegramChatId,
                telegramToken = p.telegramToken,
                messengerToken = p.messengerToken,
                messengerChatId = p.messengerChatId,
                backOfficeId = p.backOfficeId,

                usuariosConvidados = p.usuariosConvidados,
                whatsAppChatId = p.whatsAppChatId,
                whatsAppToken = p.whatsAppToken,
                conversas = p.conversas,
                dataHoraEnviadoAtendente = p.dataHoraEnviadoAtendente,
                usuariosEnvioNotificao = p.usuariosEnvioNotificao
            };
        }

        public static void ConvertMongoAtendimentoChat(AtendimentoChat p, AtendimentoChatMongo atendimentoChatMongo)
        {
            atendimentoChatMongo.cliente = p.cliente;
            atendimentoChatMongo.conversationSid = p.conversationSid;
            atendimentoChatMongo.dataAbertura = p.dataAbertura;
            atendimentoChatMongo.dataEncerramento = p.dataEncerramento;
            atendimentoChatMongo.descricaoProblema = p.descricaoProblema;
            atendimentoChatMongo.mensagens = p.mensagens;
            atendimentoChatMongo.conversas = p.conversas;
            atendimentoChatMongo.motivo = p.motivo;
            atendimentoChatMongo.protocolo = p.protocolo;
            atendimentoChatMongo.suporte = p.suporte;
            atendimentoChatMongo.usuarios = p.usuarios;
            atendimentoChatMongo.idCanal = p.idCanal;
            atendimentoChatMongo.bot = p.bot;
            atendimentoChatMongo.idUltimoEvento = p.idUltimoEvento;
            atendimentoChatMongo.TwilioAccountId = p.TwilioAccountId;
            atendimentoChatMongo.TwilioAuthToken = p.TwilioAuthToken;
            atendimentoChatMongo.telegramChatId = p.telegramChatId;
            atendimentoChatMongo.telegramToken = p.telegramToken;
            atendimentoChatMongo.messengerToken = p.messengerToken;
            atendimentoChatMongo.messengerChatId = p.messengerChatId;
            atendimentoChatMongo.backOfficeId = p.backOfficeId;
            atendimentoChatMongo.instagramChatId = p.instagramChatId;
            atendimentoChatMongo.JsonArmazenamento = p.JsonArmazenamento;
            atendimentoChatMongo.whatsAppChatId = p.whatsAppChatId;
            atendimentoChatMongo.whatsAppToken = p.whatsAppToken;
            atendimentoChatMongo.whatsAppChatId = p.whatsAppChatId;
            atendimentoChatMongo.whatsAppToken = p.whatsAppToken;
            atendimentoChatMongo.dataHoraEnviadoAtendente = p.dataHoraEnviadoAtendente;
            atendimentoChatMongo.usuariosEnvioNotificao = p.usuariosEnvioNotificao;
            atendimentoChatMongo.crud_telegram_whatsapp = p.crud_telegram_whatsapp;
        }


        public async Task<bool> TransferirChat(string protocolo, string novoSuporte, string novoDepartamento = null)
        {
            var semaphore = _locks.GetOrAdd(protocolo, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();

            try
            {
                return await TransferirChatInterno(protocolo, novoSuporte, novoDepartamento);
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return false;
            }
            finally
            {
                semaphore.Release();

                // Remove o semáforo se ele não estiver mais sendo usado
                if (semaphore.CurrentCount == 1)
                {
                    SemaphoreSlim removed;
                    _locks.TryRemove(protocolo, out removed);
                }
            }
        }

        public async Task<bool> TransferirChatInterno(string protocolo, string novoSuporte, string novoDepartamento = null)
        {

            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                {

                    if (Atendimentos.ContainsKey(protocolo))
                    {
                        var atendimento = Atendimentos[protocolo];

                        //sair de atendimento 
                        var transferir = false;
                        if (String.IsNullOrEmpty(novoSuporte) == false || String.IsNullOrEmpty(novoDepartamento) == false)
                        {
                            transferir = true;
                        }

                        if (transferir)
                        {
                            var suporte = atendimento?.suporte?.idSuporte;
                            if (suporte != null)
                            {
                                await SairChatAtendimento(protocolo, suporte);
                                atendimento.suporte = null;
                            }

                            //verificar mudanca de setor?
                            if (!String.IsNullOrEmpty(novoDepartamento))
                            {
                                await AlterarDepartamento(protocolo, novoDepartamento, novoSuporte ?? suporte);
                            }

                            var usuarios = atendimento.usuarios.FindAll(x => x.connectionid != Context.ConnectionId && x.suporte);
                            foreach (var item in usuarios)
                            {
                                if (item.connectionid != null)
                                {
                                    await Clients.Client(item.connectionid).NotificarTransferenciaChat(protocolo);
                                }
                            }
                        }

                        AtendimentoChatMongo atendimentoChatMongo = await repository.Find(p => p.protocolo == protocolo);

                        if (String.IsNullOrEmpty(novoSuporte) == false)
                        {

                            await EntrarChatAtendimento(protocolo, novoSuporte, null);
                            atendimentoChatMongo.suporte = SuporteDisponiveis.Find(p => p.idSuporte == novoSuporte) ?? new SuporteChatModels { idSuporte = novoSuporte };
                            await Clients.Group(protocolo, atendimentoChatMongo.suporte?.idConexaoChat).AtendimentoTransferido(protocolo);
                        }

                        if (atendimentoChatMongo != null)
                        {
                            await repository.Update(p => p.protocolo == atendimento.protocolo, atendimentoChatMongo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return true;
        }

        public static List<AcompanhamentoAtendimentoModel> AcompanhamentoAtendimento(String Usuario, bool VerificarSetor)
        {
            using (AtendimentoApiController api = new AtendimentoApiController())
            {
                List<AcompanhamentoAtendimentoModel> retorno = api.getAcompanhamentoAtendimento(Atendimentos.Select(p => p.Key).ToList(), Usuario, VerificarSetor).Result;

                retorno.Select(async retorno =>
                {
                    if (retorno != null && Atendimentos[retorno.protocolo].suporte != null)
                    {
                        using (ISession session = HibernateUtil.GetSession())
                        using (Conexao<IdentityUser> repoUsuario = new Conexao<IdentityUser>(session))
                        {
                            var usuario = await repoUsuario.selectFirst(p => p.Id == Atendimentos[retorno.protocolo].suporte.idSuporte);
                            if (usuario != null)
                            {
                                var nome = new List<string>
                                {
                                    !String.IsNullOrEmpty(usuario.Id) ? usuario.Id : String.Empty,
                                    !String.IsNullOrEmpty(usuario.Nome) ? usuario.Nome : (usuario?.UserName ?? String.Empty)
                                };

                                retorno.agente = string.Join("-", nome.Where(n => !string.IsNullOrEmpty(n)));
                            }
                        }
                    }

                    return retorno;
                }).Where(p => p != null).ToList();

                return retorno;
            }

        }

        public async Task<bool> ConvidarAtendente(string protocolo, string Suporte)
        {

            return true;
        }
    }
}
