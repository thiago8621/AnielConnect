using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_AREA")]
    public class Tb_Area
    {
        [PVChave("Cod_Escritorio")]
        public String Cod_Escritorio { get; set; }
        [PVChave("Cod_Area")]
        public String Cod_Area { get; set; }
        public String Descricao { get; set; }
    }
}
