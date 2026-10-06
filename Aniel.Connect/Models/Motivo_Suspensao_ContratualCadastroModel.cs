using Aniel.Connect.Core;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Motivo_Suspensao_ContratualListaModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual String Motivo { get; set; }
        public virtual String Motivo_Para { get; set; }
        public virtual String Status { get; set; }
    }

    public class Motivo_Suspensao_ContratualCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual String Motivo { get; set; }

        public virtual Status_Motivo_Para Motivo_Para { get; set; }

        public virtual Boolean Status { get; set; }

        public virtual String Observacao_Motivo_Suspensao_Plano_Cliente_Outros { get; set; }
    }

    public class Motivo_Suspensao_Contratual_DataTableRequest : DataTableRequest
    {

    }
}