using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    
    public class SLA_Doc_Prod : BaseClass
    {
        public virtual Tipo_Servico_Equipe Tipo_Servico { get; set; }
        public virtual Categoria_Cliente Categoria_Cliente { get; set; }
        public virtual Criticidade_Doc_Prod Criticidade { get; set; }
        public virtual Int32 Porc_Ini { get; set; }
        public virtual Int32 Porc_Fim { get; set; }
        public virtual Int16 Cor { get; set; }
    }
}