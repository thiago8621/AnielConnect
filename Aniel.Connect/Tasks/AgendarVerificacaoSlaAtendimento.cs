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
using Aniel.Connect.ControllersWebApi;

namespace Aniel.Connect.Tasks
{
    public class AgendarVerificacaoSlaAtendimentoTask : BackgroundTask
    {
        public GetApiRetorno AgendarVerificacaoAtendimentoSla()
        {
            GetApiRetorno retorno = new GetApiRetorno();
            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                using (var sessionConnect = HibernateUtil.GetSession())
                using (var repo = new Conexao<Agendamento_EquipeAniel>(session))
                using (var repoDoc = new Conexao<Atendimento>(session))
                using (var repoNoti = new Conexao<Notificacoes>(sessionConnect))
                using (var apiDoc = new Documento_ProducaoApiController())
                {
                   
                   List<Atendimento> atendimentos = repoDoc.select(p => p.DataHora_Indicador >= DateTime.Today &&
                                                                                  p.IdJobNotificacaoSla == null).Result.ToList();
                    foreach (var item in atendimentos)
                    {
                        var datahoraindicador = item.DataHora_Indicador.Value;
                        var datahoracriacao = item.DataHoraCriacao.Value;
                        var datahoraencerramento = item.Data_Hora_Fechamento.Value;


                        var sla = apiDoc.GetDescricaoSla(datahoraindicador, datahoracriacao, datahoraencerramento);

                        //Calcula quando deverá ser o próximo ao vencimento
                        if (sla == Resources.RsCadastros.DENTRO_PRAZO || sla == Resources.RsCadastros.ATENCAO)
                        {
                            var tempoAteExecucao = datahoraEstrouraPorcentagem(51, datahoracriacao, datahoraindicador);

                            string idJob = BackgroundJob.Schedule(() => (new NotificarSlaVencidoAtendimentoTask()).NotificarSlaAtendimento(item.Protocolo),
                                                                                                                                    tempoAteExecucao);
                        }

                        //Calcula quando deverá estar em risco de perder o prazo
                        else if (sla == Resources.RsCadastros.VENCIMENTO_PROXIMO)
                        {
                            var tempoAteExecucao = datahoraEstrouraPorcentagem(76, datahoracriacao, datahoraindicador);

                            string idJob = BackgroundJob.Schedule(() => (new NotificarSlaVencidoAtendimentoTask()).NotificarSlaAtendimento(item.Protocolo),
                                                                                                                                    tempoAteExecucao);

                        }

                        //Calcula quando deverá estar vencido
                        else if (sla == Resources.RsCadastros.RISCO_PERDA_PRAZO)
                        {
                            var tempoAteExecucao = datahoraEstrouraPorcentagem(100, datahoracriacao, datahoraindicador);

                            string idJob = BackgroundJob.Schedule(() => (new NotificarSlaVencidoAtendimentoTask()).NotificarSlaAtendimento(item.Protocolo),
                                                                                                                                    tempoAteExecucao);

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


        public static DateTime datahoraEstrouraPorcentagem(double porcentagem, DateTime datahoraCriacao, DateTime datahoraIndicador)
        {
            var porcentagemDecimal = porcentagem / 100;
            var tempoTotal = datahoraIndicador.Subtract(datahoraCriacao).TotalMinutes;
            var fator = (tempoTotal * porcentagemDecimal);
            return datahoraCriacao.AddMinutes(fator);
        }


    }
}