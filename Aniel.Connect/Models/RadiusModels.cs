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
    public class RadiusListaModel
    {
        public virtual Int64  Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String GrupoRadius { get; set; }
        public virtual String Ip { get; set; }
        public virtual String Porta { get; set; }
        public virtual String Status { get; set; }
    }

    public class RadiusCadastroModel : IViewModel<Int64>
    {
        public Int64 Id { get; set; }

        public String Descricao { get; set; }
        public Boolean Status { get; set; }

        public String Ip { get; set; }
        public String Porta_Ssh { get; set; }
        public Int32 Timeout { get; set; }

        public String Senha { get; set; }
        public String Dominio { get; set; }

        public String GrupoRadius { get; set; }
        public Int64 IdPop { get; set; }
    }

    public class ExisteUsuarioCadastradoNoRadius
    {
        public virtual Int64 Id_Radius { get; set; }
        public virtual String Login_Usuario { get; set; }
    }

    public class Radius_DataTableRequest : DataTableRequest
    {

    }
}