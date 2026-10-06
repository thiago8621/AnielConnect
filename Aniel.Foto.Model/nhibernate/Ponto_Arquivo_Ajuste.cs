using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Ponto_Arquivo_Ajuste
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64 Id_Documento_Ajuste { get; set; }
        public virtual String Nome_Arquivo { get; set; }
        public virtual String Tipo { get; set; }
        public virtual Byte[] Arquivo { get; set; }
    }
}
