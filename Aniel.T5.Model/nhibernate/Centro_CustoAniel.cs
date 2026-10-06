using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class Centro_CustoAniel
    {
        public virtual Int32 Cod_Ccusto { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Codigo_Auxiliar { get; set; }
        public virtual DateTime? Data_Criacao { get; set; }
        public virtual DateTime? Hora_Criacao { get; set; }
        public virtual String Ativo { get; set; }
    }
}
