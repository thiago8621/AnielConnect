using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_CFOP")]
    public class Tb_Cfop
    {
        [PVChave("COD_CFOP")]
        public String Cfop { get; set; }
        public String Descricao { get; set; }
    }
}
