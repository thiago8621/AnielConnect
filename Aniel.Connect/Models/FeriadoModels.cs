using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
    public class FeriadoListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Data { get; set; }
        public virtual Int64 DataOrdem { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Abrangencia { get; set; }      
        public virtual String Status { get; set; }
        public virtual String Pais { get; set; }
        public virtual String UF { get; set; }
        public virtual String Cidade { get; set; }
    }


    public class FeriadoCadastroModel : IViewModel<Int64>
    {


        public virtual Int64 Id { get; set; }

        /// <summary>
        ///     Data para feriados fixos, que não pode ter o ano alterado
        /// </summary>
        [Display(Name = "DATA", ResourceType = typeof(Resources.RsCadastros))]
        [MaxLength(10, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Data { get; set; }

        /// <summary>
        ///     Data para feriados variáveis, que pode ter o ano alterado
        /// </summary>
        [Display(Name = "DATA", ResourceType = typeof(Resources.RsCadastros))]
        [MaxLength(10, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String DataVariavel { get; set; }
        public virtual String Ano { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        public virtual Tipo_Feriado? Tipo { get; set; }
        public virtual Abrangencia? Abrangencia { get; set; }
        public virtual Boolean Status { get; set; }

        public virtual Int64? Pais { get; set; }

        [Display(Name = "UF", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public UF? UF { get; set; }

        public virtual Int64? Id_Cidade { get; set; }

    }

    public class Feriado_DataTableRequest : DataTableRequest
    {
        public virtual Tipo_Feriado? TipoFiltro { get; set; }
        public virtual Abrangencia? AbrangenciaFiltro { get; set; }
        public virtual UF? UF_Filtro { get; set; }
        public virtual String Data_Inicial { get; set; }
        public virtual String Data_Final { get; set; }
        public virtual String Pais_Filtro { get; set; }
        public virtual String Cidade_Filtro { get; set; }
        public virtual Int32? Status_Filtro { get; set; }
    }

}