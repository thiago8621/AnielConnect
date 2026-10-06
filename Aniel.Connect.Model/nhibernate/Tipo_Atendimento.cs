using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Tipo_Atendimento : BaseModel
    {
        private IList<Tipo_Atendimento_Motivo_Status> _motivos;

        public virtual String Nome { get; set; }

        public virtual Script_Atendimento Script { get; set; }

        public virtual Prioridade_Atendimento? Prioridade_Padrao { get; set; }

        public virtual Status_Default Status { get; set; }

        public virtual Categoria_Atendimento Categoria_Atendimento { get; set; }

        public virtual Boolean Criar_Os_Encerrar { get; set; }

        public virtual Boolean Exibir_Ola { get; set; }
        public virtual Boolean Vincular_Motivos { get; set; }
        public virtual Boolean Solicitacao_Cancelamento { get; set; }

        //public virtual Int16? Id_Tipo_Servico { get; set; }

        public virtual String Desc_Tipo_Servico { get; set; }
        public virtual Int16? Id_Departamento_Padrao { get; set; }

        public virtual bool Trava_Atendimento_OS { get; set; }
        public virtual bool Encerrar_Atendimento_Junto_OS { get; set; }
        public virtual Boolean Criacao_Atendimento_Email { get; set; }
        public virtual Boolean Criacao_Atendimento_DominioEmail { get; set; }

        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual Script_SuporteTecnico Script_SuporteTecnico { get; set; }

        public virtual IList<Tipo_Atendimento_Motivo_Status> Motivos
        {
            get
            {
                if (_motivos == null)
                    _motivos = new List<Tipo_Atendimento_Motivo_Status>();
                return _motivos;
            }
            set
            {
                _motivos = value;
            }
        }
    }
}
