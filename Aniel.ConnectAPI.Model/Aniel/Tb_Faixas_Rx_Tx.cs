using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_FAIXA_RX_TX")]
    public class Tb_Faixas_Rx_Tx
    {
        [PVChave("Id_Faixa")]
        public Int32 Id { get; set; }
        public String Codct { get; set; }
        public String Projeto { get; set; }
        public Int32 Min_Rx { get; set; }
        public Int32 Max_Rx { get; set; }
        public Int32 Min_Tx { get; set; }
        public Int32 Max_Tx { get; set; }
        public Int32 Min_Snr { get; set; }
        public Int32 Max_Snr { get; set; }
    }
}
