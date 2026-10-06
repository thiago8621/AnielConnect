using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_PERFIL_EQUIPE")]
    public class Tb_Perfil_Equipe
    {
        [PVChave("Cod_Perfil_Equipe")]
        public Int32 Cod_Perfil_Equipe { get; set; }
        public String Nome { get; set; }
    }
}
