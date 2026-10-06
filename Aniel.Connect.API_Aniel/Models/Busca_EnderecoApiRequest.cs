using System;

namespace Aniel.Connect.API_Aniel.Models
{
    public class Busca_EnderecoApiRequest: ApiRequest
    {
        public virtual String Cod_Logradouro { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual String Cod_Bairro { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Cod_Cidade { get; set; }
        public virtual String Cidade { get; set; }
    }

    public class Pendencia_EnderecoApiRequest : ApiRequest
    {
        public virtual String Cod_Cliente { get; set; }
        public virtual String Cod_Contrato { get; set; }
        public virtual String CodConexao { get; set; }
        public virtual String Cliente { get; set; }
        public virtual String Cpf { get; set; }
        public virtual String Contrato_Cancelado { get; set; }
        public virtual String Conexao_Bloqueada { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual String Numero { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String CodFatura { get; set; }
        public virtual String Valor_Total { get; set; }
        public virtual String Liquidado { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Fatura_Suspensa { get; set; }
        public virtual String Data_Vencimento { get; set; }
        public virtual String Cd_Motivo_Suspensao { get; set; }
        public virtual String Descricao_1 { get; set; }
    }
}