using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.T5
{
    public class Tipo_ElementoIconeListModel
    {
        public virtual Int64? Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Classe { get; set; }
    }

    public class Tipo_ElementoIconeDataTableRequest : DataTableRequest 
    {
        public virtual String Classe { get; set; }
    }
    public class Tipo_ElementoIconeCadastroModel
    {
        public virtual String Descricao { get; set; }
        public virtual String Classe { get; set; }
    }
    public class Tipo_ElementoIconeComboModel : ObjComboResult<String> 
    {
        public virtual Boolean Padrao_connect { get; set; }
        public virtual String Name { get; set; }
    }
}