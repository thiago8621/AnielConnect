using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;

namespace Aniel.T5.Model.nhibernate
{
    public class CallbackDesktop : BaseModel
    {
        public virtual string correlation_id { get; set; }
        public virtual bool success { get; set; }
        public virtual string error_message { get; set; }
        public virtual int pu_id { get; set; }
        public virtual long old_pu_id { get; set; }
        public virtual string pppoe_user { get; set; }
        public virtual string old_pppoe_user { get; set; }
        public virtual int sac_login { get; set; }
        public virtual string sac_password { get; set; }
        public virtual string wifi_ssid { get; set; }
        public virtual string wifi_password { get; set; }
        public virtual bool certificated { get; set; }
        public virtual string message { get; set; }
        public virtual double onu_signal { get; set; }
        public virtual double cto_avg_sinal { get; set; }
        public virtual bool pppoe_connected { get; set; }

    }
}
