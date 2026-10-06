using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Area_Acesso_OLA : BaseModel
    {
        public virtual string Cod_Area_Acesso { get; set; }
        public virtual bool Ocultar_Acesso { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Observacao { get; set; }
    }
}
