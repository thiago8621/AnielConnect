using Sinapse.Model;
using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class SupervisorEquipeVenda : BaseModel
    {
        public virtual EquipeVenda EquipeVenda { get; set; }
        public virtual Funcionario Supervisor { get; set; }

    }
}