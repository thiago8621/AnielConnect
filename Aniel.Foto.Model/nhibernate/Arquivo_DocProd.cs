using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Arquivo_DocProd
    {
        public virtual Int64 Id { get; set; }
        public virtual String Chave { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Tipo { get; set; }
        public virtual Byte[] Arquivo { get; set; }
        public virtual String Usuario { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual DateTime Hora { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String OsDigital { get; set; }
    }
}
