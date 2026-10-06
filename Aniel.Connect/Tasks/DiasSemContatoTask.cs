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
    public class DiasSemContatoTask : BackgroundTask
    {
        public GetApiRetorno<string> DiasSemContato()
        {

            GetApiRetorno<string> retorno = new GetApiRetorno<string>();
            try
            {
                try
                {
                    using (ISession session_connect = HibernateUtil.GetSession())
                    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                    using (var repoCliente = new Conexao<Cliente>(session_connect))
                    using (var repoVenda = new Conexao<Venda>(session_connect))
                    using (var repoConfig = new Conexao<Configuracoes_Gerais>(session_connect))
                    using (var repositoryNoti = new Conexao<Notificacoes>(session_connect))
                    using (var apiNotifi = new NotificacoesApiController())
                    {
                        var config = repoConfig.selectFirst().Result;
                        int diasSemContato = Convert.ToInt32(config.Max_Dias_Sem_Contato);
                        var data = DateTime.Now.AddDays(-diasSemContato);

                        var dataInicio = new DateTime(data.Year, data.Month, data.Day, 0, 0, 0);
                        var dataFim = new DateTime(data.Year, data.Month, data.Day, 23, 59, 59);

                        var contatoClientes = repoCliente.select(p => p.Dt_Ultimo_Contato >= dataInicio && p.Dt_Ultimo_Contato <= dataFim).Result;
                        var notificacao = repositoryNoti.selectFirst(p => p.Id == 36).Result;

                        if (contatoClientes.Count > 0 && notificacao.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                        {
                            foreach (var cliente in contatoClientes)
                            {
                                var vendas = repoVenda.select(p => p.Id_Cliente == cliente.Id && p.Status == Model.enumerado.Status_Venda.Analise_Cliente).Result;
                                foreach (var venda in vendas)
                                {
                                    var notifica = apiNotifi.EnviaNotificacoesComercial(36, venda, cliente.NomeFantasia);
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
                CONSTANTES.log4Net.Error(ex);
            }

            return retorno;
        }


    }
}