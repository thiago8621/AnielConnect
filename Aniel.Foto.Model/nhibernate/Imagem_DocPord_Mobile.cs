using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Imagem_DocPord_Mobile
    {
        public virtual Int64 Id { get; set; }
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32 Num_Doc { get; set; }
        public virtual String Tipo { get; set; }
        public virtual Int16 Status { get; set; }
        public virtual String Usuario { get; set; }
        public virtual DateTime Dia { get; set; }
        public virtual DateTime Hora { get; set; }
        public virtual Byte[] Arquivo_Depois { get; set; }
        public virtual Byte[] Arquivo { get; set; }
    }
}
