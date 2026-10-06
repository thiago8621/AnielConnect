using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class URL_SistemaListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Int32 Id_Sistema { get; set; }
        public virtual String Sistema { get; set; }
        public virtual String URL { get; set; }
    }

    public class URL_SistemaCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual Sistema Sistema { get; set; }
        public virtual String URL { get; set; }
        public virtual Int64? Id_Configuracoes_Gerais { get; set; }

        
    }
    
    public class URL_Sistema_DataTableRequest : DataTableRequest
    {

    }
}