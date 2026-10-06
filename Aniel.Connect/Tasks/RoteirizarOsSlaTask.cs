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
using Aniel.Connect.ControllersWebApi.T5;

namespace Aniel.Connect.Tasks
{
    public class RoteirizarOsSlaTask : BackgroundTask
    {
        public GetApiRetorno RoteirizarOsSla()
        {
            GetApiRetorno retorno = new GetApiRetorno();
            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                using (var sessionConnect = HibernateUtil.GetSession())
                using (var repo = new Conexao<Documento_ProducaoAniel>(session))
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(sessionConnect))
                using (var api_Roteirizacao = new Painel_RoteirizacaoApiController())
                {

                    Configuracoes_Gerais config = repoConfig.selectFirst().Result;
                    var servicos = config.EncaixeOsProximoVencimento ? repo.select(p => p.Data_Indicador >= DateTime.Today).Result : repo.select(p => p.Dia >= DateTime.Now.AddDays(-1)).Result;


                    if (config.RoteirizarSemLatLong == false)
                    {
                        servicos = servicos.Where(p => string.IsNullOrEmpty(p.Latitude_Str) == false).ToList();
                    }

                    var servicosRoteirizar = new List<Documento_ProducaoAniel>();

                    foreach (var item in servicos)
                    {
                        if (config.EncaixeOsProximoVencimento)
                        {
                            var tempoLimite = item.TipoServico.TempoLimiteEncaixe;
                            if (tempoLimite.HasValue)
                            {
                                var datahorasla = item.Data_Indicador.Value.Add(item.Hora_Indicador.Value.TimeOfDay);
                                var datahoraatual = DateTime.Now;
                                var temposla = datahorasla - datahoraatual;

                                if (temposla.TotalMinutes <= tempoLimite &&
                                    item.Equipe is null &&
                                    item.RoteirizadoAuto != "S")
                                {
                                    servicosRoteirizar.Add(item);
                                    item.RoteirizadoAuto = "S";
                                    repo.Update(item).Wait();
                                }
                            }
                        }
                        else
                        {
                            var datahorasla = item.Data_Indicador.Value.Add(item.Hora_Indicador.Value.TimeOfDay);
                            var datahoracriacao = item.Dia.Value.Add(item.Hora.Value.TimeOfDay);
                            var temposla = datahorasla - datahoracriacao;

                            if (temposla <= config.TempoMinimoSlaRoteirizacaoAutomatico.Value.TimeOfDay && 
                                item.Equipe is null &&
                                item.RoteirizadoAuto != "S")
                            {
                                servicosRoteirizar.Add(item);
                                item.RoteirizadoAuto = "S";
                                repo.Update(item).Wait();
                            }
                        }
                    }

                    if (servicosRoteirizar.Count > 0)
                    {
                        var resultado_roteirizacao = api_Roteirizacao.Roteirizar(new ParamWebApi<RoteirizarServicosModel>
                        {
                            Objeto = new RoteirizarServicosModel
                            {
                                ids = servicosRoteirizar.Select(p => String.Join("|", new List<String> { p.CodCt, p.Projeto, p.Num_Obra, p.Num_Doc.ToString() })).ToList(),
                                data = DateTime.Now.ToString("dd/MM/yyyy"),
                                numero_max_atendimento = 1000,
                                encaixe = true,
                                encaixeAutomatico = true
                            },
                            Usuario = "ANIEL",
                        });
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