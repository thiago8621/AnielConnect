using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_SENHA")]
    public class Tb_Senha
    {
        [PVChave("Usuario_Aniel")]
        public String Usuario { get; set; }
        public String Nome { get; set; }
        public String Senha { get; set; }
        public String Num_Fre { get; set; }
        public String Ativo { get; set; }
    }
}
