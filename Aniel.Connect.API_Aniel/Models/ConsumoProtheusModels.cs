using System;

namespace Aniel.Connect.API_Aniel.Models
{
    public class ConsumoProtheusRequest : ApiRequest
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string CodSequencialRMDM{ get; set; }
    }

    public class ConsumoProtheusResponse
    {
        public Int64? Id { get; set; }
        public string CodComplemento { get; set; }
        public string CodContrato { get; set; }
        public string Equipe { get; set; }
        public string Importado { get; set; }
        public string Estorno { get; set; }
        public string Projeto { get; set; }
        public string SequenciaRMDM { get; set; }
        public Double? Quantidade { get; set; }
        public string DataImportacao { get; set; }
        public string NomeEquipe { get; set; }
        public string CodMaterial { get; set; }
        public string QTD { get; set; }
        public string Tipo_Doc { get; set; }
        public string Retorno { get; set; }



    }
}