using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Ponto_Venda_Vendedor : BaseModel
    {
        public virtual DateTime? DataInicial { get; set; }
        public virtual DateTime? DataFinal { get; set; }

        public virtual Funcionario Vendedor { get; set; }

        public virtual Ponto_Venda Ponto_Venda { get; set; }
    }
}
