using Aniel.ConnectAPI.Model.Aniel.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    public class Tb_Pv_Filial : Tb_Pv
    {
        public Int16 Codempresa { get; set; }
        public Int16 Codfilial { get; set; }
    }
}
