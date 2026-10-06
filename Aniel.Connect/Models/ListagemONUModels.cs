using Aniel.Connect.Core;
using System;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.Models
{
    public class ONUListModel
    {
        public string Id { get; set; }

        public long IdOLT { get; set; }
        public string NumSlot { get; set; }
        public string NumPON { get; set; }
        
        public string Status { get; set; }
        public string Serial { get; set; }
        public string SerialConvertido { get; set; }
        public string Mac { get; set; }
        public string Cliente { get; set; }
        public string PlanoProduto { get; set; }
        public string ONTID { get; set; }
        public string DataUltimaAtualizacaoStatus { get; set; }

    }
    
    public class StatusONU
    {
        public string Serial { get; set; }
        public StatusONU Status { get; set; }



    }

    public class ListagemONU_DataTableRequest: DataTableRequest
    {
        public Int64? idSlot { get; set; }
        public Int64? idOLT { get; set; }
        
        public Int64? idPON { get; set; }
        
        public Int64? idCliente { get; set; }
        public Int64? idPlano { get; set; }
        public Int64? idLoginRadius { get; set; }
        public Int64? idPOP { get; set; }
        public Int64? idVLAN { get; set; }
        
        
        public Int64? idCaixaFTTH { get; set; }
        public Int64? idPortaFTTH { get; set; }
        public Int32? IdStatus { get; set; }

    }
}