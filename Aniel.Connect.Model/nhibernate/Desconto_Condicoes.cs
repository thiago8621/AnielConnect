using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Desconto_Condicoes : BaseModel
    {
        public virtual Boolean CondicaoCombinada { get; set; }
        public virtual String TipoCondicao { get; set; }
        public virtual String ValorCondicao  { get; set; }
        public virtual String ValorCondicaoMaximo { get; set; }
        public virtual Desconto Desconto { get; set; }

    }
}









