using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Faixas_Rx_Tx_Snr : BaseModel
    {
        public virtual Projeto Projeto { get; set; }
        public virtual Int32? Min_Rx { get; set; }
        public virtual Int32? Max_Rx { get; set; }
        public virtual Int32? Min_Tx { get; set; }
        public virtual Int32? Max_Tx { get; set; }
        public virtual Int32? Min_Snr { get; set; }
        public virtual Int32? Max_Snr { get; set; }
    }
}
