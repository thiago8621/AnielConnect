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
    public class AtualizarVeiculoTask : BackgroundTask
    {
        public GetApiRetorno<EquipeQuilometragemAniel> AtualizarVeiculo()
        {

            GetApiRetorno<EquipeQuilometragemAniel> retorno = new GetApiRetorno<EquipeQuilometragemAniel>();
            try
            {
                try
                {
                    using (ISession session_connect = HibernateUtil.GetSession())
                    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                    using (var repositoryIntegracoesExternas = new Conexao<IntegracoesExternas>(session_connect))
                    using (var repositoryVeiculo = new Conexao<VeiculoAniel>(sessionT5))
                    {
                        var integracoesExternas = (repositoryIntegracoesExternas.select().Result).ToList();
                        IntegracaoRastreamento integracaoRastreamento = null;
                        List<VeiculoAniel> veiculos = (repositoryVeiculo.select().Result).ToList();
                        if (integracoesExternas.Find(p => p.IntegracaoExterna == Model.enumerado.IntegracaoExterna.COBLI) != null)
                        {
                            var integracaoCobli = integracoesExternas.Find(p => p.IntegracaoExterna == Model.enumerado.IntegracaoExterna.COBLI);
                            integracaoRastreamento = new Cobli(integracaoCobli.TokenAcesso);
                        }

                        if (integracaoRastreamento != null)
                        {
                            foreach (var item in veiculos)
                            {
                                var responseOdometro = integracaoRastreamento.GetOdometro(item.Placa);
                                if (responseOdometro.Sucesso && item.KmOdometro != responseOdometro.DistanciaTotalKm)
                                {
                                    item.KmOdometro = Convert.ToInt32(responseOdometro.DistanciaTotalKm);
                                    repositoryVeiculo.Update(item).Wait();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex.InnerException);
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