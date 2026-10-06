using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Integracoes.Saperx.Models
{
    public class DidIntegracaoRequest
    {
        /// <summary>
        /// Número DID
        /// </summary>
        public string number { get; set; }
        /// <summary>
        /// Cidade de registro do DID, deve ser escrito no formato: São Paulo / SP Respeitando o espaçamento entre a barra.
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// Valor cobrado pelo uso do DID Formato: 100.00
        /// </summary>
        public string client_value { get; set; }
        /// <summary>
        /// Aceita chamada a cobrar
        /// </summary>
        public bool ACB { get; set; }
        /// <summary>
        /// Número apto a venda
        /// </summary>
        public bool can_sell { get; set; }
        /// <summary>
        /// Número reservado para DDR 0 – Não 1 – Sim
        /// </summary>
        public bool ddr_reserved { get; set; }
        /// <summary>
        /// ID do ramal sip para realizar o vínculo.
        /// </summary>
        public int? id_sip { get; set; }

    }


    public class DidIntegracaoResponseResult
    {
        //Esse ID do Saperx e o nosso sequencial gerado nomomento da criação.
        public string id { get; set; }
        //Número DID
        public string number { get; set; }
        //Cidade de registro do DID, deve ser escrito no formato: São Paulo / SP Respeitando o espaçamento entre a barra.
        public string city { get; set; }
        //Valor cobrado pelo uso do DID Formato: 100.00
    }

    public class DidIntegracaoResponse
    {
        public string message { get; set; }
        public DidIntegracaoResponseResult result { get; set; }
    }


}