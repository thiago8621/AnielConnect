using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_FOLHA_DEPTO")]
    public class Tb_Folha_Depto
    {
        [PVChave("Cod_DEPTO")]
        public Int16 Cod_DEPTO { get; set; }
        public String Num_DEPTO { get; set; }
        public String Descricao { get; set; }
    }
}
