using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class HistoricoAlterEscala : BaseModel
    {
        public virtual String Num_Fre { get; set; }
        public virtual Horario_Connect Horario_Anterior { get; set; }
        public virtual Horario_Connect Horario_Atual { get; set; }
        public virtual DateTime? DataInicio { get; set; }
        public virtual PerfilAgendamento PerfilAgendamentoAnterior { get; set; }
        public virtual PerfilAgendamento PerfilAgendamentoAtual { get; set; }
        public virtual Boolean? IndisponivelAnterior { get; set; }
        public virtual Boolean? IndisponivelAtual { get; set; }
        public virtual DateTime? Data { get; set; }
    }
}
