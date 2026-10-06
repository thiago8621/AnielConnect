using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_PERFIL_USU_ACESSO")]
    public class Tb_Perfil_Usuario_Acesso
    {
        [PVChave("Sigla_Modulo")]
        public String Sigla_Modulo { get; set; }
        [PVChave("Cod_Area")]
        public String Cod_Area { get; set; }
        [PVChave("Cod_Perfil_Usuario")]
        public Int32 Cod_Perfil_Usuario { get; set; }
        public String Tipo_Acesso { get; set; }
        public String Todos_Acessos { get; set; }
    }
}
