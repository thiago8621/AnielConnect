using System;
using System.Collections.Generic;
using System.Text;
using Aniel.ConnectAPI.Model.Atributo;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_ACESSO")]
    public class Tb_Acesso
    {
        [PVChave("Sigla_Modulo")]
        public String Sigla_Modulo { get; set; }
        [PVChave("Cod_Area")]
        public String Cod_Area { get; set; }
        [PVChave("Usuario_Aniel")]
        public String Usuario { get; set; }
        public String Tipo_Acesso { get; set; }
        public String Todos_Acessos { get; set; }
    }
}
