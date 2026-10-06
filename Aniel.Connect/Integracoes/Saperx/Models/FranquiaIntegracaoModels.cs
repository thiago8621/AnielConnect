using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Integracoes.Saperx.Models
{
  

    public class FranquiaIntegracaoRequest
    {
        /// <summary>
        /// ID do plano *(Obrigatório)
        /// </summary>
        public int id_plan { get; set; }
        /// <summary>
        /// Nome da franquia *(Obrigatório)
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 1 – Sim 0 – Não
        /// </summary>
        public bool LC { get; set; }
        /// <summary>
        /// 1 – Sim 0 – Não
        /// </summary>
        public bool LDN { get; set; }
        /// <summary>
        /// 1 – Sim 0 – Não
        /// </summary>
        public bool VC1 { get; set; }
        /// <summary>
        /// 1 – Sim 0 – Não
        /// </summary>
        public bool VC2 { get; set; }
        /// <summary>
        /// 1 – Sim 0 – Não
        /// </summary>
        public bool VC3 { get; set; }
        /// <summary>
        /// Minutos da franquia
        /// </summary>
        public int franchise_minutes { get; set; }
    }

    public class FranquiaIntegracaoResponseResult
    {
        //Esse ID do Saperx e o nosso sequencial gerado nomomento da criação.
        public string id { get; set; }
        //Número DID
        public string number { get; set; }
        //Cidade de registro do DID, deve ser escrito no formato: São Paulo / SP Respeitando o espaçamento entre a barra.
        public string city { get; set; }
        //Valor cobrado pelo uso do DID Formato: 100.00
    }

    public class FranquiaIntegracaoResponse
    {
        public string message { get; set; }
        public DidIntegracaoResponseResult result { get; set; }
    }


}