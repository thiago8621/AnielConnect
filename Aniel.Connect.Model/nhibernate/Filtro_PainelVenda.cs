using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Filtro_PainelVenda : BaseModel
    {
        public virtual String Valor { get; set; }
        public virtual Modelo_PainelVenda Modelo_PainelVenda { get; set; }
        public virtual Filtro_PainelVendaEnum Filtro { get; set; }
        public virtual ModeloPainel_Data? Data { get; set; }
    }
}
