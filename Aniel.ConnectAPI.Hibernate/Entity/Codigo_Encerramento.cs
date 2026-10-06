using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    public class Codigo_Encerramento : BaseClass
    {
        public virtual String Cod_Encerramento { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Int16 Status_Execucao { get; set; }

        public virtual Contrato Contrato { get; set; }
    }
}
