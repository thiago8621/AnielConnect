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

namespace Aniel.Connect.Tasks
{
    public class EquipeQuilometragemTask : BackgroundTask
    {
        public GetApiRetorno<EquipeQuilometragemAniel> PreencherQuilometragem(string equipeParam, DateTime? dataParam,bool gravar)
        {

            GetApiRetorno<EquipeQuilometragemAniel> retorno = new GetApiRetorno<EquipeQuilometragemAniel>();
            try
            {

                using (var ISession = HibernateUtil.GetSessionT5())
                using (var sessionConnect = HibernateUtil.GetSession())
                using (var documentoRepo = new Conexao<Documento_ProducaoAniel>(ISession))
                using (var integracoesExternasRepo = new Conexao<IntegracoesExternas>(sessionConnect))
                using (var equipeRepo = new Conexao<EquipeAniel>(ISession))
                using (var equipeQuilometragemRepo = new Conexao<EquipeQuilometragemAniel>(ISession))
                using (var localidadeRepo = new Conexao<LocalidadeAniel>(ISession))
                {
                    var data = dataParam ?? DateTime.Today.AddDays(-1);
                    IList<EquipeAniel> equipesAniel = String.IsNullOrEmpty(equipeParam) ? equipeRepo.select().Result : equipeRepo.select(p => p.Matricula == equipeParam).Result;

                    var integracoesExternas = integracoesExternasRepo.select().Result.ToList();
                    IntegracaoRastreamento integracaoRastreamento = null;
                    //integracaoRastreamento = new Cobli("XbRSg3j.84ce2e07-174a-4a15-82bc-b8abc7a09416");
                    if (integracoesExternas.Find(p => p.IntegracaoExterna == Model.enumerado.IntegracaoExterna.COBLI) != null)
                    {
                        var integracaoCobli = integracoesExternas.Find(p => p.IntegracaoExterna == Model.enumerado.IntegracaoExterna.COBLI);
                        integracaoRastreamento = new Cobli(integracaoCobli.TokenAcesso);
                    }

                    var rastreamentoResponse = integracaoRastreamento != null ? new IntegracaoRastreamentoRepository().GetPath(integracaoRastreamento, data.Date, Convert.ToDateTime(data.ToString("dd/MM/yyyy 23:59"))) : null;
                    foreach (var equipe in equipesAniel)
                    {

                        var servicos = documentoRepo.select(p => p.Equipe == equipe.Matricula && p.Data_Maxima == data).Result;

                        foreach (var item in servicos.Where(p => p.Latitude_Str == null))
                        {
                            var localidade = localidadeRepo.selectFirst(p => p.Cod_Escritorio == item.Cod_Escritorio && p.Cod_Area == item.Cod_Area && p.Cod_Localidade == item.Cod_Localidade).Result;
                            if (localidade != null)
                            {
                                item.Latitude_Str = Convert.ToString(localidade.Latitude);
                                item.Longitude_Str = Convert.ToString(localidade.Longitude);
                            }
                        }


                        servicos = servicos.Where(p => p.Latitude_Str != null).OrderBy(p => p.Hora_Maxima).ToList();
                        List<(double? latitude, double? longitude)> localizacoes = new List<(double? latitude, double? longitude)>();
                        List<(double? latitude, double? longitude)> localizacoesRealizadas = GetLocalizacoesRealizadas(sessionConnect, equipe.Matricula, data);

                        if (equipe.Ponto_Partida != null)
                        {
                            localizacoes.Add((latitude: equipe.Ponto_Partida.Latitude,
                                              longitude: equipe.Ponto_Partida.Longitude)
                                            );
                        }

                        localizacoes.AddRange(servicos.Where(p => p.GetLatitude_Str() != null && p.GetLongitude_Str() != null).Select(p => (p.GetLatitude_Str(), p.GetLongitude_Str())));
                        double totalKM = QuilometragemTotal(localizacoes);
                        double totalKmRealizado = QuilometragemTotal(localizacoesRealizadas);

                        var registrosAntigos = equipeQuilometragemRepo.select(p => p.Equipe.Matricula == equipe.Matricula && p.Data == data).Result;
                        equipeQuilometragemRepo.Remove(registrosAntigos).Wait();

                        var quilometragem = new EquipeQuilometragemAniel
                        {
                            Quantidade_OS = servicos.Count(),
                            Equipe = equipe,
                            Data = data,
                            DistanciaPlanejada = totalKM,
                            DistanciaRealizada = totalKmRealizado,
                            DataHoraCriacao = DateTime.Now
                        };

                        if (rastreamentoResponse != null && rastreamentoResponse.Sucesso)
                        {
                            var rastreamentoEquipe = rastreamentoResponse.Dados.Find(p => p.nome != null && p.nome.ToUpper() == equipe.Nome.ToUpper() && p.data == data);
                            if (rastreamentoEquipe != null && equipe.Nome.ToUpper() == rastreamentoEquipe.nome.ToUpper())
                            {
                                quilometragem.DistanciaRealRastreamento = rastreamentoEquipe.metrosPercorridos.Value / 1000;
                                quilometragem.TempoParalisadoMinutos = rastreamentoEquipe.minutosParados.Value;
                            }
                        }


                        retorno.Objeto = quilometragem;

                        if (gravar)
                            equipeQuilometragemRepo.SaveOrUpdate(quilometragem).Wait();
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

        public double QuilometragemTotal(List<(double? latitude, double? longitude)> localizacoes)
        {
            double retorno = 0;

            var postHttp = new PostHttp(CONSTANTES.log4Net);
            string latlongPlanejada = null;

            var listaQuebrada = ListService.GetListaQuebrada(localizacoes, 170);
            for (int j = 0; j < listaQuebrada.Count; j++)
            {
                latlongPlanejada = null;
                var grupoLocalizacoes = listaQuebrada[j + 1];

                for (int i = 1; i < grupoLocalizacoes.Count(); i++)
                {
                    var latlngAnterior = ($"{grupoLocalizacoes[i - 1].longitude}|{grupoLocalizacoes[i - 1].latitude}").Replace(",", ".").Replace("|", ",");
                    var latlngAtual = ($"{grupoLocalizacoes[i].longitude}|{grupoLocalizacoes[i].latitude}").Replace(",", ".").Replace("|", ",");

                    latlongPlanejada += String.Concat(latlngAnterior, ";", latlngAtual);

                    if (i + 1 < grupoLocalizacoes.Count())
                    {
                        latlongPlanejada += ";";
                    }

                }

                try
                {
                    var url = $"{@CONSTANTES.URLROUTE}{latlongPlanejada}?geometries=geojson&steps=true";
                    var resultado = JsonService.DeserializeJasonMapper<OsrmDistanceMatrixModel>(postHttp.ExecuteAsync(url, 10000));
                    retorno += resultado.routes.Where(p => p?.distance != null).Sum(p => p.distance);
                }
                catch
                {
                    retorno += 0;
                }

            }

            return Math.Round(retorno / 1000, 2);

        }





        public List<(double? latitude, double? longitude)> GetLocalizacoesRealizadas(ISession sessionConnect, string equipe, DateTime data)
        {

            List<Equipe_MapaModel_Realizada> ListaRealizada = new List<Equipe_MapaModel_Realizada>();
            List<Equipe_MapaModel_Realizada> temp = new List<Equipe_MapaModel_Realizada>();
            using (ConexaoMongo conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<LogLocalizacaoDataMongo>(conexaoMongo)) /*using (var repository = new RepositoryMongo<LogLocalizacaoDataMongo>(conexaoMongo))*/
            {
                try
                {

                    Boolean Exibe_RDOs = (new Conexao<Configuracoes_Gerais>(sessionConnect).selectFirst().Result).ExibeRDOs;
                    //var dateMongo = BsonValue.Create(data.Date.ToLocalTime());
                    //LogLocalizacaoDataMongo logLocalizacao = repository.GetQueryable().Where(x => x.Equipe == equipe && x.DataDocumento == dateMongo).SingleOrDefault();

                    //if (logLocalizacao != null)
                    //{
                    //    foreach (var item in logLocalizacao.localizacoes)
                    //    {
                    //        var Aparelho = item.Aparelho;
                    //        var modelo_aparelho = Aparelho.Modelo;
                    //        var datahora = item.DataHora.Value.ToLocalTime();

                    //        if (!String.IsNullOrEmpty(modelo_aparelho))
                    //        {
                    //            temp.Add(new Equipe_MapaModel_Realizada()
                    //            {
                    //                DataHora = datahora,
                    //                DataHoraStr = datahora.ToString("dd/MM/yyyy HH:mm"),
                    //                Latitude = item.latitude,
                    //                Longitude = item.longitude
                    //            });
                    //        }
                    //    }
                    //}

                    var _collection = repository.GetCollection();
                    StringBuilder projecao = new StringBuilder();
                    projecao.Append("{ ");
                    projecao.Append("   Equipe: '$Equipe', ");
                    projecao.Append("   Localizacoes: { ");
                    projecao.Append("          $filter: { ");
                    projecao.Append("                input: '$localizacoes', ");
                    projecao.Append("                as :'localizacoes', ");
                    projecao.Append("                cond: { ");
                    projecao.Append("                     $and: [ ");
                    projecao.Append("                       {$gte:['$$localizacoes.DataHora', " + data.Date.ToJson() + "]}, ");
                    projecao.Append("                       {$lte:['$$localizacoes.DataHora', " + data.Date.AddDays(1).AddSeconds(-1).ToJson() + "]} ");
                    projecao.Append("                     ] ");
                    projecao.Append("                }");
                    projecao.Append("         }");
                    projecao.Append("   }");
                    projecao.Append("}");

                    var query = _collection.Aggregate().Match(p => p.Equipe == equipe).Project(projecao.ToString());
                    var lista = query.ToList();
                    lista.ForEach(p =>
                    {
                        Sinapse.Util.CollectionHelper.ForEach(p.GetValue("Localizacoes").AsBsonArray.Values, l =>
                        {
                            var Aparelho = l.AsBsonDocument.GetValue("Aparelho");
                            var modelo_aparelho = Aparelho.AsBsonDocument.GetValue("Modelo") != BsonNull.Value ? Aparelho.AsBsonDocument.GetValue("Modelo").AsString : null;
                            var datahora = Convert.ToDateTime(l.AsBsonDocument.GetValue("DataHora").ToLocalTime());

                            if (!String.IsNullOrEmpty(modelo_aparelho))
                            {
                                temp.Add(new Equipe_MapaModel_Realizada()
                                {
                                    DataHora = datahora,
                                    DataHoraStr = datahora.ToString("dd/MM/yyyy HH:mm"),
                                    Latitude = l.AsBsonDocument.GetValue("latitude").AsDouble,
                                    Longitude = l.AsBsonDocument.GetValue("longitude").AsDouble,

                                });
                            }
                        });

                    });

                    temp = temp.Where(p => (Convert.ToString(p.Latitude).Contains(',') || Convert.ToString(p.Latitude).Contains('.')) &&
                                               (Convert.ToString(p.Longitude).Contains(',') || Convert.ToString(p.Longitude).Contains('.'))).ToList();

                    if (temp.Count() > 0)
                    {
                        Equipe_MapaModel_Realizada ultimaRota = temp.First();
                        Double tolerancia = 100; //metros
                        var primeiroPonto = temp.First();
                        primeiroPonto.Ponto_Partida = equipe;
                        ListaRealizada.Add(primeiroPonto);

                        foreach (var obj in temp)
                        {
                            if (obj.Latitude.HasValue && obj.Longitude.HasValue && ultimaRota.Latitude.HasValue && ultimaRota.Longitude.HasValue)
                            {

                                var distanciaKM = MapaService.GetMenorDistancia(obj.Latitude, obj.Longitude, ultimaRota.Latitude, ultimaRota.Longitude) / 1852;
                                var distanciaMetros = distanciaKM * 1000;

                                if (!String.IsNullOrEmpty(obj.Num_Obra) ||
                                    distanciaMetros > tolerancia)
                                {
                                    ListaRealizada.Add(obj);
                                    ultimaRota = obj;
                                }
                            }

                        }
                    }

                    var localizacoesRealizadas = ListaRealizada.Select(p => (p.Latitude, p.Longitude)).ToList();


                }
                catch
                {

                }
            }

            return ListaRealizada.Select(p => (p.Latitude, p.Longitude)).ToList();
        }



    }
}