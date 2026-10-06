using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{

    public class Motivo_Desistencia_VendaListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
        public virtual String ExibeAppOla { get; set; }
        public virtual String Motivo { get; set; }
    }

    public class Motivo_Desistencia_VendaCadastroModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Codigo { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual Boolean ExibeAppOla { get; set; } 
        public virtual EnumMotivo_Desistencia_Venda Motivo { get; set; }
    }

    public class Motivo_Desistencia_Venda_DataTableRequest : DataTableRequest
    {
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
        public virtual String Motivo { get; set; }
        public virtual Boolean ExibeAppOla { get; set; }
    }
}