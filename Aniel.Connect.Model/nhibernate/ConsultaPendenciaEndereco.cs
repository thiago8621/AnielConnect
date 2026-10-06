using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class ConsultaPendenciaEndereco : BaseModel
    {
        public virtual String codpessoa { get; set; }
        public virtual String codcontrato { get; set; }
        public virtual String codconexao { get; set; }
        public virtual String nome_razaosocial { get; set; }
        public virtual String cpf { get; set; }
        public virtual String cancelado { get; set; }
        public virtual String conexao_bloqueada { get; set; }
        public virtual String logradouro { get; set; }
        public virtual String numero { get; set; }
        public virtual String bairro { get; set; }
        public virtual String cidade { get; set; }
        public virtual String codfatura { get; set; }
        public virtual String valor_total { get; set; }
        public virtual String liquidado { get; set; }
        public virtual String descricao { get; set; }
        public virtual String suspenso { get; set; }
        public virtual String dataVencimento { get; set; }
        public virtual String motivo_suspensao { get; set; }        
        
    }
}


