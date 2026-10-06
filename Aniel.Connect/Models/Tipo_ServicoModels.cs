using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{



    public class Tipo_ServicoListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Natureza { get; set; }
        public virtual String CodCt { get; set; }

    }

    public class Tipo_Servico_DataTableRequest : DataTableRequest
    {

    }

    public class Tipo_ServicoComboModel : ObjComboResult<Int32>
    {
        public virtual String Natureza { get; set; }
        public virtual long? CodNatureza { get; set; }
        public virtual String CodCt { get; set; }
        public virtual long? PrioridadeNatureza { get; set; }
        public virtual Int16? IdDepartamentoPadrao { get; set; }
    }

    public class Tipo_AtendimentoComboModel : ObjComboResult<Int64>
    {
        public virtual String Categoria_Atendimento { get; set; }
        public virtual String Prioridade_Padrao { get; set; }
        public virtual String Script { get; set; }
        public virtual String Criar_Os_Encerrar { get; set; }
        public virtual String Vincular_Motivos { get; set; }

        public virtual String Cod_Encerramento { get; set; }

        public virtual String Descricao { get; set; }

    }
}