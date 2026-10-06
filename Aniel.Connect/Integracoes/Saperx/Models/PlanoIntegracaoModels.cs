using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Integracoes.Saperx.Models
{
    public class PlanoIntegracaoRequest
    {
        /// <summary>
        /// Nome do plano nacional
        /// </summary>
        public string name { get; set; }
        public string obs { get; set; }
        
        public int id { get; set; }
        public string lc_value { get; set; }
        public string lc_cad { get; set; }
        public string ldn_value { get; set; }
        public string ldn_cad { get; set; }
        public string vc1_value { get; set; }
        public string vc1_cad { get; set; }
        public string vc2_value { get; set; }
        public string vc2_cad { get; set; }
        public string vc3_value { get; set; }
        public string vc3_cad { get; set; }
        //public string did_value { get; set; }
        public bool active { get; set; }
        public bool charge_intragroup { get; set; }
        public bool charge_intragroup_lc { get; set; }
        public bool minimum_consumption { get; set; }
        public string minimum_consumption_value { get; set; }
        //public string standard_charging { get; set; }
        public int consumption_callerid { get; set; }
        public string consumption_callerid_value { get; set; }
        public bool consumption_sigame { get; set; }
        //public string consumption_sigame_value { get; set; }
        public bool consumption_blacklist { get; set; }
        //public string consumption_blacklist_value { get; set; }
        public bool consumption_acb { get; set; }
        //public string consumption_acb_type { get; set; }
        //public string consumption_acb_value { get; set; }


    }
    public class PlanoIntegracaoResponse
    {
        public int id { get; set; }
        public string name { get; set; }
    }


}