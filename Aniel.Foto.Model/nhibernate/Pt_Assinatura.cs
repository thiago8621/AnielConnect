using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Pt_Assinatura
    {
        public virtual Int32 Id { get; set; }
        public virtual String NumFre { get; set; }
        public virtual Int16 Vinculo_Atual { get; set; }
        public virtual Byte[] Assinatura { get; set; }
        public virtual DateTime DataHoraCriacao { get; set; }
    }
}
