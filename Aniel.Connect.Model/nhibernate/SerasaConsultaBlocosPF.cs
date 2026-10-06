using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class ConsultaBlocosSerasaPF : BaseModel
    {
        public virtual String cpf { get; set; }
        public virtual String cnpj { get; set; }
        public virtual String BlocoN200 { get; set; }
        public virtual String BlocoN210 { get; set; }
        public virtual String BlocoN220 { get; set; }
        public virtual String BlocoN230 { get; set; }
        public virtual String BlocoN240 { get; set; }
        public virtual String BlocoN250 { get; set; }
        public virtual String BlocoN270 { get; set; }
        public virtual String BlocoN410 { get; set; }
        public virtual String BlocoN420 { get; set; }
        public virtual String BlocoN430 { get; set; }
        public virtual String BlocoN440 { get; set; }
        public virtual String BlocoN460 { get; set; }
        public virtual String BlocoN690 { get; set; }     
        public virtual String BlocoF900 { get; set; }

        #region Mobile
        public virtual String BlocoN500 { get; set; }
        #endregion

    }
}
