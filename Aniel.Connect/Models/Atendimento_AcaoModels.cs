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
    public class Atendimento_AcaoListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual int Numero { get; set; }

        public virtual string Nome { get; set; }

        public virtual String Script { get; set; }
        
        public virtual String Setor { get; set; }
        public virtual String Responsavel { get; set; }
        public virtual String Motivo_Status { get; set; }

        public virtual String DataHoraCriacao { get; set; }
    }
    
    public class Atendimento_AcaoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        // Cliente
        public virtual Int64 Id_Cliente { get; set; }

        public virtual String Nome_Cliente { get; set; }
                
        // Sobre o Atendimento 
        public virtual String Numero { get; set; }

        public virtual Int64? Id_Origem_Atendimento { get; set; }
               
        public virtual String Ultima_Acao { get; set; }

        public virtual String Arquivo { get; set; }

        public virtual Boolean Status { get; set; }
    }

    public class Atendimento_AcaoMotivoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual Int64? Id_Motivo { get; set; }
        public virtual String Desc_Motivo { get; set; }
    }


    public class Atendimento_Acao_DataTableRequest : DataTableRequest
    {
        
    }

}