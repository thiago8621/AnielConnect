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
    public class Organizacao_Contato : BaseModel
    {
        public virtual Organizacao Organizacao { get; set; }
        public virtual String Contato { get; set; }
        public virtual String Informacao_Adicional { get; set; }
        public virtual Tipo_Contato Tipo_Contato { get; set; }

    }
}
