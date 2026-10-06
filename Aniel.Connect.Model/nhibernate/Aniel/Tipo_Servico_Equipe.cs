using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Tipo_Servico_Equipe : BaseModel
    {
        public virtual Int32 Cod_Tipo_Serv { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Descricao_Aux { get; set; }
        public virtual Int32 Cod_Natureza { get; set; }
    }
}
