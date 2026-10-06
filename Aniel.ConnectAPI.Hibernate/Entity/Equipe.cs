using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    
    public class Equipe : BaseClass
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
