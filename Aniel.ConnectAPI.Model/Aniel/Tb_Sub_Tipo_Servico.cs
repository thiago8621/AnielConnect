using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_SUB_TIPO_SERVICO")]
    public class Tb_Sub_Tipo_Servico
    {
        [PVChave("Cod_Tipo_Serv")]
        public Int32 Cod_Tipo_Serv { get; set; }
        [PVChave("Cod_Sub_Tipo_Serv")]
        public String Cod_Sub_Tipo_Serv { get; set; }
        public String Descricao { get; set; }
    }
}
