using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;

namespace Aniel.T5.Model.nhibernate
{
    public class ProgramacaoRoteirizacaoProjeto : BaseModel
    {
        public virtual ProgramacaoRoteirizacaoAutomatica ProgramacaoRoteirizacaoAutomatica { get; set; }
        public virtual string CodCt { get; set; }
        public virtual string Projeto { get; set; }
    }

}
