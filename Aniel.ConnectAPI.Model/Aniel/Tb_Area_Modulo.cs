using System;
using System.Collections.Generic;
using System.Text;
using Aniel.ConnectAPI.Model.Atributo;

namespace Aniel.ConnectAPI.Model.Aniel
{   
    [PVTabela("TB_PV_AREA_MODULO")]
    public class Tb_Area_Modulo
    {
        [PVChave("Sigla_Modulo")]
        public String Sigla_Modulo { get; set; }
        [PVChave("Cod_Area")]
        public String Cod_Area { get; set; }
        public String Descricao { get; set; }
        public String Sub_Acesso { get; set; }
        public String Observacao { get; set; }
        public String Detalhamento { get; set; }
    }
}
