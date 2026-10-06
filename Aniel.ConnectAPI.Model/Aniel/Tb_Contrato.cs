using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_CONTRATO")]
    public class Tb_Contrato
    {
        [PVChave("CodCt")]
        public String CodCt { get; set; }
        public String Descricao { get; set; }
        public Int16? CodEmpresa { get; set; }
        public Int16? CodFilial { get; set; }
        public String Localidade { get; set; }
        public String Fornec_Mat_Padrao { get; set; }
        public String TipoSaldo { get; set; }
        public String TipoSaldoAplic { get; set; }
        public String CodCt_Regulador { get; set; }
        public String Projeto_Regulador { get; set; }
    }
}
