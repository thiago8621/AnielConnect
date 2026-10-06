using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{

    //elementos carregados na pagina index
    public class Perfil_DashboardListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
        public virtual String Perfil_Inicial { get; set; }
        public virtual String Perfil_Gerencial { get; set; }

    }

    //elementos carregados na pagina cadastro / alteracao
    public class Perfil_DashboardCadastroModel : IViewModel<Int64>
    {
        //obrigatorio para implementar a interface IViewModel
        public virtual Int64 Id { get; set; }

        public virtual String Id_User { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }
        public virtual DashboardInicialConnect Dashboard_Inicial { get; set; }
        public virtual Enum_VisualizacaoDashboard? VisualizacaoDashboard { get; set; }
        public virtual DashBoard_Gerencial DashBoard_Gerencial { get; set; }
        public virtual List<Perfil_DashboardRecursoModel> RespostasInicial { get; set; }
        public virtual List<Perfil_DashboardRecursoModel> RespostasGerencial { get; set; }
        public virtual Boolean Status { get; set; }
    }

    public class Perfil_DashboardRecursoModel
    {
        public virtual Int32? Recurso { get; set; }
    }

    public class Perfil_Dashboard_DataTableRequest : DataTableRequest
    {

    }
}