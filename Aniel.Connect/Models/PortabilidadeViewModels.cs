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
    public class Portabilidade_ListViewModel
    {
        public virtual String Id { get; set; }
        public virtual String Codigo { get; set; }
        public virtual String Did { get; set; }
        public virtual String UF { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Cliente { get; set; }
        public virtual String Numero_Circuito { get; set; }
        public virtual String Dia_Previsto_Portabilidade { get; set; }
        //public virtual String Lixeira { get; set;  }


    }
    public class Portabilidade_CadastroViewModel
    {
        public virtual Int64? Codigo { get; set; }
        public virtual String Did { get; set; }
        public virtual String Pais { get; set; }
        public virtual String UF { get; set; }
        public virtual String Cidade { get; set; }
        public virtual Int64? Cliente { get; set; }
        public virtual Int64? Did_Substituido { get; set; }
        public virtual Int64? Id_Circuito { get; set; }
        public virtual Int64? Numero_Sip { get; set; }
        public virtual String Dia_Previsto_Portabilidade { get; set; }
        public virtual Status_Portabilidade Status { get; set; }

    }
    public class Portabilidade_DatatableRequest : DataTableRequest
    {
        public virtual String Cliente { get; set; }
        public virtual String DID { get; set; }
        public virtual String Numero_Circuito { get; set; }
        public virtual String Status { get; set; }

    }
}