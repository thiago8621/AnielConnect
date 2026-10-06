using Aniel.Connect.Model.enumerado;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Email_Atendimento : BaseModel
    {
        public virtual Atendimento Atendimento { get; set; }
        public virtual Atendimento_Acao Atendimento_Acao { get; set; }
        public virtual Status_Atendimento? Mudanca_Status { get; set; }
        public virtual Conta_Email Conta_Email { get; set; }
        public virtual Boolean Enviado { get; set; }
        public virtual DateTime? DataHoraEnvio { get; set; }
        public virtual String Mensagem { get; set; }
        public virtual String ReferenciaMessageID { get; set; }
        public virtual long ReferenciaUltimoEnvioCLI { get; set; }
    }
}
