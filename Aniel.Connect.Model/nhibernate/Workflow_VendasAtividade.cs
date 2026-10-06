using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Workflow_VendasAtividade : BaseModel
    {
        public virtual Workflow_Vendas WorkFlow { get; set; }
        public virtual bool Ativ_AgendaTarefa { get; set; }
        public virtual Status_Default Ativ_Status { get; set; }
        public virtual String Ativ_Descricao { get; set; }
        public virtual Int32? Ativ_Prazo_Minutos { get; set; }
        public virtual Tipo_HORA Ativ_Tipodias { get; set; }
        public virtual String Ativ_Tipodias_Desc { get; set; }
        public virtual String Horario_Atendimento { get; set; }
        public virtual Atividade_StatusVenda Ativ_StatusVenda { get; set; }
        public virtual Departamento SetorResponsavel { get; set; }
        public virtual IdentityUser Responsavel { get; set; }
        public virtual Boolean Definir_Responsavel { get; set; }
        public virtual Atividade_Decisoria Ativ_Decisao { get; set; }
        public virtual Tipo_Atividade_Decisoria Ativ_TipoDecisao { get; set; }
        public virtual Int32? Ativ_NAtivAprovada { get; set; }
        public virtual Int32? Ativ_NAtivReprovada { get; set; }
    }
}



