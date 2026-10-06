using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class ConsultaPendenciaEnderecoModel
    {
    }

    public class ConsultaPendenciaEnderecoListModel
    {
        public virtual String Endereco_request { get; set; }
        public virtual String ConsultaResponse { get; set; }

        public virtual String codpessoa { get; set; }
        public virtual String codcontrato { get; set; }
        public virtual String codconexao { get; set; }
        public virtual String codFatura { get; set; }
        public virtual String nome_razaosocial { get; set; }
        public virtual String cpf { get; set; }
        public virtual String contratoCancelado { get; set; }
        public virtual String conexao_bloqueada { get; set; }
        public virtual String logradouro { get; set; }
        public virtual String numero { get; set; }
        public virtual String bairro { get; set; }
        public virtual String cidade { get; set; }
        public virtual String codfatura { get; set; }
        public virtual String valor_total { get; set; }
        public virtual String liquidado { get; set; }
        public virtual String descricao { get; set; }
        public virtual String descricaoDebito { get; set; }
        public virtual String suspenso { get; set; }
        public virtual String dataVencimento { get; set; }
        public virtual String motivoSuspensao { get; set; }

    }

    public class ConsultaPendenciaEnderecoApiRequest
    {
        public virtual String Endereco_request { get; set; }
        public virtual String ConsultaResponse { get; set; }
    }

    public class ConsultaPendenciaEnderecoDatatableRequest : DataTableRequest
    {
        public virtual String Endereco_request { get; set; }
        public virtual String ConsultaResponse { get; set; }

        public virtual String codpessoa { get; set; }
        public virtual String codcontrato { get; set; }
        public virtual String codconexao { get; set; }
        public virtual String nome_razaosocial { get; set; }
        public virtual String cpf { get; set; }
        public virtual String contratoCancelado { get; set; }
        public virtual String conexao_bloqueada { get; set; }
        public virtual String logradouro { get; set; }
        public virtual String numero { get; set; }
        public virtual String bairro { get; set; }
        public virtual String cidade { get; set; }
        public virtual String codfatura { get; set; }
        public virtual String valor_total { get; set; }
        public virtual String liquidado { get; set; }
        public virtual String descricao { get; set; }
        public virtual String descricaoDebito { get; set; }
        public virtual String suspenso { get; set; }
        public virtual String dataVencimento { get; set; }
        public virtual String motivoSuspensao { get; set; }
    }

}