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
    public class MetaListaModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }

        public virtual String Ano { get; set; }
        public virtual String Status { get; set; }
        public virtual String Janeiro { get; set; }

        public virtual String Fevereiro { get; set; }

        public virtual String Marco { get; set; }

        public virtual String Abril { get; set; }

        public virtual String Maio { get; set; }

        public virtual String Junho { get; set; }

        public virtual String Julho { get; set; }

        public virtual String Agosto { get; set; }

        public virtual String Setembro { get; set; }

        public virtual String Outubro { get; set; }

        public virtual String Novembro { get; set; }

        public virtual String Dezembro { get; set; }
    }


    public class MetaCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //          ErrorMessageResourceName = "REQUIRED")]
        public virtual Int16 Ano { get; set; }


        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Int32 Janeiro { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Int32 Fevereiro { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Int32 Marco { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Int32 Abril { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Int32 Maio { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Int32 Junho { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Int32 Julho { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Int32 Agosto { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Int32 Setembro { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Int32 Outubro { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Int32 Novembro { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Int32 Dezembro { get; set; }

        public virtual Boolean Status { get; set; }

        public virtual Int64? Id_Filial { get; set; }
    }

    public class Meta_DataTableRequest : DataTableRequest
    {
        public virtual String Ano { get; set; }
        public virtual String Status { get; set; }
      
    }

    #region MOBILE MetaCampanha
    public class MetaCampanha
    {
        public Int32 Mes { get; set; }
        public Int64 Valor_meta { get; set; }
        public String Periodo { get; set; }
        public Int32? Valor_alcancado { get; set; }
        public DateTime Data_inicio { get; set; }
        public DateTime Data_final { get; set; }
    }

    public class MetaPeriodo
    {
        public Int32 Periodo { get; set; }
        public DateTime Primeiro_Dia { get; set; }
        public DateTime Ultimo_Dia { get; set; }
        public Int32 Ano { get; set; }
    }
    #endregion 
}