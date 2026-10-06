using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.mongo;
using Aniel.T5.Model.nhibernate;
using MongoDB.Bson;
using MongoDB.Driver;
using NHibernate;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Integracao.Rastreamento.Repositorio;
using Sinapse.Integracao.Rastreamento.Integracoes;
using Sinapse.Util;
using RestSharp.Extensions;
using Sinapse.Integracao.SMS.Interface;
using NHibernate.Util;
using Hangfire;
using OpenXmlPowerTools;
using Aniel.Connect.ConnectBusiness.T5;
using Microsoft.Ajax.Utilities;
using Aniel.Connect.ControllersWebApi;

namespace Aniel.Connect.Tasks
{
    public class NotificarSlaVencidoAtendimentoTask : BackgroundTask
    {
        public GetApiRetorno NotificarSlaAtendimento(string protocolo)
        {
            GetApiRetorno retorno = new GetApiRetorno();
            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                using (var sessionConnect = HibernateUtil.GetSession())
                using (var repo = new Conexao<Atendimento>(session))
                using (var repoSenha = new Conexao<SenhaAniel>(session))
                using (var repoAgendamento = new Conexao<Agendamento_EquipeAniel>(session))
                using (var repoNotiEnvio = new Conexao<NotificacoesEnvio>(sessionConnect))
                using (var repoNoti = new Conexao<Notificacoes>(sessionConnect))
                using (var apiDoc = new Documento_ProducaoApiController())
                using (var apiNotifi = new NotificacoesApiController())
                {
                    NotificacaoBusiness notiBuss = new NotificacaoBusiness();

                    Atendimento atendimento = repo.selectFirst(p => p.Protocolo == protocolo).Result;

                    var datahoraindicador = atendimento.DataHora_Indicador.Value;
                    var datahoracriacao = atendimento.DataHoraCriacao.Value;
                    var datahoraencerramento = atendimento.Data_Hora_Fechamento.Value;

                    var sla = apiDoc.GetDescricaoSla(datahoraindicador, datahoracriacao, datahoraencerramento);

                    if (sla == Resources.RsCadastros.VENCIDO)
                    {
                        Notificacoes noti = repoNoti.selectFirst(p => p.Id == 3).Result;
                        if (noti.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                        {
                            var notifica = apiNotifi.EnviaNotificacoesAtendimento(3, atendimento, null);
                        }
                    }
                    else if (sla == Resources.RsCadastros.VENCIMENTO_PROXIMO)
                    {
                        Notificacoes noti = repoNoti.selectFirst(p => p.Id == 1).Result;
                        if (noti.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                        {
                            var notifica = apiNotifi.EnviaNotificacoesAtendimento(1, atendimento, null);
                        }
                    }
                    else if (sla == Resources.RsCadastros.RISCO_PERDA_PRAZO)
                    {
                        Notificacoes noti = repoNoti.selectFirst(p => p.Id == 2).Result;
                        if (noti.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                        {
                            var notifica = apiNotifi.EnviaNotificacoesAtendimento(2, atendimento, null);
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
                CONSTANTES.log4Net.Error(ex);
            }

            return retorno;
        }




    }
}