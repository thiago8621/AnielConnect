using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_CAT_CLIENTE")]
    public class Tb_Categoria_Cliente
    {
        [PVChave("Id_Cat_Cliente")]
        public Int32 Id { get; set; }
        public String Descricao { get; set; }
    }
}
