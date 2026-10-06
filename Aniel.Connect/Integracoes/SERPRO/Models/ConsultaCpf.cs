namespace Aniel.Connect.Integracoes.SERPRO.Models
{
    public class ConsultaCpf
    {
        public string ni { get; set; }
        public string nome { get; set; }
        public Situacao situacao { get; set; }
        public string nascimento { get; set; }
        public string obito { get; set; }
    }
}