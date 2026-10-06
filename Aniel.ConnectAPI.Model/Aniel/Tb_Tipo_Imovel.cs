using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_TIPO_IMOVEL")]
    public class Tb_Tipo_Imovel
    {
        [PVChave("Cod_Tipo_Imovel")]
        public Int32 Cod_Tipo_Imovel { get; set; }
        public String Descricao { get; set; }
    }
}
