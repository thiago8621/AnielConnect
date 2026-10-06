using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class CamposObrigatoriosDocumento_Producao : BaseModel
    {
        public virtual CamposObrigatoriosDocumentoProducaoEnum Campo { get; set; }
        public virtual Boolean Visivel { get; set; }
        public virtual Boolean Obrigatorio { get; set; }
        public virtual String Titulo { get; set; }
    }
}
