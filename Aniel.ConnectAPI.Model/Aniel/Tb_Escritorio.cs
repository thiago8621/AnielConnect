using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_ESCRITORIO")]
    public class Tb_Escritorio
    {
        [PVChave("Cod_Escritorio")]
        public String Cod_Escritorio { get; set; }
        public String Descricao { get; set; }
    }
}
