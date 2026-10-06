using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_TURNO_SERVICO")]
    public class Tb_Turno_Servico
    {
        [PVChave("Id_Turno_Servico")]
        public Int32 Id { get; set; }
        public String Descricao { get; set; }
    }
}
