using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{

    public class PaymentMethod
    {
        public int id { get; set; }
        public string description { get; set; }
    }

    public class RetornoConsultaDescontos
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<PaymentMethod> payment_methods { get; set; }
    }

    public class PendenciaEnderecoListModel
    {
        public string Cod_Cliente { get; set; }
        public string Cod_Contrato { get; set; }
        public string CodConexao { get; set; }
        public string Cliente { get; set; }
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
        public string Contrato_Cancelado { get; set; }
        public string Conexao_Bloqueada { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string CodFatura { get; set; }
        public string Valor_Total { get; set; }
        public string Liquidado { get; set; }
        public string Descricao { get; set; }
        public string Fatura_Suspensa { get; set; }
        public string Data_Vencimento { get; set; }
        public string Motivo_Suspensao { get; set; }
        public bool FlagCEP { get; set; }
    }

    public class ExistePendenciaEnderecoResponse
    {
        public long? IdConsultaPendenciaEndereco { get; set; }
        public bool ExisteConsulta { get; set; }
        public IEnumerable<PendenciaEnderecoListModel> Dados { get; set; }
    }
}
