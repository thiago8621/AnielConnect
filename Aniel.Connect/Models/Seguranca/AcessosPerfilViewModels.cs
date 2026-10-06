using Sinapse.Core;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.MGTEC.Models
{

    public class AcessosPerfilFiltroModel : IViewModel<Int64>
    {
        private ICollection<SelectListItem> _listaModulos;

        public Int64 Id { get; set; }

        [Display(Name = "MODULO", ResourceType = typeof(Resources.Resources))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resources),
                  ErrorMessageResourceName = "MODULOREQUIRED")]
        public String SistemaWeb { get; set; }

        public ICollection<SelectListItem> ListaModulos
        {
            get
            {
                if (_listaModulos == null)
                    _listaModulos = new List<SelectListItem>();
                return _listaModulos;
            }
            set
            {
                _listaModulos = value;
            }
        }

        [Display(Name = "PERFIL", ResourceType = typeof(Resources.Resources))]
        public String DescricaoPerfil { get; set; }
    }

    public class AcessosPerfilListaModel : IViewModel<Int64>
    {
        public Int64 Id { get; set; }

        public String SistemaWeb { get; set; }

        [Display(Name = "CONTROLE", ResourceType = typeof(Resources.Resources))]
        public String CaptionControle { get; set; }

        public String Controle { get; set; }

        [Display(Name = "PAGINAS", ResourceType = typeof(Resources.Resources))]
        public AcessoPerfilPaginaListaModel Pagina { get; set; }

        public String CamposBloqueadosJSON { get; set; }

    }

    public class AcessoPerfilPaginaListaModel : IViewModel<Int64>
    {
        private List<AcessoPerfilRecursoListaModel> _recursos;

        public Int64 Id { get; set; }

        [Display(Name = "PAGINA", ResourceType = typeof(Resources.Resources))]
        public string Descricao { get; set; }

        public string Action { get; set; }

        public List<AcessoPerfilRecursoListaModel> Recursos
        {
            get
            {
                if (_recursos == null)
                    _recursos = new List<AcessoPerfilRecursoListaModel>();
                return _recursos;
            }
            set
            {
                _recursos = value;
            }
        }

        public Boolean TemCamposBloqueados { get; set; }

        public Boolean TemSubAcesso { get; set; }

    }

    public class AcessoPerfilRecursoListaModel : IViewModel<Int64>
    {
        public Int64 Id { get; set; }

        public String Descricao { get; set; }

        public Boolean Habilitado { get; set; }
    }

    public class AcessoPerfilWebApi : IViewModel<Int64>
    {
        public Int64 Id {get; set; }
        public Int64 IdPerfil {get; set; } 
        public String Controle { get; set; }
        public Int64 IdPagina { get; set; }
        public String RecursosJSON { get; set; }
        public String CamposBloqueadosJSON { get; set; }
        public Boolean TodosRecursos { get; set; }

        public String SistemaWeb { get; set; }
    }

    public class AcessoCampoModelView : IViewModel<Int64>
    {
        public Int64 Id { get; set; }

        List<AcessoCampoItensModelView> itens;
        public List<AcessoCampoItensModelView> Itens 
        { 
            get
            {
                if (itens == null)
                    itens = new List<AcessoCampoItensModelView>();
                return itens;
            }
            set
            {
                itens = value;
            }
        }
    }

    public class AcessoCampoItensModelView : IViewModel<Int64>
    {
        public Int64 Id { get; set; }

        [Display(Name = "CAMPO", ResourceType = typeof(Resources.Resources))]
        public String Campo { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.Resources))]
        public String DescCampo { get; set; }

        [Display(Name = "VISIVEL", ResourceType = typeof(Resources.Resources))]
        public Boolean Visivel { get; set; }
    }

}