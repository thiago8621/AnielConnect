using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Script_Atendimento : BaseModel
    {
        private IList<Script_AtendimentoItem> _perguntas;

        public virtual String Nome { get; set; }

        public virtual Int64 Numero { get; set; }

        public virtual Status_Default Status { get; set; }

        public virtual IList<Script_AtendimentoItem> Perguntas
        {
            get
            {
                if (_perguntas == null)
                    _perguntas = new List<Script_AtendimentoItem>();
                return _perguntas;
            }
            set
            {
                _perguntas = value;
            }
        }
    }
    
}
