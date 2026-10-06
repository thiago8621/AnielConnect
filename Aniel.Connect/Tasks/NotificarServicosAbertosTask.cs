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
    public class NotificarServicosAbertosTask : BackgroundTask
    {
        public GetApiRetorno NotificarServicosAbertos(string codCt, string projeto, int numDoc, string numObra, DateTime dataHoraPrevista)
        {
            GetApiRetorno retorno = new GetApiRetorno();
            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                using (var sessionConnect = HibernateUtil.GetSession())
                using (var repo = new Conexao<Documento_ProducaoAniel>(session))
                using (var repoSenha = new Conexao<SenhaAniel>(session))
                using (var repoAgendamento = new Conexao<Agendamento_EquipeAniel>(session))
                using (var repoNotiEnvio = new Conexao<NotificacoesEnvio>(sessionConnect))
                using (var repoNoti = new Conexao<Notificacoes>(sessionConnect))
                using (var apiNotifi = new NotificacoesApiController())
                {
                    NotificacaoBusiness notiBuss = new NotificacaoBusiness();
                    Notificacoes noti = repoNoti.selectFirst(p => p.Id == 24).Result;

                    if (noti.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                    {
                        var documento = repo.select(p => p.CodCt == codCt &&
                                                                                  p.Projeto == projeto &&
                                                                                  p.Num_Doc == numDoc &&
                                                                                  p.Num_Obra == numObra && p.Agendamento == null).Result;


                            var notifica = apiNotifi.EnviaNotificacoesServico(24, codCt, projeto, numObra, numDoc, documento.Count().ToString());
                        

                        if (noti.FrequenciaNotificaoMinutos.HasValue)
                            BackgroundJob.Schedule(() => NotificarServicosAbertos(codCt, projeto, numDoc, numObra, dataHoraPrevista), TimeSpan.FromMinutes(noti.FrequenciaNotificaoMinutos.Value));


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