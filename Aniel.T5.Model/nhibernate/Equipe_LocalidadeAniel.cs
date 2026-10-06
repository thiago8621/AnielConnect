using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class Equipe_LocalidadeAniel : BaseModel
    {
        public virtual EquipeAniel Equipe { get; set; }

        public virtual String Cod_Escritorio { get; set; }

        public virtual String Cod_Area { get; set; }

        public virtual Int32? Cod_Localidade { get; set; }
        public virtual Int16? Nivel { get; set; }

        public virtual Boolean Todas_Localidades { get; set; }
    }
}
