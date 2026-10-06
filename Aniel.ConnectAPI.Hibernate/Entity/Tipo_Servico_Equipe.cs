using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    public class Tipo_Servico_Equipe : BaseClass
    {
        public virtual Int32 Cod_Tipo_Serv { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Descricao_Aux { get; set; }
        public virtual Int32 Cod_Natureza { get; set; }
    }
}
