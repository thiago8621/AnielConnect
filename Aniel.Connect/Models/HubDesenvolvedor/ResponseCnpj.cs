using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.HubDesenvolvedor
{
    public class ResponseCnpj
    {
        public virtual String numero_de_inscricao { get; set; }
        public virtual String abertura { get; set; }
        public virtual String nome { get; set; }
        public virtual String fantasia { get; set; }
        public virtual String logradouro { get; set; }
        public virtual String numero { get; set; }
        public virtual String cep { get; set; }
        public virtual String bairro { get; set; }
        public virtual String municipio { get; set; }
        public virtual String uf { get; set; }
        public virtual String email { get; set; }
        public virtual String telefone { get; set; }
        public virtual String situacao { get; set; }
        public virtual String dt_situacao_cadastral { get; set; }
    }
}