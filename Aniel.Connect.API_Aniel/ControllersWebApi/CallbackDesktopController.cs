using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using DocumentFormat.OpenXml.Spreadsheet;
using FirebirdSql.Data.FirebirdClient;
using NHibernate;
using OpenXmlPowerTools;
using Org.BouncyCastle.Asn1.Ocsp;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class CallbackDesktopController : BaseApiController
    {
        [HttpPost]
        [Route("api/provision/callback/{correlation_id}")]
        public async Task<Response> DesktopCallback(string correlation_id, [FromBody] CallbackDesktopModels data)
        {
            var response = new Response();
            try
            {

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(data.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (ISession session = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                {

                    var retorno = new CallbackDesktopModels
                    {
                        success = data.success,
                        error_message = data.error_message,
                        pu_id = data.pu_id,
                        old_pu_id = data.old_pu_id,
                        pppoe_user = data.pppoe_user,
                        old_pppoe_user = data.old_pppoe_user,
                        sac_login = data.sac_login,
                        sac_password = data.sac_password,
                        wifi_ssid = data.wifi_ssid,
                        wifi_password = data.wifi_password,
                        certificated = data.certificated,
                        message = data.message,
                        onu_signal = data.onu_signal,
                        cto_avg_sinal = data.cto_avg_sinal,
                        pppoe_connected = data.pppoe_connected
                    };




                    StringBuilder queryBuilder = new StringBuilder();
                    queryBuilder.Append("INSERT INTO TB_DESKTOP_INTEGRACAO (ID, CORRELATION_ID, SUCCESS, ERROR_MESSAGE, " +
                                        "PU_ID, OLD_PU_ID, PPPOE_USER, OLD_PPPOE_USER, SAC_LOGIN, SAC_PASSWORD, WIFI_SSID, " +
                                        "WIFI_PASSWORD, CERTIFICATED, MESSAGE, ONU_SIGNAL, CTO_AVG_SINAL, PPPOE_CONNECTED) ");
                    queryBuilder.Append("VALUES ((SELECT GEN_ID(GN_DESKTOP_INTEGRACAO, 1) FROM RDB$DATABASE), @correlation_id, @success, @error_message, @pu_id, " +
                                        "@old_pu_id, @pppoe_user, @old_pppoe_user, @sac_login, " +
                                        "@sac_password, @wifi_ssid, @wifi_password, @certificated, " +
                                        "@message, @onu_signal, @cto_avg_sinal, @pppoe_connected)");

                    com.CommandText = queryBuilder.ToString();

                    com.Parameters.Add(new FbParameter("@correlation_id", correlation_id));
                    com.Parameters.Add(new FbParameter("@success", retorno.success));
                    com.Parameters.Add(new FbParameter("@error_message", retorno.error_message));
                    com.Parameters.Add(new FbParameter("@pu_id", retorno.pu_id));
                    com.Parameters.Add(new FbParameter("@old_pu_id", retorno.old_pu_id));
                    com.Parameters.Add(new FbParameter("@pppoe_user", retorno.pppoe_user));
                    com.Parameters.Add(new FbParameter("@old_pppoe_user", retorno.old_pppoe_user));
                    com.Parameters.Add(new FbParameter("@sac_login", retorno.sac_login));
                    com.Parameters.Add(new FbParameter("@sac_password", retorno.sac_password));
                    com.Parameters.Add(new FbParameter("@wifi_ssid", retorno.wifi_ssid));
                    com.Parameters.Add(new FbParameter("@wifi_password", retorno.wifi_password));
                    com.Parameters.Add(new FbParameter("@certificated", retorno.certificated));
                    com.Parameters.Add(new FbParameter("@message", retorno.message));
                    com.Parameters.Add(new FbParameter("@onu_signal", retorno.onu_signal));
                    com.Parameters.Add(new FbParameter("@cto_avg_sinal", retorno.cto_avg_sinal));
                    com.Parameters.Add(new FbParameter("@pppoe_connected", retorno.pppoe_connected));

                    com.ExecuteNonQuery();



                    response.result = true;
                    response.message = "Dados inseridos com sucesso!";


                }




            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.result = false;
            }

            return response;

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        [Route("api/provision/getAllCallbackData/")]
        public async Task<GetApiRetorno<List<CallbackDesktopModels>>> getCallbackByCorrelationId()
        {
            GetApiRetorno<List<CallbackDesktopModels>> retorno = new GetApiRetorno<List<CallbackDesktopModels>>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder queryBuilder = new StringBuilder();
                    queryBuilder.Append($"SELECT * FROM TB_DESKTOP_INTEGRACAO");

                    com.CommandText = queryBuilder.ToString();


                    using (var reader = await com.ExecuteReaderAsync())
                    {

                        if (reader.HasRows)
                        {

                            var dataList = new List<CallbackDesktopModels>();

                            while (await reader.ReadAsync())
                            {

                                dataList.Add(new CallbackDesktopModels
                                {  
                                    correlation_id = Convert.ToString(reader["CORRELATION_ID"]),
                                    success = Convert.ToInt16(reader["SUCCESS"]) != 0,
                                    error_message = Convert.ToString(reader["ERROR_MESSAGE"]),
                                    pu_id = Convert.ToInt32(reader["PU_ID"]),
                                    old_pu_id = Convert.ToInt64(reader["OLD_PU_ID"]),
                                    pppoe_user = Convert.ToString(reader["PPPOE_USER"]),
                                    old_pppoe_user = Convert.ToString(reader["OLD_PPPOE_USER"]),
                                    sac_login = Convert.ToInt32(reader["SAC_LOGIN"]),
                                    sac_password = Convert.ToString(reader["SAC_PASSWORD"]),
                                    wifi_ssid = Convert.ToString(reader["WIFI_SSID"]),
                                    wifi_password = Convert.ToString(reader["WIFI_PASSWORD"]),
                                    certificated = Convert.ToInt16(reader["CERTIFICATED"]) != 0,
                                    message = Convert.ToString(reader["MESSAGE"]),
                                    onu_signal = Convert.ToDouble(reader["ONU_SIGNAL"]),
                                    cto_avg_sinal = Convert.ToDouble(reader["CTO_AVG_SINAL"]),
                                    pppoe_connected = Convert.ToInt16(reader["PPPOE_CONNECTED"]) != 0
                                });


                                
                            };

                            retorno.Objeto = dataList;
                            retorno.Ok = true;


                        }
                        else
                        {

                            retorno.Mensagem = "No data found in the TB_DESKTOP_INTEGRACAO table.";
                            retorno.Ok = false;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }
    }
}