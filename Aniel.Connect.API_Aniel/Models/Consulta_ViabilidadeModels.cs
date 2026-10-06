namespace Aniel.Connect.API_Aniel.Models
{
    public class Consulta_ViabilidadeRequestModel : ApiRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long? Venda { get; set; }
        public long? Cliente { get; set; }
        public long? Cliente_PlanoProduto { get; set; }
    }
}