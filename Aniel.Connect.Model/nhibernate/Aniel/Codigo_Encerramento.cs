using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Codigo_Encerramento : BaseModel
    {
        public virtual String Cod_Encerramento { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Int16 Status_Execucao { get; set; }

        public virtual Contrato Contrato { get; set; }
    }
}
