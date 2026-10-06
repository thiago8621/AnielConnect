using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{

    public class Motivo_Aprovacao_RestricaoListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
        public virtual String ExibeAppOla { get; set; }
        public virtual String Tipo_Restricao { get; set; }
        public virtual String Usuario_Criacao { get; set; }

    }

    public class Motivo_Aprovacao_RestricaoCadastroModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Codigo { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual Boolean ExibeAppOla { get; set; }
        public virtual EnumMotivo_Aprovacao_Restricao? Tipo_Restricao { get; set; }
    }

    public class Motivo_Aprovacao_Restricao_DataTableRequest : DataTableRequest
    {
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
        public virtual String Tipo_Restricao { get; set; }
        public virtual Boolean ExibeAppOla { get; set; }
    }
}