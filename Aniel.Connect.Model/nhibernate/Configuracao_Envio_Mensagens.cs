using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Configuracao_Envio_Mensagens: BaseModel
    {
        public virtual bool Envia_Msg_Ao_Iniciar_Deslocamento { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual bool Utiliza_Envio_SMS { get; set; }
        public virtual bool Utiliza_Envio_Email { get; set; }
        public virtual String Mensagem_Corpo_SMS { get; set; }
        public virtual String Mensagem_Corpo_Email { get; set; }
        public virtual String Assunto_Email { get; set; }
        public virtual string Descricao { get; set; }
    }
}
