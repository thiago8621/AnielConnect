using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    
    public class Empresa : BaseClass
    {
        public virtual Int16 CodEmpresa { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Razao_Social { get; set; }

        public virtual DateTime? Data_Alteracao { get; set; }
        public virtual Byte[] Logotipo { get; set; }
    }
}
