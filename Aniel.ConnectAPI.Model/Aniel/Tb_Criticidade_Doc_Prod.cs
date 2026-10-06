using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_CRITICIDADE_PROD")]
    public class Tb_Criticidade_Doc_Prod
    {
        [PVChave("Criticidade_Prod")]
        public Int64 Id { get; set; }
        public String Descricao { get; set; }
    }
}
