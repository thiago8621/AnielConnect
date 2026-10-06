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
    public class ConsultaPendenciaEnderecoMap : ClassMapBase<ConsultaPendenciaEndereco>
    {
        public ConsultaPendenciaEnderecoMap() : base()
        {
            Table("TB_CONSULTA_PENDENCIA_ENDERECO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PENDENCIA_ENDERECO_MK");
            Id(u => u.codpessoa, "COD_PESSOA");
            Id(u => u.codcontrato, "COD_CONTRATO");
            Id(u => u.codconexao, "COD_CONEXAO");
            Id(u => u.nome_razaosocial, "NOME_RAZAOSOCIAL");
            Id(u => u.cpf, "CPF");
            Id(u => u.cancelado, "CANCELADO");
            Id(u => u.conexao_bloqueada, "CONEXAO_BLOQUEADA");
            Id(u => u.logradouro, "LOGRADOURO");
            Id(u => u.numero, "NUMERO");
            Id(u => u.bairro, "BAIRRO");
            Id(u => u.cidade, "CIDADE");
            Id(u => u.codfatura, "COD_FATURA");
            Id(u => u.valor_total, "VALORTOTAL");
            Id(u => u.liquidado, "LIQUIDADO");
            Id(u => u.descricao, "DESCRICAO");
            Id(u => u.suspenso, "SUSPENSO");
            Id(u => u.dataVencimento, "DATAVENCIMENTO");
            Id(u => u.motivo_suspensao, "MOTIVO_SUSPENSAO");
           
        }
    }
}
