using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_PROJETO")]
    public class Tb_Projeto
    {
        [PVChave("CodCt")]
        public String CodCt { get; set; }
        [PVChave("Projeto")]
        public String Projeto { get; set; }
        public Int16? CodEmpresa { get; set; }
        public Int16? CodFilial { get; set; }
        public String Localidade { get; set; }
        public DateTime? InicPrev { get; set; }
        public DateTime? TermPrev { get; set; }
        public DateTime? InicReal { get; set; }
        public DateTime? TermReal { get; set; }
        public DateTime? TermReal2 { get; set; }

        public String CodCt_Regulador { get; set; }
        public String Projeto_Regulador { get; set; }
    }
}
