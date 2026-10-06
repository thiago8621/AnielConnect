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

namespace Aniel.Connect.Tasks
{
    public class NotificarAtrasoAgendamentoTask : BackgroundTask
    {
        public GetApiRetorno NotificarAgendamento(string codCt, string projeto, int numDoc, string numObra, DateTime dataHoraPrevista)
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
                {
                    NotificacaoBusiness notiBuss = new NotificacaoBusiness();
                    Notificacoes noti = repoNoti.selectFirst(p => p.Id == 1).Result;

                    if (noti.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                    {
                        Documento_ProducaoAniel documento = repo.selectFirst(p => p.CodCt == codCt &&
                                                                                  p.Projeto == projeto &&
                                                                                  p.Num_Doc == numDoc &&
                                                                                  p.Num_Obra == numObra).Result;

                        Agendamento_EquipeAniel agendamento = repoAgendamento.selectFirst(p => p.CodCt == codCt &&
                                                                                  p.Projeto == projeto &&
                                                                                  p.Num_Doc == numDoc &&
                                                                                  p.Num_Obra == numObra).Result;


                        if (agendamento?.DataHora_Inicio == dataHoraPrevista)
                        {
                            //Verificar se está atrasado
                            if (documento.DataHora_Ini_Atend.HasValue == false && DateTime.Now > agendamento?.DataHora_Inicio)
                            {
                                foreach (var item in noti.Destinatarios)
                                {
                                    //Buscar Usuario
                                    string usuario = null;
                                    if (item.Tipo_Destinatario == Model.enumerado.TipoUsuarioEnum.MONITOR_EQUIPE)
                                    {
                                        usuario = documento.EquipeAniel?.Monitor;
                                    }
                                    if (item.Tipo_Destinatario == Model.enumerado.TipoUsuarioEnum.SUPERVISOR_EQUIPE && documento.EquipeAniel?.CodSuperv != null)
                                    {
                                        usuario = repoSenha.selectFirst(p => p.NumFre == documento.EquipeAniel.CodSuperv).Result?.Usuario;
                                    }

                                    if (usuario != null)
                                    {
                                        //Inserir Notificicao
                                        string textoNoti = notiBuss.AplicarVariaveis(noti.Notificacao, documento, null);
                                        NotificacoesEnvio envio = new NotificacoesEnvio
                                        {
                                            Data_Envio = DateTime.Now,
                                            Texto_Notificacao = textoNoti,
                                            Status_Notificacao = Model.enumerado.Status_Notificacao.NaoLido,
                                            Usuario_Destino = usuario,
                                            Notificacoes = noti,
                                            DataHoraCriacao = DateTime.Now,
                                            Chave = $"{documento.CodCt}|{documento.Projeto}|{documento.Num_Doc}|{documento.Num_Obra}"

                                        };

                                        repoNotiEnvio.SaveOrUpdate(envio).Wait();
                                    }

                                }

                                if(DateTime.Now < agendamento.DataHora_Fim && noti.FrequenciaNotificaoMinutos.HasValue)
                                    BackgroundJob.Schedule(() => NotificarAgendamento(codCt, projeto, numDoc, numObra, dataHoraPrevista), TimeSpan.FromMinutes(noti.FrequenciaNotificaoMinutos.Value));
                            }

                        }
                        else
                        {
                            agendamento.IdJobNotificacaoAtraso = null;
                            repoAgendamento.Update(agendamento).Wait();
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