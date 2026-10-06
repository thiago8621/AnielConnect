using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Pt_Foto
    {
        public virtual Int64 Id { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual String NumFre { get; set; }
        public virtual Int64 Id_Mascacao { get; set; }
        public virtual Byte[] Foto { get; set; }
        public virtual Int32 Id_Assinatura { get; set; }
    }
}
