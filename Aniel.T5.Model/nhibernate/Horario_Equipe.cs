using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class Horario_Equipe : BaseModel        
    {
        public virtual EquipeAniel Equipe { get; set; }

        public virtual Status_Default Status { get; set; }

        public virtual Horario_Connect Horario { get; set; }

        public virtual DateTime Data { get; set; }

        public virtual Boolean ApenasNoDia { get; set; }

        public virtual Boolean Indisponivel { get; set; }
        public virtual PerfilAgendamento PerfilAgendamento { get; set; }

    }
}
