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
    public class DepartamentoListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Codigo { get; set; }

        public virtual String Descricao { get; set; }
        
    }
    
}