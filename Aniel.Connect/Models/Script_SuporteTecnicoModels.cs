using Aniel.Connect.Core;
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
    public class Script_SuporteTecnicoListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }

        public virtual String Status { get; set; }
        public virtual List<Script_SuporteTecnicoItemListaModel> Perguntas { get; set; }
    }

    public class Script_SuporteTecnicoItemListaModel
    {
        public virtual String Informacao { get; set; }

        public virtual Int64 Ordem { get; set; }
    }


    public class Script_SuporteTecnicoCadastroModel : Script_AtendimentoItemModel, IViewModel<Int64>
    {
        private List<Script_SuporteTecnicoItemModel> _perguntas;



        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsCadastros))]
        [MaxLength(30, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual String Descricao { get; set; }

        public virtual String Informacao { get; set; }

        public virtual Boolean Status { get; set; }

        public virtual List<Script_SuporteTecnicoItemModel> Perguntas
        {
            get
            {
                if (_perguntas == null)
                    _perguntas = new List<Script_SuporteTecnicoItemModel>();
                return _perguntas;
            }
            set
            {
                _perguntas = value;
            }
        }
    }

    public class Script_SuporteTecnicoItemModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Informacao { get; set; }

        public virtual Int64 Ordem { get; set; }
    }
    
    public class Script_SuporteTecnico_DataTableRequest : DataTableRequest
    {
        public string Status { get; set; }
    }

}