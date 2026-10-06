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
    public class Script_AtendimentoListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Nome { get; set; }

        public virtual String Status { get; set; }

        public virtual String Numero { get; set; }
    }


    public class Script_AtendimentoCadastroModel : Script_AtendimentoItemModel, IViewModel<Int64>
    {
        private List<Script_AtendimentoItemModel> _perguntas;



       
        [MaxLength(30, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Nome { get; set; }

        public virtual Boolean Status { get; set; }

        public virtual List<Script_AtendimentoItemModel> Perguntas
        {
            get
            {
                if (_perguntas == null)
                    _perguntas = new List<Script_AtendimentoItemModel>();
                return _perguntas;
            }
            set
            {
                _perguntas = value;
            }
        }
    }

    public class Script_AtendimentoItemModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Pergunta { get; set; }

        public virtual String Explicacao_Cliente { get; set; }

        public virtual String Explicacao_Operador { get; set; }

        public virtual String Respostas_Possiveis { get; set; }
        public virtual String Observacoes { get; set; }
        public virtual Int64 Ordem { get; set; }

        public virtual Script_Tipo_Resposta Tipo_Resposta { get; set; }
    }
    
    public class Script_Atendimento_DataTableRequest : DataTableRequest
    {
        public string Status { get; set; }
    }

}