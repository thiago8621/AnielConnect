using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class TB_DESKTOP_INTEGRACAOMAP : ClassMapBase<CallbackDesktop>
    {
        public TB_DESKTOP_INTEGRACAOMAP()
        {
            Table("TB_DESKTOP_INTEGRACAO");

            Id(p => p.Id, "ID").GeneratedBy.Sequence("GN_DESKTOP_INTEGRACAO");

            Map(p => p.correlation_id, "CORRELATION_ID").Length(255);
            Map(p => p.success, "SUCCESS");
            Map(p => p.error_message, "ERROR_MESSAGE").Length(800);
            Map(p => p.pu_id, "PU_ID");
            Map(p => p.old_pu_id, "OLD_PU_ID");
            Map(p => p.pppoe_user, "PPPOE_USER").Length(255);
            Map(p => p.old_pppoe_user, "OLD_PPPOE_USER").Length(255);
            Map(p => p.sac_login, "SAC_LOGIN");
            Map(p => p.sac_password, "SAC_PASSWORD").Length(255);
            Map(p => p.wifi_ssid, "WIFI_SSID").Length(255);
            Map(p => p.wifi_password, "WIFI_PASSWORD").Length(255);
            Map(p => p.certificated, "CERTIFICATED");
            Map(p => p.message, "MESSAGE").Length(600);
            Map(p => p.onu_signal, "ONU_SIGNAL");
            Map(p => p.cto_avg_sinal, "CTO_AVG_SINAL");
            Map(p => p.pppoe_connected, "PPPOE_CONNECTED");



        }
    }
}
