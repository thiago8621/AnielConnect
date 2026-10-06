using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    public class Tipo_Imovel : BaseClass
    {
        public virtual Int32 Cod_Tipo_Imovel { get; set; }
        public virtual String Descricao { get; set; }
    }
}
