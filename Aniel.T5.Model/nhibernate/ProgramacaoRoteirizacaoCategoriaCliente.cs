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
    public class ProgramacaoRoteirizacaoCategoriaCliente : BaseModel
    {
        public virtual ProgramacaoRoteirizacaoAutomatica ProgramacaoRoteirizacaoAutomatica { get; set; }
        public virtual Categoria_ClienteAniel Categoria_ClienteAniel { get; set; }
    }

}
