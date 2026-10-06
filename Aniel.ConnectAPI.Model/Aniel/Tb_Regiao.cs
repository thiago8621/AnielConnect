using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_REGIAO")]
    public class Tb_Regiao
    {
        [PVChave("Cod_Regiao")]
        public Int16 Cod_Regiao { get; set; }
        public String Nome { get; set; }
    }
}
