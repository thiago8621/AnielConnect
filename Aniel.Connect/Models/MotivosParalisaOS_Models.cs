using Aniel.Connect.Core;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
    public class MotivosParalisaOS_Models
    {
        public virtual long Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual bool Status { get; set; }
    }

    public class MotivosParalisaOS_ListModels
    {
        public virtual long Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Status { get; set; }
    }

    public class MotivosParalisaOS_CadastroModels : IViewModel<String>
    {
        public virtual String Id { get; set; }
        public virtual Int64 Codigo { get; set; }
        [Required]
        public virtual String Descricao { get; set; }
        public virtual Boolean Status { get; set; }
    }

    public class MotivosParalisaOS_DataTableRequest : DataTableRequest
    {
        public virtual Int64 Codigo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
    }

    public class AlterarMotivosParalisaOSViewModel
    {
        public List<string> ids { get; set; }
        public Int16 status { get; set; }
    }

    public class ObjComboMotivosParalisaOS<T> : IObjComboResult<T>
    {
        public virtual T Id { get; set; }
        public virtual String Text { get; set; }
    }
}