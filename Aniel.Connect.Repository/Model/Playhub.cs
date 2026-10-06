namespace Aniel.Connect.Repository.Model
{
    /// <summary>
    ///     Dados do cliente formatado para a API de integração com o Playhub
    /// </summary>
    public class PlayhubCustomer
    {
        /// <summary>
        ///     Campo obrigatório para inserção
        /// </summary>
        public string Username { get; set; }
        public string Document { get; set; }
        
        /// <summary>
        ///     Campo obrigatório para inserção
        /// </summary>
        public string Password { get; set; }
        
        /// <summary>
        ///     Campo obrigatório para inserção
        /// </summary>
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
    }

    /// <summary>
    ///     Dados de uma assinatura vinculada ao cliente no Playhub
    /// </summary>
    public class PlayhubSubscription
    {
        /// <summary>
        ///     Campo obrigatório para inserção
        /// </summary>
        public string ProductId { get; set; }
        public string Voucher { get; set; }
    }

    /// <summary>
    ///     Classe utilizada para registrar uma nova assinatura no Playhub
    /// </summary>
    public class PlayhubSubscriptionCreation
    {
        public string ProductId { get; set; }
    }

    /// <summary>
    ///     Dados exigidos pelo endpoint de autenticação da API do Playhub
    /// </summary>
    public class PlayhubAuthRequest
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }

    /// <summary>
    ///     Dados retornados pelo endpoint de autenticação da API do Playhub
    /// </summary>
    public class PlayhubAuthResponse
    {
        public string AccessToken { get; set; }
    }
}
