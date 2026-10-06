using Aniel.ConnectAPI.Model.Aniel.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    public class Tb_Pv_Documento_Producao : Tb_Pv
    {
        public String Codct { get; set; }
        public String Projeto { get; set; }
        public String Num_Obra { get; set; }
        public Int32 Num_Doc { get; set; }
    }
}
