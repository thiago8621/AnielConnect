using Aniel.Connect.Repository.Exceptions;
using Aniel.Connect.Repository.Model;
using RestSharp;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Net;

namespace Aniel.Connect.Repository.Playhub
{
    /// <summary>
    ///     Classe para realizar requisições para a API do Playhub
    /// </summary>
    public class PlayhubApiRepository : IPlayhubRepository
    {
        /// <summary>
        ///     Client HTTP com a URL base da API do Playhub
        /// </summary>
        private readonly RestClient _httpClient;

        private readonly string _apiKey;
        private readonly string _apiSecret;

        private string _jwtToken;

        private string JwtToken
        {
            get
            {
                if (String.IsNullOrEmpty(this._jwtToken))
                    this._jwtToken = this.GetAccessToken();

                return this._jwtToken;
            }
        }

        /// <param name="apiKey">Chave necessária para autenticação das requisições</param>
        /// <param name="apiSecret">Secret necessário para autenticação das requisições</param>
        public PlayhubApiRepository(string apiKey, string apiSecret)
        {
            if (String.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException("apiKey");

            if (String.IsNullOrEmpty(apiSecret))
                throw new ArgumentNullException("apiSecret");

            this._httpClient = new RestClient("https://www.playhub.com.br/API/PlayhubApi/api/v3");
            this._apiKey = apiKey;
            this._apiSecret = apiSecret;
        }

        /// <summary>
        ///     Acessa o endpoint de autenticação da API do Playhub para oter um novo token de acesso
        /// </summary>
        private string GetAccessToken()
        {
            var request = new RestRequest("authentication/tokens", Method.POST);

            request.AddHeader("Accept", "application/json");

            var authRequest = new PlayhubAuthRequest
            {
                ApiKey = this._apiKey,
                ApiSecret = this._apiSecret
            };

            request.AddJsonBody(authRequest);

            IRestResponse<PlayhubAuthResponse> response = this._httpClient.Execute<PlayhubAuthResponse>(request);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new PlayhubRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new PlayhubAuthenticationException();
            }

            PlayhubAuthResponse responseBody = JsonService.DeserializeObject<PlayhubAuthResponse>(response.Content);

            return responseBody?.AccessToken;
        }

        /// <summary>
        ///     Limpa o token de autenticação anterior para que ele possa ser renovado
        /// </summary>
        private void ResetAccessToken()
        {
            this._jwtToken = "";
        }

        /// <summary>
        ///     Valida o objeto com os dados para criação do cliente no playhub jogando
        ///     uma exception quando um campo não está preenchido conforme o esperado
        /// </summary>
        /// <param name="customer">Objeto com os dados do cliente</param>
        /// <exception cref="PlayhubParamaterEmptyException"></exception>
        /// <exception cref="PlayhubStringParameterLengthException"></exception>
        private void ValidateCustomer(PlayhubCustomer customer)
        {
            if (String.IsNullOrEmpty(customer.Username))
            {
                throw new PlayhubParamaterEmptyException("username");
            }
            else if (customer.Username.Length < 8 || customer.Username.Length > 100)
            {
                throw new PlayhubStringParameterLengthException("username", 8, 100);
            }

            if (!String.IsNullOrEmpty(customer.Document) && customer.Document.Length > 14)
            {
                throw new PlayhubStringParameterLengthException("document", maxLength: 14);
            }

            if (String.IsNullOrEmpty(customer.Password))
            {
                throw new PlayhubParamaterEmptyException("password");
            }
            else if (customer.Password.Length < 6 || customer.Password.Length > 30)
            {
                throw new PlayhubStringParameterLengthException("password", 6, 30);
            }

            if (String.IsNullOrEmpty(customer.Name))
            {
                throw new PlayhubParamaterEmptyException("name");
            }
            else if (customer.Name.Length < 3 || customer.Name.Length > 200)
            {
                throw new PlayhubStringParameterLengthException("name", 3, 200);
            }

            if (!String.IsNullOrEmpty(customer.Mobile) && customer.Mobile.Length > 11)
            {
                throw new PlayhubStringParameterLengthException("mobile", maxLength: 11);
            }
        }

        public PlayhubCustomer GetCustomer(string username)
        {
            if (username == null)
                throw new ArgumentNullException("username");

            var request = new RestRequest("customers/" + username, Method.GET);

            request.AddHeader("Authorization", "Bearer " + this.JwtToken);
            request.AddHeader("Accept", "application/json");

            IRestResponse<PlayhubCustomer> response = this._httpClient.Execute<PlayhubCustomer>(request);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new PlayhubRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PlayhubCustomerNotFoundException(username);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new PlayhubAuthenticationException();
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                this.ResetAccessToken();
                this.GetCustomer(username);
            }

            return JsonService.DeserializeObject<PlayhubCustomer>(response.Content);
        }

        public void CreateCustomer(PlayhubCustomer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            this.ValidateCustomer(customer);

            var request = new RestRequest("customers", Method.POST);

            request.AddHeader("Authorization", "Bearer " + this.JwtToken);
            request.AddHeader("Accept", "application/json");

            request.AddJsonBody(customer);

            IRestResponse response = this._httpClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new PlayhubRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new PlayhubAuthenticationException();
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                this.ResetAccessToken();
                this.CreateCustomer(customer);
            }
        }

        public void UpdateCustomer(string username, PlayhubCustomer customer)
        {
            if (username == null)
                throw new ArgumentNullException("username");

            if (customer == null)
                throw new ArgumentNullException("customer");

            var request = new RestRequest("customers/" + username, Method.PUT);

            request.AddHeader("Authorization", "Bearer " + this.JwtToken);
            request.AddHeader("Accept", "application/json");

            request.AddJsonBody(customer);

            IRestResponse response = this._httpClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new PlayhubRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PlayhubCustomerNotFoundException(username);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new PlayhubAuthenticationException();
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                this.ResetAccessToken();
                this.UpdateCustomer(username, customer);
            }
        }

        public IEnumerable<PlayhubSubscription> GetSubscriptions(string username)
        {
            if (username == null)
                throw new ArgumentNullException("username");

            var request = new RestRequest("customers/" + username + "/subscriptions", Method.GET);

            request.AddHeader("Authorization", "Bearer " + this.JwtToken);
            request.AddHeader("Accept", "application/json");

            IRestResponse<IEnumerable<PlayhubSubscription>> response =
                this._httpClient.Execute<IEnumerable<PlayhubSubscription>>(request);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new PlayhubRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PlayhubSubscriptionNotFoundException(username);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new PlayhubAuthenticationException();
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                this.ResetAccessToken();
                this.GetSubscriptions(username);
            }

            return JsonService.DeserializeObject<List<PlayhubSubscription>>(response.Content);
        }

        public void CreateSubscription(string username, PlayhubSubscriptionCreation subscription)
        {
            if (username == null)
                throw new ArgumentNullException("username");

            if (subscription == null)
                throw new ArgumentNullException("subscription");

            var request = new RestRequest("customers/" + username + "/subscriptions", Method.POST);

            request.AddHeader("Authorization", "Bearer " + this.JwtToken);
            request.AddHeader("Accept", "application/json");

            request.AddJsonBody(subscription);

            IRestResponse response = this._httpClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new PlayhubCreateSubscriptionException();
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new PlayhubRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PlayhubCustomerNotFoundException(username);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new PlayhubAuthenticationException();
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                this.ResetAccessToken();
                this.CreateSubscription(username, subscription);
            }
        }

        public void DisableSubscription(string username, string productId)
        {
            if (username == null)
                throw new ArgumentNullException("username");

            if (productId == null)
                throw new ArgumentNullException("productId");

            var request = new RestRequest("customers/" + username + "/subscriptions/" + productId, Method.DELETE);

            request.AddHeader("Authorization", "Bearer " + this.JwtToken);
            request.AddHeader("Accept", "application/json");

            IRestResponse response = this._httpClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new PlayhubDisableSubscriptionException();
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new PlayhubRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PlayhubCustomerNotFoundException(username);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new PlayhubAuthenticationException();
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                this.ResetAccessToken();
                this.DisableSubscription(username, productId);
            }
        }
    }
}
