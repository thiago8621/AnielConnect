using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Mapa_EquipesModel
    {
        public virtual String Equipe { get; set; }

        public virtual String Nome { get; set; }

        public virtual String Endereco { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String Longitude { get; set; }

        public virtual String Precisao { get; set; }
        public virtual String Bateria { get; set; }

        public virtual String StatusGPS { get; set; }
        public virtual String Dispositivo { get; set; }

        public virtual String Versao_Android { get; set; }
        public virtual String Versao_App { get; set; }

        public virtual String Tipo { get; set; }

        public virtual String InternetMovel { get; set; }
        public virtual String WiFi { get; set; }
        public virtual String IMEI { get; set; }

        public virtual String Atualizado_Em { get; set; }
    }

    public class Mapa_EquipesDataTableRequest : DataTableRequest
    {

    }
}