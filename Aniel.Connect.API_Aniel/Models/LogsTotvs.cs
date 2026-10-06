using System;

namespace Aniel.Connect.API_Aniel.Models
{
    public class LogsTotvsRequest : ApiRequest
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string CCusto { get; set; }
    }

    public class LogsTotvsResponse
    {
        public string CCusto { get; set; }
        public string Descricao { get; set; }
        public string Retorno { get; set; }
        public string Data_Hora { get; set; }
        public string Usuario { get; set; }  

    }

    public class LogsTotvsMatResponse
    {
        public string CodMat { get; set; }
        public string Unidade { get; set; }
        public string Descricao { get; set; }
        public string Retorno { get; set; }
        public string Data_Hora { get; set; }
        public string Usuario { get; set; }

    }
    public class LogsTotvsRecResponse
    {
        public string Pedido { get; set; }
        public string Nota_Fiscal { get; set; }
        public string CodCt { get; set; }
        public string Projeto { get; set; }
        public string Codmat { get; set; }
        public string Retorno { get; set; }
        public string Usuario { get; set; }
        public string Data_Hora { get; set; }


    }
    public class LogsTotvsEqResponse
    {
        public string Equipe { get; set; }
        public string Nome { get; set; }
        public string Desligamento { get; set; }
        public string Projeto { get; set; }
        public string Codmat { get; set; }
        public string Retorno { get; set; }
        public string Usuario { get; set; }
        public string Data_Hora { get; set; }

    }

    public class LogsTotvsFcResponse
    {      
        public string Cod_Func { get; set; }
        public string Descricao { get; set; }
        public string Retorno { get; set; }
        public string Usuario { get; set; }
        public string Data_Hora { get; set; }

    }
    public class LogsTotvsFuNCResponse
    {      
        public string Cod_Func { get; set; }
        public string Nome { get; set; }
        public string Retorno { get; set; }
        public string Usuario { get; set; }
        public string Data_Hora { get; set; }

    }


}