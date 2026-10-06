using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class MaterialTarefa : BaseModel
    {
        public virtual bool Alterado { get; set; }
        public virtual long? Id_Cliente { get; set; }
        public virtual string CodMat_Inst { get; set; }
        public virtual decimal? Quantidade { get; set; }
        public virtual decimal? Quantidade_Removida { get; set; }
    }
}
