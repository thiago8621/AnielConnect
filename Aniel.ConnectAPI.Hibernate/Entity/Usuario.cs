using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    
    public class Usuario
    {
        [Column(ColumnTargets.Key)]
        public virtual String Id { get; set; }

        public virtual String Usuario_Aniel { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Senha_Aniel { get; set; }
        public virtual String SecurityStamp { get; set; }

        public virtual Int64? IdFuncionario { get; set; }

        public virtual Int64? IdDepartamento { get; set; }

        public virtual Int16? Ativo { get; set; }
    }
}
