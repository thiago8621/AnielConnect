using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Quiz_Foto
    {
        public virtual Int64 Id { get; set; }
        public virtual String Usuario { get; set; }
        public virtual DateTime Dia { get; set; }
        public virtual DateTime Hora { get; set; }

        public virtual Int64 Id_Answer_Capa { get; set; }
        public virtual Int64 Id_Quiz_Answer_Item { get; set; }
        public virtual Int64 Id_Question { get; set; }
        public virtual Int64 Id_Question_Answer { get; set; }

        public virtual Byte[] Arquivo { get; set; }
    }
}
