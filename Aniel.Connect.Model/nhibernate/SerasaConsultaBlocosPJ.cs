using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class SerasaConsultaBlocosPJ : BaseModel
    {
        public virtual String Cnpj { get; set; }
        public virtual String BlocoN200 { get; set; }
        public virtual String BlocoN210 { get; set; }
        public virtual String BlocoN230 { get; set; }
        public virtual String BlocoN240 { get; set; }
        public virtual String BlocoN250 { get; set; }
        public virtual String BlocoN270 { get; set; }
        public virtual String BlocoN440 { get; set; }
        public virtual String BlocoN700 { get; set; }
        public virtual String BlocoF900 { get; set; }
        public virtual String BlocoN705 { get; set; }
    }
}
