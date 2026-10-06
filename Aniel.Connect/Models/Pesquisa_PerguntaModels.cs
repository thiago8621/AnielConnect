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
    public class Pesquisa_PerguntaListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Nome { get; set; }

        public virtual String Titulo_Da_Pesquisa { get; set; }
                
        public virtual Boolean Status { get; set; }

        public virtual String Data_Hora_Criacao { get; set; }

        public virtual String Data_Hora_Criacao_Fim { get; set; }
    }


    public class Pesquisa_PerguntaCadastroModel :  IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "MAXLENGHT")]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
          ErrorMessageResourceName = "REQUIRED")]
        public virtual String Titulo_Da_Pesquisa { get; set; }

        //public virtual Status_Default Status { get; set; }
        public virtual Boolean Status { get; set; }

        public virtual Boolean Status_Pesquisa_Satisfacao { get; set; }
        
        public virtual String Nome { get; set; }

        //private List<Pesquisa_PerguntasItemModel> _perguntas;
        //public virtual List<Pesquisa_PerguntasItemModel> Perguntas
        //{
        //    get
        //    {
        //        if (_perguntas == null)
        //            _perguntas = new List<Pesquisa_PerguntasItemModel>();
        //        return _perguntas;
        //    }
        //    set
        //    {
        //        _perguntas = value;
        //    }
        //}
        

        #region Temporarios de Itens
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "MAXLENGHT")]        
        public virtual String Descricao { get; set; }

        public virtual Boolean Obrigatorio { get; set; }

        public virtual Boolean Status_Pesquisa_Pergunta { get; set; }

        public virtual String Escala_Linear_Ini { get; set; }

        public virtual String Escala_Linear_Fin { get; set; }

        public virtual Int64 Id_Pesquisa_Satisfacao { get; set; }

        public virtual String Respostas_Possiveis { get; set; }

        public virtual Pesquisa_Tipo_Resposta Tipo_Resposta { get; set; }
        #endregion

    }

    //public class Pesquisa_PerguntasItemModel
    //{
    //    public virtual Int64 Id { get; set; }        
        
    //    public virtual String Descricao { get; set; }

    //    //public virtual String Obrigatorio { get; set; }
    //    public virtual Boolean Obrigatorio { get; set; }

    //    //public virtual String Status_Pesquisa_Pergunta { get; set; }
    //    public virtual Boolean Status_Pesquisa_Pergunta { get; set; }

    //    public virtual String Escala_Linear_Ini { get; set; }

    //    public virtual String Escala_Linear_Fin { get; set; }

    //    public virtual Int64 Id_Pesquisa_Satisfacao { get; set; }

    //    public virtual String Respostas_Possiveis { get; set; }

    //    public virtual Script_Tipo_Resposta Tipo_Resposta { get; set; }
    //}
    
    public class Pesquisa_Pergunta_DataTableRequest : DataTableRequest
    {
        public virtual Int64 Id_Cliente { get; set; }
    }

}