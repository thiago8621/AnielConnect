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
    public class RoteirizacaoEncaixeTask : BackgroundTask
    {
        public async Task<GetApiRetorno> RoteirizacaoEncaixe()
        {
            GetApiRetorno retorno = new GetApiRetorno();
            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("UPDATE TB_DOCUMENTO_PRODUCAO");
                    sql.AppendLine("SET DATA_DASHBOARD = CURRENT_DATE");
                    sql.AppendLine("WHERE CAST(DATA_MAXIMA AS DATE) = CAST(CURRENT_DATE AS DATE)");
                    sql.AppendLine("  AND EQUIPE IS NOT NULL");
                    sql.AppendLine("  AND TRIM(EQUIPE) <> '';");

                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                {
                    retorno.InnerException = ex.InnerException.Message;
                }

                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }
    }

}