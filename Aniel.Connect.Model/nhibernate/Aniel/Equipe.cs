using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Equipe : BaseModel
    {
        public virtual String Matricula { get; set; }
        public virtual String EquipeAux { get; set; }
        public virtual String Nome { get; set; }
        public virtual DateTime? Encerramento { get; set; }

        public virtual Funcionario Funcionario { get; set; }
        public virtual Funcionario Supervisor { get; set; }
        public virtual Perfil_Equipe Perfil_Equipe { get; set; }
        public virtual Regiao Regiao { get; set; }

    }
}
