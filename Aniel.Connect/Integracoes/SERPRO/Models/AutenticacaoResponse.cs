namespace Aniel.Connect.Integracoes.SERPRO.Models
{
    public class AutenticacaoResponse
    {
        public string scope { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string access_token { get; set; }
    }
}