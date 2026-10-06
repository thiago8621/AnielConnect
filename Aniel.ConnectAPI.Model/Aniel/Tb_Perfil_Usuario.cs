using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_PERFIL_USUARIO")]
    public class Tb_Perfil_Usuario
    {
        [PVChave("Cod_Perfil_Usuario")]
        public Int32 Cod_Perfil_Usuario { get; set; }
        public String Descricao { get; set; }
    }
}
