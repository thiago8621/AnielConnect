using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class TarefaVendedor : BaseModel
    {
        private IList<ParticipanteTarefa> _participantes;

        public virtual long IdTipoTarefa { get; set; }
        public virtual long IdVenda { get; set; }
        public virtual long IdVendedor { get; set; }
        public virtual string Responsavel { get; set; }
        public virtual Venda Venda { get; set; }
        public virtual string Assunto { get; set; }
        public virtual DateTime DataInicio { get; set; }
        public virtual DateTime DataFim { get; set; }
        public virtual string HorarioInicio { get; set; }
        public virtual string HorarioFim { get; set; }
        public virtual string Local { get; set; }
        public virtual string Observacao { get; set; }
        public virtual NivelPrioridadeTarefaVendedorEnum Prioridade { get; set; }
        public virtual StatusTarefaEnum Status { get; set; }
        public virtual IList<ParticipanteTarefa> Participantes 
        {
            get
            {
                if (_participantes == null)
                    _participantes = new List<ParticipanteTarefa>();
                return _participantes;
            }
            set
            {
                _participantes = value;
            }
        }

        public virtual bool Repetir { get; set; }
    }

    public class ParticipanteTarefa : BaseModel
    {
        public virtual long IdTarefa { get; set; }
        public virtual string Email { get; set; }
        public virtual TarefaVendedor Tarefa { get; set; }
    }
}
