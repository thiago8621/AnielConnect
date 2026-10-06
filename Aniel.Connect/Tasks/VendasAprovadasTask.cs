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
using Resources;
using Aniel.Connect.ControllersWebApi;

namespace Aniel.Connect.Tasks
{
    public class VendasAprovadasTask : BackgroundTask
    {
        public GetApiRetorno<string> VendasAprovadas()
        {

            GetApiRetorno<string> retorno = new GetApiRetorno<string>();
            try
            {
                try
                {
                    using (ISession session_connect = HibernateUtil.GetSession())
                    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                    using (var repositoryVenda = new Conexao<Venda>(session_connect))
                    using (var repositoryNoti = new Conexao<Notificacoes>(session_connect))
                    using (var apiNotifi = new NotificacoesApiController())
                    {

                        var vendas = repositoryVenda.select(p => p.Data_Alteracao_Status == DateTime.Now && p.Status == Model.enumerado.Status_Venda.Venda_Aprovada).Result;
                        var notificacao = repositoryNoti.selectFirst(p => p.Id == 30).Result;

                        if (vendas.Count > 0 && notificacao.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                        {
                            var notifica = apiNotifi.EnviaNotificacoesComercial(30, null, vendas.Count().ToString());
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
                CONSTANTES.log4Net.Error(ex);
            }

            return retorno;
        }


    }
}