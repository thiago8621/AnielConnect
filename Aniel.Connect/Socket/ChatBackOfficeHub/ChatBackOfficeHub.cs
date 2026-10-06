using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using NHibernate;
using Sinapse.DAO;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.Socket.ChatBackOfficeHub
{
    public class ChatBackOfficeHub : Hub
    {
        public static String ContextIdConnection = null;
        public static String ContextNumFre = null;

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

        public static List<ServicoChat> AtendimentosAguardandoConfirmacao = new List<ServicoChat>();
        public static Dictionary<string, ServicoChat> Servicos
        {
            get
            {
                return getServicos();
            }
        }

        public static IChat GetChatSrategy(ServicoChat chat)
        {

            var chatStrategy = new ChatBackOffice(chat.num_os, chat.backOfficeId, ContextIdConnection, ContextNumFre);
            chatStrategy.HubContext = GlobalHost.ConnectionManager.GetHubContext<ChatBackOfficeHub>();
            return chatStrategy;
        }


        public async Task IniciarChat(ServicoChat chat)
        {
            try
            {
                var connectionId = Context.ConnectionId;
                ContextIdConnection = connectionId;
                var num_fre_connection = Context.Headers.Get("num_fre") ?? Context.QueryString["num_fre"];

                ContextNumFre = num_fre_connection;


                String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Atendimento\Chat\Teste";

                if (!Directory.Exists(_pasta))
                {
                    Directory.CreateDirectory(_pasta);
                }


                foreach (var item in chat.usuarios)
                {
                    if (item.num_fre == num_fre_connection)
                        item.connectionid = connectionId;
                }

                File.WriteAllText($"{_pasta}/teste.txt", "textoteste");
                await GetChatSrategy(chat).StartChat(chat);
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                throw new Exception("Erro ao se conectar ao chat.");
            }
        }

        //START EDIT
        public override Task OnReconnected()
        {

            var id = Context.ConnectionId;
            var usuario = Context.Headers.Get("num_fre");
            //verificar se o id da conexão é igual e atualizar no Mongo

            /*var connectionId = Context.ConnectionId; 
            ContextIdConnection = connectionId; 
            var num_fre_connection = Context.Headers.Get("num_fre");
            ContextNumFre = num_fre_connection;
            try
            {
                var num_os_connection = Context.Headers.Get("num_os");
                var chat = UpdateConnectionIdUsuario(num_fre_connection, num_os_connection).Result;


            }
            catch
            {

            }*/



            return base.OnReconnected();
        }



        public override Task OnConnected()
        {
            Console.WriteLine("OnConnected");
            return base.OnConnected();
        }

        public async Task<ServicoChat> UpdateConnectionIdUsuario(String num_os, String num_fre)
        {
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<ServicoChatMongo>(conexaoMongo))
            {
                IMongoQueryable<ServicoChatMongo> query = repository.GetQueryable();
                var servicos = query.ToList().FindAll(p => p.num_os == num_os).ToList();
                var servicoChatMongo = servicos.FirstOrDefault();
                if (servicoChatMongo != null)
                {
                    foreach (var usuario in servicoChatMongo.usuarios)
                    {
                        if (usuario.num_fre == num_fre)
                            usuario.connectionid = ContextIdConnection;
                    }

                    await repository.Update(p => p.num_os == num_os, servicoChatMongo);

                    return ConvertSeriableServicoChat(servicoChatMongo);
                }
                else return null;
            }
        }

        //FIM EDIT
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
                                status = suporte.status
                            };

                            await repository.Save(suporteMongo);
                        }
                        catch (Exception)
                        {

                        }
                    }

                    //await FindClienteNecessitaSuporte(suporte);
                }

                else
                {
                    var tecnico = SuporteDisponiveis.Find(x => x.idSuporte == suporte.idSuporte);
                    tecnico.Categorias = suporte.Categorias;
                    tecnico.Localidades = suporte.Localidades;
                    tecnico.idConexaoChat = suporte.idConexaoChat;

                    if (tecnico.MotivoAusencia == "Realizou Logoff")
                    {
                        tecnico.MotivoAusencia = null;
                        tecnico.status = StatusAtendimento.DISPONIVEL;
                        await repository.Update(p => p.idSuporte == tecnico.idSuporte, tecnico);

                    }
                }

                //Clients.Client(suporte.idConexaoChat).statusUsuario(getSuporteStatus(suporte.idSuporte).status != StatusAtendimento.AUSENTE);
            }
        }

        //public void finalizarConexaoSuporte(String id_suporte)
        //{
        //    using (var conexaoMongo = new ConexaoMongo())
        //    using (var repository = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
        //    using (var API = new AtendimentoApiController())
        //    {
        //        var responseStatus = getSuporteStatus(id_suporte);

        //        if (responseStatus.status != StatusAtendimento.AUSENTE)
        //        {
        //            API.AlterarStatusAgente(new AlterarStatusModels
        //            {
        //                online = false,
        //                idSuporte = id_suporte,
        //                motivoAusencia = "Realizou Logoff"
        //            }).Wait();
        //        }
        //    }

        //}

        public async Task finalizarConexaoSuporte(ServicoChat servicoChat, String idsuporte)
        {
            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
                {
                    repository.Delete(p => p.idSuporte == idsuporte).Wait();

                    if (Servicos.ContainsKey(servicoChat.num_os))
                    {
                        await GetChatSrategy(servicoChat).EnviarNotificacaoSuporte(servicoChat, true);
                    }
                }

            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
        }

        public static async Task EnviarNotificacaoSuporte(ServicoChat chat, Boolean reincidente = false, String SuporteReincidente = null)
        {
            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
                {


                    if (Servicos.ContainsKey(chat.num_os))
                    {
                        /*await GetChatSrategy(chat).EnviarNotificacaoSuporte(chat, reincidente, SuporteReincidente);*/
                    }
                }

            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
        }




        //public async Task AceitarAtendimento(ServicoChat atendimento)
        //{
        //    using (var conexaoMongo = new ConexaoMongo())
        //    using (var repository = new RepositoryMongo<ServicoChatMongo>(conexaoMongo))
        //    using (var repositorySuporte = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
        //    {
        //        if (atendimento.suporte != null)
        //        {

        //            var index = AtendimentosAguardandoConfirmacao.FindIndex(x => x.protocolo == atendimento.protocolo);
        //            AtendimentosAguardandoConfirmacao.RemoveAt(index);

        //            var suporte = SuporteDisponiveis.Find(x => x.idSuporte == atendimento.suporte.idSuporte);
        //            suporte.status = StatusAtendimento.EM_ATENDIMENTO;
        //            await repositorySuporte.Update(p => p.idSuporte == suporte.idSuporte, suporte);

        //            atendimento.suporte = suporte;
        //            await Clients.Group(atendimento.protocolo, Context.ConnectionId).suporteAceito(atendimento.suporte);


        //            if (Atendimentos.ContainsKey(atendimento.protocolo))
        //            {
        //                await EntrarChatAtendimento(atendimento.protocolo, atendimento.suporte.idSuporte, Context.ConnectionId);
        //                Atendimentos[atendimento.protocolo.suporte] = atendimento.suporte;
        //                using (var atendimento_api = new AtendimentoApiController())
        //                {
        //                    await atendimento_api.StatusEmAtendimento(atendimento.num_os, atendimento.suporte.idSuporte);
        //                }

        //                var FilaEsperaCliente = FilasEsperaCliente.Find(p => p.Id_CategoriaCliente == atendimento.cliente.idCategoriaCliente).fila;
        //                var fila = FilaEsperaCliente.ToList().FindAll(x => x.num_os == atendimento.num_os);
        //                if (fila.Count() > 0)
        //                {
        //                    FilaEsperaCliente.Dequeue();
        //                }


        //                IMongoQueryable<ServicoChatMongo> query = repository.GetQueryable();
        //                var atendimentos = query.ToList().FindAll(p => p.num_os == atendimento.num_os).ToList();
        //                ServicoChatMongo atendimentoChatMongo = atendimentos.FirstOrDefault();

        //                if (atendimentoChatMongo != null)
        //                {
        //                    ConvertMongoAtendimentoChat(atendimento, atendimentoChatMongo);
        //                    await repository.Update(p => p.num_os == atendimento.num_os, atendimentoChatMongo);
        //                }

        //            }
        //        }
        //    }
        //}

        //public async Task AlterarDepartamento(String protocolo, String Departamento, String username)
        //{
        //    var chat = Atendimentos[protocolo];
        //    var suporte = SuporteDisponiveis.Find(p => p.idSuporte == username);

        //    using (ISession session = HibernateUtil.GetSession())
        //    {
        //        var departamento = new DepartamentoApiController().FindById(Convert.ToInt64(Departamento)).Result;

        //        using (var conexaoMongo = new ConexaoMongo())
        //        using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
        //        {
        //            await GetChatSrategy(chat).EnviarMensagem(protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
        //            {
        //                dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
        //                enviadoPeloSuporte = true,
        //                usuario = suporte?.nomeAgente,
        //                fluxoChat = true,
        //                statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
        //                mensagem = "Chat transferido para o setor " + departamento[0].Label
        //            }));
        //        }
        //    }
        //}

        public async Task EntrarChatAtendimento(String num_os, String username, string idConexao)
        {
            var connectionId = Context.ConnectionId; //EDIT
            ContextIdConnection = connectionId; //EDIT
            var num_fre_connection = Context.Headers.Get("num_fre"); //EDIT
            ContextNumFre = num_fre_connection;
            if (Servicos.ContainsKey(num_os))
            {
                var chat = Servicos[num_os];
                var usuario = chat.usuarios.Find(p => p.num_fre == username);
                if (usuario?.num_fre == null)
                {
                    chat.usuarios.Add(new ServicoUsuario { num_fre = username, connectionid = idConexao });
                    var suporte = SuporteDisponiveis.Find(p => p.idSuporte == username);

                    if (suporte != null)
                    {
                        using (var conexaoMongo = new ConexaoMongo())
                        using (var repository = new RepositoryMongo<ServicoChatMongo>(conexaoMongo))
                        {
                            ServicoChatMongo chatMongo = await repository.Find(p => p.num_os == num_os);
                            await repository.Update(p => p.num_os == num_os, chatMongo);

                            if (String.IsNullOrEmpty(suporte?.inicioAtendimento) == false)
                            {
                                await GetChatSrategy(chat).EnviarMensagem(num_os, JsonConvert.SerializeObject(new Mensagem_ChatBackOfficeModels
                                {
                                    dataHora = DateTime.Now,
                                    statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                                    mensagem = "{NOME DO AGENTE} entrou na conversa"
                                }));

                                await GetChatSrategy(chat).EnviarMensagem(num_os, JsonConvert.SerializeObject(new Mensagem_ChatBackOfficeModels
                                {
                                    dataHora = DateTime.Now,
                                    statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                                    mensagem = suporte.inicioAtendimento
                                }));
                            }
                            else if (String.IsNullOrEmpty(suporte?.inicioAtendimento) == false)
                            {
                                await GetChatSrategy(chat).EnviarMensagem(num_os, JsonConvert.SerializeObject(new Mensagem_ChatBackOfficeModels
                                {
                                    dataHora = DateTime.Now,
                                    statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                                    mensagem = "{NOME DO AGENTE} entrou na conversa"
                                }));

                                await GetChatSrategy(chat).EnviarMensagem(num_os, JsonConvert.SerializeObject(new Mensagem_ChatBackOfficeModels
                                {
                                    dataHora = DateTime.Now,
                                    statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                                    mensagem = suporte.inicioAtendimento
                                }));
                            }
                        }
                    }

                    if (idConexao != null)
                    {
                        await Groups.Add(idConexao, num_os);
                    }

                }
                else if (usuario.connectionid != idConexao)
                {
                    usuario.connectionid = idConexao;
                    if (idConexao != null)
                    {
                        await Groups.Add(idConexao, num_os);
                    }
                }
            }


        }
        public async Task SairChatAtendimento(String num_os, String idsuporte)
        {
            if (Servicos.ContainsKey(num_os))
            {
                var atendimento = Servicos[num_os];
                var usuarios = atendimento.usuarios.FindAll(p => p.num_fre == idsuporte);

                atendimento.usuarios.RemoveAll(p => p.num_fre == idsuporte);


                await GetChatSrategy(atendimento).EnviarMensagem(num_os, JsonConvert.SerializeObject(new Mensagem_ChatBackOfficeModels
                {
                    dataHora = DateTime.Now,
                    //usuario = atendimento?.suporte?.nomeAgente == null ? idsuporte : atendimento?.suporte?.nomeAgente,
                    statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                    mensagem = "{NOME DO AGENTE} saiu da conversa"
                }));

                foreach (var item in usuarios)
                {
                    try
                    {
                        await Groups.Remove(item.connectionid, num_os);
                    }
                    catch (Exception ex)
                    {
                        CONSTANTES.log4Net.Error(ex.Message, ex);
                    }
                }


            }
        }


        public async Task<bool> EnviaNotificacaoMensagem(String num_os, Mensagem_ChatBackOfficeModels mensagem)
        {
            //Envia notificação de nova mensagem a todos os participantes
            //1) Recupera todos os usuários, exceto quem enviou a mensagem
            var chat = Servicos[num_os];
            var usuarios = chat.usuarios.Where(p => p.num_fre != mensagem.usuario.num_fre).ToList();

            using ISession sessionT5 = HibernateUtil.GetSessionT5();
            using (var repositoryDocumento = new Conexao<Documento_ProducaoAniel>(sessionT5))
            {
                var documento = await repositoryDocumento.selectFirst(p => p.CodCt == chat.codct
                                                       && p.Projeto == chat.projeto
                                                       && p.Num_Doc == Convert.ToInt32(chat.num_doc)
                                                       && p.Num_Obra == chat.num_os);

                CONSTANTES.log4Net.Debug(String.Format("BackOffice Notificação 1 - Num OS: {0} \n ConnectionID: {1} \n ContextNumFre: {2} \n Mensagem: {3}", num_os, ContextIdConnection, ContextNumFre, documento.Numero_Obra_Original));


                if (documento != null)
                await new NotificacoesApiController().EnviaNotificacaoNovaMensagem(usuarios, documento);
            }

            return true;
        }

        public async Task<bool> EnviarMensagem(String num_os, String m)
        {
            var connectionId = Context.ConnectionId;
            ContextIdConnection = connectionId;
            var num_fre_connection = Context.Headers.Get("num_fre") ?? Context.QueryString["num_fre"];

            ContextNumFre = num_fre_connection;

            var mensagem = JsonConvert.DeserializeObject<Mensagem_ChatBackOfficeModels>(m);
            CONSTANTES.log4Net.Debug(String.Format("BackOffice EnviarMensagem - Num OS: {0} \n ConnectionID: {1} \n ContextNumFre: {2} \n Mensagem: {3}", num_os, ContextIdConnection, ContextNumFre, mensagem.mensagem));


            try
            {
                var servico = Servicos[num_os];

                await GetChatSrategy(servico).EnviarMensagem(num_os, m);

                await EnviaNotificacaoMensagem(num_os, mensagem);

                return false;
            }
            catch (Exception ex)
            {
                foreach (var arq in mensagem.arquivos)
                {
                    if (File.Exists(arq.url))
                    {
                        File.Delete(arq.url);
                    }
                }
                CONSTANTES.log4Net.Error(String.Format("BackOffice EnviarMensagem - Num OS: {0} \n ConnectionID: {1} \n ContextNumFre: {2} \n Mensagem: {3}", num_os, ContextIdConnection, ContextNumFre, m), ex);

                return false;
            }
        }

        public async Task<bool> EnviarMensagemAnexo(String num_os, String mensagemMongoId)
        {
            var connectionId = Context.ConnectionId;
            ContextIdConnection = connectionId;
            var num_fre_connection = Context.Headers.Get("num_fre") ?? Context.QueryString["num_fre"];

            ContextNumFre = num_fre_connection;

            //var mensagem = JsonConvert.DeserializeObject<Mensagem_ChatBackOfficeModels>(m);
            CONSTANTES.log4Net.Debug(String.Format("BackOffice EnviarMensagemAnexo - Num OS: {0} \n ConnectionID: {1} \n ContextNumFre: {2} \n MensagemId: {3}", num_os, ContextIdConnection, ContextNumFre, mensagemMongoId));
            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<ServicoChatMongo>(conexaoMongo))
                {
                    await GetChatSrategy(Servicos[num_os]).EnviarMensagemAnexo(num_os, mensagemMongoId);
                }


                return false;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(String.Format("BackOffice EnviarMensagemAnexo - Num OS: {0} \n ConnectionID: {1} \n ContextNumFre: {2} \n MensagemId: {3}", num_os, ContextIdConnection, ContextNumFre, mensagemMongoId), ex);

                return false;
            }
        }

        private async Task SendFromClient(ServicoChat chat, String mensagem)
        {
            var connectionId = Context.ConnectionId; //EDIT
            ContextIdConnection = connectionId; //EDIT
            var num_fre_connection = Context.Headers.Get("num_fre") ?? Context.QueryString["num_fre"]; //EDIT
            ContextNumFre = num_fre_connection;
            var m = new Mensagem_ChatBackOfficeModels()
            {
                dataHora = DateTime.Now,
                mensagem = mensagem,
                statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIANDO

            };
            chat.mensagens.Add(m);
            Servicos[chat.num_os] = chat;

            foreach (var user in chat.usuarios)
            {
                await Clients.Client(user.connectionid).MensagemCliente(m);
            }
        }

        public async Task<String> DownloadAnexo(String num_os, String idMensagem)
        {
            var mensagens = new List<Mensagem_ChatBackOfficeModels>();
            if (Servicos.ContainsKey(num_os))
            {
                var servicos = Servicos[num_os];
                mensagens.AddRange(servicos.mensagens);

                foreach (var item in servicos.mensagens)
                {
                    if (item.anexo != null)
                    {
                        var arquivo = await GetAnexoChatBytes(num_os, item.id, item.anexo.fileExtension);

                        string nomeArquivo = $"{item.anexo.name}.{item.anexo.fileExtension}";

                        var caminhoArquivo = $@"{CONSTANTES.CaminhoFisico}\Arquivos-Chat\{nomeArquivo}";
                        if (File.Exists(caminhoArquivo) == false)
                            File.WriteAllBytes(caminhoArquivo, arquivo);
                    }
                }

                return JsonConvert.SerializeObject(mensagens.FindAll(p => p.statusEnvio == Model.enumerado.StatusEnvioEnum.ENVIADO));
            }
            else
            {
                using (var chatApi = new ChatApiController())
                {
                    //var conversas = await chatApi.GetHistoricoMensagensChat(num_os);
                    return null;
                }
            }
        }

        public async Task<String> MensagensChat(String num_os, String num_fre)
        {
            try
            {
                var mensagens = new List<Mensagem_ChatBackOfficeModels>();
                if (Servicos.ContainsKey(num_os))
                {
                    var servicos = Servicos[num_os];

                    if (servicos.mensagens != null)
                    {
                        ServicoUsuario usuario = servicos.usuarios.Where(p => p.num_fre == num_fre).FirstOrDefault();
                        if (usuario?.dataDesvinculacao != null)
                        {
                            mensagens.AddRange(servicos.mensagens.Where(p => p.dataHora >= usuario.dataDesvinculacao));
                        }
                        else
                        {
                            mensagens.AddRange(servicos.mensagens);
                        }
                        return JsonConvert.SerializeObject(mensagens.FindAll(p => p.statusEnvio == Model.enumerado.StatusEnvioEnum.ENVIADO));
                    }
                    else
                    {
                        Mensagem_ChatBackOfficeModels mensagemVazia = new Mensagem_ChatBackOfficeModels();
                        mensagemVazia.num_os = num_os;
                        mensagemVazia.usuario = new ServicoUsuario();
                        mensagemVazia.mensagem = "Não há mensagens. Para iniciar uma nova conversa, envie uma nova mensagem.";
                        mensagemVazia.statusEnvio = Model.enumerado.StatusEnvioEnum.SEPARADOR;
                        mensagens.Add(mensagemVazia);

                        return JsonConvert.SerializeObject(mensagens);
                    }
                }
                else
                {
                    Mensagem_ChatBackOfficeModels mensagemVazia = new Mensagem_ChatBackOfficeModels();
                    mensagemVazia.num_os = num_os;
                    mensagemVazia.usuario = new ServicoUsuario();
                    mensagemVazia.mensagem = "Não há mensagens. Para iniciar uma nova conversa, envie uma nova mensagem.";
                    mensagemVazia.statusEnvio = Model.enumerado.StatusEnvioEnum.SEPARADOR;
                    mensagens.Add(mensagemVazia);

                    return JsonConvert.SerializeObject(mensagens);
                }
            }
            catch(Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                throw new Exception("Erro ao buscar mensagens da conversa");
            }
        }


        public static bool ChatActive(string numOs)
        {
            if (Servicos.ContainsKey(numOs))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetMensagemReplace(ServicoChat chat, String mensagem, string usuario)
        {
            return mensagem.Replace("{NOME DO AGENTE}", usuario).Replace("{NOME DO CLIENTE}", chat.cliente.nome);
        }

        private static Dictionary<string, ServicoChat> getServicos()
        {
            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<ServicoChatMongo>(conexaoMongo))
                {
                    IMongoQueryable<ServicoChatMongo> query = repository.GetQueryable();
                    query = query.Where(p => p.dataEncerramento == null);
                    var servicos = query.ToList().Select(p => ConvertSeriableServicoChat(p)).Where(p => p.num_os != null).ToList();
                    if (servicos != null && servicos.Count > 0)
                        return servicos.ToDictionary(p => p.num_os, p => p);
                    else
                        return new Dictionary<string, ServicoChat>();
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return new Dictionary<string, ServicoChat>();
            }
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

                chat.mensagens.Add(mensagem);
                await repository.Update(p => p.num_os == num_os, chat);
            }

            return retorno;
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

        public static ServicoChat ConvertSeriableServicoChat(ServicoChatMongo p)
        {
            return new ServicoChat
            {
                cliente = p.cliente,
                dataAbertura = p.dataAbertura,
                dataEncerramento = p.dataEncerramento,
                mensagens = p.mensagens,
                usuarios = p.usuarios,
                backOfficeId = p.backOfficeId,
                num_os = p.num_os,
                codct = p.codct,
                projeto = p.projeto,
                num_doc = p.num_doc,
                dataAgendamento = p.dataAgendamento,
            };
        }

        public static void ConvertMongoServicoChat(ServicoChat p, ServicoChatMongo servicoChatMongo)
        {
            servicoChatMongo.cliente = p.cliente;
            servicoChatMongo.dataAbertura = p.dataAbertura;
            servicoChatMongo.dataEncerramento = p.dataEncerramento;
            servicoChatMongo.mensagens = p.mensagens;
            servicoChatMongo.usuarios = p.usuarios;
            servicoChatMongo.num_os = p.num_os;
            servicoChatMongo.backOfficeId = p.backOfficeId;
            servicoChatMongo.codct = p.codct;
            servicoChatMongo.projeto = p.projeto;
            servicoChatMongo.num_doc = p.num_doc;
            servicoChatMongo.dataAgendamento = p.dataAgendamento;

        }

    }
}