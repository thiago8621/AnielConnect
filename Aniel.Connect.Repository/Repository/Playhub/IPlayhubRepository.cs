using Aniel.Connect.Repository.Model;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Playhub
{
    public interface IPlayhubRepository
    {
        /// <summary>
        ///     Retorna as informações de um cliente registrado no Playhub
        /// </summary>
        /// <param name="username">Identificador do cliente</param>
        PlayhubCustomer GetCustomer(string username);

        /// <summary>
        ///     Registra um novo cliente no Playhub
        /// </summary>
        /// <param name="customer">Objeto com os dados do cliente</param>
        void CreateCustomer(PlayhubCustomer customer);

        /// <summary>
        ///     Atualiza os dados de um cliente já existente no Playhub
        /// </summary>
        /// <param name="username">Identificador do cliente</param>
        /// <param name="customer">Objeto com os dados do cliente</param>
        void UpdateCustomer(string username, PlayhubCustomer customer);

        /// <summary>
        ///     Retorna as assinaturas vinculadas ao cliente no Playhub
        /// </summary>
        /// <param name="username">Identificador do cliente que terá suas assinaturas listadas</param>
        IEnumerable<PlayhubSubscription> GetSubscriptions(string username);

        /// <summary>
        ///     Registra uma assinatura para determinado cliente no Playhub
        /// </summary>
        /// <param name="username">Identificador do cliente ao qual a assinatura será vinculada</param>
        /// <param name="subscription">Objeto com os dados da assinatura</param>
        void CreateSubscription(string username, PlayhubSubscriptionCreation subscription);

        /// <summary>
        ///     Inativa uma assinatura para determinado cliente no Playhub
        /// </summary>
        /// <param name="username">Identificador do cliente</param>
        /// <param name="productId">Identificador da assinatura que será inativada</param>
        void DisableSubscription(string username, string productId);
    }
}
