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
    public class Pesquisa_RespostaListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Titulo_Da_Pesquisa { get; set; }
                
        public virtual Boolean Status { get; set; }

        public virtual String Data_Hora_Criacao { get; set; }
    }


    public class Pesquisa_RespostaCadastroModel :  IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        
        public virtual String Titulo_Da_Pesquisa { get; set; }
                
        public virtual Boolean Status { get; set; }

        public virtual Boolean Status_Pesquisa_Satisfacao { get; set; }
        
        //[MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
        //                ErrorMessageResourceName = "MAXLENGHT")]
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //          ErrorMessageResourceName = "REQUIRED")]
        public virtual String Nome { get; set; }
        

        #region Temporarios de Itens
        public virtual String Descricao { get; set; }

        public virtual Pesquisa_Tipo_Resposta Tipo_Resposta { get; set; }
        #endregion

    }

    public class Pesquisa_RespostaItemModel
    {
        public virtual Int64 Id { get; set; }        
        
        public virtual String Descricao { get; set; }


    }
    
    public class Pesquisa_Resposta_DataTableRequest : DataTableRequest
    {
        
    }

}