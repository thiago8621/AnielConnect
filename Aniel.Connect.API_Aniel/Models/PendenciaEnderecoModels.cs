namespace Aniel.Connect.API_Aniel.Models
{
    /// <summary>
    /// Request de consultas pendência de faturas pelo endereço do banco MKSolutions 
    /// </summary>
    public class PendenciaEnderecoRequest : ApiRequest
    {
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Cidade { get; set; }
        public string CEP { get; set; }
        public long? IdVenda { get; set; }
    }
}