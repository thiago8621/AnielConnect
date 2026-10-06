using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class IntegracaoProtheusReverseAniel
    {
        public virtual Int64 Id { get; set; }
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String D3_Filial { get; set; }
        public virtual String D3_Local { get; set; }
        public virtual String Empresa { get; set; }
    }
}
