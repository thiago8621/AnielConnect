using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_USUARIO_PERFIL")]
    public class Tb_Usuario_Perfil
    {
        [PVChave("Usuario_Aniel")]
        public String Usuario { get; set; }
        [PVChave("Cod_Perfil_Usuario")]
        public Int16 Cod_Perfil_Usuario { get; set; }
    }
}
