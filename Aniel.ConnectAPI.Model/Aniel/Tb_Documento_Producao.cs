using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_DOC_PRODUCAO")]
    public class Tb_Documento_Producao
    {
        [PVChave("CodCt")]
        public String CodCt { get; set; }
        [PVChave("Projeto")]
        public String Projeto { get; set; }
        [PVChave("Num_Obra")]
        public String Num_Obra { get; set; }
        [PVChave("Num_Doc")]
        public Int32 Num_Doc { get; set; }

        public String Equipe { get; set; }

        public String Num_RVT { get; set; }
        public String Tarefa { get; set; }
        public String Cod_Escritorio { get; set; }
        public String Cod_Area { get; set; }
        public Int32? Cod_Localidade { get; set; }
        public String Cod_Encerramento { get; set; }
        public Int16? Cod_Tipo_Serv { get; set; }
        public String Cod_Sub_Tipo_Serv { get; set; }
        public DateTime? Data_Maxima { get; set; }
        public DateTime? Hora_Maxima { get; set; }
        public DateTime? Data_Prod { get; set; }
        public DateTime? Hora_Prod { get; set; }
        public String Observacao { get; set; }
        public String Endereco { get; set; }
        public String Endereco_Num { get; set; }
        public String Endereco_Compl { get; set; }
        public String Endereco_Bairro { get; set; }
        public String Endereco_Cpl { get; set; }
        public Double? Latitude { get; set; }
        public Double? Longitude { get; set; }
    }
}
