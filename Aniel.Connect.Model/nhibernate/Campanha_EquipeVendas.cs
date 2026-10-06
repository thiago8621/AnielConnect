using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Campanha_EquipeVendas : BaseModel
    {
        public virtual Int64? Setor_Departamento { get; set; }
        public virtual String Id_Funcionario { get; set; }
        public virtual String Id_Supervisor { get; set; }
        public virtual Double? Meta { get; set; }
        public virtual Campanha Campanha { get; set; }


    }
}


