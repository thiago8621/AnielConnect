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
    public class Script_SuporteTecnico : BaseModel
    {
        private IList<Script_SuporteTecnicoItem> _informacoes;

        public virtual String Descricao { get; set; }

        public virtual Status_Default Status { get; set; }

        public virtual IList<Script_SuporteTecnicoItem> Perguntas
        {
            get
            {
                if (_informacoes == null)
                    _informacoes = new List<Script_SuporteTecnicoItem>();
                return _informacoes;
            }
            set
            {
                _informacoes = value;
            }
        }
    }
    
}
