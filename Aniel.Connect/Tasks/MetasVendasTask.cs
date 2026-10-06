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
using Sinapse.Model;

namespace Aniel.Connect.Tasks
{
    public class MetasVendasTask : BackgroundTask
    {
        public async Task<GetApiRetorno<string>> MetaVendaAsync()
        {

            GetApiRetorno<string> retorno = new GetApiRetorno<string>();
            try
            {
                try
                {
                    using (ISession session_connect = HibernateUtil.GetSession())
                    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                    using (var repoVenda = new Conexao<Venda>(session_connect))
                    using (var repoVendedores = new Conexao<Funcionario>(session_connect))
                    using (var repositoryNoti = new Conexao<Notificacoes>(session_connect))
                    using (var apiNotifi = new NotificacoesApiController())
                    {
                        var vendedores = repoVendedores.select(p => p.Vendedor == true).Result;

                        foreach (var vendedor in vendedores)
                        {
                            using (ISession session = HibernateUtil.GetSession())
                            {
                                var ano = DateTime.Now.Year;
                                var diaConfig = new Configuracoes_GeraisApiController().FindById(1).Result.Objeto.Dia_Mes_Finaliza_Medicao_Comissao;

                                string dataInicio;
                                string dataFim;

                                if (diaConfig <= 30)
                                    dataInicio = (Convert.ToInt32(ano) - 1).ToString() + "-12-" + (Convert.ToInt32(diaConfig) + 1).ToString();
                                else
                                    dataInicio = ano + "-01-" + "01";

                                dataFim = ano + "-12-" + diaConfig;


                                DateTime dateTimeInicio = Convert.ToDateTime(dataInicio);
                                DateTime dateTimeFim = Convert.ToDateTime(dataFim);
                                Int64 Id_Funcionario = 0;
                                Double vendas;
                                Double meta = 0;

                                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                                //Select
                                using (var com = session.Connection.CreateCommand())
                                {

                                    var mes = DateTime.Now.ToString("MMMM").ToUpper();

                                    if (mes == "MARÇO")
                                    {
                                        string mesTratado = mes.Replace(mes, "MARCO");
                                        mes = mesTratado;
                                    }

                                    StringBuilder sql = new StringBuilder(@$" SELECT C.ID AS FUNCIONARIO_ID, COALESCE(B.{mes}, F.{mes}) AS META FROM ASPNETUSERS A
                                                                LEFT JOIN TB_FUNCIONARIO C
                                                                  ON (C.ID = A.ID_FUNCIONARIO)                    
                                                                LEFT JOIN TB_FUNCIONARIO E
                                                                  ON (C.ID_SUPERVISOR = E.ID)
                                                                LEFT JOIN TB_META B
                                                                  ON (C.id_meta = B.ID)
                                                                LEFT JOIN TB_META F
                                                                  ON (E.ID_META = F.ID)
                                                                WHERE A.id = @USUARIO AND COALESCE(B.ANO,F.ANO) = '{DateTime.Now.Year}'
                                                                GROUP BY B.{mes} ,F.{mes} , FUNCIONARIO_ID
                                                                ORDER BY B.{mes}, F.{mes}");



                                    parametros.Add("@USUARIO", vendedor.Num_Fre);

                                    var totalVendasDias = repoVenda.select(p => p.Data_Venda == DateTime.Now.AddDays(-1) && p.Vendedor.Id == vendedor.Id && p.Status == Model.enumerado.Status_Venda.Venda_Aprovada).Result.Count;

                                    //Busca com paginacao
                                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                                    {
                                        while (await reader.ReadAsync())
                                        {
                                            meta = reader["META"] != DBNull.Value ? Convert.ToDouble(reader["META"]) : 0;
                                            Id_Funcionario = Convert.ToInt64(reader["FUNCIONARIO_ID"]);

                                            int diasMes = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                                            var metaDiaria = meta / diasMes;

                                            //Meta Não Cumprida
                                            if(totalVendasDias == 0)
                                            {
                                                var notificacao = repositoryNoti.selectFirst(p => p.Id == 32).Result;

                                                if (notificacao.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                                                {
                                                    var notifica = apiNotifi.EnviaNotificacoesComercial(31, null, vendedor.Nome);
                                                }
                                            }
                                            else if(totalVendasDias >= metaDiaria)
                                            {
                                                var notificacao = repositoryNoti.selectFirst(p => p.Id == 33).Result;
                                                if (notificacao.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                                                {
                                                    var notifica = apiNotifi.EnviaNotificacoesComercial(33, null, vendedor.Nome);
                                                }
                                            }
                                        }
                                    }
                                    retorno.Ok = true;

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