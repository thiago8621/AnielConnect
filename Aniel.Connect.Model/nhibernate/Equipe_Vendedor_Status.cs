using Sinapse.Model;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Equipe_Vendedor_Status : BaseModel
    {
        public virtual Funcionario Vendedor { get; set; }
        public virtual String CEP { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Numero { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }

        public virtual Double? Precisao { get; set; }
        public virtual Double? Bateria { get; set; }



        public virtual String StatusGPS { get; set; }
        public virtual String Dispositivo { get; set; }

        public virtual String Versao_Android { get; set; }
        public virtual String Versao_App { get; set; }

        public virtual String Status_App { get; set; }

        public virtual String Justificativa_Ausencia { get; set; }

        //public virtual EquipeStatusTipo? Tipo { get; set; }
        public virtual Boolean? Logado { get; set; }
        public virtual DateTime? DataHora_Login { get; set; }

        public virtual String InternetMovel { get; set; }
        public virtual Boolean WiFi { get; set; }
        public virtual String IMEI { get; set; }
    }
}
