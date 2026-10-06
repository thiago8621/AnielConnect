using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class PerfilAgendamentoArea : BaseModel
    {
        public virtual string Escritorio { get; set; }
        public virtual string Area { get; set; }
        public virtual string Localidade { get; set; }
        public virtual Int16 Nivel { get; set; }
        public virtual bool Todas_Localidades { get; set; }
        public virtual bool Todas_Areas { get; set; }
        public virtual PerfilAgendamento PerfilAgendamento { get; set; }
    }

}
