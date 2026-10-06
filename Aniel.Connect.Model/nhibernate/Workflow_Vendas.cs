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
    public class Workflow_Vendas : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual Tipo_Pessoa? TipoPessoa { get; set; }
        public virtual String Categoria { get; set; }
        public virtual Boolean Definir_Responsavel { get; set; }
        public virtual String Captacao { get; set; }
        public virtual UF? Uf { get; set; }
        public virtual Int64? Cidade { get; set; }
        public virtual Int64 Prazo { get; set; }
        public virtual Tipo_SLA Tipodias { get; set; }
        public virtual Int32 Validador  { get; set; }



        private IEnumerable<Workflow_VendasAtividade> _atividades;
        public virtual IEnumerable<Workflow_VendasAtividade> Atividades
        {
            get
            {
                if (_atividades == null)
                    _atividades = new List<Workflow_VendasAtividade>();
                return _atividades;
            }
            set
            {
                _atividades = value;
            }
        }
    }

   
}