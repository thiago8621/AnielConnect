using Aniel.Connect.Model.enumerado;
using System;
using Sinapse.Model.nhibernate;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.Model.nhibernate
{
    public class Conta_Email : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual String Email { get; set; }
        public virtual String Senha { get; set; }
        public virtual String Servidor_Imap { get; set; }
        public virtual Int32 Porta_Imap { get; set; }
        public virtual String Servidor_Smtp { get; set; }
        public virtual Int32 Porta_Smtp { get; set; }
        public virtual Boolean Principal { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual AuthMethodImap AuthMethodImap { get; set; }

        public virtual Boolean Utilizar_Ssl_Imap { get; set; }
        public virtual Boolean Utilizar_Ssl_Smtp { get; set; }
        public virtual DateTime? UltimaData { get; set; }

    }
}
