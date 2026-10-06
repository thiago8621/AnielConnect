using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class SLA_Atendimento : BaseModel
    {

        public virtual Tipo_Atendimento Tipo_Atendimento { get; set; }
        public virtual Prioridade_Atendimento Prioridade_Atendimento { get; set; }
        public virtual Categoria_Cliente Categoria_Cliente { get; set; }
        public virtual Int32? Minutos { get; set; }
        public virtual Boolean Calendario_Padrao { get; set; }
        public virtual Status_Default? Sla_PlanoProduto { get; set; }
        public virtual Status_Default? Status { get; set; }
        public virtual Int64? Id_Horario { get; set; }

        public virtual Contagem_Prazo Contagem_Prazo { get; set; }

        private IList<SLA_Atendimento_PlanoProduto> _planosprodutos;

        public virtual IList<SLA_Atendimento_PlanoProduto> PlanosProdutos
        {
            get
            {
                if (_planosprodutos == null)
                    _planosprodutos = new List<SLA_Atendimento_PlanoProduto>();
                return _planosprodutos;
            }
            set
            {
                _planosprodutos = value;
            }
        }
    }
}
