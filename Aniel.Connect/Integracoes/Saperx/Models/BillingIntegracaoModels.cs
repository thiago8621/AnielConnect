using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Integracoes.Saperx.Models
{
    public class BillingRequest
    {
        public DateTime time_start { get; set; }
        public DateTime time_end { get; set; }
        public string id_sip { get; set; }
        public string month_bill { get; set; }

    }
    public class Sip
    {
        public string name { get; set; }
        public string sip_number { get; set; }
        public int cod_cli { get; set; }
    }

    public class Cdr
    {
        public string source { get; set; }
        public string destiny { get; set; }
        public string dialed_number { get; set; }
        public string way { get; set; }
    }

    public class BillingResponse
    {
        public int id { get; set; }
        public string month_bill { get; set; }
        public int id_cdr { get; set; }
        public string source { get; set; }
        public string destination { get; set; }
        public int duration { get; set; }
        public DateTime time_start { get; set; }
        public DateTime time_end { get; set; }
        public int id_sip { get; set; }
        public string sip_cost { get; set; }
        public string city { get; set; }
        public string call_type { get; set; }
        public int id_ramal { get; set; }
        public Sip sip { get; set; }
        public Cdr cdr { get; set; }
    }



}