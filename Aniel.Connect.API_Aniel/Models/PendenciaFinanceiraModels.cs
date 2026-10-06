namespace Aniel.Connect.API_Aniel.Models
{
    public class PendenciaFinanceiraRequest : ApiRequest
    {
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
        public long? IdVenda { get; set; }
    }
}