using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Playhub;
using Sinapse.Util;
using System;

namespace Aniel.Connect.ConnectBusiness.Playhub
{
    public class PlayhubBusiness
    {
        private readonly IPlayhubRepository _repository;

        public PlayhubBusiness(IPlayhubRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        ///     Registra um novo cliente no Playhub
        /// </summary>
        /// <param name="cliente">Objeto com os dados do cliente do Connect</param>
        /// <returns>Nome do usuário do cliente no Playhub</returns>
        public string CadastrarCliente(Cliente cliente)
        {
            string documento = cliente.CpfCnpj
                                      .Replace(".", String.Empty)
                                      .Replace("-", String.Empty)
                                      .Replace("/", String.Empty);

            string celular = cliente.Telefone_Celular
                                    .Replace("(", String.Empty)
                                    .Replace(")", String.Empty)
                                    .Replace("-", String.Empty)
                                    .Replace(" ", String.Empty);

            var dadosClientePlanyhub = new PlayhubCustomer
            {
                Name = cliente.NomeFantasia,
                Document = documento,

                Username = StringService.GetRandomString(12),
                Password = StringService.GetRandomString(12),
                
                Email = cliente.Email_Principal,
                Mobile = celular
            };

            this._repository.CreateCustomer(dadosClientePlanyhub);

            return dadosClientePlanyhub.Username;
        }

        /// <summary>
        ///     Atualiza os dados de um cliente já existente no Playhub
        /// </summary>
        /// <param name="username">Identificador do cliente</param>
        /// <param name="cliente">Objeto com os dados do cliente no Connect</param>
        public void AtualizarCadastroCliente(string username, Cliente cliente)
        {
            string celular = cliente.Telefone_Celular
                                    .Replace("(", String.Empty)
                                    .Replace(")", String.Empty)
                                    .Replace("-", String.Empty)
                                    .Replace(" ", String.Empty);

            var dadosClientePlayhub = new PlayhubCustomer
            {
                Name = cliente.NomeFantasia,
                Email = cliente.Email_Principal,
                Mobile = celular
            };

            this._repository.UpdateCustomer(username, dadosClientePlayhub);
        }

        /// <summary>
        ///     Registra uma assinatura para determinado cliente no Playhub
        /// </summary>
        /// <param name="username">Usuário do cliente no playhub ao qual o plano será vinculado</param>
        /// <param name="productId">ID do plano no playhub</param>
        public void AdicionarPlanoUsuario(string username, string productId)
        {
            var dadosPlanoPlayhub = new PlayhubSubscriptionCreation
            {
                ProductId = productId
            };

            this._repository.CreateSubscription(username, dadosPlanoPlayhub);
        }

        /// <summary>
        ///     Remove uma assinatura para determinado cliente no Playhub
        /// </summary>
        /// <param name="username">Usuário do cliente no playhub</param>
        /// <param name="productId">ID do plano no playhub</param>
        public void RemoverPlanoUsuario(string username, string productId)
        {
            this._repository.DisableSubscription(username, productId);
        }
    }
}
