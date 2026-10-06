using Aniel.Connect.Core;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Aniel.Connect.Models
{
    public class LegendaTimelineTipoServicos_ListModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string CodCt { get; set; }
        public virtual string Contrato { get; set; }

        public virtual string Natureza { get; set; }

        public virtual string TipoServico { get; set; }

        public virtual string Cor { get; set; }
        public virtual string Status { get; set; }
    }

    public class LegendaTimelineTipoServicos_CadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual string Contrato { get; set; }

        public virtual string Natureza { get; set; }
        public virtual string TipoServico { get; set; }
        public virtual string Cor { get; set; }

        public virtual Boolean Status { get; set; }
    }

    public class LegendaTimelineTipoServicos_DataTableRequest : DataTableRequest
    {
        public virtual Int64 Id { get; set; }
        public virtual string Contrato { get; set; }

        public virtual string Natureza { get; set; }
        public virtual string TipoServico { get; set; }
        public virtual string Cor { get; set; }

        public virtual string Status { get; set; }
    }

    public class GravarListaViewModel
    {
        public List<Int64> ids { get; set; }
        public virtual string cor { get; set; }
    }

}
