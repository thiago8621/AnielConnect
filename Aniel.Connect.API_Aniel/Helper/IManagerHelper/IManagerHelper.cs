using Aniel.Connect.API_Aniel.Models.IManager;
using log4net;
using RestSharp;
using Sinapse.Core;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Aniel.Connect.API_Aniel.Helper.IManagerHelper
{
    public class IManagerHelper
    {
        public ConsultaCTOIndividualResponse GetCTO(Models.Config_Aniel_Integrator config, ConsultaCTOIndividualRequest objToPost)
        {
            try
            {                
                string endpoint = config.URL+ "/api/rede/consulta_redecto";                
              

                var requestStr = JsonService.SerializeObject(objToPost);
                var responseStr = this.Get(endpoint, config.Token, requestStr);

                LogManager.GetLogger("Aniel.Connect.API_Aniel.IManager").Error($"[IManager - GetCTO] - Json Request: {requestStr} \n  Json Response: {responseStr} {Environment.NewLine} {Environment.NewLine}");

                return JsonService.DeserializeObject<ConsultaCTOIndividualResponse>(responseStr);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na comunicação com o IManager: {ex.Message}");
            }
        }

        public ReservarPortaResponse ReservarCaixaPorta(Models.Config_Aniel_Integrator obj, ReservarPortaRequest objToPost)
        {
            var retorno = new ReservarPortaResponse();
            //TODO:A rota do endpoint atualmente é de homologação, posteriormente quando implementarem em produção teremos de alterar
            string endpoint = obj.URL + "/api/hml/rede/vincularcto";

            string responseApi = this.Post(endpoint, obj.Token, JsonService.SerializeObject(objToPost));
            retorno = JsonService.DeserializeObject<ReservarPortaResponse>(responseApi);
            LogManager.GetLogger("Aniel.Connect.API_Aniel.IManager").Error($"[IManager - ReservarCaixaPorta] - Json Request: {JsonService.SerializeObject(objToPost)} \n  Json Response: {responseApi} {Environment.NewLine} {Environment.NewLine}");
            return retorno;
        }

        public LimparPortaResponse LimparCaixaPorta(Models.Config_Aniel_Integrator obj, LimparPortaRequest objToPost)
        {
            var retorno = new LimparPortaResponse();
            //TODO:A rota do endpoint atualmente é de homologação, posteriormente quando implementarem em produção teremos de alterar
            string endpoint = obj.URL + "/api/hml/rede/desvincularcto";

            string responseApi = this.Post(endpoint, obj.Token, JsonService.SerializeObject(objToPost));
            retorno = JsonService.DeserializeObject<LimparPortaResponse>(responseApi);
            LogManager.GetLogger("Aniel.Connect.API_Aniel.IManager").Error($"[IManager - LimparCaixaPorta] - Json Request: {JsonService.SerializeObject(objToPost)} \n  Json Response: {responseApi} {Environment.NewLine} {Environment.NewLine}");
            return retorno;
        }

        #region RestApi Methods
        protected String Get(String pUrl, String pToken, String pJsonParam)
        {

            ServicePointManager.ServerCertificateValidationCallback +=
            (sender, certificate, chain, sslPolicyErrors) => true;

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Ssl3;

            //Consultas GET na API do IManager enviam Json no body
            //E a biblioteca RestRequest não possui suporte a requests assim, sendo necessário usar HttpWebRequest burlando essa diretriz de APIs Rest.
            var request = System.Net.WebRequest.Create(pUrl);
            request.ContentType = "application/json";
            request.Method = "GET";
            var httpWebRequest = (System.Net.HttpWebRequest)request;

            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.Headers.Add("IntegrationSecret", pToken);
            httpWebRequest.Headers.Add("Content-Type", "application/json");
            httpWebRequest.Headers.Add("cache-control", "no-cache");

            //Burlando a diretriz/flag que impede o método GET de ter um body
            var type = httpWebRequest.GetType();
            var currentMethod = type.GetProperty("CurrentMethod",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(httpWebRequest);
            var methodType = currentMethod.GetType();
            methodType.GetField("ContentBodyNotAllowed", System.Reflection.BindingFlags.NonPublic |
                     System.Reflection.BindingFlags.Instance).SetValue(currentMethod, false);            
                       
            string retorno = null;
            //Passando o body em Json
            using (var streamWriter = new
                    StreamWriter(httpWebRequest.GetRequestStream()))
            {

                streamWriter.Write(Newtonsoft.Json.JsonConvert.SerializeObject(pJsonParam));
            }
            using (var response = (System.Net.HttpWebResponse)httpWebRequest.GetResponse())
            using (var responseStream = response.GetResponseStream())
            {
                var streamReader = new StreamReader(responseStream, Encoding.Default);
                var retornoJson = streamReader.ReadToEnd();

                if(response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Erro HTTP {(int)response.StatusCode} - {retornoJson}");
                }

                retorno = retornoJson;
            }                

            return retorno;            
        }

        protected String Post(String pUrl, String pToken, String pJsonParam)
        {

            ServicePointManager.ServerCertificateValidationCallback +=
            (sender, certificate, chain, sslPolicyErrors) => true;

            var client = new RestClient(pUrl);
            client.Timeout = -1;

            var request = new RestRequest(Method.POST);
            request.AddHeader("IntegrationSecret", pToken);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", pJsonParam, ParameterType.RequestBody);
            request.AddHeader("cache-control", "no-cache");
           
            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Erro HTTP {(int)response.StatusCode} - {response.Content}");
            }

            return response.Content;          
        }
        #endregion
    }
}