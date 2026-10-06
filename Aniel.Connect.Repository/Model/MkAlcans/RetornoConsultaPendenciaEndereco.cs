using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model.MkAlcans
{
    public class LatterFee
    {
        public string due { get; set; }
        public string amount { get; set; }
        public string city { get; internal set; }
        public string neighborhood { get; internal set; }
        public string street { get; internal set; }
        public string number { get; internal set; }
        public string document { get; internal set; }
        public string fee { get; internal set; }
        public string contract { get; internal set; }
        public string customer { get; internal set; }
    }

    public class RetornoConsultaPendenciaEndereco
    {
        public List<LatterFee> latterFee { get; set; }
    }
}