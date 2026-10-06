using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.SignalR;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Resources;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Aniel.Connect.Socket.ChatHub
{
    public class ChatOLA : IChat
    {
        public IHubContext HubContext { get; set; }
        public String ConnectionId { get; set; }

        public async Task StartChat(AtendimentoChat chat)
        {
            try
            {
                using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
                using (var session = HibernateUtil.GetSession())
                using (var cliente_api = new ClienteApiController())
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                {
                    if (String.IsNullOrEmpty(chat.dataAbertura))
                        chat.dataAbertura = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                    chat.cliente.idCliente = await cliente_api.GetIdCliente(chat.cliente.idplanoProduto);
                    chat.cliente.idCategoriaCliente = chat.cliente.idCategoriaCliente ?? await cliente_api.GetIdCategoriaCliente(chat.cliente.idCliente.Value);
                    chat.cliente.Escritorio_Area = chat.cliente.Escritorio_Area ?? await cliente_api.GetEscritorio_Area(chat.cliente.idplanoProduto);
                    if (ChatHub.FilasEsperaCliente.FindAll(p => (p.Id_CategoriaCliente, p.Escritorio_Area.Cod_Escritorio, p.Escritorio_Area.Cod_Area) == (chat.cliente.idCategoriaCliente, chat.cliente.Escritorio_Area.Cod_Escritorio, chat.cliente.Escritorio_Area.Cod_Area)).Count == 0)
                    {
                        ChatHub.FilasEsperaCliente.Add((new Queue<AtendimentoChat>(), chat.cliente.idCategoriaCliente, new Escritorio_Area { Cod_Escritorio = chat.cliente.Escritorio_Area.Cod_Escritorio, Cod_Area = chat.cliente.Escritorio_Area.Cod_Area }));
                    }

                    var FilaEsperaCliente = ChatHub.FilasEsperaCliente.Find(p => (p.Id_CategoriaCliente, p.Escritorio_Area.Cod_Escritorio, p.Escritorio_Area.Cod_Area) == (chat.cliente.idCategoriaCliente, chat.cliente.Escritorio_Area.Cod_Escritorio, chat.cliente.Escritorio_Area.Cod_Area)).fila;

                    if (FilaEsperaCliente.ToList().FindAll(x => x.protocolo == chat.protocolo).Count() <= 0)
                    {
                        if (chat.protocolo != null)
                        {
                            if (ChatHub.Atendimentos.ContainsKey(chat.protocolo))
                            {
                                //await EntrarChatAtendimento(chat.protocolo, chat.cliente.cpf_cnpj, ConnectionId);
                            }
                        }
                        else
                        {

                            chat.protocolo = await new AtendimentoApiController().CriarAtendimentoChatOla(chat.cliente.idCliente.Value, chat.cliente.idTipoAtendimento, chat.cliente.idplanoProduto, chat.motivo, chat.descricaoProblema);
                            await HubContext.Clients.Client(chat.cliente.idConexaoChat).InicioAtendimento(chat); // <- Mobile
                            chat.usuarios = new List<AtendimentoUsuario>();
                            ChatHub.Atendimentos.Add(chat.protocolo, chat);
                            
                            AtendimentoChatMongo atendimentoChatMongo = new AtendimentoChatMongo();
                            ChatHub.ConvertMongoAtendimentoChat(chat, atendimentoChatMongo);
                            await repository.Save(atendimentoChatMongo);

                            if (String.IsNullOrEmpty(chat.motivo) == false || String.IsNullOrEmpty(chat.descricaoProblema) == false)
                            {
                                await EnviarMensagem(chat.protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                                {
                                    dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                                    enviadoPeloSuporte = false,
                                    usuario = chat.cliente.nome,
                                    statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                                    mensagem = $"{RsCadastros.ASSUNTO}: {chat.motivo ?? "-"}\n{RsCadastros.PROBLEMA_RELATADO}: {chat.descricaoProblema ?? "-"}",
                                }));
                            }

                            await EntrarChatAtendimento(chat.protocolo, chat.cliente.cpf_cnpj, ConnectionId);

                            //Verifica se existe algum bot associado ao perfil do agente
                            var agenteApi = new AgenteApiController();
                            Bot bot = agenteApi.getBotAgenteCliente(chat.cliente,session).Result;

<<<<<<< HEAD
                            if (dadosAgente.IdBot == null)
                            {
=======
                            if (bot == null)
>>>>>>> d6506fa4dd17793383b3237fa0cf906861450154
                                await EnviarNotificacaoSuporte(chat);
                            }else
                            {
<<<<<<< HEAD
                                await DefinirAtendente(chat.protocolo);
=======
                                await DefinirAtendente(chat.protocolo, bot);
>>>>>>> d6506fa4dd17793383b3237fa0cf906861450154
                            }

                            //await DefinirAtendente(chat.protocolo);
                            //await EnviarNotificacaoSuporte(chat);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
            }
        }

        public async Task DefinirAtendente(String protocolo)
        {
            using (var session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Perfil_Agente>(session))
            {
                AtendimentoChat chat = ChatHub.Atendimentos[protocolo];
                Bot bot = repository.selectFirst(p => p.Bot != null).Result.Bot;

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
                else if (usuario.connectionid != idConexao)
                {
                    usuario.connectionid = idConexao;
                    if (idConexao != null)
                    {
                        await HubContext.Groups.Add(idConexao, protocolo);
                    }
                }
            }


        }

        public async Task EnviarMensagem(String protocolo, String m)
        {

            var mensagem = JsonConvert.DeserializeObject<Mensagem_ChatModels>(m);
            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            using (var conexaoMongo = new ConexaoMongo())
            using (var session = HibernateUtil.GetSession())
            using (var repoBot = new Conexao<Bot>(session))
            using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            {

                if (ChatHub.Atendimentos.ContainsKey(protocolo))
                {
                    AtendimentoChatMongo chat = await repository.Find(p => p.protocolo == protocolo);

                    //var chat = ChatHub.Atendimentos[protocolo];
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
                        var agenteApi = new AgenteApiController();
<<<<<<< HEAD
                        AgenteCadastroModel dadosAgente = agenteApi.getAgenteCliente(chat.cliente.idCliente.ToString()).Result;
                        Bot bot = await repoBot.selectFirst(p => p.Id == dadosAgente.IdBot);
=======
                        Bot bot = agenteApi.getBotAgenteCliente(chat.cliente, session).Result;

>>>>>>> d6506fa4dd17793383b3237fa0cf906861450154

                            if (chat.bot)
                            {
                                mensagem.mensagem = (bot.Nome) + ": " + mensagem.mensagem;
                            }
                            else
                            {
                                mensagem.mensagem = (mensagem.usuario) + ": " + mensagem.mensagem;
                            }

                        mensagem.mensagem = ChatHub.GetMensagemReplace(chat, mensagem.mensagem, mensagem.usuario);

                        ////Envio de arquivos
                        //if (mensagem.arquivos != null)
                        //{
                        //    foreach (var anexos in mensagem.arquivos)
                        //    {
                        //        mensagem.mensagem = anexos.url;
                        //    }
                        //}
                    }

                    mensagem.statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO;
                    mensagem.dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    mensagem.protocolo = protocolo;
                    chat.mensagens.Add(mensagem);

                    if (ConnectionId != null)
                    {
                        await HubContext.Clients.Client(ConnectionId).NotificarEnvio(mensagem.id);
                        await HubContext.Clients.Group(protocolo, ConnectionId).MensagemResposta(mensagem);
                    }
                    else
                    {
                        //await hubContext.Clients.Client().NotificarEnvio(mensagem.id);
                        //await HubContext.Clients.Group(protocolo).MensagemResposta(mensagem);

                        if (mensagem.enviadoPeloSuporte)
                        {
                            await HubContext.Clients.Client(chat.cliente.idConexaoChat).MensagemResposta(mensagem);
                        }
                        else
                        {
                            //await HubContext.Clients.Client(ConnectionId).NotificarEnvio(mensagem.id);
                            //await HubContext.Clients.Client(chat.cliente.idConexaoChat).MensagemResposta(mensagem);
                            await HubContext.Clients.Group(protocolo, chat.cliente.idConexaoChat).MensagemResposta(mensagem);
                            await HubContext.Clients.Client(chat.cliente.idConexaoChat).NotificarEnvio(mensagem.id);
                        }
                    }

                    IMongoQueryable<AtendimentoChatMongo> query = repository.GetQueryable();
                    var atendimentos = query.ToList().FindAll(p => p.protocolo == protocolo).ToList();
                    var atendimentoChatMongo = atendimentos.FirstOrDefault();
                    if (atendimentoChatMongo != null)
                    {
                        ChatHub.ConvertMongoAtendimentoChat(chat, atendimentoChatMongo);
                        await repository.Update(p => p.protocolo == protocolo, atendimentoChatMongo);
                    }
                }
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
                var retornoBot = chatControlador.RunBot(input);

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

        public async Task ExecutarBot(string protocolo, string input, Bot bot, AtendimentoChatMongo chat)
        {
            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            using (var session = HibernateUtil.GetSession())
            using (var repositoryEvento = new Conexao<BotEvento>(session))
            using (var repositoryCanal = new Conexao<Canal_Comunicacao>(session))
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            using (var repositoryPerfilAgente = new Conexao<Perfil_Agente>(session))
            {
                if (bot == null)
                {
                    bot = repositoryPerfilAgente.selectFirst(p => p.Bot != null).Result.Bot;
                }

                BotEvento ultimoEvento = chat.idUltimoEvento.HasValue ? repositoryEvento.selectFirst(p => p.Id == Convert.ToInt64(chat.idUltimoEvento)).Result : null;

                var chatControlador = new ChatBotBusiness(bot, ultimoEvento, logExecucao, chat);
                var retornoBot = chatControlador.RunBot(input);

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

        //Busca se extá vinculado a um bot
        public async Task BuscaSuporteBot(AtendimentoChat chat)
        {
            try
            {
                var SuporteDisponiveisCriterios = ChatHub.SuporteDisponiveis.Where(p => p.qtdMaximaAtendimentos > ChatHub.Atendimentos.Values.Where(y => y.suporte?.idSuporte == p.idSuporte).Count()).ToList();

                SuporteDisponiveisCriterios = SuporteDisponiveisCriterios.Select(p =>
                {
                    bool atendeCriterio = true;
                    if (p.Categorias.Count > 0 && p.Categorias.Contains(chat.cliente.idCategoriaCliente.Value) == false)
                    {
                        atendeCriterio = false;
                    }

                    if (p.Localidades.Count() > 0)
                    {
                        if (p.Localidades.Any(y => y.Cod_Area == chat.cliente.Escritorio_Area.Cod_Area && y.Cod_Escritorio == chat.cliente.Escritorio_Area.Cod_Escritorio) == false)
                        {
                            atendeCriterio = false;
                        }
                    }

                    if (atendeCriterio == true)
                        return p;
                    else
                        return null;

                }).ToList().Where(p => p != null).ToList();

            }
            catch
            {

            }
        }

        public async Task EnviarNotificacaoSuporte(AtendimentoChat chat, Boolean reincidente = false, String SuporteReincidente = null)
        {

            try
            {
                var SuporteDisponiveisCriterios = ChatHub.SuporteDisponiveis.Where(p => p.qtdMaximaAtendimentos > ChatHub.Atendimentos.Values.Where(y => y.suporte?.idSuporte == p.idSuporte).Count()).ToList();

                SuporteDisponiveisCriterios = SuporteDisponiveisCriterios.Select(p =>
                {
                    bool atendeCriterio = true;
                    if (p.Categorias.Count > 0 && p.Categorias.Contains(chat.cliente.idCategoriaCliente.Value) == false)
                    {
                        atendeCriterio = false;
                    }

                    if (p.Localidades.Count() > 0)
                    {
                        if (p.Localidades.Any(y => y.Cod_Area == chat.cliente.Escritorio_Area.Cod_Area && y.Cod_Escritorio == chat.cliente.Escritorio_Area.Cod_Escritorio) == false)
                        {
                            atendeCriterio = false;
                        }
                    }

                    if (atendeCriterio == true)
                        return p;
                    else
                        return null;

                }).ToList().Where(p => p != null).ToList();

                if (SuporteDisponiveisCriterios.FindAll(x => x.status == StatusAtendimento.DISPONIVEL || x.status == StatusAtendimento.AGUARD_CONFIRMACAO).Count() == 0)
                {
                    var FilaEsperaCliente = ChatHub.FilasEsperaCliente.Find(p => (p.Id_CategoriaCliente, p.Escritorio_Area.Cod_Escritorio, p.Escritorio_Area.Cod_Area) == (chat.cliente.idCategoriaCliente, chat.cliente.Escritorio_Area.Cod_Escritorio, chat.cliente.Escritorio_Area.Cod_Area)).fila;
                    if (FilaEsperaCliente.ToList().FindAll(x => x.protocolo == chat.protocolo).Count() <= 0)
                    {
                        FilaEsperaCliente.Enqueue(chat);
                        await SendFromClient(chat, $"Você está na posição {ChatHub.GetPosicaoAtendimento(chat)}º. \n Em breve um de nossos agentes irá atendê-lo. ");
                    }
                }
                else
                {
                    if (chat.suporte == null || reincidente == true)
                    {

                        try
                        {

                            if (reincidente == true && !String.IsNullOrEmpty(SuporteReincidente))
                                SuporteDisponiveisCriterios = SuporteDisponiveisCriterios.FindAll(p => p.idSuporte != SuporteReincidente).OrderBy(a => ChatHub.getSuporteStatus(a.idSuporte).qtdAtendimento).ThenBy(a => Guid.NewGuid()).ToList();
                            else
                            {
                                SuporteDisponiveisCriterios = SuporteDisponiveisCriterios.OrderBy(a => ChatHub.getSuporteStatus(a.idSuporte).qtdAtendimento).ToList();
                            }
                        }
                        catch (Exception ex)
                        {
                            CONSTANTES.log4Net.Error(ex);
                        }


                        foreach (var suporte in SuporteDisponiveisCriterios)
                        {
                            if (suporte.status == StatusAtendimento.DISPONIVEL)
                            {
                                if (reincidente == false)
                                {
                                    var mensagem = $"Por favor, aguarde. Em breve um de nossos agentes irá atendê-lo. \n Protocolo .:{chat.protocolo}";
                                    await SendFromClient(chat, mensagem);

                                    await HubContext.Clients.Client(suporte.idConexaoChat).ClienteAguardandoAtendimento($"Cliente Aguardando Atendimento ... \n Deseja Iniciar o Atendimento?", chat);

                                    var FilaEsperaCliente = ChatHub.FilasEsperaCliente.Find(p => (p.Id_CategoriaCliente, p.Escritorio_Area.Cod_Escritorio, p.Escritorio_Area.Cod_Area) == (chat.cliente.idCategoriaCliente, chat.cliente.Escritorio_Area.Cod_Escritorio, chat.cliente.Escritorio_Area.Cod_Area)).fila;
                                    var fila = FilaEsperaCliente.ToList().FindAll(x => x.protocolo == chat.protocolo);
                                    if (fila.Count() > 0)
                                    {
                                        FilaEsperaCliente.Dequeue();

                                    }


                                    ChatHub.AtendimentosAguardandoConfirmacao.Add(chat);
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                //var suporte = chat.suporte;
                                await HubContext.Clients.Client(suporte.idConexaoChat).ClienteAguardandoAtendimento($"Cliente Aguardando Atendimento ... \n Deseja Iniciar o Atendimento?", chat);
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

        public async Task SendFromClient(AtendimentoChat chat, String mensagem)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            var m = new Mensagem_ChatModels()
            {
                dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                enviadoPeloSuporte = true,
                mensagem = mensagem,
                statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIANDO

            };
            chat.mensagens.Add(m);
            ChatHub.Atendimentos[chat.protocolo] = chat;
            await hubContext.Clients.Client(chat.cliente.idConexaoChat).MensagemCliente(m);
        }

    }
}