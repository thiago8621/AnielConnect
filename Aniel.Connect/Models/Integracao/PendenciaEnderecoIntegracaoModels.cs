using Aniel.Connect.Core;

namespace Aniel.Connect.Models.Integracao
{
    public class ConsultaPendenciaEnderecoRequest
    {
        public long? IdVenda { get; set; }
        public long? IdTroca { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Cidade { get; set; }
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
        public string CEP { get; set; }
    }

    public class ExecutarConsultaPendenciaEnderecoResponse
    {
        public bool PossuiPendencia { get; set; }
        public long IdConsultaPendenciaEndereco { get; set; }
        public System.DateTime? DataHoraCriacao { get; set; }
    }

    public class GetFaturaByEnderecoDataTableRequest : DataTableRequest
    {
        public string Logradouro { get; set; }
        public string Numero { get; set; }
    }

    public class GetFaturaEnderecoByVendaDataTableRequest : DataTableRequest
    {
        public long IdVenda { get; set; }
    }
}