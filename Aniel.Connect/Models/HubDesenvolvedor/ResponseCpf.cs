using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.HubDesenvolvedor
{
    public class ResponseCpf
    {
        public virtual String numero_de_cpf { get; set; }
        public virtual String nome_da_pf { get; set; }
        public virtual String data_nascimento { get; set; }
        public virtual String situacao_cadastral { get; set; }
        public virtual String data_inscricao { get; set; }
        public virtual String digito_verificador { get; set; }
        public virtual String comprovante_emitido { get; set; }
        public virtual String comprovante_emitido_data { get; set; }
    }
}