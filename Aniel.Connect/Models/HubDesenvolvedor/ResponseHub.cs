using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.HubDesenvolvedor
{
    public class ResponseHub<T>
    {
        public virtual Boolean status { get; set; }
        [JsonProperty(PropertyName = "return")]
        public virtual String retorno { get; set; }
        public virtual String message { get; set; }
        public virtual T result { get; set; }
    }
}