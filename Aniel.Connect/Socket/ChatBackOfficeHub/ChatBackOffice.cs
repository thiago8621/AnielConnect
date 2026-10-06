using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Microsoft.AspNet.Identity;
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
using NHibernate;
using Sinapse.Model.Identity;
using Sinapse.Model;
using Aniel.T5.Model.nhibernate;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Models.T5;

namespace Aniel.Connect.Socket.ChatBackOfficeHub
{
    public class ChatBackOffice : IChat
    {
        public IHubContext HubContext { get; set; }
        public String ConnectionId { get; set; }
        public String ContextNumfre { get; set; }
        public String numOs { get; set; }
        public String backOfficeId { get; set; }

        public ChatBackOffice(String numOs, String backOfficeId, String connectionId, String contextNumFre)
        {
            this.HubContext = GlobalHost.ConnectionManager.GetHubContext<ChatBackOfficeHub>();
            this.numOs = numOs;
            this.backOfficeId = backOfficeId;
            this.ConnectionId = connectionId;
            this.ContextNumfre = contextNumFre;
        }

        public async Task StartChat(ServicoChat chat)
        {
            try
            {
                using (LogExecucao logExecucao = new LogExecucao("LogExecucaoChatBO", CONSTANTES.LogExecucaoAnielChannel))
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var cliente_api = new ClienteApiController())
                using (var gestao_api = new Gestao_EquipeApiController())
                using (var chatApi = new ChatApiController())
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<ServicoChatMongo>(conexaoMongo))
                using (var repoIdentityUser = new Conexao<IdentityUser>(session))
                using (var repoDocumento = new Conexao<Documento_ProducaoAniel>(sessionT5))
                {
                    if (String.IsNullOrEmpty(chat.dataAbertura))
                        chat.dataAbertura = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    //1) Buscar os participantes dessa conversa e marcar como ativo
                    var usuario = await gestao_api.GetParticipantes(chat.codct, chat.projeto, chat.num_os, chat.num_doc);

                    //Caso não encontre participantes vinculados, critica
                    if (usuario.Count == 0)
                    {
                        throw new Exception("Não há monitores, técnico ou supervisores vinculados a essa OS.");
                    }
                    else
                    {
                        var usuarioConversa = chat.usuarios.Count > 0 ? chat.usuarios.First() : null;

                        var usuarioVinculado = chatApi.GetPermissaoOS(usuarioConversa.num_fre, numOs, chat.codct, chat.projeto, Convert.ToString(chat.num_doc)).Result;

                        if (usuarioVinculado.Count == 0)
                        {
                            List<AreaModuloModel> areas = new List<AreaModuloModel>();
                            using (var acessoApi = new AcessoApiController())
                            {
                                areas = (await acessoApi.GetArea_ModuloUsuario(usuarioConversa.num_fre)).Objeto;
                            }

                            //possui área de acesso para visualizar OS
                            if (areas.Where(p => p.Cod_Area == "282").Count() > 0)
                            {
                                ServicoUsuario usuarioMensagem = new ServicoUsuario()
                                {
                                    foto_usuario = null,
                                    nome = usuarioConversa.nome,
                                    num_fre = usuarioConversa.num_fre,
                                    ativo = true,
                                    connectionid = usuarioConversa.connectionid,
                                    tipo_usuario = TipoUsuarioChatEnum.AGENTE,
                                };
                                usuario.Add(usuarioMensagem);
                            }
                        }

                        chat.usuarios = usuario;

                        var doc = await repoDocumento.selectFirst(p => p.Num_Obra == numOs);
                        chat.num_doc = doc.Num_Doc;
                        chat.projeto = doc.Projeto;
                        chat.codct = doc.CodCt;

                        //IMPORTANTE:
                        //Caso já exista a conversa vai entrar no chat, caso contrário criará uma nova conversa
                        if (ChatBackOfficeHub.Servicos.ContainsKey(chat.num_os))
                        {
                            ServicoChatMongo servicoChatMongo = await repository.Find(p => p.num_os == chat.num_os);
                            servicoChatMongo.usuarios = chat.usuarios;
                            servicoChatMongo.num_doc = chat.num_doc;
                            servicoChatMongo.projeto = chat.projeto;
                            servicoChatMongo.codct = chat.codct;
                            //ChatBackOfficeHub.ConvertMongoServicoChat(chat, servicoChatMongo);
                            await repository.Update(p => p.num_os == servicoChatMongo.num_os, servicoChatMongo);

                            await EntrarChatAtendimento(chat.num_os, this.ConnectionId); //EDIT
                        }
                        else
                        {
                            await HubContext.Clients.Client(usuarioConversa.connectionid).InicioAtendimento(chat); // <- Mobile
                            ChatBackOfficeHub.Servicos.Add(chat.num_os, chat);

                            ServicoChatMongo servicoChatMongo = new ServicoChatMongo();
                            ChatBackOfficeHub.ConvertMongoServicoChat(chat, servicoChatMongo);
                            await repository.Save(servicoChatMongo);

                            await EntrarChatAtendimento(chat.num_os, ConnectionId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(String.Format("BackOffice - Num OS: {0} \n ConnectionID: {1} \n ContextNumFre: {2}", chat.num_os, ConnectionId, ContextNumfre), ex);
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
        }

        public async Task EntrarChatAtendimento(String num_os, string idConexao)
        {
            if (ChatBackOfficeHub.Servicos.ContainsKey(num_os))
            {
                var chat = ChatBackOfficeHub.Servicos[num_os];


                //var mensagem = chat.mensagens.Find(p => p.num_os == num_os); //EDIT
                ServicoUsuario usuario = chat.usuarios.Find(p => p.connectionid == idConexao || p.num_fre == ContextNumfre); //EDIT

                /*if (usuario?.num_fre == null)
                {
                    //chat.usuarios.Add(new ServicoUsuario { num_fre = usuario.num_fre, connectionid = idConexao });                  

                    if (idConexao != null)
                    {
                        usuario.connectionid = idConexao;
                        await UpdateConnectionIdUsuario(chat, usuario);
                           
                        await HubContext.Groups.Add(idConexao, num_os);
                    }

                }*/
                //else 
                if (usuario?.connectionid != idConexao)
                {
                    usuario.connectionid = idConexao;
                }

                if (idConexao != null)
                {
                    await UpdateConnectionIdUsuario(chat, usuario);
                    await HubContext.Groups.Add(idConexao, num_os);
                }

            }
        }

        public async Task UpdateConnectionIdUsuario(ServicoChat chat, ServicoUsuario usuarioAlterado)
        {
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<ServicoChatMongo>(conexaoMongo))
            {
                IMongoQueryable<ServicoChatMongo> query = repository.GetQueryable();
                var servicos = query.ToList().FindAll(p => p.num_os == numOs).ToList();
                var servicoChatMongo = servicos.FirstOrDefault();
                if (servicoChatMongo != null)
                {
                    foreach (var usuario in servicoChatMongo.usuarios)
                    {
                        if (usuario.num_fre == usuarioAlterado.num_fre)
                            usuario.connectionid = usuarioAlterado.connectionid;
                    }

                    ChatBackOfficeHub.ConvertMongoServicoChat(chat, servicoChatMongo);
                    await repository.Update(p => p.num_os == numOs, servicoChatMongo);
                }
            }
        }

        public async Task EnviarMensagem(String numOs, String m)
        {

            var mensagem = JsonConvert.DeserializeObject<Mensagem_ChatBackOfficeModels>(m);
            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoChatBO", CONSTANTES.LogExecucaoAnielChannel))
            using (var conexaoMongo = new ConexaoMongo())
            using (var session = HibernateUtil.GetSession())
            using (var repoBot = new Conexao<Bot>(session))
            using (var repositoryEvento = new Conexao<BotEvento>(session))
            using (var repository = new RepositoryMongo<ServicoChatMongo>(conexaoMongo))
            using (var repoIdentityUser = new Conexao<IdentityUser>(session))
            {

                logExecucao.RegistraLog("Log backOffice: " + m);

                //if (ChatHub.Atendimentos.ContainsKey(protocolo))
                //{
                ServicoChatMongo chat = await repository.Find(p => p.num_os == numOs);
                mensagem.num_os = numOs;
                mensagem.statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO;

                //Preenchendo as datas
                if (mensagem.dataHora == null)
                {
                    mensagem.dataHora = Convert.ToDateTime(mensagem.dataHoraString);
                }
                else if (mensagem.dataHoraString == null)
                {
                    mensagem.dataHoraString = mensagem.dataHora.Value.ToString("dd/MM/yyyy HH:mm:ss");
                }

                if (mensagem.usuario == null)
                {
                    mensagem.usuario = chat.usuarios.FirstOrDefault();
                }
                else
                {
                    if (String.IsNullOrEmpty(mensagem.usuario.nome))
                    {
                        IdentityUser dadosUsuario = await repoIdentityUser.selectFirst(p => p.Id == mensagem.usuario.num_fre);
                        mensagem.usuario.nome = dadosUsuario.UserName;
                    }
                }

                mensagem.usuario.connectionid = ConnectionId; //EDIT

                if (chat?.mensagens == null)
                {
                    chat.mensagens = new List<Mensagem_ChatBackOfficeModels>();
                }

                chat.mensagens.Add(mensagem); //EDIT
                if (ConnectionId != null)
                {
                    await HubContext.Clients.Client(ConnectionId).NotificarEnvio(mensagem.id);
                    await HubContext.Clients.Group(numOs, ConnectionId).MensagemResposta(mensagem);

                    await NotificarMobile(chat, mensagem, mensagem.usuario.num_fre);
                }
                else
                {
                    //await hubContext.Clients.Client().NotificarEnvio(mensagem.id);
                    //await HubContext.Clients.Group(protocolo).MensagemResposta(mensagem);

                    await HubContext.Clients.Group(numOs, mensagem.usuario.connectionid).MensagemResposta(mensagem); //EDIT
                    await HubContext.Clients.Client(mensagem.usuario.connectionid).NotificarEnvio(mensagem.id); //EDIT

                    await NotificarMobile(chat, mensagem, mensagem.usuario.num_fre);
                }

                IMongoQueryable<ServicoChatMongo> query = repository.GetQueryable();
                var servicos = query.ToList().FindAll(p => p.num_os == numOs).ToList();
                var servicoChatMongo = servicos.FirstOrDefault();
                if (servicoChatMongo != null)
                {
                    ChatBackOfficeHub.ConvertMongoServicoChat(chat, servicoChatMongo);
                    await repository.Update(p => p.num_os == numOs, servicoChatMongo);
                }
            }
        }

        public async Task EnviarMensagemAnexo(String numOs, String mensagemMongoId)
        {
            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoChatBO", CONSTANTES.LogExecucaoAnielChannel))
            using (var conexaoMongo = new ConexaoMongo())
            using (var session = HibernateUtil.GetSession())
            using (var repoBot = new Conexao<Bot>(session))
            using (var repositoryEvento = new Conexao<BotEvento>(session))
            using (var repository = new RepositoryMongo<ServicoChatMongo>(conexaoMongo))
            using (var repoIdentityUser = new Conexao<IdentityUser>(session))
            {

                logExecucao.RegistraLog("Log backOffice: " + mensagemMongoId);

                //if (ChatHub.Atendimentos.ContainsKey(protocolo))
                //{
                ServicoChatMongo chat = await repository.Find(p => p.num_os == numOs);

                //chat.mensagens.Add(mensagem); //EDIT

                foreach (var msg in chat.mensagens)
                {
                    if (msg.id == mensagemMongoId)
                    {
                        msg.statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO;


                        //Preenchendo as datas
                        if (msg.dataHora == null)
                        {
                            msg.dataHora = Convert.ToDateTime(msg.dataHoraString);
                        }
                        else if (msg.dataHoraString == null)
                        {
                            msg.dataHoraString = msg.dataHora.Value.ToString("dd/MM/yyyy HH:mm:ss");
                        }

                        //msg.dataHoraString = msg.dataHora.Value.ToString("dd/MM/yyyy HH:mm:ss");

                        if (msg.usuario == null)
                        {
                            msg.usuario = chat.usuarios.FirstOrDefault();
                        }
                        else
                        {
                            IdentityUser dadosUsuario = await repoIdentityUser.selectFirst(p => p.Id == msg.usuario.num_fre);
                            if (dadosUsuario != null)
                                msg.usuario.nome = dadosUsuario.UserName;
                        }

                        var mensagemSemAnexoByte = msg;
                        mensagemSemAnexoByte.anexo.arquivoBytes = null;

                        if (ConnectionId != null)
                        {
                            msg.usuario.connectionid = ConnectionId;
                            await HubContext.Clients.Client(ConnectionId).NotificarEnvio(msg.id);
                            await HubContext.Clients.Group(numOs, ConnectionId).MensagemResposta(mensagemSemAnexoByte);

                            await NotificarMobile(chat, mensagemSemAnexoByte, msg.usuario.num_fre);
                        }
                        else
                        {
                            await HubContext.Clients.Group(numOs, msg.usuario.connectionid).MensagemResposta(mensagemSemAnexoByte);
                            await HubContext.Clients.Client(msg.usuario.connectionid).NotificarEnvio(msg.id);

                            await NotificarMobile(chat, mensagemSemAnexoByte, msg.usuario.num_fre);
                        }
                    }
                }

                await repository.Update(p => p.num_os == numOs, chat);
            }
        }

        public async Task NotificarMobile(ServicoChat chat, Mensagem_ChatBackOfficeModels mensagem, String num_fre_quem_enviou)
        {
            using (ISession session = HibernateUtil.GetSessionT5())
            using (ISession sessionNH = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<T5.Model.nhibernate.EquipeAniel>(sessionNH))
            {
                foreach (var usuario in chat.usuarios.Where(p => p.tipo_usuario != TipoUsuarioChatEnum.AGENTE).ToList())
                {
                    if (!usuario.num_fre.Equals(num_fre_quem_enviou) && usuario.ativo)
                    {
                        var deviceId = (await repository.selectFirst(p => p.Num_Fre == usuario.num_fre))?.Device_Id;
                        if (!String.IsNullOrEmpty(deviceId))
                        {
                            var mensagem_json = JsonService.SerializeObject(mensagem);


                            Core.Service.MobileService.NotificarChatBackOffice(deviceId, "Nova mensagem no Chat", "Nova mensagem no Chat",
                                                                               chat.codct, chat.projeto, Convert.ToString(chat.num_doc), chat.num_os,
                                                                               usuario.num_fre, mensagem_json);
                        }
                    }
                }
            }
        }

        public async Task EnviarNotificacaoSuporte(ServicoChat chat, Boolean reincidente = false, String SuporteReincidente = null)
        {

            try
            {

                //var suporte = chat.suporte;
                //await HubContext.Clients.Client(chat.usuarios..idConexaoChat).ClienteAguardandoAtendimento($"Cliente Aguardando Atendimento ... \n Deseja Iniciar o Atendimento?", chat);
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

        }

        public async Task SendFromClient(ServicoChat chat, String mensagem)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatBackOfficeHub>();
            var m = new Mensagem_ChatBackOfficeModels()
            {
                dataHora = DateTime.Now,
                mensagem = mensagem,
                statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIANDO

            };
            chat.mensagens.Add(m);
            ChatBackOfficeHub.Servicos[chat.num_os] = chat;

            //await hubContext.Clients.Client(ConnectionId).MensagemCliente(m);
            await HubContext.Clients.Group(chat.num_os, ConnectionId).MensagemResposta(mensagem);

        }

        public async Task InativaAtivaUsuario(String equipe, String numOs, FuncionarioAniel funcionario)
        {
            var chat = ChatBackOfficeHub.Servicos[numOs];

            using (var session = HibernateUtil.GetSession())
            using (var conexaoMongo = new ConexaoMongo())
            using (var gestao_api = new Gestao_EquipeApiController())
            using (var repository = new RepositoryMongo<ServicoChatMongo>(conexaoMongo))
            using (var repositoryUsu = new RepositoryMongo<ServicoUsuario>(conexaoMongo))
            using (var repoIdentityUser = new Conexao<IdentityUser>(session))
            {

                //1) Buscar os participantes dessa conversa e marcar como ativo
                List<ServicoUsuario> usuariosNovos = await gestao_api.GetParticipantes(null, null, numOs, 0);

                foreach (var item in chat.usuarios)
                {
                    if (item.tipo_usuario != TipoUsuarioChatEnum.AGENTE)
                    {
                        item.ativo = false;
                        item.dataDesvinculacao = DateTime.Now;
                    }
                }

                foreach (var novos in usuariosNovos)
                {
                    ServicoUsuario tecnicoAntigo = chat.usuarios.Find(l => l.num_fre == novos.num_fre);

                    if (tecnicoAntigo != null)
                    {
                        tecnicoAntigo.ativo = true;
                        tecnicoAntigo.dataDesvinculacao = null;
                    }
                    else
                    {
                        ServicoUsuario usuarioNovo = new ServicoUsuario()
                        {
                            foto_usuario = null,
                            nome = funcionario.Nome,
                            num_fre = funcionario.Num_Fre,
                            ativo = true,
                            tipo_usuario = novos.tipo_usuario
                        };

                        chat.usuarios.Add(usuarioNovo);
                    }

                    //Permanece sem alterações
                    //if (tecnicoAntigo.ativo == true && tecnicoAntigo.tipo_usuario == novos.tipo_usuario)
                    //{
                    //    usuario.Add(tecnicoAntigo);
                    //}
                    //else if (tecnicoAntigo.ativo != true && tecnicoAntigo.tipo_usuario == novos.tipo_usuario)
                    //{
                    //    tecnicoAntigo.ativo = true;
                    //    usuario.Add(tecnicoAntigo);
                    //}
                    //else if (tecnicoAntigo.tipo_usuario != novos.tipo_usuario)
                    //{
                    //    ServicoUsuario usuarioNovo = new ServicoUsuario()
                    //    {
                    //        foto_usuario = null,
                    //        nome = funcionario.Nome,
                    //        num_fre = funcionario.Num_Fre,
                    //        ativo = true,
                    //        tipo_usuario = novos.tipo_usuario
                    //    };

                    //    usuario.Add(usuarioNovo);
                    //}
                }


                //    foreach (var novos in usuariosNovos)
                //{
                //    var servicoUsuario = await repositoryUsu.Find(p => p.num_fre == novos.num_fre && p.tipo_usuario == novos.tipo_usuario);

                //    //Caso esteja inativo
                //    if(servicoUsuario.ativo == false)
                //    {
                //        servicoUsuario.ativo = true;
                //    }
                //    else
                //    {
                //        servicoUsuario.ativo = false;
                //    }

                //    usuario.Add(servicoUsuario);
                //}



                //    foreach (var usuarioChat in chat.usuarios)
                //{
                //    if (usuarioChat.num_fre != equipe && usuarioChat.tipo_usuario == 0)
                //    {
                //        if(usuarioChat.dataDesvinculacao == null)
                //        {
                //            usuarioChat.dataDesvinculacao = DateTime.Now;
                //        }
                //        usuarioChat.ativo = false;
                //        usuario.Add(usuarioChat);
                //    }
                //    else
                //    {
                //        usuarioChat.dataDesvinculacao = null;
                //        usuario.Add(usuarioChat);
                //    }
                //}

                //ServicoUsuario usuarioNovo = new ServicoUsuario()
                //{
                //    foto_usuario = null,
                //    nome = funcionario.Nome,
                //    num_fre = funcionario.Num_Fre,
                //    ativo = true,
                //};



                ServicoChatMongo servicoChatMongo = await repository.Find(p => p.num_os == chat.num_os);
                servicoChatMongo.usuarios = chat.usuarios;
                await repository.Update(p => p.num_os == servicoChatMongo.num_os, servicoChatMongo);
            }
        }

    }
}

