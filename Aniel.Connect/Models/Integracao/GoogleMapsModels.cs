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
    public class RetornoGoogle
    {
        public virtual EnderecoGoogle[] results { get; set; }

        public virtual String status { get; set; }
    }
    public class AddressComponentGoogle
    {
        public virtual String long_name { get; set; }

        public virtual String short_name { get; set; }

        public virtual String[] types { get; set; }
    }
    public class EnderecoGoogle
    {
        public virtual AddressComponentGoogle[] address_components { get; set; }

        public virtual String formatted_address { get; set; }

        public virtual GeometryGoogle geometry { get; set; }

        public virtual String[] types { get; set; }
        public virtual bool partial_match { get; set; }
    }
    public class GeometryGoogle
    {
        public virtual BoundsGoogle bounds { get; set; }

        public virtual LatLongGoogle location { get; set; }

        public virtual String location_type { get; set; }

        public virtual ViewPortGoogle viewport { get; set; }
    }

    public class ViewPortGoogle
    {
        public virtual LatLongGoogle northeast { get; set; }

        public virtual LatLongGoogle southwest { get; set; }
    }

    public class BoundsGoogle
    {
        public virtual LatLongGoogle northeast { get; set; }

        public virtual LatLongGoogle southwest { get; set; }
    }

    public class LatLongGoogle
    {
        public virtual String lat { get; set; }

        public virtual String lng { get; set; }

        public virtual Double distancia { get; set; }
    }

}