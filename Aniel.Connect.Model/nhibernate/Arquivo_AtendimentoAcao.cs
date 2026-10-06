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
    public class Arquivo_AtendimentoAcao : BaseModel
    {
        public virtual String Arquivo { get; set; }

        public virtual String Extensao { get; set; }

        public virtual Atendimento_Acao Atendimento_Acao { get; set; }
    }
}
