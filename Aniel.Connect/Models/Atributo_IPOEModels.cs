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

    //elementos carregados na pagina index
    public class Atributo_IPOEListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Nome { get; set; }
    }

    //elementos carregados na pagina cadastro / alteracao
    public class Atributo_IPOECadastroModel : IViewModel<Int64>
    {
        //obrigatorio para implementar a interface IViewModel
        public virtual Int64 Id { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Atributo_Autenticacao { get; set; }
        public virtual String Atributo_Bloqueio { get; set; }
        public virtual String Atributo_ReducaoVelocidade { get; set; }
        public virtual String Atributo_VelocidadeNormal { get; set; }
    }

    public class Atributo_IPOE_DataTableRequest : DataTableRequest
    {

    }
}