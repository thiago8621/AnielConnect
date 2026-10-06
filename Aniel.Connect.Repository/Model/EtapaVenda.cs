using Aniel.Connect.Model.enumerado;

namespace Aniel.Connect.Repository.Model
{
    public class EtapaVendaViewModel
    {
        public int Id { get; set; }
        public int Etapa { get; set; }
        public string Descricao { get; set; }
        public int Status { get; set; }
        public string MotivoAprovacaoManual { get; set; }
    }
}
