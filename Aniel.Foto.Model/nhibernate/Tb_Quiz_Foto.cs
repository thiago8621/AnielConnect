using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Foto.Model.nhibernate
{
    public class Tb_Quiz_Foto
    {
        public virtual long Id { get; set; }
        public virtual long IdAnswerCapa { get; set; }
        public virtual long IdAnswerItem { get; set; }
        public virtual long IdQuestion { get; set; }
        public virtual byte[] Arquivo { get; set; }
    }
}
