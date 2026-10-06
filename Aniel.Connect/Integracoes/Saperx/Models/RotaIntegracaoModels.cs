using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Integracoes.Saperx.Models
{
    public class RotaIntegracaoRequest
    {
        /// <summary>
        /// nome da rota
        /// </summary>
        public string name { get; set; }

    }


    public class RotaIntegracaoResponseResult
    {
        //Esse ID do Saperx e o nosso sequencial gerado nomomento da criação.
        public string id { get; set; }
        //nome da rota
        public string name { get; set; }
     
    }

    public class RotaIntegracaoResponse
    {
        public string message { get; set; }
        public RotaIntegracaoResponseResult result { get; set; }
    }


}