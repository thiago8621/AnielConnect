namespace Aniel.Connect.Models
{
    public class ClienteDocContratoModel
    {
        public long IdCliente { get; set; }
        public string IdModeloSelecionado { get; set; }
        public string NumeroContrato { get; set; }
        public bool Aditivo { get; set; }
        public long IdCliContrato { get; set; }
        public long IdClienteAnexo { get; set; }
    }
}


