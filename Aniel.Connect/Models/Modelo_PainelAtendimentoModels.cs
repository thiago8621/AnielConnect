using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Modelo_PainelAtendimentoListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }
        public virtual String Ordenar_Por { get; set; }
        public virtual String Visibilidade { get; set; }
        public virtual String Padrao { get; set; }
        public virtual String Status { get; set; }
    }


    public class Modelo_PainelAtendimentoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsCadastros))]
        [MaxLength(20, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Codigo { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        public virtual Boolean Status { get; set; }
        public virtual Visibilidade Visibilidade { get; set; }
        public virtual Coluna_PainelAtendimentoEnum? Coluna_Ordenar { get; set; }
        public virtual Padrao Padrao { get; set; }
        public virtual Recurso_PainelAtendimentoEnum? TipoFiltro { get; set; }


        private IEnumerable<Coluna_PainelAtendimentoModel> colunas;

        public virtual IEnumerable<Coluna_PainelAtendimentoModel> Colunas
        {
            get
            {
                if (colunas == null)
                    colunas = new List<Coluna_PainelAtendimentoModel>();
                return colunas;
            }
            set
            {
                colunas = value;
            }
        }

        private IEnumerable<Filtro_PainelAtendimentoModel> filtros;

        public virtual IEnumerable<Filtro_PainelAtendimentoModel> Filtros
        {
            get
            {
                if (filtros == null)
                    filtros = new List<Filtro_PainelAtendimentoModel>();
                return filtros;
            }
            set
            {
                filtros = value;
            }
        }
    }

    public class Filtro_PainelAtendimentoModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Valor { get; set; }
        public virtual Recurso_PainelAtendimentoEnum Filtro { get; set; }
        public virtual String Filtro_Descricao { get; set; }
        public virtual String Descricao { get; set; }

        
    }

    public class Coluna_PainelAtendimentoModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Coluna_PainelAtendimentoEnum Coluna { get; set; }
        public virtual String Descricao { get; set; }
        


    }
    public class GetColunas_Model
    {
        public virtual List<Int32> colunas { get; set; }
        public virtual Int32? ordenar_por { get; set; }

    }

    public class Modelo_PainelAtendimento_DataTableRequest : DataTableRequest
    {
        public virtual Boolean AdministradorModelos { get; set; }
    }

}