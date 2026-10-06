using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class FamiliaMaterialRequest : ApiRequest
    {
        /// <summary> Família do Material </summary>
        public string Familia { get; set; }        
        /// <summary> Descrição do Material </summary>
        public string Descricao { get; set; }
        /// <summary> Foi Desenvolvido Sinteticamente? </summary>
        public bool Sintetica { get; set; }
    }

    public class FamiliaMaterialModel
    {
        public string Familia { get; set; }
        public string Descricao { get; set; }
        public bool Sintetica { get; set; }
    }
}