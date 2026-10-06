using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ModelLog.nhibernate
{
    public class LogImportacaoVinculoPerfilAtivacao : BaseModel
    {
        public virtual string Data { get; set; }
        public virtual string Hora { get; set; }
        public virtual string Erro { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string Codmat { get; set; }
        public virtual long? Id_OLT { get; set; }

    }
}
