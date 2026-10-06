
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model.Hubsoft;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using RestSharp.Extensions;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Integracao.MetaBusiness.Integracoes;
using Sinapse.Integracao.WhatsAppBusiness.Repositorio;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Conversations.V1;
using Twilio.Rest.Conversations.V1.Conversation;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using System.Collections.Concurrent;
using System.Data.SqlTypes;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;


namespace Aniel.Connect.Socket.ChatHub
{
    public class ChatBotWhatsApp : IChat
    {
        public IHubContext HubContext { get; set; }
        public String ConnectionId { get; set; }
        public String WhatsAppToken { get; set; }
        public String IdBusinessWha { get; set; }
        public String IdNumeroTelefone { get; set; }
        public Int64? IdCanal { get; set; }

        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new ConcurrentDictionary<string, SemaphoreSlim>();
        public ChatBotWhatsApp(String WhatsAppToken, String IdNumeroTelefone, string ConnectionId)
        {
            this.HubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            this.IdNumeroTelefone = IdNumeroTelefone;
            this.WhatsAppToken = WhatsAppToken;
            this.ConnectionId = ConnectionId;
        }

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

                        chat.dataAbertura = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");


                    if (chat.cliente.idCliente == 0 && chat.cliente.idplanoProduto != 0)
                    {
                        chat.cliente.idCliente = await cliente_api.GetIdCliente(chat.cliente.idplanoProduto);
                    }

                    chat.cliente.idCategoriaCliente = chat.cliente.idCategoriaCliente ?? await cliente_api.GetIdCategoriaCliente(chat.cliente.idCliente.Value);

                    if (chat.cliente.Escritorio_Area == null && chat.cliente.idplanoProduto != 0)
                    {
                        chat.cliente.Escritorio_Area = chat.cliente.Escritorio_Area ?? await cliente_api.GetEscritorio_Area(chat.cliente.idplanoProduto);
                    }

                    if (ChatHub.FilasEsperaCliente.FindAll(p => (p.Id_CategoriaCliente, p.Escritorio_Area?.Cod_Escritorio, p.Escritorio_Area?.Cod_Area) == (chat.cliente.idCategoriaCliente, chat.cliente.Escritorio_Area?.Cod_Escritorio, chat.cliente.Escritorio_Area?.Cod_Area)).Count == 0)
                    {
                        ChatHub.FilasEsperaCliente.Add((new ConcurrentQueue<AtendimentoChat>(), chat.cliente.idCategoriaCliente.Value, new Escritorio_Area { Cod_Escritorio = chat.cliente.Escritorio_Area?.Cod_Escritorio, Cod_Area = chat.cliente.Escritorio_Area?.Cod_Area }));
                    }

                    var FilaEsperaCliente = ChatHub.FilasEsperaCliente.Find(p => (p.Id_CategoriaCliente, p.Escritorio_Area?.Cod_Escritorio, p.Escritorio_Area?.Cod_Area) == (chat.cliente.idCategoriaCliente, chat.cliente.Escritorio_Area?.Cod_Escritorio, chat.cliente.Escritorio_Area?.Cod_Area)).fila;

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

                            chat.protocolo = await new AtendimentoApiController().CriarAtendimentoChatOla(chat.cliente.idCliente.Value, chat.cliente.idTipoAtendimento, chat.cliente.idplanoProduto, chat.motivo, chat.descricaoProblema, chat.whatsAppChatId);
                            chat.usuarios = new List<AtendimentoUsuario>();

                            ChatHub.Atendimentos.TryAdd(chat.protocolo, chat);


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
        public async Task StartChatAvulso(AtendimentoChat chat)
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

                        chat.dataAbertura = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    if (chat.protocolo != null)
                    {
                        if (ChatHub.Atendimentos.ContainsKey(chat.protocolo))
                        {
                            //await EntrarChatAtendimento(chat.protocolo, chat.cliente.cpf_cnpj, Context.ConnectionId);
                        }
                    }
                    else
                    {

                        if (chat.conversas == null)
                        {
                            chat.conversas = new List<Conversas_Chat>();
                        }

                        chat.conversas.Add(new Conversas_Chat
                        {
                            id = Convert.ToString(Guid.NewGuid()),
                            data_inicio = chat.dataAbertura
                        });

                        foreach (var item in chat.mensagens)
                        {
                            item.conversasId = chat.conversas.FirstOrDefault()?.id;
                        }

                        chat.protocolo = await new AtendimentoApiController().CriarAtendimentoAvulso(chat);
                        chat.usuarios = new List<AtendimentoUsuario>();
                        ChatHub.Atendimentos.TryAdd(chat.protocolo, chat);


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
                            await EnviarModeloMensagem(protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                            {

                                dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
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

                    using (var conexaoMongo = new ConexaoMongo())
                    using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                    {
                        AtendimentoChatMongo atendimentoChatMongo = await repository.Find(p => p.protocolo == protocolo);
                        atendimentoChatMongo.usuarios.Add(new AtendimentoUsuario { userid = username, connectionid = idConexao, suporte = true });
                        await repository.Update(x => x.protocolo == protocolo, atendimentoChatMongo);
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

        public async Task<Response> EnviarMensagem(String protocolo, String m)
        {
            var response = new Response();
            var mensagem = JsonConvert.DeserializeObject<Mensagem_ChatModels>(m);
            if (mensagem.idModeloMensagem.HasValue)
            {
                return await EnviarModeloMensagem(protocolo, m);
            }

            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var session = HibernateUtil.GetSession())
                using (var repoCanal = new Conexao<Canal_Comunicacao>(session))
                using (var repoModeloMensagem = new Conexao<ModeloMensagem>(session))
                using (var repoConfCanal = new Conexao<Configuracao_Canal_Comunicacao>(session))
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                using (var repositoryEvento = new Conexao<BotEvento>(session))
                using (var repositoryAntendimentoNh = new Conexao<Model.nhibernate.Atendimento>(session))
                using (var api = new NotificacoesApiController())

                {

                    AtendimentoChatMongo chat = await repository.Find(p => p.protocolo == protocolo);

                    if (chat != null)
                    {
                        if (mensagem.enviadoPeloSuporte == false)
                        {
                            using (var atendimento_api = new AtendimentoApiController())
                            {
                                var id = atendimento_api.FindIdByProtocolo(protocolo).Result;

                                if (id.HasValue)
                                {
                                    var atend = atendimento_api.FindById(id.Value).Result;
                                    if (!string.IsNullOrEmpty(atend.Objeto?.Nome_Cliente))
                                        mensagem.usuario = atend.Objeto?.Nome_Cliente;
                                }

                            }

                            if (chat.bot)
                            {
                                await ExecutarBot(protocolo, mensagem.mensagem, null, chat);
                            }
                        }

                        if (mensagem.enviadoPeloSuporte == true)
                        {

                            var atendimento = await repositoryAntendimentoNh.selectFirst(p => p.Protocolo == protocolo);
                            var suporte = ChatHub.SuporteDisponiveis.Find(p => p.idSuporte == atendimento?.Responsavel?.Id);
                            var retornoNotificacao = await api.EnviaNotificacoesAtendimento(39, atendimento);
                            await HubContext.Clients.All.atualizarPainelConversas(mensagem);

                            if (retornoNotificacao != null && retornoNotificacao.IdObjeto != 0 && suporte != null && !string.IsNullOrEmpty(suporte?.idConexaoChat))
                            {
                                await HubContext.Clients.Client(suporte.idConexaoChat).AtualizarNotificacoes();
                            }

                        }

                        if (mensagem.enviadoPeloSuporte == true)
                        {
                            if (!chat.bot && !String.IsNullOrEmpty(mensagem?.usuario))
                            {
                                if (chat?.suporte?.idSuporte != mensagem?.matriculaUsuario && chat?.usuariosConvidados.Find(x => x.usuarioAceite == mensagem.matriculaUsuario) == null)
                                {
                                    throw new Exception("{ERROR_NOT_USER}");
                                }
                            }

                            var conf = await repoConfCanal.selectFirst();
                            var canal = await repoCanal.selectFirst(p => p.Id == chat.idCanal);

                            var ultimaMensagemSuporte = chat.mensagens.LastOrDefault(p => p.enviadoPeloSuporte == false);
                            DateTime? dataUltimaMensagem = ultimaMensagemSuporte != null ? Convert.ToDateTime(ultimaMensagemSuporte.dataHora) : (DateTime?)null;

                            if (dataUltimaMensagem.HasValue) // && canal.IdModeloPadrao.HasValue()
                            {


                                //Troca variáveis
                                var atendimento = new AtendimentoApiController();
                                mensagem.mensagem = atendimento.RemoveVariaveis(mensagem.mensagem, chat).Result;

                                mensagem.nome_usuario = chat.bot ? canal.Bot.Nome : await atendimento.GetNomeUsuario(mensagem.matriculaUsuario);

                                if (conf != null)
                                {
                                    if (conf.MostrarNomeAtendente && mensagem.nome_usuario != null)
                                    {
                                        mensagem.mensagem = chat.bot ? "" + (canal.Bot.Nome) + ": " + mensagem.mensagem : (mensagem.nome_usuario) + ": " + mensagem.mensagem;
                                    }
                                    else if (conf.MostrarNomeAtendente && mensagem.nome_usuario == null && mensagem.fluxoChat == false)
                                    {
                                        mensagem.nome_usuario = canal.Bot.Nome;
                                        mensagem.mensagem = mensagem.nome_usuario + ": " + mensagem.mensagem;
                                    }
                                }

                                mensagem.mensagem = ChatHub.GetMensagemReplace(chat, mensagem.mensagem, mensagem.nome_usuario);

                                if (mensagem.arquivos != null)
                                {
                                    if (mensagem.arquivos.Any(p => UltrapassaLimite16MB(p)))
                                    {
                                        throw new Exception("Arquivo ultrapassa o limite de 16MB permitido pelo WhatsApp Business API.");
                                    }


                                    foreach (var anexos in mensagem.arquivos)
                                    {

                                        if (anexos.tipo == Model.enumerado.Tipo_Bot_Evento_Anexo.Url)
                                        {
                                            var retornoIntegracaoUrl = await new IntegracaoWhatsAppBusinessRepository().EnviaMensagemLivreWhatsAppBusiness(new MetaWhatsAppBusiness(canal.TokenWhatsAppBusiness),
                                                                                                                        chat.whatsAppChatId,
                                                                                                                        canal.IdNumeroWhatsAppBusiness,
                                                                                                                        mensagem.mensagem);
                                            mensagem.id = retornoIntegracaoUrl?.messages[0]?.id;
                                        }
                                        else
                                        {


                                            if (chat.bot)
                                            {
                                                var retornoIntegracaoMidia = await new IntegracaoWhatsAppBusinessRepository().EnviaMensagemMidiaWhatsAppBusiness(new MetaWhatsAppBusiness(canal.TokenWhatsAppBusiness),
                                                                                chat.whatsAppChatId,
                                                                                canal.IdNumeroWhatsAppBusiness,
                                                                                mensagem.mensagem,
                                                                                $@"{CONSTANTES.URLPortal}/Arquivos/BotEvento/" + anexos.name,
                                                                                anexos.tipo);
                                                mensagem.id = retornoIntegracaoMidia?.messages[0]?.id;
                                            }
                                            else
                                            {
                                                var retornoIntegracaoMidia = await new IntegracaoWhatsAppBusinessRepository().EnviaMensagemMidiaWhatsAppBusiness(new MetaWhatsAppBusiness(canal.TokenWhatsAppBusiness),
                                                                                chat.whatsAppChatId,
                                                                                canal.IdNumeroWhatsAppBusiness,
                                                                                mensagem.mensagem,
                                                                                $@"{CONSTANTES.URLPortal}/Arquivos/Atendimento/Chat/" + anexos.name,
                                                                                anexos.tipo);


                                                mensagem.id = retornoIntegracaoMidia?.messages[0]?.id;
                                            }
                                        }

                                    }
                                }


                                var retornoIntegracao = await new IntegracaoWhatsAppBusinessRepository().EnviaMensagemLivreWhatsAppBusiness(new MetaWhatsAppBusiness(canal.TokenWhatsAppBusiness),
                                                                                    chat.whatsAppChatId,
                                                                                    canal.IdNumeroWhatsAppBusiness,
                                                                                    mensagem.mensagem);

                                if (retornoIntegracao != null)
                                {
                                    if (retornoIntegracao.messages != null)
                                    {
                                        mensagem.id = retornoIntegracao?.messages[0]?.id;
                                    }
                                }
                            }
                        }

                        mensagem.statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO;

                        if (String.IsNullOrEmpty(mensagem.dataHora))
                            mensagem.dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff");

                        mensagem.protocolo = protocolo;



                        chat.mensagens = (await repository.Find(p => p.protocolo == protocolo)).mensagens;

                        if (chat.mensagens == null)
                        {
                            chat.mensagens = new List<Mensagem_ChatModels>();
                        }

                        if (chat.conversas == null)
                        {
                            chat.conversas = new List<Conversas_Chat>();
                        }

                        if (chat.conversas.Count > 0)
                        {
                            mensagem.conversasId = chat.conversas[chat.conversas.Count - 1].id;
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

                response.result = true;

            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                response.result = false;
                response.message = ex.Message;
            }


            return response;
        }

        private bool UltrapassaLimite16MB(ArquivoMensagem_ChatModels arquivo)
        {
            string caminhoArquivo = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Atendimento\Chat\{arquivo.name}";

            if (!File.Exists(caminhoArquivo))
                throw new FileNotFoundException($"Arquivo não encontrado: {caminhoArquivo}");

            FileInfo fi = new FileInfo(caminhoArquivo);

            const long limite16MB = 16 * 1024 * 1024; // 16MB em bytes

            return fi.Length > limite16MB;
        }

        public async Task<Response> EnviarModeloMensagem(String protocolo, String m)
        {
            var response = new Response();
            var mensagem = JsonConvert.DeserializeObject<Mensagem_ChatModels>(m);

            try
            {

                using (var conexaoMongo = new ConexaoMongo())
                using (var session = HibernateUtil.GetSession())
                using (var repoCanal = new Conexao<Canal_Comunicacao>(session))
                using (var repoConfCanal = new Conexao<Configuracao_Canal_Comunicacao>(session))
                using (var repoCanalComunicacao = new Conexao<Canal_Comunicacao>(session))
                using (var repoModeloMensagem = new Conexao<ModeloMensagem>(session))
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                using (var repositoryEvento = new Conexao<BotEvento>(session))
                {

                    AtendimentoChatMongo chat = await repository.Find(p => p.protocolo == protocolo);
                    var canal_comunicao = await repoCanalComunicacao.selectFirst(p => p.Id == Convert.ToInt64(chat.idCanal));

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
                            if (!chat.bot && !String.IsNullOrEmpty(mensagem?.usuario))
                            {
                                if (chat?.suporte?.idSuporte != mensagem?.matriculaUsuario && chat?.usuariosConvidados.Find(x => x.usuarioAceite == mensagem.matriculaUsuario) == null)
                                {
                                    throw new Exception("{ERROR_NOT_USER}");
                                }
                            }

                            var conf = await repoConfCanal.selectFirst();
                            var canal = await repoCanal.selectFirst(p => p.Id == chat.idCanal);

                            //Troca variáveis
                            var atendimento = new AtendimentoApiController();
                            mensagem.mensagem = atendimento.RemoveVariaveis(mensagem.mensagem, chat).Result;

                            mensagem.nome_usuario = chat.bot ? canal.Bot.Nome : await atendimento.GetNomeUsuario(mensagem.matriculaUsuario);

                            if (conf != null)
                            {
                                if (conf.MostrarNomeAtendente && mensagem.nome_usuario != null)
                                {
                                    mensagem.mensagem = chat.bot ? "" + (canal.Bot.Nome) + ": " + mensagem.mensagem : (mensagem.nome_usuario) + ": " + mensagem.mensagem;
                                }
                                else if (conf.MostrarNomeAtendente && mensagem.nome_usuario == null && mensagem.fluxoChat == false)
                                {
                                    mensagem.nome_usuario = canal.Bot.Nome;
                                    mensagem.mensagem = mensagem.nome_usuario + ": " + mensagem.mensagem;
                                }
                            }

                            mensagem.mensagem = ChatHub.GetMensagemReplace(chat, mensagem.mensagem, mensagem.nome_usuario);

                            var modelo = repoModeloMensagem.selectFirst(p => p.Id == mensagem.idModeloMensagem).Result;

                            var retornoIntegracao = await new IntegracaoWhatsAppBusinessRepository().EnviaModeloMensagemWhatsAppBusiness(new MetaWhatsAppBusiness(canal.TokenWhatsAppBusiness),
                                                                                                                                            chat.whatsAppChatId,
                                                                                                                                            canal.IdNumeroWhatsAppBusiness,
                                                                                                                                            modelo.Nome);
                            mensagem.id = retornoIntegracao.Messages[0].id;
                            mensagem.statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO;
                            mensagem.dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff");
                            mensagem.protocolo = protocolo;

                            if (chat.mensagens == null)
                            {
                                chat.mensagens = new List<Mensagem_ChatModels>();
                            }

                            if (chat.conversas.Count > 0)
                            {
                                mensagem.conversasId = chat.conversas[chat.conversas.Count - 1].id;
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

                            response.result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                response.result = false;
                response.message = ex.Message;
            }

            return response;
        }


        public async Task<ResponseEnviarNotificacaoSuporte> EnviarNotificacaoSuporte(AtendimentoChat chat, bool reincidente = false, string SuporteReincidente = null)
        {
            var semaphore = _locks.GetOrAdd(chat.protocolo, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();
            ResponseEnviarNotificacaoSuporte retorno = new ResponseEnviarNotificacaoSuporte();
            try
            {
                chat = ChatHub.Atendimentos[chat.protocolo];
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
                    var suportesDisponiveis = ChatHub.SuporteDisponiveisAtualizado?
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
                        if (chat?.suporte?.idSuporte == null || reincidente)
                        {
                            if (chat.usuariosEnvioNotificao == null)
                            {
                                chat.usuariosEnvioNotificao = new List<EnvioAtendimentoUsuario>();
                            }

                            if (reincidente && !String.IsNullOrEmpty(SuporteReincidente))
                            {
                                var ultimaMensagem = chat.usuariosEnvioNotificao.OrderByDescending(p => Convert.ToDateTime(p.datahoraEnvio)).FirstOrDefault();
                                if (ultimaMensagem?.recusado == false && (DateTime.Now - Convert.ToDateTime(chat.dataHoraEnviadoAtendente)).TotalSeconds < 30)
                                {
                                    criterios = new List<SuporteChatMongo>();

                                }
                                else
                                {
                                    var criteriosFiltro = criterios.FindAll(p => p.idSuporte != SuporteReincidente && p.status != StatusAtendimento.AUSENTE);
                                    if (criteriosFiltro.Count == 0)
                                    {
                                        criteriosFiltro = criterios.FindAll(p => p.status != StatusAtendimento.AUSENTE);
                                    }

                                    criterios = criteriosFiltro
                                    .OrderBy(p => ChatHub.MensagemEnviada(chat, p.idSuporte))
                                    .ThenBy(p => ChatHub.getSuporteStatus(p.idSuporte)?.qtdAtendimento ?? 0).ThenByDescending(p => Convert.ToDateTime(p.datahoraAlive))
                                        .ToList();
                                }


                            }
                            else
                            {
                                var ultimaMensagem = chat.usuariosEnvioNotificao.OrderByDescending(p => Convert.ToDateTime(p.datahoraEnvio)).FirstOrDefault();
                                if (ultimaMensagem?.recusado == false && (DateTime.Now - Convert.ToDateTime(chat.dataHoraEnviadoAtendente)).TotalSeconds < 30)
                                {
                                    criterios = new List<SuporteChatMongo>();
                                }
                                else
                                {
                                    if (chat.dataHoraEnviadoAtendente == null || (DateTime.Now - Convert.ToDateTime(chat.dataHoraEnviadoAtendente)).TotalSeconds > 30)
                                    {
                                        if (ultimaMensagem == null || (DateTime.Now - Convert.ToDateTime(ultimaMensagem.datahoraEnvio)).TotalSeconds > 15)
                                        {
                                            var criteriosFiltro = criterios.FindAll(p => chat.usuariosEnvioNotificao.Find(y => y.userid == p.idSuporte && (DateTime.Now - Convert.ToDateTime(y.datahoraEnvio)).TotalSeconds < 15) == null && p.status != StatusAtendimento.AUSENTE);
                                            criterios = criteriosFiltro
                                            .OrderBy(p => ChatHub.MensagemEnviada(chat, p.idSuporte))
                                            .ThenBy(p => ChatHub.getSuporteStatus(p.idSuporte)?.qtdAtendimento ?? 0).ThenByDescending(p => Convert.ToDateTime(p.datahoraAlive))
                                        .ToList();
                                        }
                                        else
                                        {
                                            criterios = new List<SuporteChatMongo>();
                                        }
                                    }
                                    else
                                    {
                                        criterios = new List<SuporteChatMongo>();
                                    }
                                }
                            }


                            if (criterios.All(p => ChatHub.MensagemEnviada(chat, p.idSuporte)))
                            {
                                List<(SuporteChatMongo agente, TimeSpan TempoDaUltimaNotificacao)> agentesEUltimaNotificacaocriterios = criterios.Select(p =>
                                {
                                    return (p, (DateTime.Now - Convert.ToDateTime(chat.usuariosEnvioNotificao.Find(y => y.userid == p.idSuporte).datahoraEnvio)));
                                }).ToList();

                                criterios = agentesEUltimaNotificacaocriterios.OrderByDescending(p => p.TempoDaUltimaNotificacao).Select(y => y.agente).ToList();
                            }

                            for (int i = 0; i < criterios.Count; i++)
                            {

                                var suporte = criterios[i];

                                var ultimaNotificacaoAtual = chat.usuariosEnvioNotificao.Find(p => p.userid == suporte.idSuporte);
                                bool temProximoQueRecebeuAMaisTempoOuNaoRecebeu = false;
                                if (ultimaNotificacaoAtual != null)
                                {
                                    var tempoEntreUltimaNotificacaoAtual = DateTime.Now - Convert.ToDateTime(ultimaNotificacaoAtual.datahoraEnvio);
                                    for (int j = i + 1; j < criterios.Count; j++)
                                    {
                                        var idProximo = criterios[j].idSuporte;
                                        var ultimaNotificacaoProximo = chat.usuariosEnvioNotificao.Find(p => p.userid == idProximo);
                                        if (ultimaNotificacaoProximo == null)
                                        {
                                            temProximoQueRecebeuAMaisTempoOuNaoRecebeu = true;
                                            break;
                                        }
                                        else
                                        {
                                            var tempoEntreUltimaNotificacaoProximo = DateTime.Now - Convert.ToDateTime(ultimaNotificacaoProximo.datahoraEnvio);
                                            if (tempoEntreUltimaNotificacaoProximo > tempoEntreUltimaNotificacaoAtual)
                                            {
                                                temProximoQueRecebeuAMaisTempoOuNaoRecebeu = true;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (temProximoQueRecebeuAMaisTempoOuNaoRecebeu)
                                {
                                    continue; // pula este item e segue para o próximo
                                }

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
                                        .ClienteAguardandoAtendimento($"Cliente Aguardando Atendimento ... \n Deseja Iniciar o Atendimento {chat.protocolo} ?", chat);


                                    retorno.notificacaoEnviada = true;
                                    if (chat.usuariosEnvioNotificao == null)
                                    {
                                        chat.usuariosEnvioNotificao = new List<EnvioAtendimentoUsuario>();
                                    }

                                    if (chat.usuariosEnvioNotificao.Find(p => p.userid == suporte.idSuporte) != null)
                                    {
                                        chat.usuariosEnvioNotificao.Find(p => p.userid == suporte.idSuporte).datahoraEnvio = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                        chat.usuariosEnvioNotificao.Find(p => p.userid == suporte.idSuporte).recusado = false;
                                    }
                                    else
                                    {
                                        chat.usuariosEnvioNotificao.Add(new EnvioAtendimentoUsuario
                                        {
                                            datahoraEnvio = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                                            userid = suporte.idSuporte,
                                            recusado = false
                                        });
                                    }

                                    foreach (var item in chat.usuariosEnvioNotificao.FindAll(p => p.userid != suporte.idSuporte))
                                    {
                                        item.recusado = true;
                                    }

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

                        chatMongo.usuariosEnvioNotificao = chat.usuariosEnvioNotificao;
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
            finally
            {
                semaphore.Release();

                // Remove o semáforo se ele não estiver mais sendo usado
                if (semaphore.CurrentCount == 1)
                {
                    SemaphoreSlim removed;
                    _locks.TryRemove(chat.protocolo, out removed);
                }
            }
            return retorno;
        }

        public async Task SendFromClient(AtendimentoChat chat, String mensagem)
        {
            var m = new Mensagem_ChatModels()
            {
                dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                enviadoPeloSuporte = true,
                mensagem = mensagem,
                statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIANDO,
            };

            if (chat.mensagens == null)
                chat.mensagens = new List<Mensagem_ChatModels>();

            chat.mensagens.Add(m);
            ChatHub.Atendimentos[chat.protocolo] = chat;

            var message = Twilio.Rest.Conversations.V1.Conversation.MessageResource.Create(
                author: "whatsapp:+14053698658",
                body: mensagem,
                pathConversationSid: chat.conversationSid
            );
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


                List<Mensagem_ChatModels> listaMensagens = new List<Mensagem_ChatModels>();

                foreach (var item in retornoBot.valor)
                {
                    var mensagem = new Mensagem_ChatModels
                    {
                        dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                        statusEnvio = StatusEnvioEnum.ENVIADO,
                        mensagem = item.texto,
                        protocolo = chat.protocolo,
                        enviadoPeloSuporte = true,
                        fluxoChat = false,
                        usuario = chat.bot == true ? bot?.Nome : null,
                        nome_usuario = bot.Nome,
                        arquivos = item.anexos.Select(p => new ArquivoMensagem_ChatModels
                        {
                            id = p.Id.ToString(),
                            name = p.Nome,
                            url = p.Url
                        }).ToList()
                    };
                    listaMensagens.Add(mensagem);
                    await EnviarMensagem(chat.protocolo, JsonConvert.SerializeObject(mensagem));
                }

                chat.mensagens = (await repository.Find(p => p.protocolo == protocolo)).mensagens;
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
            {
                if (bot == null)
                {
                    bot = repositoryCanal.selectFirst(p => p.Id == chat.idCanal).Result.Bot;
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

                List<Mensagem_ChatModels> listaMensagens = new List<Mensagem_ChatModels>();

                foreach (var item in retornoBot.valor)
                {
                    var mensagem = new Mensagem_ChatModels
                    {

                        dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                        statusEnvio = StatusEnvioEnum.ENVIADO,
                        mensagem = item.texto,
                        protocolo = chat.protocolo,
                        enviadoPeloSuporte = true,
                        fluxoChat = false,
                        usuario = bot?.Nome,
                        nome_usuario = bot?.Nome,
                        arquivos = item.anexos.Select(p => new ArquivoMensagem_ChatModels
                        {
                            id = p.Id.ToString(),
                            name = p.Nome,
                            url = p.Url,
                            tipo = p.Tipo,
                        }).ToList()

                    };

                    await EnviarMensagem(chat.protocolo, JsonConvert.SerializeObject(mensagem));

                    listaMensagens.Add(mensagem);
                    //await repository.Update(p => p.protocolo == protocolo, chat);
                }
                chat.mensagens = (await repository.Find(p => p.protocolo == protocolo)).mensagens;
                chat.idUltimoEvento = chatControlador.EventoAtual?.Id;
                await repository.Update(p => p.protocolo == protocolo, chat);
            }
        }

    }
}



