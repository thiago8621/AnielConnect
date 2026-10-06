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
using System.Net.Http;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ControllersWebApi;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Data.Common;
using System.Globalization;
using NHibernate.Hql.Ast;

namespace Aniel.Connect.Tasks
{
    public class AtualizarPortaCaixaVendaTask : BackgroundTask
    {

        public async Task<GetApiRetorno<Venda>> AtualizarPortaCaixaVenda(List<VendaListViewModel> vendaList)
        {
            GetApiRetorno<Venda> retorno = new GetApiRetorno<Venda>();

            try
            {
                using (ISession session_connect = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())     
                 
                {
                    VendaApiController vendaApiController = new VendaApiController();
                    var vendas = await BuscaFiltroConsultaVenda();



                    foreach (var venda in vendas.Objeto)
                    {
                        Int64 id = venda.Id;
                        Int64 idVendedor = venda.IdVendedor;
                        Int64 idCliente = venda.Id_Cliente;
                        string status = venda.Status;
                        string DataEtapa = venda.DataMudEtapa;

                        var resultado = vendaApiController.getPendenciasPorta(id, idVendedor, idCliente, status, DataEtapa);

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

        public async Task<GetApiRetorno<List<VendaListViewModel>>> BuscaFiltroConsultaVenda()
        {
            GetApiRetorno<List<VendaListViewModel>> retorno = new GetApiRetorno<List<VendaListViewModel>>
            {
                Objeto = new List<VendaListViewModel>()
            };
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    //Select
                    using (DbCommand com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        List<string> filtros = new List<string>();



                        sql.AppendLine("SELECT A.ID,A.ID_VENDEDOR,A.ID_CLIENTE ,A.STATUS,A.DATA_ALTERACAO_STATUS  ");
                        sql.AppendLine(" FROM TB_VENDA A ");

                        sql.AppendLine("INNER JOIN TB_ETAPA_VENDA B  ");
                        sql.AppendLine("ON B.ID_VENDA =A.ID  ");


                        sql.AppendLine("WHERE A.STATUS <>6 AND A.STATUS <>7 AND A.STATUS <>4 AND DATA_ALTERACAO_STATUS IS NOT NULL  ");
                        sql.AppendLine("GROUP BY A.ID,A.ID_VENDEDOR,A.ID_CLIENTE ,A.STATUS,A.DATA_ALTERACAO_STATUS  ");


                        using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new VendaListViewModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    IdVendedor = Convert.ToInt64(reader["ID_VENDEDOR"]),
                                    Id_Cliente = Convert.ToInt64(reader["ID_CLIENTE"]),
                                    DataMudEtapa = Convert.ToString(reader["DATA_ALTERACAO_STATUS"]),

                                    Status = Convert.ToString(reader["STATUS"])
                                });
                            }

                            retorno.Ok = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }

            return retorno;
        }




    }



}