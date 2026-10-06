using Aniel.Connect.Core;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{

    public class FranquiaListaModel
    {
        public virtual string Id { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Minutos { get; set; }
        public virtual string Vc1 { get; set; }
        public virtual string Vc2 { get; set; }
        public virtual string Vc3 { get; set; }
        public virtual string Lc { get; set; }
        public virtual string Ldn { get; set; }
        public virtual string Status { get; set; }
    }

    public class FranquiaCadastroModel : IViewModel<Int64>
    {
        //obrigatorio para implementar a interface IViewModel
        public virtual Int64 Id { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Minutos { get; set; }
        public virtual bool Vc1 { get; set; }
        public virtual bool Vc2 { get; set; }
        public virtual bool Vc3 { get; set; }
        public virtual bool Lc { get; set; }
        public virtual bool Ldn { get; set; }
        public virtual bool Ldi { get; set; }
        public virtual bool Status { get; set; }
    }

    public class Franquia_DataTableRequest : DataTableRequest
    {
    }

}