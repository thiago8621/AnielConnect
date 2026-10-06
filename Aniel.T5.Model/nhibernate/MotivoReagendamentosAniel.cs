using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class MotivoReagendamentosAniel
    {
        public virtual Int64 Codigo { get; set; }
        public virtual int Cod_Auxiliar { get; set; }

        public virtual String Descricao { get; set; }
        public virtual Boolean Status { get; set; }
    }
}
