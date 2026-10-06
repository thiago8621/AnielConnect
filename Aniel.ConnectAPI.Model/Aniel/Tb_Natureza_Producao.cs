using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_NATUREZA_PROD")]
    public class Tb_Natureza_Producao
    {
        [PVChave("Id_Natureza")]
        public Int32 Id { get; set; }
        public String Descricao { get; set; }
    }
}
