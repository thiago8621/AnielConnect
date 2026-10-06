using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aniel.Connect.API_Aniel.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Sinapse.DAO.repository;
using Sinapse.Model;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using NHibernate;
using System.Net.Http;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Aniel.Connect.API_Aniel.Models;
using System.Web.Http;




namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class IntegracaoConsultaLoginMKApiController : BaseApiController
    {
        
        [HttpPost()]
        [Route("api/IntegracaoConsultaLoginMKApi/VerificaClienteAutenticado")]
        public async Task<GetApiRetorno<RetornoAPIMKModels>> VerificaClienteAutenticado(string loginPppoe, RetornoAPIMKModels request)
        {
            GetApiRetorno<RetornoAPIMKModels> response = new GetApiRetorno<RetornoAPIMKModels>();

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (ISession sessionNh = HibernateUtil.GetSession())
                using (var repoIntegracoes = new Conexao<IntegracoesExternas>(sessionNh))
                {
                    var configsIntegracao = repoIntegracoes.select(p => p.TipoIntegracao == EnumTipoIntegracao.PROVISIONAMENTO).Result.ToList();
                    //Fluxo integrações
                    if(configsIntegracao != null && configsIntegracao.Count() > 0)
                    {
                        switch(configsIntegracao.FirstOrDefault().IntegracaoExterna)
                        {
                            case IntegracaoExterna.MK_SOLUTION:
                                {
                                    var configIntegracaoMK = configsIntegracao.FirstOrDefault();
                                    string token = configIntegracaoMK.TokenAcesso;
                                    string url = configIntegracaoMK.UrlApi;

                                    string urlCompleta = $"{url}/autenticacao?username={loginPppoe}";

                                    using (HttpClient client = new HttpClient())
                                    {

                                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                        HttpResponseMessage httpResponse = await client.GetAsync(urlCompleta);

                                        if (httpResponse.IsSuccessStatusCode)
                                        {

                                            string responseData = await httpResponse.Content.ReadAsStringAsync();
                                            response = JsonConvert.DeserializeObject<GetApiRetorno<RetornoAPIMKModels>>(responseData);

                                            /*if (response.Objeto?.Result == null)
                                            {
                                                response.Objeto.Result = new List<object>();
                                            } */

                                            response.Ok = true;
                                        }
                                        else
                                        {
                                            response.Ok = false;
                                            response.Mensagem = $"Error: {httpResponse.ReasonPhrase}";
                                        }
                                    }
                                }
                                break;                           
                            default:
                                throw new NotImplementedException("Integração de provisionamento não implementada");                                
                        }
                    }
                    else //Fluxo AnielRadius
                    {
                        //TODO: 
                        throw new NotImplementedException("Integração de provisionamento não implementada");                        
                    }

                    /*IntegracoesExternas configIntegracaoMK = repoIntegracoes.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.MK_SOLUTION && p.TipoIntegracao != EnumTipoIntegracao.PROVISIONAMENTO).Result;

                    if (configIntegracaoMK == null)
                    {
                        response.Ok = false;
                        response.Mensagem = "Nenhuma integração MK para provisionamento cadastrada";
                        return response;
                    } 

                    string token = configIntegracaoMK.TokenAcesso;
                    string url = configIntegracaoMK.UrlApi;*/
                  

                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"Exception: {ex.Message}";
            }

            return response;

        }
       
    }

}