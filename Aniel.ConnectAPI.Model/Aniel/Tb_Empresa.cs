using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_EMPRESA")]
    public class Tb_Empresa
    {
        [PVChave("Codempresa")]
        public Int16 Codempresa { get; set; }
        public String Empresa { get; set; }
        public String Razao_Social { get; set; }
        public virtual DateTime? Data_Alteracao { get; set; }
        public virtual Byte[] Logotipo { get; set; }
    }
}
