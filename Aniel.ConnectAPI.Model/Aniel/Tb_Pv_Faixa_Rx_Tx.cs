using Aniel.ConnectAPI.Model.Aniel.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    public class Tb_Pv_Faixa_Rx_Tx : Tb_Pv
    {
        public Int64 Id_Faixa { get; set; }
        public String CodCt { get; set; }
        public String Projeto { get; set; }
    }
}
