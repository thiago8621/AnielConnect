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
using Sinapse.Model;

namespace Aniel.Connect.Models.T5
{
    public class Tipo_Elemento_MaterialListaModel : BaseModel
    {
        public virtual String CodMat { get; set; }
        public virtual String Tipo { get; set; }


    }
    public class Tipo_Elemento_MaterialRequest : DataTableRequest
    {


    }
    public class Tipo_Elemento_MaterialCadastroModel
    {
        public virtual String CodMat { get; set; }
        public virtual Tipo_Elemento_Rede Tipo { get; set; }



    }
}