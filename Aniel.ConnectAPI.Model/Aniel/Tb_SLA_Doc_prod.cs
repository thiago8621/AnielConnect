using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_SLA_PROD")]
    public class Tb_SLA_Doc_Prod
    {
        [PVChave("Id_Sla_Prod")]
        public Int64 Id { get; set; }
        public Int16 Cod_Tipo_Serv { get; set; }
        public Int32 Id_Categoria { get; set; }
        public Int64 Id_Criticidade { get; set; }
        public Int32 Porc_Ini { get; set; }
        public Int32 Porc_Fim { get; set; }
        public Int16 Cor { get; set; }
    }
}
