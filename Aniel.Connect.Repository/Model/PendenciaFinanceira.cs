using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    public class PendenciaFinanceiraBaseModel
    {
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
    }

    public class PendenciaFinanceiraListModel
    {
        public string Cod_Cliente { get; set; }
        public string Cod_Contrato { get; set; }
        public string CodConexao { get; set; }
        public string Cliente { get; set; }
        public string CpfCnpj { get; set; }
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
    }

    public class PendenciaFinanceiraListModelMK : PendenciaFinanceiraBaseModel
    {
        public string Cod_Cliente { get; set; }
        public string Cod_Contrato { get; set; }
        public string CodConexao { get; set; }
        public string Cliente { get; set; }
        public string CpfCnpj { get; set; }
        public string Contrato_Cancelado { get; set; }
        public string Conexao_Bloqueada { get; set; }   
        public string CodFatura { get; set; }
        public string Valor_Total { get; set; }
        public string Liquidado { get; set; }
        public string Descricao { get; set; }
        public string Fatura_Suspensa { get; set; }
        public string Data_Vencimento { get; set; }
        public string Motivo_Suspensao { get; set; }
    }

    public class PendenciaFinanceiraListModelVoalle : PendenciaFinanceiraBaseModel 
    {

    }

    public class ExistePendenciaFinanceiraResponse
    {
        public long? IdConsultaPendenciaFinanceira { get; set; }
        public bool ExisteConsulta { get; set; }
        public IEnumerable<PendenciaFinanceiraListModel> Dados { get; set; } 
    }
}
