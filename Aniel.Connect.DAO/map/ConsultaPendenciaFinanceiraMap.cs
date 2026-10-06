using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class ConsultaPendenciaFinanceiraMap : ClassMapBase<ConsultaPendenciaFinanceira>
    {
        public ConsultaPendenciaFinanceiraMap() : base()
        {
            Table("TB_CONSULTA_PENDENCIA_FIN");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PENDENCIA_FINANCEIRA_MK");
            Id(u => u.Cod_Cliente, "COD_CLIENTE");
            Id(u => u.Cod_Contrato, "COD_CONTRATO");
            Id(u => u.CodConexao, "COD_CONEXAO");
            Id(u => u.Cliente, "CLIENTE");
            Id(u => u.Cpf, "CPF");
            Id(u => u.Cnpj, "CNPJ");
            Id(u => u.Contrato_Cancelado, "CONTRATO_CANCELADO");
            Id(u => u.Conexao_Bloqueada, "CONEXAO_BLOQUEADA");
            Id(u => u.Logradouro, "LOGRADOURO");
            Id(u => u.Numero, "NUMERO");
            Id(u => u.Bairro, "BAIRRO");
            Id(u => u.Cidade, "CIDADE");
            Id(u => u.CodFatura, "COD_FATURA");
            Id(u => u.Valor_Total, "VALOR_TOTAL");
            Id(u => u.Liquidado, "LIQUIDADO");
            Id(u => u.Descricao, "DESCRICAO");
            Id(u => u.Fatura_Suspensa, "FATURA_SUSPENSA");
            Id(u => u.Data_Vencimento, "VENCIMENTO");
            Id(u => u.Motivo_Suspensao, "MOTIVO_SUSPENSAO");                   
        }
    }
}
