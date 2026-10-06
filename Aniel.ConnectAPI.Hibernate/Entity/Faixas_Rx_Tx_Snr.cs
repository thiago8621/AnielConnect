using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    public class Faixas_Rx_Tx_Snr : BaseClass
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
