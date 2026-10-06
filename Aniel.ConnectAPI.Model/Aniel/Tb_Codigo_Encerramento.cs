using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_COD_ENCERRAMENTO")]
    public class Tb_Codigo_Encerramento
    {
        [PVChave("Cod_Encerramento")]
        public String Cod_Encerramento { get; set; }
        [PVChave("Codct")]
        public String Codct { get; set; }
        [PVChave("Cod_Tipo_Serv ")]
        public Int32 Cod_Tipo_Serv { get; set; }
        public String Descricao { get; set; }
        public Int16 Status_Execucao { get; set; }
    }
}
