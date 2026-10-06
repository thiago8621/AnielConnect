using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_CLASSIF_QUALID")]
    public class Tb_Classificacao_Qualidade
    {
        [PVChave("Id_Classif_Qualid")]
        public Int32 Id { get; set; }
        public String Qualidade { get; set; }
    }
}
