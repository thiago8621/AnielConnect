namespace Aniel.Connect.API_Aniel.Models
{
    public class ArquivoOrdemServico
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public string Tipo { get; set; }
        public byte[] Arquivo { get; set; }
    }

    public class ArquivoOrdemServicoRequest : ApiRequest
    {
        public string NumeroOS {  get; set; }
        public string CodCt {  get; set; }
        public string Projeto {  get; set; }
        public string NumDoc {  get; set; }
    }
}