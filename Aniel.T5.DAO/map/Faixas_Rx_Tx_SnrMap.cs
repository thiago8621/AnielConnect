using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    class Faixas_Rx_Tx_SnrMap : ClassMapBase<Faixas_Rx_Tx_Snr>
    {
        public Faixas_Rx_Tx_SnrMap() : base()
        {
            Table("TB_FAIXAS_RX_TX_SNR");

            Id(x => x.Id);

            Map(x => x.Min_Rx, "MIN_RX");
            Map(x => x.Max_Rx, "MAX_RX");
            Map(x => x.Min_Tx, "MIN_TX");
            Map(x => x.Max_Tx, "MAX_TX");
            Map(x => x.Min_Snr, "MIN_SNR");
            Map(x => x.Max_Snr, "MAX_SNR");

            References(x => x.Projeto, "ID_PROJETO");
        }
    }
}
