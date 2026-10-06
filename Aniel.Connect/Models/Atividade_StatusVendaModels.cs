using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Atividade_StatusVendaListModels
            {
        public virtual Int64 Id { get; set; }
        public virtual String StatusPadrao { get; set; }
        public virtual String Descricao { get; set; }
        
        public virtual String Status { get; set; }
    }
    public class Atividade_StatusVendaCadastroModels : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual Boolean Status { get; set; }

        public virtual Status_Venda? StatusPadrao { get; set; }
        public virtual String Descricao { get; set; }
        
    }

    public class Atividade_StatusVenda_DataTableRequest : DataTableRequest
    {

    }

    public class ObjComboStatusVenda<T> : IObjComboResult<T>
    {
        public virtual T Id { get; set; }
        public virtual String Text { get; set; }
        public virtual String StatusFixo { get; set; }
        public virtual long IdStatusPadrao { get; set; }
    }
}



