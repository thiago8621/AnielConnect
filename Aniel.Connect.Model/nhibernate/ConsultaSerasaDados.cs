using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class ConsultaSerasaDados : BaseModel
    {
        public virtual String consulta { get; set; }
        public virtual String cpf { get; set; }
        public virtual String cnpj { get; set; }
    }
}
