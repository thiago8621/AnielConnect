using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Cliente_Desconto :BaseModel
    {

        public virtual Cliente  Cliente   { get; set; }
        public virtual Desconto Desconto  { get; set; }
        public virtual Double   Percentual_Desconto { get; set; }
        public virtual Double   Valor_Desconto { get; set; }
        public virtual String   Numero_ContratoVinculado { get; set; }
        public virtual DateTime? DataAtribuicaoDesconto { get; set; }
        public virtual DateTime? DataVencimentoDesconto { get; set; }
        public virtual Boolean DescontoIndeterminado { get; set; }
    }
}
