using Telegram.Bot;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Aniel.Connect.Socket.ChatHub
{
    public class ChatBotInstagram : IChat 
    {
        public IHubContext HubContext { get; set; }
        public String ConnectionId { get; set; }
        public String InstagramToken { get; set; }
        public String instagramChatId { get; set; }
        public Int64? IdCanal { get; set; }

        public ChatBotInstagram(String InstagramToken, String instagramChatId)
        {
            this.HubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            this.InstagramToken = InstagramToken;
            this.instagramChatId = instagramChatId;
        }

        //ENVIAR MENSAGEM
        public async Task EnviarMensagem(String protocolo, String m)
        {
            var mensagem = JsonConvert.DeserializeObject<Mensagem_ChatModels>(m);
            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            using (var conexaoMongo = new ConexaoMongo())
            using (var session = HibernateUtil.GetSession())
            using (var repoCanal = new Conexao<Canal_Comunicacao>(session))
            using (var repoConfCanal = new Conexao<Configuracao_Canal_Comunicacao>(session))
            using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            {

                AtendimentoChatMongo chat = await repository.Find(p => p.protocolo == protocolo);
                var BotMessenger = new AtendimentoApiController();
                if (chat != null)
                {
                    if (mensagem.enviadoPeloSuporte == false && String.IsNullOrEmpty(mensagem.usuario))
                    {
                        using (var atendimento_api = new AtendimentoApiController())
                        {
                            var id = atendimento_api.FindIdByProtocolo(protocolo).Result;

                            if (id.HasValue)
                            {
                                var atend = atendimento_api.FindById(id.Value).Result;
                                mensagem.usuario = atend.Objeto.Nome_Cliente;
                            }

                        }

                        if (chat.bot)
                        {
                            await ExecutarBot(protocolo, mensagem.mensagem, null, chat);
                        }
                    }

                    if (mensagem.enviadoPeloSuporte == true)
                    {
                        //var conf = await repoConfCanal.selectFirst();
                        var canal = await repoCanal.selectFirst(p => p.Id == chat.idCanal);

                        if (chat.bot)
                        {
                            if (chat.bot)
                            {
                                mensagem.mensagem = "*" + (canal.Bot.Nome) + ":* " + mensagem.mensagem;

                            }
                            else
                            {
                                mensagem.mensagem = "*" + (mensagem.usuario) + ":* " + mensagem.mensagem;
                            }
                        }

                        mensagem.mensagem = ChatHub.GetMensagemReplace(chat, mensagem.mensagem, mensagem.usuario);


                        new BaseController("", "").EnviarMensagemFacebook(
                            chat.instagramToken,
                            chat.instagramChatId,
                            mensagem.mensagem,
                            "RESPONSE");

                        if (mensagem.arquivos != null)
                        {

                            foreach (var anexos in mensagem.arquivos)
                            {

                                if (anexos.tipo == Model.enumerado.Tipo_Bot_Evento_Anexo.Url)
                                {

                                    new BaseController("", "").EnviarMensagemFacebook(
                                        chat.instagramToken,
                                        chat.instagramChatId,
                                        anexos.url,
                                        "RESPONSE");
                                }
                                else
                                {

                                    try
                                    {
                                        if (chat.bot)
                                        {
                                            var mimetype = "";

                                            if (anexos.tipo == Model.enumerado.Tipo_Bot_Evento_Anexo.Arquivo)
                                            {
                                                mimetype = "file";
                                            }
                                            if (anexos.tipo == Model.enumerado.Tipo_Bot_Evento_Anexo.Audio)
                                            {
                                                mimetype = "audio";
                                            }
                                            if (anexos.tipo == Model.enumerado.Tipo_Bot_Evento_Anexo.Video)
                                            {
                                                mimetype = "video";
                                            }
                                            if (anexos.tipo == Model.enumerado.Tipo_Bot_Evento_Anexo.Imagem)
                                            {
                                                mimetype = "image";
                                            }

                                            new BaseController("", "").EnviarMidiaFacebook(
                                                chat.instagramToken,
                                                chat.instagramChatId,
                                                anexos.url,
                                                mimetype);
                                        }
                                        else
                                        {
                                            var caminho = $@"{CONSTANTES.URLPortal}/Arquivos/Atendimento/Chat/" + anexos.name;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CONSTANTES.log4Net.Error(ex);
                                    }
                                }

                            }
                        }
                    }

                    mensagem.statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO;
                    mensagem.dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    mensagem.protocolo = protocolo;

                    if (chat.mensagens == null)
                    {
                        chat.mensagens = new List<Mensagem_ChatModels>();
                    }

                    chat.mensagens.Add(mensagem);

                    if (ConnectionId != null)
                    {
                        await HubContext.Clients.Client(ConnectionId).NotificarEnvio(mensagem.id);
                        await HubContext.Clients.Group(protocolo, ConnectionId).MensagemResposta(mensagem);
                    }
                    else
                    {
                        //await hubContext.Clients.Client().NotificarEnvio(mensagem.id);
                        await HubContext.Clients.Group(protocolo).MensagemResposta(mensagem);
                    }

                    await repository.Update(p => p.protocolo == protocolo, chat);

                }
            }
        }

        public async Task ExecutarBot(string protocolo, string input, Bot bot, AtendimentoChatMongo chat)
        {
            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            using (var session = HibernateUtil.GetSession())
            using (var repositoryEvento = new Conexao<BotEvento>(session))
            using (var repositoryCanal = new Conexao<Canal_Comunicacao>(session))
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            {
                if (bot == null)
                {
                    bot = repositoryCanal.selectFirst(p => p.Id == chat.idCanal).Result.Bot;
                }

                BotEvento ultimoEvento = chat.idUltimoEvento.HasValue ? repositoryEvento.selectFirst(p => p.Id == Convert.ToInt64(chat.idUltimoEvento)).Result : null;

                var chatControlador = new ChatBotBusiness(bot, ultimoEvento, logExecucao, chat);
                var retornoBot = await chatControlador.RunBot(input);

                if (chat.bot == false)
                    chat.bot = true;

                foreach (var item in retornoBot.valor)
                {
                    var mensagem = new Mensagem_ChatModels
                    {
                        dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                        statusEnvio = StatusEnvioEnum.ENVIADO,
                        mensagem = item.texto,
                        protocolo = chat.protocolo,
                        enviadoPeloSuporte = true,
                        fluxoChat = false,
                        usuario = null,
                        arquivos = item.anexos.Select(p => new ArquivoMensagem_ChatModels
                        {
                            id = p.Id.ToString(),
                            name = p.Id.ToString() + p.Extensao,
                            url = p.Url,
                            tipo = p.Tipo,
                        }).ToList()

                    };

                    await EnviarMensagem(chat.protocolo, JsonConvert.SerializeObject(mensagem));
                }

                chat.idUltimoEvento = chatControlador.EventoAtual?.Id;

                await repository.Update(p => p.protocolo == protocolo, chat);
            }
        }

        public async Task ExecutarBot(string protocolo, string input, Bot bot, NHibernate.ISession session)
        {
            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            using (var repositoryEvento = new Conexao<BotEvento>(session))
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            {
                AtendimentoChatMongo chat = await repository.Find(p => p.protocolo == protocolo);
                BotEvento ultimoEvento = chat.idUltimoEvento.HasValue ? repositoryEvento.selectFirst(p => p.Id == Convert.ToInt64(chat.idUltimoEvento)).Result : null;
                var chatControlador = new ChatBotBusiness(bot, ultimoEvento, logExecucao, chat);
                var retornoBot = await chatControlador.RunBot(input);

                if (chat.bot == false)
                    chat.bot = true;

                foreach (var item in retornoBot.valor)
                {
                    var mensagem = new Mensagem_ChatModels
                    {
                        dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                        statusEnvio = StatusEnvioEnum.ENVIADO,
                        mensagem = item.texto,
                        protocolo = chat.protocolo,
                        enviadoPeloSuporte = true,
                        fluxoChat = false,
                        usuario = null,
                        arquivos = item.anexos.Select(p => new ArquivoMensagem_ChatModels
                        {
                            id = p.Id.ToString(),
                            name = p.Id.ToString() + p.Extensao,
                            url = p.Url
                        }).ToList()
                    };

                    await EnviarMensagem(chat.protocolo, JsonConvert.SerializeObject(mensagem));
                }

                chat.idUltimoEvento = chatControlador.EventoAtual?.Id;
                await repository.Update(p => p.protocolo == protocolo, chat);
            }
        }


        //MESSENGER STARTCHAT
        public async Task StartChat(AtendimentoChat chat)
        {
            try
            {
                using (var cliente_api = new ClienteApiController())
                using (var conexaoMongo = new ConexaoMongo())
                using (var session = HibernateUtil.GetSession())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                using (var repositoryCanal = new Conexao<Canal_Comunicacao>(session))
                {

                    if (String.IsNullOrEmpty(chat.dataAbertura))
                        chat.dataAbertura = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                    if (chat.cliente.idCliente == 0 && chat.cliente.idplanoProduto != 0)
                    {
                        chat.cliente.idCliente = await cliente_api.GetIdCliente(chat.cliente.idplanoProduto);
                    }

                    chat.cliente.idCategoriaCliente = chat.cliente.idCategoriaCliente ?? await cliente_api.GetIdCategoriaCliente(chat.cliente.idCliente.Value);

                    if (chat.cliente.Escritorio_Area == null && chat.cliente.idplanoProduto != 0)
                    {
                        chat.cliente.Escritorio_Area = chat.cliente.Escritorio_Area ?? await cliente_api.GetEscritorio_Area(chat.cliente.idplanoProduto);
                    }

                    if (ChatHub.FilasEsperaCliente.FindAll(p => (p.Id_CategoriaCliente, p.Escritorio_Area.Cod_Escritorio, p.Escritorio_Area.Cod_Area) == (chat.cliente.idCategoriaCliente, chat.cliente.Escritorio_Area?.Cod_Escritorio, chat.cliente.Escritorio_Area?.Cod_Area)).Count == 0)
                    {
                        ChatHub.FilasEsperaCliente.Add((new Queue<AtendimentoChat>(), chat.cliente.idCategoriaCliente.Value, new Escritorio_Area { Cod_Escritorio = chat.cliente.Escritorio_Area?.Cod_Escritorio, Cod_Area = chat.cliente.Escritorio_Area?.Cod_Area }));
                    }

                    var FilaEsperaCliente = ChatHub.FilasEsperaCliente.Find(p => (p.Id_CategoriaCliente, p.Escritorio_Area.Cod_Escritorio, p.Escritorio_Area.Cod_Area) == (chat.cliente.idCategoriaCliente, chat.cliente.Escritorio_Area?.Cod_Escritorio, chat.cliente.Escritorio_Area?.Cod_Area)).fila;

                    if (FilaEsperaCliente.ToList().FindAll(x => x.protocolo == chat.protocolo).Count() <= 0)
                    {
                        if (chat.protocolo != null)
                        {
                            if (ChatHub.Atendimentos.ContainsKey(chat.protocolo))
                            {
                                //await EntrarChatAtendimento(chat.protocolo, chat.cliente.cpf_cnpj, Context.ConnectionId);
                            }
                        }
                        else
                        {

                            chat.protocolo = await new AtendimentoApiController().CriarAtendimentoChatOla(chat.cliente.idCliente.Value, chat.cliente.idTipoAtendimento, chat.cliente.idplanoProduto, chat.motivo, chat.descricaoProblema, chat.conversationSid);
                            chat.usuarios = new List<AtendimentoUsuario>();
                            ChatHub.Atendimentos.Add(chat.protocolo, chat);


                            AtendimentoChatMongo atendimentoChatMongo = new AtendimentoChatMongo();
                            ChatHub.ConvertMongoAtendimentoChat(chat, atendimentoChatMongo);
                            await repository.Save(atendimentoChatMongo);

                            await EntrarChatAtendimento(chat.protocolo, chat.cliente.cpf_cnpj, ConnectionId);

                            if (chat.idCanal.HasValue == false)
                                await EnviarNotificacaoSuporte(chat);
                            else
                            {
                                await DefinirAtendente(chat.protocolo);
                            }


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
            }
        }

        public async Task EntrarChatAtendimento(String protocolo, String username, string idConexao)
        {
            if (ChatHub.Atendimentos.ContainsKey(protocolo))
            {
                var chat = ChatHub.Atendimentos[protocolo];
                var usuario = chat.usuarios.Find(p => p.userid == username);
                if (usuario?.userid == null)
                {
                    chat.usuarios.Add(new AtendimentoUsuario { userid = username, connectionid = idConexao });
                    var suporte = ChatHub.SuporteDisponiveis.Find(p => p.idSuporte == username);

                    if (suporte != null)
                    {
                        if (String.IsNullOrEmpty(suporte?.inicioAtendimento) == false)
                        {
                            await EnviarMensagem(protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                            {
                                dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                                enviadoPeloSuporte = true,
                                usuario = suporte.nomeAgente,
                                fluxoChat = true,
                                statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                                mensagem = "{NOME DO AGENTE} entrou na conversa"
                            }));

                            await EnviarMensagem(protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                            {
                                dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                                enviadoPeloSuporte = true,
                                usuario = suporte.nomeAgente,
                                statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                                mensagem = suporte.inicioAtendimento
                            }));
                        }
                    }

                    if (idConexao != null)
                    {
                        await HubContext.Groups.Add(idConexao, protocolo);
                    }

                }
                else if (usuario?.connectionid != idConexao)
                {
                    usuario.connectionid = idConexao;
                    if (idConexao != null)
                    {
                        await HubContext.Groups.Add(idConexao, protocolo);
                    }
                }
            }


        }

        public async Task DefinirAtendente(String protocolo)
        {
            using (var session = HibernateUtil.GetSession())
            using (var repositoryCanal = new Conexao<Canal_Comunicacao>(session))
            using (var repositoryEvento = new Conexao<BotEvento>(session))
            {
                AtendimentoChat chat = ChatHub.Atendimentos[protocolo];
                Bot bot = repositoryCanal.selectFirst(p => p.Id == Convert.ToInt64(chat.idCanal)).Result.Bot;

                if (bot != null)
                {
                    await ExecutarBot(protocolo, null, bot, session);
                }
                else
                {
                    await EnviarNotificacaoSuporte(chat);
                }
            }
        }

        public async Task SendFromClient(AtendimentoChat chat, String mensagem)
        {
            TelegramBotClient Bot = new TelegramBotClient(chat.telegramToken);
            var m = new Mensagem_ChatModels()
            {
                dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                enviadoPeloSuporte = true,
                mensagem = mensagem,
                statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIANDO,
            };

            if (chat.mensagens == null)
                chat.mensagens = new List<Mensagem_ChatModels>();

            chat.mensagens.Add(m);
            ChatHub.Atendimentos[chat.protocolo] = chat;

            await Bot.SendTextMessageAsync(
                 chatId: chat.telegramChatId,
                 text: mensagem);

        }

        public Task EnviarNotificacaoSuporte(AtendimentoChat chat, bool reincidente = false, string SuporteReincidente = null)
        {
            throw new System.NotImplementedException();
        }
        public async Task StartChatAvulso(AtendimentoChat chat)
        {
            try
            {
                using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
                using (var cliente_api = new ClienteApiController())
                using (var conexaoMongo = new ConexaoMongo())
                using (var session = HibernateUtil.GetSession())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                using (var repositoryCanal = new Conexao<Canal_Comunicacao>(session))
                {

                    if (String.IsNullOrEmpty(chat.dataAbertura))
                        chat.dataAbertura = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                    if (chat.protocolo != null)
                    {
                        if (ChatHub.Atendimentos.ContainsKey(chat.protocolo))
                        {
                            //await EntrarChatAtendimento(chat.protocolo, chat.cliente.cpf_cnpj, Context.ConnectionId);
                        }
                    }
                    else
                    {

                        chat.protocolo = await new AtendimentoApiController().CriarAtendimentoAvulso(chat);
                        chat.usuarios = new List<AtendimentoUsuario>();
                        ChatHub.Atendimentos.Add(chat.protocolo, chat);

                        logExecucao.RegistraLog("Token Teste: " + chat.messengerToken);
                        AtendimentoChatMongo atendimentoChatMongo = new AtendimentoChatMongo();
                        ChatHub.ConvertMongoAtendimentoChat(chat, atendimentoChatMongo);
                        await repository.Save(atendimentoChatMongo);

                        await EntrarChatAtendimento(chat.protocolo, chat.cliente.cpf_cnpj, ConnectionId);

                        if (chat.idCanal.HasValue == false)
                            await EnviarNotificacaoSuporte(chat);
                        else
                        {
                            await DefinirAtendente(chat.protocolo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
            }
        }
    }
}