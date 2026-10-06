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
    public class Categoria_Cliente : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual Tipo_Prioridade? Tipo_Prioridade { get; set; }
        public virtual Status_Default? Status { get; set; }
        public virtual Int32 Id_Cfop { get; set; }


        public virtual String CodigoTipoClienteIntegracao { get; set; }
    }
}
