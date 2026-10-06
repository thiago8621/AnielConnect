using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class URL_Sistema : BaseModel
    {

        public virtual Sistema Sistema { get; set; }
        public virtual String URL { get; set; }
        public virtual long? Id_Configuracoes_Gerais { get; set; }

    }
}
