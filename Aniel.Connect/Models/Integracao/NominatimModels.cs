using Aniel.Connect.Core.Models;
using Aniel.Connect.Core.Service;
using Sinapse.Core.Entity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.Integracao
{
    public class NominatimLatLong
    {
        public virtual String lat { get; set; }
        public virtual String lon { get; set; }
        public virtual Double distancia { get; set; }
        public virtual NominatimAddressLatLong address { get; set; }
    }

    public class NominatimAddressLatLong
    {
        public virtual String road { get; set; }
        public virtual String town { get; set; }
        public virtual String city_district { get; set; }
        public virtual String county { get; set; }
        public virtual String state_district { get; set; }
        public virtual String state { get; set; }
        public virtual String country { get; set; }
        public virtual String country_code { get; set; }
    }
}