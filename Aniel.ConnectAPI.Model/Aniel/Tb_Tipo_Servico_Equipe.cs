using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_TP_SERV_EQUIPE")]
    public class Tb_Tipo_Servico_Equipe
    {
        [PVChave("Cod_Tipo_Serv")]
        public Int16 Cod_Tipo_Serv { get; set; }
        public String Descricao { get; set; }
        public String Descricao_Aux { get; set; }
        public Int32? Cod_Natureza { get; set; }
    }
}
