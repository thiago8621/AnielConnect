using Aniel.Connect.Core;

namespace Aniel.Connect.Models.Integracao
{
    public class ConsultaPendenciaFinanceiraRequest
    {
        public long? IdVenda { get; set; }
        public long? IdTrocaTitularidade { get; set; }
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
    }

    public class ExecutarConsultaPendenciaFinanceiraResponse
    {
        public bool PossuiPendencia { get; set; }
        public long IdConsultaPendenciaFinanceira { get; set; }
    }

    public class GetFaturaByCpfCnpjDataTableRequest : DataTableRequest
    {
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
    }

    public class GetFaturaByVendaDataTableRequest : DataTableRequest
    {
        public long IdVenda { get; set; }
    }
}