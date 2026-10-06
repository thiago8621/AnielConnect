using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_LOCALIDADE")]
    public class Tb_Localidade
    {
        [PVChave("Cod_Escritorio")]
        public String Cod_Escritorio { get; set; }
        [PVChave("Cod_Area")]
        public String Cod_Area { get; set; }
        [PVChave("Localidade")]
        public String Localidade { get; set; }
        public Int32 Cod_Localidade { get; set; }
    }
}
