namespace Aniel.Connect.Integracoes.SERPRO.Models
{
    public class EnderecoConsulta
    {
        public string tipoLogradouro { get; set; }
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string cep { get; set; }
        public string bairro { get; set; }
        public MunicipioConsulta municipio { get; set; }
        public string uf { get; set; }
        public PaisConsulta pais { get; set; }
    }
}