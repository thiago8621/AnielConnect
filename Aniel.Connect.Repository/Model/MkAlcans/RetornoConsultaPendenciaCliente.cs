using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model.MkAlcans
{
    public class Fee
    {
        public string document { get; set; }

        public int customer { get; set; }
        public int contract { get; set; }
        public int fee { get; set; }
        public string due { get; set; }
        public string amount { get; set; }
        public string neighborhood { get; set; }
        public string street { get; set; }
        public string house_number { get; set; }
        public string city { get; set; }
    }

    public class RetornoConsultaPendenciaCliente
    {
        public List<Fee> fees { get; set; }
    }
}