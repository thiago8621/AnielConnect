using Aniel.Connect.Core;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{

    public class Configuracao_Envio_MensagensListModel
    {
        public virtual long Id { get; set; }
        public virtual bool Envia_Msg_Ao_Iniciar_Deslocamento { get; set; }
        public virtual bool Utiliza_Envio_SMS { get; set; }
        public virtual bool Utiliza_Envio_Email { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Status { get; set; }
        public virtual string Mensagem_SMS { get; set; }
        public virtual string Mensagem_Corpo_Email { get; set; }
        public virtual string Assunto_Email { get; set; }


    }


    public class Configuracao_Envio_MensagensCadastroModel : IViewModel<Int64>
    {
        public virtual long Id { get; set; }
        public virtual bool Envia_Msg_Ao_Iniciar_Deslocamento {get; set;}
        public virtual bool Utiliza_Envio_SMS { get; set; }
        public virtual bool Utiliza_Envio_Email { get; set; }
        public virtual string Descricao { get; set; }
        public virtual bool Status { get; set; }
        public virtual String Mensagem_SMS { get; set; }
        public virtual string Mensagem_Corpo_Email { get; set; }
        public virtual string Assunto_Email { get; set; }
    }


    public class Configuracao_Envio_Mensagens_DataTableRequest : DataTableRequest
    {

    }
}