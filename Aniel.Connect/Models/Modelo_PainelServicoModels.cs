using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using DocumentFormat.OpenXml.Wordprocessing;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Modelo_PainelServicoListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }
        public virtual String Ordenar_Por { get; set; }
        public virtual String Visibilidade { get; set; }
        public virtual String Padrao { get; set; }
        public virtual String Status { get; set; }
    }

    public class Modelo_PainelServicoCadastroModel
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
        public virtual ColunaOrdenarPainelServicoEnum? Coluna_Ordenar { get; set; }
        public virtual Padrao Padrao { get; set; }
        public virtual Filtro_PainelServicoEnum? TipoFiltro { get; set; }

        private IEnumerable<Coluna_PainelServicoModel> colunas;

        public virtual IEnumerable<Coluna_PainelServicoModel> Colunas
        {
            get
            {
                if (colunas == null)
                    colunas = new List<Coluna_PainelServicoModel>();
                return colunas;
            }
            set
            {
                colunas = value;
            }
        }

        private IEnumerable<Filtro_PainelServicoModel> filtros;

        public virtual IEnumerable<Filtro_PainelServicoModel> Filtros
        {
            get
            {
                if (filtros == null)
                    filtros = new List<Filtro_PainelServicoModel>();
                return filtros;
            }
            set
            {
                filtros = value;
            }
        }
    }

    public class Filtro_PainelServicoModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Valor { get; set; }
        public virtual String ValorPai { get; set; }
        public virtual String ValorAvo { get; set; }
        public virtual Filtro_PainelServicoEnum Filtro { get; set; }
        public virtual String Filtro_Descricao { get; set; }
        public virtual String Descricao { get; set; }
    }

    public class Coluna_PainelServicoModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Coluna_PainelServicoEnum Coluna { get; set; }
        public virtual String Descricao { get; set; }
    }

    public class GetServicoColunas_Model
    {
        public virtual List<Int32> colunas { get; set; }
        public virtual Int32? ordenar_por { get; set; }
    }

    public class Modelo_PainelServico_DataTableRequest : DataTableRequest
    {
        public virtual Boolean AdministradorModelos { get; set; }
    }

}