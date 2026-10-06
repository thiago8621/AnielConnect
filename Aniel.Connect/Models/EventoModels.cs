using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class EventoListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }

        public virtual String Status { get; set; }

        public virtual String TextoEnviado { get; set; }
        public virtual String VariaveisDisponiveisEvento { get; set; }
        public int IDArquivo { get; set; }
        public string Nome { get; set; }
        public int Tamanho { get; set; }
        public string Tipo { get; set; }
        public string Caminho { get; set; }
 
    public virtual String AtivarAcao { get; set; }
        public virtual String ItensInseridos { get; set; }
        public virtual Int64 AcaoExecutada { get; set; }
    }


    public class Evento_CadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Status { get; set; }

        public virtual Int64 Id_Categoria_Aniel { get; set; }

        public virtual String TextoEnviado { get; set; }
        public virtual String VariaveisDisponiveisEvento { get; set; }
        public virtual String ImagemEvento { get; set; }
        public virtual String ItensInseridos { get; set; }
        public virtual Boolean AtivarAcao { get; set; }
        public virtual Int64 AcaoExecutada { get; set; }

    }


}