using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using MongoDB.Driver;
using NHibernate;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Util;
using Aniel.Connect.Socket.ChatHub;
using Aniel.Connect.Model.enumerado;
using Sinapse.Integracao.MetaBusiness.Integracoes;
using Sinapse.Integracao.WhatsAppBusiness.Repositorio;
using System.Web;
using Newtonsoft.Json;
using System.Web.Http;
using Aniel.Connect.ControllersWebApi;
using OpenXmlPowerTools;

namespace Aniel.Connect.Tasks
{
    public class ConsultaTempoTransferenciaBotTask : BackgroundTask
    {
        public async Task<GetApiRetorno> ConsultaTempoTransferenciaBot()
        {
            GetApiRetorno retorno = new GetApiRetorno();

            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                {

                    var chatBot = await repository.FindAll(p => p.bot == true && p.dataEncerramento == null);

                    foreach (var chat in chatBot)
                    {
                        bool transfereChat = await ConsultaTransferenciaBot(chat);

                        if (transfereChat)
                        {
                            await ChatHub.GetChatSrategy(chat).EnviarNotificacaoSuporte(chat);
                        }
                    }


                }

            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        public async Task<bool> ConsultaTransferenciaBot(AtendimentoChatMongo chat)
        {
            bool retorno = false;

            using (var conexaoMongo = new ConexaoMongo())
            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            using (var repositoryBot = new Conexao<Bot>(session))
            using (var repositoryCanal = new Conexao<Canal_Comunicacao>(session))
            {
                if (chat.bot)
                {
                    Canal_Comunicacao canal = await repositoryCanal.selectFirst(p => p.Id == chat.idCanal);

                    if (canal?.Bot != null)
                    {
                        Bot bot = await repositoryBot.selectFirst(p => p.Id == canal.Bot.Id);

                        if (bot?.Minutos.HasValue == true)
                        {
                            var mensagemMaisAntiga = chat.mensagens.OrderBy(m => m.dataHora).FirstOrDefault();

                            if (mensagemMaisAntiga != null && DateTime.TryParse(mensagemMaisAntiga.dataHora, out DateTime dataHoraMensagem))
                            {
                                var tempoDecorrido = DateTime.Now - dataHoraMensagem;

                                if (tempoDecorrido.TotalMinutes > bot.Minutos.Value)
                                {
                                    retorno = true;
                                }
                            }
                        }
                    }
                }
            }

            return retorno;
        }

        public async Task<GetApiRetorno> ConsultaTempoAtendimentoAberto24h()
        {
            GetApiRetorno retorno = new GetApiRetorno();

            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                {
                    var chatBot = await repository.FindAll(p => p.dataEncerramento == null && p.whatsAppChatId != null);

                    foreach (var chat in chatBot)
                    {
                        var mensagemMaisRecenteCliente = chat.mensagens.Where(m => !m.enviadoPeloSuporte).OrderByDescending(m => m.dataHora).FirstOrDefault();
                        DateTime.TryParse(mensagemMaisRecenteCliente.dataHora, out DateTime dataHoraMensagem);

                        var tempoDecorrido = DateTime.Now - dataHoraMensagem;
                        /* 86400 = 60*60*24 */
                        if (tempoDecorrido.TotalSeconds > 86400)
                        {
                            using (var api = new AtendimentoApiController())
                            {
                                await api.FinalizarChat(chat.protocolo, false, "Trigger-Hangfire", 0);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }
    }
}