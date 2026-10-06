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



    public class Sub_Tipo_ServicoListaModel
    {
        public virtual String Id { get; set; }

        public virtual String Descricao { get; set; }
        
    }

    public class Sub_Tipo_Servico_DataTableRequest : DataTableRequest
    {

    }
}