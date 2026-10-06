using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Tipo_Imovel : BaseModel
    {
        public virtual Int32 Cod_Tipo_Imovel { get; set; }
        public virtual String Descricao { get; set; }
    }
}
