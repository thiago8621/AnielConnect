using Aniel.ConnectAPI.Model.Aniel.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    public class Tb_Pv_Localidade : Tb_Pv
    {
        public String Cod_Escritorio { get; set; } 
        public String Cod_Area { get; set; }
        public String Localidade { get; set; }
    }
}
