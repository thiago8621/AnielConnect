using Sinapse.Model.nhibernate;
using System;

namespace Aniel.T5.Model.nhibernate
{
    public class Tb_NCM 

    {
        public virtual Int64 Id { get; set; }
        public virtual String Codigo_NCM { get; set; }
        public virtual String Descricao_NCM { get; set; }
        public virtual String Codigo_CEST { get; set; }
       
    }
}
