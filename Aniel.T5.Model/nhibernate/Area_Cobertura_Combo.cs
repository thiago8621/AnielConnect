using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;


namespace Aniel.T5.Model.nhibernate
{
    public class Area_Cobertura_Combo
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Codigo_Combo { get; set; }
        public virtual Area_Cobertura Area { get; set; }

    }


}
