namespace Aniel.Connect.Models
{
    public class Comercial_SMSModel
    {
        public bool NumeroValidado { get; set; }
        public string UrlLogo { get; set; }
        public string Empresa { get; set; }
        public string Mensagem { get; set; }
    }
    public class ValidaNumeroRequest
    {
        public long Cliente { get; set; }
        public string NumeroCelular { get; set; }
    }
}