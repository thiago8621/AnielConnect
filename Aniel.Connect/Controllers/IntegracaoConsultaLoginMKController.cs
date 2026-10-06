using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Sinapse.DAO.repository;
using Sinapse.Model;
using System.Web.Mvc;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using NHibernate;
using System.Net.Http;
using Newtonsoft.Json;


namespace Aniel.Connect.Controllers
{ /*
    public class IntegracaoConsultaLoginMKController
    {

        public class RetornoAPIMK
        {
            public bool Success { get; set; }
            public int Length { get; set; }
            public string Message { get; set; }
            public List<object> Result { get; set; }
        }

        public async Task<RetornoAPIMK> VerificaClienteAutenticado(string loginPppoe)
        {
            RetornoAPIMK response = new RetornoAPIMK();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (var repoIntegracoes = new Conexao<IntegracoesExternas>(sessionNh))
                {
                    IntegracoesExternas configIntegracaoMK = repoIntegracoes.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.MK_SOLUTION).Result;
                    string token = configIntegracaoMK.TokenAcesso;
                    string url = configIntegracaoMK.UrlApi;

                    string urlCompleta = $"{url}{loginPppoe}";

                    using (HttpClient client = new HttpClient())
                    {

                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage httpResponse = await client.GetAsync(urlCompleta);

                        if (httpResponse.IsSuccessStatusCode)
                        {
                           
                            string responseData = await httpResponse.Content.ReadAsStringAsync();
                            response = JsonConvert.DeserializeObject<RetornoAPIMK>(responseData);

                            if (response.Result == null)
                            {
                                response.Result = new List<object>();
                            }
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = $"Error: {httpResponse.ReasonPhrase}";
                        }
                    }

                }                    
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Exception: {ex.Message}";
            } 
            
            return response;

        }
    } */
}