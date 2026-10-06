namespace Aniel.Connect.Repository.ConfigUrl
{
    public interface IConfigUrlRepository
    {
        /// <summary>
        /// Busca na tabela de URLS pela url do connect
        /// </summary>
        string GetUrlConnect();

        /// <summary>
        /// Busca na tabela de URLS pela url do PREMIER
        /// </summary>
        string GetUrlPremier();

        /// <summary>
        /// Busca na tabela de URLS pela url do API_ANIEL
        /// </summary>
        string GetUrlApiAniel();

        /// <summary>
        /// Busca na tabela de URLS pela url do FINANCEIRO
        /// </summary>
        string GetUrlApiFinanceiro();
        string GetUrlIntegra();
        string GetUrlEVA();

        /// <summary>
        /// Busca na tabela de URLS pela url do OSRM
        /// </summary>
        string GetUrlOSRM();
    }
}
