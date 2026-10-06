using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_ST_ATEND_DOCPROD")]
    public class Tb_Status_Atend_Doc_Prod
    {
        [PVChave("Status")]
        public Int16 Status { get; set; }
        public String Descricao { get; set; }
    }
}
