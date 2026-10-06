using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Coluna_PainelVenda : BaseModel
    {
        public virtual Modelo_PainelVenda Modelo_PainelVenda { get; set; }
        public virtual Coluna_PainelVendaEnum Coluna { get; set; }
    }
}
