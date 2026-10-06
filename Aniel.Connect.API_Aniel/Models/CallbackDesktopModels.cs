namespace Aniel.Connect.API_Aniel.Models
{
    public class CallbackDesktopModels : ApiRequest
    {
        public string correlation_id { get; set; }
        public bool success { get; set; }
        public string error_message { get; set; }
        public int pu_id { get; set; }
        public long old_pu_id { get; set; }
        public string pppoe_user { get; set; }
        public string old_pppoe_user { get; set; }
        public int sac_login { get; set; }
        public string sac_password { get; set; }
        public string wifi_ssid { get; set; }
        public string wifi_password { get; set; }
        public bool certificated { get; set; }
        public string message { get; set; }
        public double onu_signal { get; set; }
        public double cto_avg_sinal { get; set; }
        public bool pppoe_connected { get; set; }

    }
}