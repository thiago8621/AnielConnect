using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_CENTRO_CUSTO")]
    public class Tb_Centro_Custo
    {
        [PVChave("Cod_Ccusto")]
        public Int32 Cod_Ccusto { get; set; }
        public String Cod_Aux { get; set; }
        public String Descricao { get; set; }
    }
}
