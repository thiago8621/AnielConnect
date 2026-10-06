using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class CamposObrigatoriosDocumentoProducao : BaseModel
    {
        public virtual CamposObrigatoriosDocumentoProducaoEnum Campo { get; set; }
        public virtual String CampoString { get; set; }
        public virtual Boolean Bloqueado { get; set; }
        public virtual Boolean Obrigatorio { get; set; }
        //TODO Alinhar com time sobre tamanho e funcionalidade
        public virtual String Titulo { get; set; }
    }
}
