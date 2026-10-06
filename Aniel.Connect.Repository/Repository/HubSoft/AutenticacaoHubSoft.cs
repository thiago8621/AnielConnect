using RestSharp;
using Aniel.Connect.Repository.Model.Hubsoft;
using System;

namespace Aniel.Connect.Repository.Repository.HubSoft
{
    public class AutenticacaoHubSoft
    {
        public static string Token;
        public static DateTime Expiracao;

        public static string Login(string urlBase, LoginHubsoftModel login)
        {
            if (string.IsNullOrEmpty(Token) || Expiracao < DateTime.Now)
            {
                var client = new RestClient(urlBase);
                var request = new RestRequest("/oauth/token", Method.POST);

                login.grant_type = "password";
                request.AddJsonBody(login);

                request.AddHeader("Content-Type", "application/json");
                var response = client.Execute<LoginResponseHubSoft>(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception($"Erro de autenticação : {response.Content}");

                Token = response.Data.access_token;
                Expiracao = DateTime.Now.AddHours(23);
            }

            return Token;
        }
    }
}