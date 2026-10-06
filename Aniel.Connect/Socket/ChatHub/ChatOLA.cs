using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.SignalR;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using NHibernate;
using Resources;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Concurrent;
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
            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            {
                try
                {

                    using (var session = HibernateUtil.GetSession())
                    using (var cliente_api = new ClienteApiController())
                    using (var conexaoMongo = new ConexaoMongo())
                    using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                    {
                        logExecucao.RegistraLog("Iniciando Chat OLA");
                        if (String.IsNullOrEmpty(chat.dataAbertura))
                            chat.dataAbertura = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                        chat.cliente.idCliente = await cliente_api.GetIdCliente(chat.cliente.idplanoProduto);

                        if(chat.cliente.idCliente.HasValue)
                            chat.cliente.cpf_cnpj = chat.cliente.cpf_cnpj ?? await cliente_api.GetCpfCliente(chat.cliente.idCliente.Value);

                        if (chat.cliente.idCliente != null)
                            logExecucao.RegistraLog("id cliente encontrado");
                        else
                            logExecucao.RegistraLog("id cliente NAO encontrado");

                        chat.cliente.idCategoriaCliente = chat.cliente.idCategoriaCliente ?? await cliente_api.GetIdCategoriaCliente(chat.cliente.idCliente.Value);

                        if (chat.cliente.idCategoriaCliente != null)
                            logExecucao.RegistraLog("id categoria encontrado");
                        else
                            logExecucao.RegistraLog("id categoria NAO encontrado");

                        chat.cliente.Escritorio_Area = chat.cliente.Escritorio_Area ?? await cliente_api.GetEscritorio_Area(chat.cliente.idplanoProduto);

                        if (chat.cliente?.Escritorio_Area?.Cod_Escritorio != null)
                        {
                            logExecucao.RegistraLog("escritorio area encontrado");
                        }
                        else
                        {
                            logExecucao.RegistraLog("escritorio NAO encontrado");
                        }

                        logExecucao.RegistraLog($"OBJETO FILA DE ESPERA CLIENTE {JsonConvert.SerializeObject(ChatHub.FilasEsperaCliente)}");

                        if (ChatHub.FilasEsperaCliente.FindAll(p => (p.Id_CategoriaCliente, p.Escritorio_Area?.Cod_Escritorio, p.Escritorio_Area?.Cod_Area) == (chat.cliente.idCategoriaCliente, chat.cliente.Escritorio_Area.Cod_Escritorio, chat.cliente.Escritorio_Area.Cod_Area)).Count == 0)
                        {
                            ChatHub.FilasEsperaCliente.Add((new ConcurrentQueue<AtendimentoChat>(), chat.cliente.idCategoriaCliente, new Escritorio_Area { Cod_Escritorio = chat.cliente.Escritorio_Area.Cod_Escritorio, Cod_Area = chat.cliente.Escritorio_Area.Cod_Area }));
                        }

                        var FilaEsperaCliente = ChatHub.FilasEsperaCliente.Find(p => (p.Id_CategoriaCliente, p.Escritorio_Area?.Cod_Escritorio, p.Escritorio_Area?.Cod_Area) == (chat.cliente.idCategoriaCliente, chat.cliente.Escritorio_Area?.Cod_Escritorio, chat.cliente.Escritorio_Area?.Cod_Area)).fila;

                        if (FilaEsperaCliente.ToList().FindAll(x => x.protocolo == chat.protocolo).Count() <= 0)
                        {
                            if (chat.protocolo != null)
                            {
                                if (ChatHub.Atendimentos.ContainsKey(chat.protocolo))
                                {
                                    //logExecucao.RegistraLog("Recuperando atendimento existente");
                                    //await EntrarChatAtendimento(chat.protocolo, chat.cliente.cpf_cnpj, ConnectionId);
                                }
                            }
                            else
                            {
                                logExecucao.RegistraLog("Entrando na rotina de criar novo atendimento");
                                chat.protocolo = await new AtendimentoApiController().CriarAtendimentoChatOla(chat.cliente.idCliente.Value, chat.cliente.idTipoAtendimento, chat.cliente.idplanoProduto, chat.motivo, chat.descricaoProblema);

                                if (chat.protocolo != null)
                                {
                                    logExecucao.RegistraLog("protocolo criado com sucesso");
                                }

                                await HubContext.Clients.Client(chat.cliente.idConexaoChat).InicioAtendimento(JsonService.SerializeObject(chat)); // <- Mobile
                                chat.usuarios = new List<AtendimentoUsuario>();
                                ChatHub.Atendimentos.TryAdd(chat.protocolo, chat);

                                AtendimentoChatMongo atendimentoChatMongo = new AtendimentoChatMongo();
                                ChatHub.ConvertMongoAtendimentoChat(chat, atendimentoChatMongo);
                                await repository.Save(atendimentoChatMongo);

                                await EntrarChatAtendimento(chat.protocolo, chat.cliente.cpf_cnpj, chat.cliente.idConexaoChat);

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

                                //Verifica se existe algum bot associado ao perfil do agente
                                var agenteApi = new AgenteApiController();
                                Bot bot = agenteApi.getBotAgenteCliente(chat.cliente, session).Result;

                                if (bot == null)
                                {
                                    await EnviarNotificacaoSuporte(chat);
                                }
                                else
                                {
                                    await DefinirAtendente(chat.protocolo, bot);
                                }

                                //await DefinirAtendente(chat.protocolo);
                                //await EnviarNotificacaoSuporte(chat);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logExecucao.RegistraLog($"mensagem : {ex.Message} - stackTrace: {ex.StackTrace}");
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
        }


        public async Task DefinirAtendente(String protocolo, Bot bot)
        {
            using (var session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Perfil_Agente>(session))
            {
                AtendimentoChat chat = ChatHub.Atendimentos[protocolo];
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

                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                {
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

        public async Task<Response> EnviarMensagem(String protocolo, String m)
        {
            var response = new Response();

            var mensagem = JsonConvert.DeserializeObject<Mensagem_ChatModels>(m);
            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            using (var conexaoMongo = new ConexaoMongo())
            using (var session = HibernateUtil.GetSession())
            using (var repoBot = new Conexao<Bot>(session))
            using (var repositoryEvento = new Conexao<BotEvento>(session))
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
                            var id = await atendimento_api.FindIdByProtocolo(protocolo);

                            if (id.HasValue)
                            {
                                var atend = await atendimento_api.FindById(id.Value);
                                mensagem.usuario = atend.Objeto.Nome_Cliente;
                            }
                        }

                        if (chat.bot)
                        {

                            await ExecutarBot(protocolo, mensagem.mensagem, null, chat);
                        }
                        else
                        {
                            if(!String.IsNullOrEmpty(mensagem.idConexao))
                                chat.cliente.idConexaoChat = mensagem.idConexao;
                        }

                    }

                    if (mensagem.enviadoPeloSuporte == true)
                    {
                        var agenteApi = new AgenteApiController();
                        Bot bot = agenteApi.getBotAgenteCliente(chat.cliente, session).Result;

                        if (chat.bot)
                        {
                            mensagem.mensagem = "*" + (bot.Nome) + ":* " + mensagem.mensagem;

                        }
                        else
                        {
                            mensagem.mensagem = (mensagem.usuario) + ": " + mensagem.mensagem;
                        }

                        mensagem.mensagem = ChatHub.GetMensagemReplace(chat, mensagem.mensagem, mensagem.usuario);

                        if (!String.IsNullOrEmpty(mensagem.idConexao))
                            chat.suporte.idConexaoChat = mensagem.idConexao;
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
                        await HubContext.Clients.Client(ConnectionId).NotificarEnvio(mensagem.id ?? "");
                        await HubContext.Clients.Group(protocolo, ConnectionId).MensagemResposta(JsonService.SerializeObject(mensagem));
                    }
                    else
                    {
                        //await hubContext.Clients.Client().NotificarEnvio(mensagem.id);
                        //await HubContext.Clients.Group(protocolo).MensagemResposta(mensagem);

                        if (mensagem.enviadoPeloSuporte)
                        {
                            await HubContext.Clients.Client(chat.cliente.idConexaoChat).MensagemResposta(JsonService.SerializeObject(mensagem));
                        }
                        else
                        {
                            //await HubContext.Clients.Client(ConnectionId).NotificarEnvio(mensagem.id);
                            //await HubContext.Clients.Client(chat.cliente.idConexaoChat).MensagemResposta(mensagem);
                            await HubContext.Clients.Group(protocolo, chat.cliente.idConexaoChat).MensagemResposta(mensagem);
                            await HubContext.Clients.Client(chat.cliente.idConexaoChat).NotificarEnvio(mensagem.id ?? "");
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

            return response;
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

                AtendimentoChatMongo chatAux = await repository.Find(p => p.protocolo == chat.protocolo);

                if (chatAux.bot == false)
                {
                    chat.bot = false;
                }
                else if (chat.bot == false)
                {
                    chat.bot = true;
                }

                foreach (var item in retornoBot.valor)
                {
                    var mensagem = new Mensagem_ChatModels
                    {
                        dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
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

                var retornoBot = await chatControlador.RunBot(input);

                AtendimentoChatMongo chatAux = await repository.Find(p => p.protocolo == chat.protocolo);

                if (chatAux.bot == false)
                {
                    chat.bot = false;
                }
                else if (chat.bot == false)
                {
                    chat.bot = true;
                }

                foreach (var item in retornoBot.valor)
                {
                    var mensagem = new Mensagem_ChatModels
                    {
                        dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
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

        public async Task<ResponseEnviarNotificacaoSuporte> EnviarNotificacaoSuporte(AtendimentoChat chat, bool reincidente = false, string SuporteReincidente = null)
        {
            ResponseEnviarNotificacaoSuporte retorno = new ResponseEnviarNotificacaoSuporte();
            try
            {
                if (chat == null)
                    throw new Exception("chat nulo");

                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                using (var session = HibernateUtil.GetSession())
                using (var repositoryAtendimento = new Conexao<Model.nhibernate.Atendimento>(session))
                using (var repositoryCanal = new Conexao<Canal_Comunicacao>(session))
                using (var repositoryUsuario = new Conexao<IdentityUser>(session))
                {
                    var atendimentoNh = await repositoryAtendimento.selectFirst(p => p.Id == Convert.ToInt64(chat.idCanal));
                    var canal = await repositoryCanal.selectFirst(p => p.Id == Convert.ToInt64(chat.idCanal));
                    var bot = canal?.Bot;
                    var atendimentos = ChatHub.Atendimentos?.Values ?? new List<AtendimentoChat>();
                    var suportesDisponiveis = ChatHub.SuporteDisponiveis?
                        .Where(p => p.qtdMaximaAtendimentos > atendimentos.Count(y => y?.suporte?.idSuporte == p.idSuporte))
                        .ToList() ?? new List<SuporteChatMongo>();

                    var criterios = suportesDisponiveis.Select(p =>
                    {
                        bool atendeCriterio = true;

                        if (p?.Categorias?.Count > 0 && !p.Categorias.Contains(chat.cliente?.idCategoriaCliente ?? 0))
                            atendeCriterio = false;

                        if (p?.Localidades?.Any() == true)
                        {
                            var escritorio = chat.cliente?.Escritorio_Area;
                            if (!p.Localidades.Any(y =>
                                y.Cod_Area == escritorio?.Cod_Area &&
                                y.Cod_Escritorio == escritorio?.Cod_Escritorio))
                            {
                                atendeCriterio = false;
                            }
                        }

                        return atendeCriterio ? p : null;
                    }).Where(p => p != null).ToList();

                    if (atendimentoNh?.Setor?.Id != null)
                    {
                        criterios = criterios.Select(p =>
                        {
                            var agente = repositoryUsuario.selectFirst(x => x.Id == p.idSuporte).Result;
                            return agente?.Departamento?.Id == atendimentoNh?.Setor?.Id ? p : null;
                        }).Where(p => p != null).ToList();
                    }

                    if (ChatHub.FilasEsperaCliente.FindAll(p => (p.Id_CategoriaCliente, p.Escritorio_Area?.Cod_Escritorio, p.Escritorio_Area?.Cod_Area) ==
                                (chat.cliente?.idCategoriaCliente, chat.cliente?.Escritorio_Area?.Cod_Escritorio, chat.cliente?.Escritorio_Area?.Cod_Area)).Count == 0)
                    {
                        ChatHub.FilasEsperaCliente.Add((new ConcurrentQueue<AtendimentoChat>(), chat.cliente.idCategoriaCliente.Value, new Escritorio_Area { Cod_Escritorio = chat.cliente.Escritorio_Area?.Cod_Escritorio, Cod_Area = chat.cliente.Escritorio_Area?.Cod_Area }));
                    }


                    var findFilaEspera = ChatHub.FilasEsperaCliente?
                            .Find(p =>
                                (p.Id_CategoriaCliente, p.Escritorio_Area?.Cod_Escritorio, p.Escritorio_Area?.Cod_Area) ==
                                (chat.cliente?.idCategoriaCliente, chat.cliente?.Escritorio_Area?.Cod_Escritorio, chat.cliente?.Escritorio_Area?.Cod_Area));

                    var filaEspera = findFilaEspera?.fila;
                    bool enfileirado = false;

                    if (filaEspera != null && filaEspera.ToList().All(x => x?.protocolo != chat.protocolo))
                    {
                        filaEspera.Enqueue(chat);
                        enfileirado = true;
                    }

                    if (criterios.All(x => x?.status == StatusAtendimento.AUSENTE))
                    {
                        if (enfileirado)
                        {
                            var mensagem = new Mensagem_ChatModels
                            {
                                dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                statusEnvio = StatusEnvioEnum.ENVIADO,
                                mensagem = $"Você está na posição {ChatHub.GetPosicaoAtendimento(chat)}º. \n Em breve um de nossos agentes irá atendê-lo.",
                                protocolo = chat.protocolo,
                                enviadoPeloSuporte = true,
                                usuario = chat.bot == true ? bot?.Nome : null,
                                nome_usuario = bot?.Nome,
                                fluxoChat = false,
                                arquivos = new List<ArquivoMensagem_ChatModels>()
                            };

                            await EnviarMensagem(chat.protocolo, JsonConvert.SerializeObject(mensagem));
                            chat = ChatHub.Atendimentos?.ContainsKey(chat.protocolo) == true ? ChatHub.Atendimentos[chat.protocolo] : chat;
                        }

                    }
                    else
                    {
                        if (chat.suporte == null || reincidente)
                        {

                            if (reincidente == true && !String.IsNullOrEmpty(SuporteReincidente) && criterios.FindAll(p => p.idSuporte != SuporteReincidente && p.status != StatusAtendimento.AUSENTE && (DateTime.Now - Convert.ToDateTime(p.datahoraAlive)).TotalMinutes <= 1).Count > 0)
                            {
                                criterios = criterios
                                    .Where(p => p?.idSuporte != SuporteReincidente && p.status != StatusAtendimento.AUSENTE && (DateTime.Now - Convert.ToDateTime(p.datahoraAlive)).TotalMinutes <= 1)
                                    .OrderBy(p => ChatHub.getSuporteStatus(p.idSuporte)?.qtdAtendimento ?? 0).ThenByDescending(p => Convert.ToDateTime(p.datahoraAlive))
                                    .ToList();
                            }
                            else
                            {
                                criterios = criterios
                                    .Where(p => p.status != StatusAtendimento.AUSENTE && (DateTime.Now - Convert.ToDateTime(p.datahoraAlive)).TotalMinutes <= 2)
                                    .OrderBy(p => ChatHub.getSuporteStatus(p.idSuporte)?.qtdAtendimento ?? 0).ThenByDescending(p => Convert.ToDateTime(p.datahoraAlive))
                                    .ToList();
                            }


                            foreach (var suporte in criterios)
                            {
                                if (enfileirado)
                                {
                                    var mensagem = new Mensagem_ChatModels
                                    {
                                        dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                        statusEnvio = StatusEnvioEnum.ENVIADO,
                                        mensagem = $"Por favor, aguarde. Em breve um de nossos agentes irá atendê-lo. \n Protocolo .:{chat.protocolo}",
                                        protocolo = chat.protocolo,
                                        enviadoPeloSuporte = true,
                                        usuario = chat.bot == true ? bot?.Nome : null,
                                        nome_usuario = bot?.Nome,
                                        fluxoChat = false,
                                        arquivos = new List<ArquivoMensagem_ChatModels>()
                                    };

                                    await EnviarMensagem(chat.protocolo, JsonConvert.SerializeObject(mensagem));
                                    ChatHub.AtendimentosAguardandoConfirmacao?.Add(chat);
                                }

                                chat = ChatHub.Atendimentos?.ContainsKey(chat.protocolo) == true ? ChatHub.Atendimentos[chat.protocolo] : chat;
                                if (!string.IsNullOrEmpty(suporte?.idConexaoChat))
                                {
                                    chat.dataHoraEnviadoAtendente = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                    await HubContext.Clients.Client(suporte.idConexaoChat)
                                        .ClienteAguardandoAtendimento($"Cliente Aguardando Atendimento ... \n Deseja Iniciar o Atendimento?", chat);

                                    retorno.notificacaoEnviada = true;
                                    break;
                                }
                            }
                        }
                    }

                    var chatMongo = await repository.Find(p => p.protocolo == chat.protocolo);
                    if (chatMongo != null)
                    {
                        if (chat.bot)
                        {
                            chatMongo.bot = false;
                        }

                        chatMongo.dataHoraEnviadoAtendente = chat.dataHoraEnviadoAtendente;
                        await repository.Update(p => p.protocolo == chatMongo.protocolo, chatMongo);
                    }


                }

                retorno.result = true;
            }
            catch (Exception ex)
            {
                retorno.result = false;
                retorno.message = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
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

        public Task StartChatAvulso(AtendimentoChat chat)
        {
            throw new NotImplementedException();
        }
    }
}