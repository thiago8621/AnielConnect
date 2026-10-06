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

namespace Aniel.Connect.Tasks
{
    public class AgendarVerificacaoAgendamentoTask : BackgroundTask
    {
        public GetApiRetorno AgendarVerificacaoAgendamento()
        {
            GetApiRetorno retorno = new GetApiRetorno();
            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                using (var sessionConnect = HibernateUtil.GetSession())
                using (var repo = new Conexao<Agendamento_EquipeAniel>(session))
                using (var repoDoc = new Conexao<Documento_ProducaoAniel>(session))
                using (var repoNoti = new Conexao<Notificacoes>(sessionConnect))
                {
                    Notificacoes noti = repoNoti.selectFirst(p => p.Id == 1).Result;
                    if (noti != null && noti.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                    {
                        List<Agendamento_EquipeAniel> agendamentos = repo.select(p => p.DataHora_Inicio >= DateTime.Now &&
                                                                                  p.IdJobNotificacaoAtraso == null &&
                                                                                  p.DataHora_Inicio < DateTime.Today.AddDays(1)).Result.ToList();
                        foreach (var item in agendamentos)
                        {
                            var doc = repoDoc.selectFirst(p => p.CodCt == item.CodCt &&
                                                               p.Projeto == item.Projeto &&
                                                               p.Num_Doc == item.Num_Doc &&
                                                               p.Num_Obra == item.Num_Obra).Result;

                            if (doc.DataHora_Ini_Atend.HasValue == false)
                            {

                                var tempoAteExecucao = item.DataHora_Inicio - DateTime.Now;
                                if (noti.FrequenciaNotificaoMinutos.HasValue)
                                {
                                    tempoAteExecucao = tempoAteExecucao.Add(TimeSpan.FromMinutes(noti.FrequenciaNotificaoMinutos.Value));
                                }

                                string idJob = BackgroundJob.Schedule(() => (new NotificarAtrasoAgendamentoTask()).NotificarAgendamento(item.CodCt,
                                                                                                                                        item.Projeto,
                                                                                                                                        item.Num_Doc,
                                                                                                                                        item.Num_Obra,
                                                                                                                                        item.DataHora_Inicio),
                                                                                                                                        tempoAteExecucao);
                                item.IdJobNotificacaoAtraso = idJob;
                                repo.Update(item).Wait();
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