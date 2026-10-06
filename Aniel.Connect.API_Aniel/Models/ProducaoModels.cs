using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    public class ProducaoModels : ApiRequest
    {
        public String CodCt { get; set; }
        public String Projeto { get; set; }
        public String Num_Obra { get; set; }
        public Int32? Num_Doc { get; set; }
        public Int32? Status_Auditoria { get; set; }

        public String Unidade_Custo { get; set; }

        public String Tarefa { get; set; }
        public String Equipe { get; set; }
        public String NomeEquipe { get; set; }

        public Int16? Cod_Tipo_Serv { get; set; }
        public String Cod_Sub_Tipo_Serv { get; set; }

        public String Tipo_Producao { get; set; }

        public String Codigo { get; set; }
        public String Descricao { get; set; }

        public String Cod_Preco { get; set; }

        public String Data_Prod { get; set; }

        public String Valor { get; set; }
        public String Quant_Prod { get; set; }
        public String Quant_Catal { get; set; }

        public String Usuario { get; set; }
        public String Dia { get; set; }
        public String Hora { get; set; }
    
}
}