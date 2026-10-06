using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Desconto_CondominiosPredios : BaseModel
    {
        public virtual String Cond_Descricao { get; set; }
        public virtual Condominio_Predio CondominioPredio { get; set; }
        public virtual Desconto Desconto { get; set; }
    }
}
