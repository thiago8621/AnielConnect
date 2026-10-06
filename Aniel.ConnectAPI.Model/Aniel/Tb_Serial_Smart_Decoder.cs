using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    public class Tb_Serial_Smart_Decoder
    {
        public Int32 Id{ get; set; }
        public String Codct { get; set; }
        public String Projeto { get; set; }
        public Int32 Num_Doc { get; set; }
        public String Num_Obra { get; set; }
        public String Terminal_Inst { get; set; }
        public String Smart_Inst { get; set; }
        public String Terminal_Ret { get; set; }
        public String Smart_Ret { get; set; }
    }
}
